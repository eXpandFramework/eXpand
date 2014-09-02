# -----------------------------------------------------------------------------
# vss.pm
# 
# Copyright (c) 2006  Microsoft Corporation
#
# Source Server indexing module for Visual SourceSafe
# -----------------------------------------------------------------------------

package VSS;

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

    if ( defined $ENV{'SSROOT'} ) {
        $$self{'SSROOT'} = $ENV{'SSROOT'};
    } else {
        $$self{'SSROOT'} = `cd`;
        chomp $$self{'SSROOT'};
    }

    my @DataToParse = `vssdump.exe -c`;

    if ( $?==0 ) {
        foreach (@DataToParse) {
            if ( m/^\s*ssdir:\s*(.*)\s*$/ ) {
                $$self{'SSDIR'} = $1;
            } elsif ( m/^\s*project:\s*(.*)\s*$/ ) {
                $$self{'SSPROJECT'} = $1;
            }
        }
    } else {
        # Trickery to get to the generally correct value as it was returned from the
        # actual executable.
        my $err = unpack('c', pack('C', $?>>8));
        ::warn_message("vssdump.exe returned errors. Exit code %d\n", $err);

        if ( scalar(@DataToParse)==0 ) {
            ::warn_message("Output from 'vssdump.exe -c' contained no text.\n");
        } else {
            foreach (@DataToParse) {
                chomp;
                ::warn_message("$_\n");
            }
        }
        ::fatal_error("Unable to continue.\n");
    }

    # Allow environment overrides for these settings.
    $$self{'SSDIR'}     = $ENV{'SSDIR'}     if (defined $ENV{'SSDIR'});
    $$self{'SSLABEL'}   = $ENV{'SSLABEL'}   if (defined $ENV{'SSLABEL'});
    $$self{'SSPROJECT'} = $ENV{'SSPROJECT'} if (defined $ENV{'SSPROJECT'});

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
                        if ( uc substr($opt[0], 1) eq "ALLROOT"  ) {
                            $$self{'SSROOT'}  = $opt[1];
                            last;
                        }

                        $$self{'SSDIR'}     = $opt[1], last if ( uc substr($opt[0], 1) eq "SERVER"  );
                        $$self{'SSROOT'}    = $opt[1], last if ( uc substr($opt[0], 1) eq "CLIENT"  );
                        $$self{'SSPROJECT'} = $opt[1], last if ( uc substr($opt[0], 1) eq "PROJECT" );
                        $$self{'SSLABEL'}   = $opt[1], last if ( uc substr($opt[0], 1) eq "LABEL"   );
                        # Remember this was unused
                        push(@unused_opts, $_);
                        1;
                    }
                # options that are just flags
                } else {
                    block: {
                        $$self{'SSOFFLINE'} = TRUE, last if ( uc substr($_, 1, 5) eq "OFFLINE");
                        # Remember this was unused
                        push(@unused_opts, $_);
                        1;
                    }
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

    # there's nothing to do if a label is not defined
    if ( ! defined $$self{'SSLABEL'} ) {
        ::fatal_error("Label must be set via the command line or in the environment.\n");
    }

    # there's nothing to do if a label is not defined
    if ( ! defined $$self{'SSDIR'} ) {
        ::fatal_error("SSDIR must be set via the command line or in the environment.\n");
    }

    # It seems SSPROJECT can contain forward slashes.  Create a DOS-like SSPROJECT
    # for use in file path checking.
    if ( defined $$self{'SSPROJECT'} ) {
        $$self{'SSPROJECT_DOSLIKE'} = $$self{'SSPROJECT'};
        $$self{'SSPROJECT_DOSLIKE'} =~ s/\//\\/g;
    }

    return($self);
}

