#
# svn.pm
#
# Contributed by Shahar Talmi
# Safend
#
# http://www.safend.com
#
# ssindex script to handle Subversion version control system
#

package SVN;

require Exporter;
use strict;

our %EXPORT_TAGS = ( 'all' => [ qw() ] );
our @EXPORT_OK   = ( @{ $EXPORT_TAGS{'all'} } );
our @EXPORT      = qw();
our $VERSION     = '0.1';

use Data::Dumper;

#
# Simple subs to make it clear when we're testing for BOOL values
#
sub TRUE   {return(1);} # BOOLEAN TRUE
sub FALSE  {return(0);} # BOOLEAN FALSE

# -----------------------------------------------------------------------------
# Create a new blessed reference that will maintain state for this instance of indexing
# -----------------------------------------------------------------------------
sub new {
    my $proto = shift;
    my $class = ref($proto) || $proto;
    my $self  = {};
    bless($self, $class);

    # Define default username and password
    $$self{'SVNUSER'} = "<N/A>";
    $$self{'SVNPASS'} = "<N/A>";
    $$self{'SVNCMD'} = "svn.exe";

    # Allow environment overrides for these settings.
    $$self{'SVNCMD'}  = $ENV{'SVNCMD'}   if (defined $ENV{'SVNCMD'});
    $$self{'SVNREV'}  = $ENV{'SVNREV'}   if (defined $ENV{'SVNREV'});
    $$self{'SVNUSER'} = $ENV{'SVNUSER'}   if (defined $ENV{'SVNUSER'});
    $$self{'SVNPASS'} = $ENV{'SVNPASS'}   if (defined $ENV{'SVNPASS'});

    $$self{'SSDEBUGMODE'} = 0;

    $$self{'FILE_LOOKUP_TABLE'} = ();

    # Block for option parsing.
    PARSEOPTIONS: {
        my @unused_opts;
        my @opt;

        foreach (@ARGV) {
            # handle command options
            if (substr($_, 0, 1) =~ /^[\/-]$/) {
                # options that set values
                if ( (@opt = split(/=/, $_))==2 ) {
                    block: {
                        $$self{'SVNREV'}   = $opt[1], last if ( uc substr($opt[0], 1) eq "REV"   );
                        $$self{'SVNUSER'}   = $opt[1], last if ( uc substr($opt[0], 1) eq "USER"   );
                        $$self{'SVNPASS'}   = $opt[1], last if ( uc substr($opt[0], 1) eq "PASS"   );
                        # Remember this was unused
                        push(@unused_opts, $_);
                        1;
                    }
                # options that are just flags
                }
            } else {
                # Remember this was unused
                push(@unused_opts, $_);
            }
        }

        # Fixup @ARGV to only contained unused options so SSIndex.cmd
        # can warn the user about them if necessary.
        @ARGV = @unused_opts;
    }

    return($self);
}

