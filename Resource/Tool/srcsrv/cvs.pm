# -----------------------------------------------------------------------------
# cvs.pm
# 
# Copyright (c) 2006  Microsoft Corporation
#
# Source Server indexing module for CVS
# -----------------------------------------------------------------------------

package CVS;

require Exporter;
use strict;

our %EXPORT_TAGS = ( 'all' => [ qw() ] );
our @EXPORT_OK   = ( @{ $EXPORT_TAGS{'all'} } );
our @EXPORT      = qw();
our $VERSION     = '0.1';

use Data::Dumper;

# -----------------------------------------------------------------------------
# Simple subs to make it clear when we're testing for BOOL values
# -----------------------------------------------------------------------------
sub TRUE   {return(1);} # BOOLEAN TRUE
sub FALSE  {return(0);} # BOOLEAN FALSE

# -----------------------------------------------------------------------------
# Get the current time and date and put it into a string
# -----------------------------------------------------------------------------

sub CurrentTime
{
    my $sec;
    my $min;
    my $hour;
    my $mday;
    my $mon;
    my $year;
    my $wday;
    my $yday;
    my $isdst;
    
    ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst) = localtime(time);
    $year += 1900;
    
    return "$year-$mon-$mday $hour:$min:$sec";
}


# -----------------------------------------------------------------------------
# Create a new blessed reference that will maintain state for this instance of
# indexing
# -----------------------------------------------------------------------------
sub new {
    my $proto = shift;
    my $class = ref($proto) || $proto;
    my $self  = {};
    bless($self, $class);

    #
    # The command to use for talking to the server.  We don't allow this
    # to be overridden at the command line.
    #
    if ( defined $ENV{'CVS_CMD'} ) {
        $$self{'CVS_CMD'} = $ENV{'CVS_CMD'};
    } else {
        $$self{'CVS_CMD'} = "cvs.exe";
    }

    $$self{'CVS_LABEL'}     = $ENV{'CVS_LABEL'}
        if ( defined $ENV{'CVS_LABEL'} );

    $$self{'CVS_DATE'}     = $ENV{'CVS_DATE'}
        if ( defined $ENV{'CVS_DATE'} );

    $$self{'CVS_ROOT'}    = $ENV{'CVSROOT'}
        if ( defined $ENV{'CVSROOT'} );

    $$self{'CVS_DEBUGMODE'} = 0;

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
                        $$self{'CVS_CMD'}     = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "CMD");
                        $$self{'CVS_LABEL'}   = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "LABEL");
                        $$self{'CVS_DATE'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "DATE");
                        $$self{'CVS_NEWROOT'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "NEWROOT");
                        $$self{'CVS_OLDROOT'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "OLDROOT");
                        $$self{'CVS_ROOT'}  = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "ROOT");

                        # Remember this was unused
                        push(@unused_opts, $_);
                        1;
                    }

                # options that are just flags
                } else {
                    block: {
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

    $$self{'FILE_LOOKUP_TABLE'} = ();

    if ( ! defined $$self{'CVS_ROOT'} ) {
        $self = undef;
        ::fatal_error("CVS_ROOT not defined. Can't continue.\n");
    } else {
        chomp $$self{'CVS_ROOT'};
        if ($$self{'CVS_ROOT'} =~ m/;(username|login)=/i) {
            $self = undef;
            ::fatal_error("CVSROOT cannot contain reference to username or password.  Can't continue.\n");
        }
    }

    # The user must specify the date or label to match for - and cannot specify both.
    
    if ( defined $$self{'CVS_DATE'} ) {
        if ( defined $$self{'CVS_LABEL'} ) {
            ::fatal_error("Both CVS_LABEL and CVS_DATE are defined. Can't continue.\n");
        }
        # Reformat the date to be something we can generate a directory name with.
        $$self{'CVS_DATE_TARG'} = $$self{'CVS_DATE'};
        $$self{'CVS_DATE_TARG'} =~ s/\//-/g;
        $$self{'CVS_DATE_TARG'} =~ s/\s/_/g;
#       $$self{'CVS_DATE_TARG'} =~ s/:/h/;
#       $$self{'CVS_DATE_TARG'} =~ s/:/m/;
#       $$self{'CVS_DATE_TARG'} .= "s";
#       ::status_message("date targ = $$self{'CVS_DATE_TARG'}\n");
    } elsif (! defined $$self{'CVS_LABEL'} ) {
        ::fatal_error("CVS_LABEL or CVS_DATE not defined. Can't continue.\n");
    }

    return($self);
}


# -----------------------------------------------------------------------------
# Dump the file has table
# -----------------------------------------------------------------------------

sub DumpFileLookupTable
{
    my $self = shift;
    my $file_entry;

    foreach  $file_entry ( keys %{$$self{'FILE_LOOKUP_TABLE'}} )
    {
        print "Entry for: $file_entry\n";
        my $file_entry_ref = ${$$self{'FILE_LOOKUP_TABLE'}}{$file_entry};
        foreach (keys %{$file_entry_ref->[0]} )
        {
            print "  Server: $_ --> $file_entry_ref->[0]->{$_}\n";
        }
        print "  Line: $file_entry_ref->[1]\n";
        print "\n";
    }
}

# -----------------------------------------------------------------------------
# Display module internal option state.
# -----------------------------------------------------------------------------
sub DisplayVariableInfo {
    my $self = shift;

    ::status_message("%-15s: %s\n",
                     "CVS Root",
                     $$self{'CVS_ROOT'});

    ::status_message("%-15s: %s\n",
                     "CVS program name",
                     $$self{'CVS_CMD'});

    ::status_message("%-15s: %s\n",
                    "CVS Label",
                    $$self{'CVS_LABEL'} ? $$self{'CVS_LABEL'}      : "<N/A>");

    ::status_message("%-15s: %s\n",
                    "CVS Date",
                    $$self{'CVS_DATE'} ? $$self{'CVS_DATE'}      : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "Old path root",
                     $$self{'CVS_OLDROOT'} ? $$self{'CVS_OLDROOT'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "New path root",
                     $$self{'CVS_NEWROOT'} ? $$self{'CVS_NEWROOT'} : "<N/A>");

}

# -----------------------------------------------------------------------------
# Execute a command and return a handle to the output.
# -----------------------------------------------------------------------------
sub ExecCmd
{
    my $self = shift;
    my $dir = shift;
    my $cmd = shift;
    
    if ( defined $$self{'SHOWCMDS'} && $$self{'SHOWCMDS'} )
    {
        ::status_message("[CWD: $dir] Running \"$cmd\"\n");
    }

    return ::ScalarOpen("$cmd 2>NUL|");
}


# -----------------------------------------------------------------------------
# Given our init data and a local source path, create a lookup table that can
# return individual stream data for each source file.
# -----------------------------------------------------------------------------
sub GatherFileInformationByDate {
    my $self       = shift;
    my $SourceRoot = shift;
    my $ServerRefs = shift;
    my %FileLookup;
    my $hProc;

    my $dir;
    my $local;
    my $remote;
    my $revision;
    my $active = 0;
    
    $hProc = ::ScalarOpen("cvs.exe status -v 2>&1|");
    
    while (<$hProc>)
    {
        # Is this a new directory?
        
        ::status_message("$_");
#        if (m/^cvs\sstatus:\sExamining:/i)
#        {
#            $dir = m/^cvs\sstatus:\sExamining:\s(.*)/i;
#            ::status_message("dir=$dir\n");
#        }
        next;
    }

    close($hProc);
    
    #DumpFileLookupTable($self);
}

# -----------------------------------------------------------------------------
# Given our init data and a local source path, create a lookup table that can
# return individual stream data for each source file.
# -----------------------------------------------------------------------------
sub GatherFileInformation {
    my $self       = shift;
    my $SourceRoot = shift;
    my $ServerRefs = shift;
    my %FileLookup;
    my $hProc;

    my %DirectoryToRepository;

    # Remember the current directory
    my $PrevDir    = `cd`;
    chomp $PrevDir;

    chdir($SourceRoot);

    open($hProc, "dir /s/b/a-d 2>NUL|");

    my $ThisFile;

    while ( $ThisFile = <$hProc> ) {
        chomp $ThisFile;

        # Skip the CVS informational directories. We'll consume those as we
        # find  actual files that we care about.
        next if ( $ThisFile =~ m/\\cvs\\[^\\*]$/i );

        my $ThisDir  = substr($ThisFile, 0, rindex($ThisFile, "\\"));
        my $cvs  = "$ThisDir\\CVS";

        if ( ! defined $DirectoryToRepository{uc $ThisDir} ) {
            if (  -e "$cvs\\Root" && ! -d "$cvs\\Root" ) {
                my $root = GetFirstFileLine("$cvs\\Root");

                if ( $root =~ /^\Q$$self{'CVS_ROOT'}\E$/i ) {

                    if ( defined $ServerRefs->{uc $root} ) {
                        my $repository = GetFirstFileLine("$cvs\\Repository");

                        if ( defined $repository ) {
                            push( @{$DirectoryToRepository{uc $ThisDir}},
                                  ($repository, $ServerRefs->{uc $root}, $root));
                        }
                    } else {
                        # Skipping server w/o var.
                    }
                }
            }
        }

        if ( defined $DirectoryToRepository{uc $ThisDir} ) {
            my $Repository   = ${$DirectoryToRepository{uc $ThisDir}}[0];
            my $ServerVar    = ${$DirectoryToRepository{uc $ThisDir}}[1];
            my $Server       = ${$DirectoryToRepository{uc $ThisDir}}[2];
            my $FilenameOnly = substr($ThisFile, rindex($ThisFile, "\\") + 1);

            my $h;
            my $line;

            if ( open($h, "$cvs\\Entries") ) {
                while ( $line=<$h> ) {
                    chomp $line;

                    if ( $line =~ /^\/\Q$FilenameOnly\E\/([\d\.]*)\//i ) {
                        my $Revision = $1;

                        # Add the information for this file to the
                        # FILE_LOOKUP_TABLE that will be referenced when
                        # SSIndex calls out GetFileInfo() function.
                        @{$$self{'FILE_LOOKUP_TABLE'}{lc $ThisFile}} =
                            ( {$ServerVar=>$Server},
                              "$ServerVar*$Repository/$FilenameOnly",
                              $Revision);
                        last;
                    }
                }
                close($h);
            }
        }
    }

    close($hProc);
    chdir($PrevDir);
    
    #DumpFileLookupTable($self);
}

# -----------------------------------------------------------------------------
# Return the SRCSRV stream data for a single file.
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
        if ( defined $$self{CVS_LABEL} && $$self{CVS_LABEL} ne "" ) {
            return( ${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[0],
                   "$name_in_pdb*${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[1]" );
        } else {
            return( ${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[0],
                   "$name_in_pdb*${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[1]".
                   "*${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[2]");
        }

    } else {
        return(undef);
    }
}

# -----------------------------------------------------------------------------
# Return the first line of a file or undef.
# -----------------------------------------------------------------------------
sub GetFirstFileLine {
    my $File = shift;
    my $h;

    if ( open($h, "$File") ) {
        my $Line = <$h>;
        chomp $Line;
        close($h);

        if ( uc $Line ne "" ) {
            return("$Line");
        } else {
            return(undef);
        }
    } else {
        return(undef);
    }
}

# -----------------------------------------------------------------------------
# The long name that should be written the SRCSRV stream to describe
# the source control system being indexed.
# -----------------------------------------------------------------------------
sub LongName {
    return("CVS");
}

# -----------------------------------------------------------------------------
# Set the debug level for output.
# -----------------------------------------------------------------------------
sub SetDebugMode {
    my $self = shift;
    $$self{'CVS_DEBUGMODE'} = shift;
}

# -----------------------------------------------------------------------------
# Return the SCS specific stream variables.
# -----------------------------------------------------------------------------
sub SourceStreamVariables {
    my $self = shift;
    my @stream;

    if ( defined $$self{CVS_DATE} && $$self{CVS_DATE} ne "" ) {
        
        push(@stream, "CVSDATE=$$self{CVS_DATE}");
        push(@stream, "CVSDATETARG=$$self{CVS_DATE_TARG}");
        push(@stream, "CVS_EXTRACT_CMD=%fnchdir%(%CVS_WORKINGDIR%)".
                                       "$$self{'CVS_CMD'} -d %fnvar%(%var2%) ".
                                       "export ".
                                       "-n -D \"%cvsdate%\" ".
                                       "-d %cvsdatetarg% " .
                                       "%var3%");
        push(@stream, "CVS_EXTRACT_TARGET=".
                      "%targ%\\%var2%\\%fnbksl%(%var3%)\\%cvsdatetarg%".
                      "\\%fnfile%(%var1%)");
    
    } elsif ( defined $$self{CVS_LABEL} && $$self{CVS_LABEL} ne "" ) {
    
        push(@stream, "CVSLABELTAG=$$self{CVS_LABEL}");
        push(@stream, "CVS_EXTRACT_CMD=%fnchdir%(%CVS_WORKINGDIR%)".
                                       "$$self{'CVS_CMD'} -d %fnvar%(%var2%) ".
                                       "export ".
                                       "-n -r %cvslabeltag% ".
                                       "-d %cvslabeltag% " .
                                       "%var3%");
        push(@stream, "CVS_EXTRACT_TARGET=".
                      "%targ%\\%var2%\\%fnbksl%(%var3%)\\%CVSLABELTAG%".
                      "\\%fnfile%(%var1%)");
    
    } else {
    
        push(@stream, "CVS_EXTRACT_TARGET=".
                      "%targ%\\%var2%\\%fnbksl%(%var3%)\\%var4%\\%fnfile%(%var1%)");

        push(@stream, "CVS_EXTRACT_CMD=$$self{'CVS_CMD'} -d %fnvar%(%var2%) ".
                      "co -r %var4% %var3% > %srcsrvtarg%");
    
    }

    push(@stream, "CVS_WORKINGDIR=%targ%\\%var2%\\%fnbksl%(%var3%)");

    push(@stream, "SRCSRVVERCTRL=CVS");
    push(@stream, "SRCSRVERRDESC=No such file or directory");
    push(@stream, "SRCSRVERRVAR=var2");


    return(@stream);
}

# -----------------------------------------------------------------------------
# Loads previously saved file information.
# -----------------------------------------------------------------------------
sub LoadFileInfo {
    my $self = shift;
    my $dir  = shift;

    if ( -e "$dir\\CVS_files.dat" ) {
        our $FileData1;
        require "$dir\\CVS_files.dat";
        $$self{'FILE_LOOKUP_TABLE'} = $FileData1;
    } else {
        ::status_message("No CVS information saved in $dir.\n");
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
    if ( open($fh, ">$dir\\CVS_files.dat") ) {
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
print<<CVS_SIMPLE_USAGE;
Source Depot specific settings:

     NAME            SWITCH      ENV. VAR        Default
  -----------------------------------------------------------------------------
  A) CVS command    CMD         CVS_CMD          cvs.exe
  B) CVS Root       Root        CVSROOT          <n/a>
  C) label          Label       CVS_LABEL        <n/a>
  D) date           Date        CVS_DATE
  E) old root       OldRoot     <n/a>            <n/a>
  F) new root       NewRoot     <n/a>            <n/a>
CVS_SIMPLE_USAGE
}

# -----------------------------------------------------------------------------
# Verbose usage ('-??')
# -----------------------------------------------------------------------------
sub VerboseUsage {
print<<CVS_VERBOSE_USAGE;
(A)  CVS Command - The name of the executable to run to issue commands to the
     Source Depot server.  The executable named here must support the same
     options as cvs.exe in order for the script to work correctly.

(B)  CVS Root - REQUIRED. The root of the CVS server.

(C)  Label - Use the given text as the file revision label to extract source.

(D)  Date - Use the given text as the file revision date to extract source
     using instead of extracting the source using the revision label.

(E)  Old Root and New Root - Allows the source indexing of symbols that were
     built on another machine.  If these are set, every source path that is
     prefixed with Old Root with have that prefix replaced with the value in
     New Root prior to attempting to resolve the local path and filename to a
     server path and filename. Both Old Root and New Root must be specified to
     use this feature.

(F)  New Root - See Old Root above.
CVS_VERBOSE_USAGE
}

1;
__END__