# -----------------------------------------------------------------------------
# Display module internal option state.
# -----------------------------------------------------------------------------
sub DisplayVariableInfo {
    my $self = shift;

    ::status_message("%-15s: %s\n",
                     "VSS Server",
                     $$self{'SSDIR'});

    ::status_message("%-15s: %s\n",
                     "VSS Client Root",
                     $$self{'SSROOT'});

    ::status_message("%-15s: %s\n",
                     "VSS Project",
                     $$self{'SSPROJECT'} ? $$self{'SSPROJECT'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "VSS Label",
                     $$self{'SSLABEL'} ? $$self{'SSLABEL'} : "<N/A>");

}

# -----------------------------------------------------------------------------
# Given our init data and a local source path, create a lookup table that can
# return individual stream data for each source file.
# -----------------------------------------------------------------------------
sub GatherFileInformation {
    my $self       = shift;
    my $SourceRoot = shift;
    my $ServerRefs = shift;
    my $OldDir     = (`cd`)[0];
    chomp $OldDir;

    chdir("$SourceRoot");

    my $CommandToRun = "vssdump.exe -s -r -t";

    my %FileMapHash;

    if ( ! defined $$self{'SSDIR'} ) {
        ::fatal_error("Server must be set via the command line or in the environment.\n");
    }

    if ( defined $$ServerRefs{uc $$self{'SSDIR'}} ) {
        $$self{'SSSERVERVAR'} = $$ServerRefs{uc $$self{'SSDIR'}};
    } else {
        ::fatal_error("$$self{'SSDIR'} missing from SrcSrv.ini\n");
    }

    #
    # Create a hash that maps files on the client to files on the server
    #
    my $hProcess;

    if ( defined $$self{'SSLABEL'} && $$self{'SSLABEL'} ne "" ) {
        $CommandToRun .= " -l:\"" . $$self{'SSLABEL'} . "\"";
    }

    if ( ! open($hProcess, "$CommandToRun|") ) {
        ::warn_message("Unable to start vssdump.exe: $!");
        return();
    }

    my $curline;

    ::status_message("Processing vssdump.exe output ...");

    # Loop on "File:" entries
    while ($curline = <$hProcess>) {
        chomp $curline;
        next if ($curline =~ /^\s*$/);

        my $curfile;
        # Parse new file
        my @Fields = split(/\|/, $curline);

        $Fields[0] =~ s/^\$\///;

        my $LookupEntry;

        if ( scalar(@Fields)==3 ) {
            $LookupEntry = "$$self{'SSSERVERVAR'}*$Fields[0]*";

            if ( defined $$self{'SSLABEL'} && $$self{'SSLABEL'} ne "") {
                $LookupEntry .= "$$self{'SSLABEL'}";
            } else {
                $LookupEntry .= "$Fields[2]";
            }

        } elsif ( scalar(@Fields)==2 ) {
            $LookupEntry = "$$self{'SSSERVERVAR'}*$Fields[0]*";

            if ( defined $$self{'SSLABEL'} && $$self{'SSLABEL'} ne "") {
                $LookupEntry .= "$$self{'SSLABEL'}";
            } else {
                ::warn_message("SSLABEL is not defined skipping $Fields[0] - no revision found\n");
                next;
            }
        } else {
            next;
        }

        @{$$self{'FILE_LOOKUP_TABLE'}{lc $Fields[1]}} = ( { "$$self{'SSSERVERVAR'}" => "$$self{'SSDIR'}"
                                                          },
                                                          "$LookupEntry"
                                                        );

    } # End loop over vssdump.exe output.

    # close the handle
    close($hProcess);

    chdir($OldDir);

    # return true if any files were found
    return( keys %{$$self{'FILE_LOOKUP_TABLE'}} != 0 );
}

# -----------------------------------------------------------------------------
# Return ths SRCSRV stream data for a single file.
# -----------------------------------------------------------------------------
sub GetFileInfo {
    my $self        = shift;
    my $file        = shift;
    my $name_in_pdb = shift;

    if ( ! defined $name_in_pdb ) {
        $name_in_pdb = $file;
    }

    # We stored the necessary information when GatherFileInformation() was
    # called so we just need to return that information.
    if ( defined $$self{'FILE_LOOKUP_TABLE'}{lc $file} ) {
        return( ${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[0],
                "$name_in_pdb*${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[1]" );
    } else {
        return(undef);
    }
}

# -----------------------------------------------------------------------------
# The long name that should be written the SRCSRV stream to describe
# the source control system being indexed.
# -----------------------------------------------------------------------------
sub LongName {
    return("Visual Source Safe");
}

# -----------------------------------------------------------------------------
# Set the debug level for output.
# -----------------------------------------------------------------------------
sub SetDebugMode {
    my $self = shift;
    $$self{'SSDEBUGMODE'} = shift;
}

# -----------------------------------------------------------------------------
# Return the SCS specific stream variables.
# -----------------------------------------------------------------------------
sub SourceStreamVariables {
    my $self = shift;
    my @stream;
    push(@stream, "SSDIR=$$self{'SSDIR'}");
    push(@stream, "SRCSRVENV=SSDIR=%$$self{'SSSERVERVAR'}%");
    push(@stream, "VSSTRGDIR=%targ%\\%var2%\\%fnbksl%(%var3%)\\%var4%");
    push(@stream, "VSS_EXTRACT_CMD=ss.exe get ".
                                   "-GL\"%vsstrgdir%\" -GF- -I-Y ".
                                   "-W \"\$/%var3%\" -VL\"%var4%\"");
    push(@stream, "VSS_EXTRACT_TARGET=".
                  "%targ%\\%var2%\\%fnbksl%(%var3%)\\%var4%\\%fnfile%(%var1%)");
    return(@stream);
}

# -----------------------------------------------------------------------------
# Loads previously saved file information.
# -----------------------------------------------------------------------------
sub LoadFileInfo {
    my $self = shift;
    my $dir  = shift;

    if ( -e "$dir\\vss_files.dat" ) {
        our $FileData1;
        require "$dir\\vss_files.dat";
        $$self{'FILE_LOOKUP_TABLE'} = $FileData1;
    } else {
        ::status_message("No VSS information saved in $dir.\n");
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
    if ( open($fh, ">$dir\\vss_files.dat") ) {
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
print<<VSS_SIMPLE_USAGE;
Visual Source Safe specific settings:

     NAME            SWITCH      ENV. VAR        Default
  -----------------------------------------------------------------------------
  A) VSS Server      Server      SSDIR            <N/A>
  B) VSS Client      Client      SSROOT           <Current directory>
  C) VSS Project     Project     SSPROJECT        <N/A>
  D) VSS Label       Label       SSLABEL          <N/A>

VSS_SIMPLE_USAGE
}

# -----------------------------------------------------------------------------
# Verbose usage (-??)
# -----------------------------------------------------------------------------
sub VerboseUsage {
print<<VSS_VERBOSE_USAGE;
(A)  VSS Dir - The VSS server to use. It corresponds to the environment variable
     SSDIR which is used to execute ss.exe from the command line.

(B)  VSS Client - Points to the local directory that is the root of your source
     enlistment. This corresponds to \$/ (\$/<project> if the project option is
     used).

(C)  VSS Project - Corresponds to the VSS project that the source comes from. If
     the source comes from more than one project, the local directory names must
     match the VSS project names on the server.  If the source all comes from
     the same project, the project option tells the script to assume that the
     client directory corresponds to \$/<project> instead of \$/.

(D)  VSS Label - Causes the script to replace individual file versions with the
     given label.  The script does not verify that the label exists.
VSS_VERBOSE_USAGE
}

1;
__END__