# -----------------------------------------------------------------------------
# Display module internal option state.
# -----------------------------------------------------------------------------
sub DisplayVariableInfo {
    my $self = shift;

    ::status_message("%-15s: %s\n",
                     "SVN Executable",
                     $$self{'SVNCMD'});

    ::status_message("%-15s: %s\n",
                     "SVN Revision",
                     $$self{'SVNREV'} ? $$self{'SVNREV'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "SVN Username",
                     $$self{'SVNUSER'} ? $$self{'SVNUSER'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "SVN Password",
                     $$self{'SVNPASS'} ? $$self{'SVNPASS'} : "<N/A>");

}

# -----------------------------------------------------------------------------
# Given our init data and a local source path, create a lookup table that can
# return individual stream data for each source file.
# -----------------------------------------------------------------------------
sub GatherFileInformation {
    my $self       = shift;
    my $SourceRoot = shift;
    my $ServerRefs = shift;

    my %FileMapHash;

    #
    # Create a hash that maps files on the client to files on the server
    #
    my $hProcess;

    if ( ! open($hProcess, "dir $SourceRoot 2>&1 |") ) {
        ::warn_message("Unable to resolve directory: $!");
        return();
    }

    my $curline;

    # Loop on "Directory of " entries
    while ($curline = <$hProcess>) {
        chomp $curline;
        next if ($curline =~ /^\s*$/);

        # Parse directory name
        if ( $curline =~ m/^[\s\t]*Directory of[\s\t]*(.*)$/i ) {
            $SourceRoot = lc $1;
            ::info_message("Resolved directory to $SourceRoot");
        }
    }

    if ( ! open($hProcess, "$$self{'SVNCMD'} info -R \"$SourceRoot\"|") ) {
        ::warn_message("Unable to start $$self{'SVNCMD'}: $!");
        return();
    }

    ::status_message("Processing $$self{'SVNCMD'} properties output ...");

    # Loop on "Path:" entries
    while ($curline = <$hProcess>) {
        chomp $curline;
        next if ($curline =~ /^\s*$/);

        my $LocalFile;
        # Parse new file
        if ( $curline =~ m/^Path:[\s\t]*(.*)$/i ) {
            $LocalFile = lc $1;

            my $FileRepository;
            my $FileRelative;
            my $FileRevision;

            # Loop through the file details
            while ($curline = <$hProcess>) {
                chomp $curline;
                next if ($curline =~ /^\s*$/);

                # Parse URL line
                if ( $curline =~ m/^URL:[\s\t]*(.*)$/i ) {
                    my $FileURL = $1;
                    if ( $FileURL =~ m/^(.*\:\/*[^\:\/]*\:*\d*\/)(.*)$/ ) {
                        $FileRepository = $1;
                        $FileRelative = $2;
                    }
                    if ( $FileURL =~ m/^(\\\\[^\\]*\\)(.*)$/ ) {
                        $FileRepository = $1;
                        $FileRelative = $2;
                    }
                    if ( $FileURL =~ m/^(.:\\)(.*)$/ ) {
                        $FileRepository = $1;
                        $FileRelative = $2;
                    }
                }

                # Parse Revision line
                if ( $curline =~ m/^Revision:[\s\t]*(\d*)$/i ) {
                    if ( ( $FileRepository )&&( $FileRelative ) ) {
                        $FileRevision = $1;
                        last;
                    }
                }

                # Check if we got to a new file
                if ( $curline =~ m/^Path:[\s\t]*(.*)$/i ) {
                    ::warn_message("Can't Get details for $LocalFile...");
                    $LocalFile = lc $1;
                }

            } # End loop through the file details

            # In the file lookup table, we create an array.  The first element is a hash
            # of file-specific variables (always the same for SVN) and the second element
            # is the actual file line for the given file.
            if ( $FileRevision ) {
                @{$$self{'FILE_LOOKUP_TABLE'}{lc $LocalFile}} = ( { }, "$LocalFile*$FileRepository*$FileRelative*$FileRevision");
            }
            else {
                ::warn_message("Can't Get details for $LocalFile...");
            }

        } # End parse new file
    } # End loop on "Path:" entries

    # close the handle
    close($hProcess);

    # return true if any files were found
    return( keys %{$$self{'FILE_LOOKUP_TABLE'}} != 0 );
}

# -----------------------------------------------------------------------------
# Return ths SRCSRV stream data for a single file.
# -----------------------------------------------------------------------------
sub GetFileInfo {
    my $self = shift;
    my $file = shift;

    # We stored the necessary information when GatherFileInformation() was
    # called so we just need to return that information.
    if ( defined $$self{'FILE_LOOKUP_TABLE'}{lc $file} ) {
        return( @{$$self{'FILE_LOOKUP_TABLE'}{lc $file}} );
    } else {
        return(undef);
    }
}

# -----------------------------------------------------------------------------
# The long name that should be written the SRCSRV stream to describe
# the source control system being indexed.
# -----------------------------------------------------------------------------
sub LongName {
    return("Subversion");
}

# -----------------------------------------------------------------------------
# Set the debug level for output.
# -----------------------------------------------------------------------------
sub SetDebugMode {
    my $self = shift;
    $$self{'SVNDEBUGMODE'} = shift;
}

# -----------------------------------------------------------------------------
# Return the SCS specific stream variables.
# -----------------------------------------------------------------------------
sub SourceStreamVariables {
    my $self = shift;
    my @stream;
	my $usercred = "";
	#
	# --username and --password is optional
	#
	if ($$self{'SVNUSER'} =~ m/^\<N\/A\>$/i) {
            # do not emit user credential (password is also ignored)
	} else {
		$usercred .= "--username %svnuser% ";
		push(@stream, "SVNUSER=$$self{'SVNUSER'}");
		if ($$self{'SVNPASS'} =~ m/^\<N\/A\>$/i) {
                    # do not emit user password
		} else {
                    push(@stream, "SVNPASS=$$self{'SVNPASS'}");
                    $usercred .= "--password %svnpass% ";
		}
	}
    push(@stream, "SVN_EXTRACT_TARGET=".
                  "%targ%\\%fnbksl%(%var3%)\\%var4%\\%fnfile%(%var1%)");
    push(@stream, 
		 "SVN_EXTRACT_CMD=cmd /c svn.exe cat ".
		 "\"%var2%%var3%\@%var4%\" ".
		 "--non-interactive " .
		 $usercred .
		 "> \"%svn_extract_target%\"");
    return(@stream);
}

# -----------------------------------------------------------------------------
# Loads previously saved file information.
# -----------------------------------------------------------------------------
sub LoadFileInfo {
    my $self = shift;
    my $dir  = shift;

    if ( -e "$dir\\svn_files.dat" ) {
        our $FileData1;
        require "$dir\\svn_files.dat";
        $$self{'FILE_LOOKUP_TABLE'} = $FileData1;
    } else {
        ::status_message("No SVN information saved in $dir.\n");
    }

    return();
}

# -----------------------------------------------------------------------------
# Saves current file information.
# -----------------------------------------------------------------------------
sub SaveFileInfo {
    my $self = shift;
    my $dir  = shift;

    my $fh;
    if ( open($fh, ">$dir\\svn_files.dat") ) {
        $Data::Dumper::Varname = "FileData";
        $Data::Dumper::Indent  = 0;
        print $fh Dumper($$self{'FILE_LOOKUP_TABLE'});
        close($fh);
    } else {
        ::status_message("Failed to save data to $dir.\n");
    }

    return();
}

# -----------------------------------------------------------------------------
# Simple usage ('-?')
# -----------------------------------------------------------------------------
sub SimpleUsage {
print<<SVN_SIMPLE_USAGE;
SVN specific settings:

     NAME            SWITCH      ENV. VAR        Default
  -----------------------------------------------------------------------------
  A) SVN Command     </N/A>      SVNCMD          svn.exe
  B) SVN Revision    Rev         SVNREV          <N/A>
  C) SVN Username    User        SVNUSER         <N/A>
  D) SVN Password    Pass        SVNPASS         <N/A>

SVN_SIMPLE_USAGE
}

# -----------------------------------------------------------------------------
# Verbose usage (-??)
# -----------------------------------------------------------------------------
sub VerboseUsage {
print<<SVN_VERBOSE_USAGE;
(A)  SVN Command - svn.exe or path to the svn executable file. This command is
     used for indexing only. When debugging, svn.exe should be in PATH 
     environment variable.
(B)  SVN Revision - Causes the script to replace individual file versions with the
     given revision.  The script does not verify that the revision exists.
(C)  SVN Username - The username used to logon into SVN when the source files are
     exported from the repository by the debugger. If not set, user's 
     cached token will be used (Recommended).
(D)  SVN Password - The password used to logon into SVN when the source files are
     exported from the repository by the debugger. If not set, user's 
     cached token will be used (Recommended).
SVN_VERBOSE_USAGE
}

1;
__END__
