@rem = '-*- Perl -*-';
@rem = '
@REM  -------------------------------------------------------------------------
@REM
@REM  SSIndex.cmd
@REM    Works with individual source indexing modules to add source
@REM    server information to debug files.
@REM
@REM  Copyright (c) Microsoft Corporation. All rights reserved.
@REM
@REM  -------------------------------------------------------------------------
@REM Make sure the required tools are in the path
@setlocal ENABLEEXTENSIONS
@set PATH=%PATH%;%~dp0;%~dp0%PROCESSOR_ARCHITECTURE%
@perl -w -x %SRCSRV_PERL_DEBUG_FLAGS% "%~f0" %*
@REM Not using -W because some dependent modules redefine their own funcs.
@exit /b %errorlevel%
@goto :EOF
';
#!perl
#line 22

use strict;
use Data::Dumper;
use Fcntl qw(O_CREAT O_EXCL O_RDWR);

$| = 1; # autoflush

#
# Simple subs to make it clear when we're testing for BOOL values
#
sub TRUE   {return(1);} # BOOLEAN TRUE
sub FALSE  {return(0);} # BOOLEAN FALSE

#
# Store just the path to the script for use by require() below. Use
# 'our' so variable is available to modules.
#
my $ScriptPath = $0;
$ScriptPath    =~ /^(.*\\)[^\\]*$/;
if ( defined $1 ) {
    $ScriptPath    = $1;
} else {
    $ScriptPath = `cd`;
    chomp $ScriptPath;
    $ScriptPath .= "\\" if (substr($ScriptPath, -1) ne "\\");
}

#
# Truncate $0 to be just a filename. Use 'our' so the variable
# is available to modules.
#
my $ScriptName = $0;
$ScriptName    =~ /.*\\(.*)$/;
$ScriptName    =$1 if (defined($1));

# Play with the sig handlers so output is prepended with our script name
# and is easily parsable by a regexp. We can't always restore using 'DEFAULT',
# so store the old handlers somewhere we can find them again.
$SIG{__DIE__}       = \&fatal_error;
$SIG{__WARN__}      = \&warn_message;
# -----------------------------------------------------------------------------
# SSIndex version
# -----------------------------------------------------------------------------

my $Version = 1;

# -----------------------------------------------------------------------------
# Global hash for control options - default values.
# -----------------------------------------------------------------------------

# These options that are common to all modules
my %Options;

# These all default to the current directory
$Options{'SRCSRV_CURDIR'}  = `cd`;
chomp $Options{'SRCSRV_CURDIR'};

$Options{'SRCSRV_SOURCE'}  = $Options{'SRCSRV_CURDIR'};
$Options{'SRCSRV_SYMBOLS'} = $Options{'SRCSRV_CURDIR'};

# Setup the path to the default srcsrv.ini we'll look for
$Options{'SRCSRV_INI'}     = $Options{'SRCSRV_CURDIR'};
$Options{'SRCSRV_INI'}    .= "\\" if ( substr($Options{'SRCSRV_INI'}, -1, 1) ne "\\" );
$Options{'SRCSRV_INI'}    .= "srcsrv.ini";

# If a srcsrv.ini doesn't exist in the current directory, set this to the
# directory the script was started from.  We don't verify the existance of
# that file here because we don't want to error out until the user has had
# a chance to override the value from the command line.
if ( ! -e $Options{'SRCSRV_INI'} ) {
    $Options{'SRCSRV_INI'} = $0;
    substr($Options{'SRCSRV_INI'}, rindex($Options{'SRCSRV_INI'}, "\\")) = "";
    $Options{'SRCSRV_INI'} .= "\\srcsrv.ini";
}

$Options{'SRCSRV_DEBUG'}      = 0;
$Options{'SRCSRV_DIEONERROR'} = 0; # Abort on otherwise non-fatal warnings/errors

# $Options{SRCSRV_SYSTEM}, $Options{SRCSRV_HELP}, $Options{SRCSRV_VERBOSE_HELP}
# are all intentionally left undefined at this point.  The first can be set
# from the command line or in the environment.  The last two are only used if
# the corresponding command line options are used.

# -----------------------------------------------------------------------------
# Global hash for control options - allow environment variable overrides
# -----------------------------------------------------------------------------
set_debug_level($ENV{'SRCSRV_DEBUG'})
    if (defined $ENV{'SRCSRV_DEBUG'});

$Options{'SRCSRV_INI'}     = $ENV{'SRCSRV_INI'}
    if (defined $ENV{'SRCSRV_INI'});

$Options{'SRCSRV_SOURCE'}  = $ENV{'SRCSRV_SOURCE'}
    if (defined $ENV{'SRCSRV_SOURCE'});

$Options{'SRCSRV_SYMBOLS'} = $ENV{'SRCSRV_SYMBOLS'}
    if (defined $ENV{'SRCSRV_SYMBOLS'});

$Options{'SRCSRV_SYSTEM'} = uc $ENV{'SRCSRV_SYSTEM'}
    if (defined $ENV{'SRCSRV_SYSTEM'});

$Options{'SRCSRV_SAVE'} = uc $ENV{'SRCSRV_SAVE'}
    if (defined $ENV{'SRCSRV_SAVE'});

$Options{'SRCSRV_LOAD'} = uc $ENV{'SRCSRV_LOAD'}
    if (defined $ENV{'SRCSRV_LOAD'});

# -----------------------------------------------------------------------------
# Code block to parse the command line parameters
# -----------------------------------------------------------------------------
PARSECOMMANDLINE: {
    my @opt;
    my @unused_argv; # to restore @ARGV for modules to parse

    foreach (@ARGV) {
        # handle command options
        if (substr($_, 0, 1) =~ /^[\/-]$/) {
            # options that set values
            if ( (@opt = split(/=/, $_))==2 ) {
                block: {
                    # -ALLROOT=<dir>
                    if ( uc substr($opt[0], 1) eq "ALLROOT"  ) {
                        $Options{'SRCSRV_SOURCE'}  = $opt[1];
                        $Options{'SRCSRV_SYMBOLS'} = $opt[1];
                        last;
                    }

                    # -OLDROOT=<Path>
                    $Options{'SRCSRV_OLDROOT'}  = $opt[1],
                        push(@unused_argv, $_), # Needed for legacy support
                        last
                        if ( uc substr($opt[0], 1) eq "OLDROOT");

                    # -NEWROOT=<Path>
                    $Options{'SRCSRV_NEWROOT'}    = $opt[1],
                        push(@unused_argv, $_), # Needed for legacy support
                        last
                        if ( uc substr($opt[0], 1) eq "NEWROOT");

                    # -INI=<file>
                    $Options{'SRCSRV_INI'}    = $opt[1], last
                        if ( uc substr($opt[0], 1) eq "INI");

                    # -LOAD=<dir>
                    $Options{'SRCSRV_LOAD'} = $opt[1], last
                        if ( uc substr($opt[0], 1) eq "LOAD");

                    # -SAVE=<dir>
                    $Options{'SRCSRV_SAVE'} = $opt[1], last
                        if ( uc substr($opt[0], 1) eq "SAVE");

                    # -SOURCE=<dir>
                    $Options{'SRCSRV_SOURCE'} = $opt[1], last
                        if ( uc substr($opt[0], 1) eq "SOURCE");

                    # -SYMBOLS=<dir>
                    $Options{'SRCSRV_SYMBOLS'}= $opt[1], last
                        if ( uc substr($opt[0], 1) eq "SYMBOLS");

                    # -SYSTEM=<string> (source control system to use)
                    $Options{'SRCSRV_SYSTEM'} = uc $opt[1], last
                        if ( uc substr($opt[0], 1) eq "SYSTEM");

                    # options to pass to modules
                    push(@unused_argv, $_);
                    1;
                }
            # options that are just flags
            } else {
                block: {
                    # -DEBUG[:<level>]
                    set_debug_level($_), last
                        if ( uc substr($_, 1, 5) eq "DEBUG");

                    # -DIEONERROR
                    $Options{'SRCSRV_DIEONERROR'} = 1,  last
                        if ( uc substr($_, 1, 10) eq "DIEONERROR");

                    # -QUIET (shortcut to -DEBUG:-1)
                    set_debug_level($_),  last
                        if ( uc substr($_, 1, 5) eq "QUIET");

                    # -?, -h
                    $Options{'SRCSRV_HELP'} = TRUE, last
                        if ( uc substr($_, 1)    eq "?");
                    $Options{'SRCSRV_HELP'} = TRUE, last
                        if ( uc substr($_, 1)    eq "h");

                    # -??
                    $Options{'SRCSRV_HELP'}         = TRUE,
                    $Options{'SRCSRV_VERBOSE_HELP'} = TRUE, last
                        if ( uc substr($_, 1)    eq "??");

                    # options to pass to modules
                    push(@unused_argv, $_);
                    1;
                }
            }
        # non-option parameters
        } else {
            # options to pass to modules
            push(@unused_argv, $_);
        }
    }

    # restore @ARGV for module use
    @ARGV = @unused_argv;

} # end PARSECOMMANDLINE

#
# First, see if help was invoked.  If so, go straight there...
#
if ( defined $Options{'SRCSRV_HELP'}       ||
     defined $Options{'SRCSRV_VERBOSE_HELP'} ) {
    # The second parameter may be an undef, but Usage() knows that.
    Usage(defined $Options{'SRCSRV_VERBOSE_HELP'}, $Options{'SRCSRV_SYSTEM'});
    exit(0);
}

#
# Pre-module option validation.  Make sure we have the information we need
# to even call out to the support modules.
#
if ( ! defined $Options{'SRCSRV_SYSTEM'} ) {
    fatal_error("A source control system must be specified using either the\n".
                "\t\"-SYSTEM=<string>\" option or by defining SRCSRV_SYSTEM in".
                " your environment.");
}

#
# Try to include the source control system module.
#
eval {  my $DIE_HANDLER = $SIG{__DIE__};
        $SIG{__DIE__} = 'DEFAULT'; # don't want out handler to be called
                                   # during eval{}
        require "$ScriptPath\\$Options{'SRCSRV_SYSTEM'}.pm";
        $SIG{__DIE__} = $DIE_HANDLER;
     };
fatal_error("Unable to find/load module for \"$Options{'SRCSRV_SYSTEM'}\"") if ( $@ );

#
# Create a blessed reference to the SRCSRV_SYSTEM.  One last eval in an attempt
# to be certain we're getting the proper module.  After this, we'll assume that
# everything is good and just rely on our die() handler to catch errors.
#
eval {  my $DIE_HANDLER = $SIG{__DIE__};
        $SIG{__DIE__} = 'DEFAULT'; # don't want out handler to be called
                                   # during eval{}
        $Options{'SRCSRV_PROVIDER'} = $Options{'SRCSRV_SYSTEM'}->new();
        $SIG{__DIE__} = $DIE_HANDLER;
     };
fatal_error("Unable to initialize \"$Options{'SRCSRV_SYSTEM'}\" provider.") if ( $@ );

#
# Now that we and the module we're using are both initialized, we can finally
# warn the user about ignored options.
#
if (@ARGV) {
    warn_message("Command line option \"$_\" is unrecognized.") foreach (@ARGV);
}

#
# Verify the expected interfaces exist
#
if (! $Options{'SRCSRV_PROVIDER'}->can( "GatherFileInformation" ) ) {
    die("Expected interface \"GatherFileInformation\" not found for module $Options{'SRCSRV_SYSTEM'}\n");
}

if ( ! $Options{'SRCSRV_PROVIDER'}->can( "GetFileInfo" ) ) {
    die("Expected interface \"GetFileInfo\" not found for module $Options{'SRCSRV_SYSTEM'}\n");
}

if ( ! $Options{'SRCSRV_PROVIDER'}->can( "SourceStreamVariables" ) ) {
    die("Expected interface \"SourceStreamVariables\" not found for module $Options{'SRCSRV_SYSTEM'}\n");
}

if ( ! $Options{'SRCSRV_PROVIDER'}->can( "LongName" ) ) {
    die("Expected interface \"LongName\" not found for module $Options{'SRCSRV_SYSTEM'}\n");
}

if ( defined $Options{'SRCSRV_LOAD'} ) {
    if ( ! $Options{'SRCSRV_PROVIDER'}->can( "LoadFileInfo" ) ) {
        die("Module $Options{'SRCSRV_SYSTEM'} doesn't support the -LOAD=<dir> option.\n");
    }
}

if ( defined $Options{'SRCSRV_SAVE'} ) {
    if ( ! $Options{'SRCSRV_PROVIDER'}->can( "SaveFileInfo" ) ) {
        die("Module $Options{'SRCSRV_SYSTEM'} doesn't support the -SAVE=<dir> option.\n");
    }
}

#
# Display var. status in debug mode.
#
print('-'x80, "\n");
status_message("%-15s: %s\n", "Server ini file", $Options{'SRCSRV_INI'}     );
status_message("%-15s: %s\n", "Source root",     $Options{'SRCSRV_SOURCE'}  );
status_message("%-15s: %s\n", "Symbols root",    $Options{'SRCSRV_SYMBOLS'} );
status_message("%-15s: %s\n", "Control system",  $Options{'SRCSRV_SYSTEM'}  );
status_message("%-15s: %s\n", "LoadFile dir",    $Options{'SRCSRV_LOAD'}  )
    if ( defined $Options{'SRCSRV_LOAD'} );
status_message("%-15s: %s\n", "SaveFile dir",    $Options{'SRCSRV_SAVE'}  )
    if ( defined $Options{'SRCSRV_SAVE'} );

#
# If the SCS module supports overriding the script version, do it.
#

if ( $Options{'SRCSRV_PROVIDER'}->can( "GetScriptVersion" ) ) {
    my $ModuleVersion = $Options{'SRCSRV_PROVIDER'}->GetScriptVersion();
    $Version = $ModuleVersion if ($ModuleVersion > $Version);
}

#
# If the SCS module support displaying variables, ask it to do so.
#
if ( $Options{'SRCSRV_PROVIDER'}->can( "DisplayVariableInfo" ) ) {
    $Options{'SRCSRV_PROVIDER'}->DisplayVariableInfo();
}

#
# Let the SCS module know what the debug level. Not all modules are required to support
# this interface.
#
if ( $Options{'SRCSRV_DEBUG'} ) {
    if ( $Options{'SRCSRV_PROVIDER'}->can( "SetDebugMode" ) ) {
        $Options{'SRCSRV_PROVIDER'}->SetDebugMode( $Options{'SRCSRV_DEBUG'} );
    }
}
print('-'x80, "\n");
status_message("Running... this will take some time...   ");

#
# Declare %ServerVars as an empty hash initially since we want to
# do the optional load before we parse the declared ini file.
#
my %ServerVars = ();
my %ExcludeInfo = ();

#
# If we're doing a load, see if there's a servers.dat file to
# read in.
#
if ( defined $Options{'SRCSRV_LOAD'} ) {
    if ( -e "$Options{'SRCSRV_LOAD'}" && -d _ ) {

        if ( -e "$Options{'SRCSRV_LOAD'}\\servers.dat" ) {
            my $ServerData1;
            require "$Options{'SRCSRV_LOAD'}\\servers.dat";

            foreach (sort keys %$ServerData1) {
                $ServerVars{$_} = $$ServerData1{$_};
            }
            undef($ServerData1);

        } else {
            ::status_message("Failed to load data from $Options{'SRCSRV_LOAD'}.\n");
        }

        $Options{'SRCSRV_PROVIDER'}->LoadFileInfo($Options{'SRCSRV_LOAD'});
    } else {
        die("Load directory \"$Options{'SRCSRV_LOAD'}\" doesn't exist.\n");
    }
}

#
# Find new servers overriding previously defined ones in case of
# conflict.
#
%ServerVars  = (%ServerVars, LoadServerVars("$Options{'SRCSRV_INI'}"));
%ExcludeInfo = LoadExcludeInfo("$Options{'SRCSRV_INI'}");

#
# Ask the source provider to find the source information by processing
# each source root.
#
my @SourceRoots = split(/;/, $Options{'SRCSRV_SOURCE'});
foreach (@SourceRoots) {
    s/^\s*(.*)\s*$/$1/;
    $Options{'SRCSRV_PROVIDER'}->GatherFileInformation($_, \%ServerVars, \%ExcludeInfo);
}

#
# If state saving was requested, do it now.  No reason to wait until the
# declared symbols paths are processed.
#
if ( defined $Options{'SRCSRV_SAVE'} ) {
    if ( ! -d "$Options{'SRCSRV_SAVE'}" ) {
        mkdir("$Options{'SRCSRV_SAVE'}", 0x777);

        if ( ! -d "$Options{'SRCSRV_SAVE'}" ) {
            die("Unable to create directory \"$Options{'SRCSRV_SAVE'}\": $!\n");
        }
    }

    my $fh;

    if ( $fh=ScalarOpen(">$Options{'SRCSRV_SAVE'}\\servers.dat") ) {
        $Data::Dumper::Varname = "ServerData";
        $Data::Dumper::Indent  = 0;
        print $fh Dumper(\%ServerVars);
        close($fh);
        $Options{'SRCSRV_PROVIDER'}->SaveFileInfo($Options{'SRCSRV_SAVE'});
    } else {
        ::info_message("Failed to save server data.");
    }

}

#
# Process each symbols root
#
my @SymbolRoots = split(/;/, $Options{'SRCSRV_SYMBOLS'});
foreach (@SymbolRoots) {
    s/^\s*(.*)\s*$/$1/;
    RecurseDirectoryTree( $_,
                          \&IndexThisPdb,
                          \%Options);
}

###############################################################################
#
# Subroutines
#
###############################################################################

# -----------------------------------------------------------------------------
# Filter out non-source files to help reduce hash size ...
# -----------------------------------------------------------------------------
sub ExcludeFile {
    my $file        = shift;
    my $ExcludesRef = shift;

    if ( defined $ExcludesRef ) {

        if ( defined $ExcludesRef->{'FILES'} ) {
            my $name = lc(substr($file, rindex($file, "/") + 1));
            foreach my $iterator ( @{$ExcludesRef->{'FILES'}} ) {
                if ( $name eq $iterator ) {
                    return TRUE;
                }
            }
        }

        if ( defined $ExcludesRef->{'EXTENSIONS'} ) {
            my $ext = lc(substr($file, rindex($file, ".")  + 1));
            foreach my $iterator ( @{$ExcludesRef->{'EXTENSIONS'}} ) {
                if ( $ext eq $iterator ) {
                    return TRUE;
                }
            }
        }
    }

    return FALSE;
}

# -----------------------------------------------------------------------------
# Index a single PDB
# -----------------------------------------------------------------------------
sub IndexThisPdb {
    my $pdb       = shift;
    my $context   = shift;
    my @files     = ();
    my $filecount = 0;
    my %FileVariables;

    # Don't index non-PDB files
    return unless (! -d $pdb && $pdb =~ /\.pdb$/i );

    # DIA doesn't like 0-length PDBs.  Since there can't be source in such a
    # beast anyhow, just skip it.
    if ( -z $pdb ) {
        warn_message("Skipping 0-length PDB: $pdb");
        return();
    }

    # If we're in verbose mode ...
    info_message("... indexing $pdb");

    my @stream;
    # Add common stream information
    push(@stream, "SRCSRV: ini ------------------------------------------------");
    push(@stream, "VERSION=$Version");
    push(@stream, "INDEXVERSION=2");
    push(@stream, "VERCTRL=".${$context}{'SRCSRV_PROVIDER'}->LongName());
    push(@stream, "DATETIME=".scalar localtime);
    push(@stream, "SRCSRV: variables ------------------------------------------");

    # Get the variables specific to this source control system
    push(@stream, $Options{'SRCSRV_PROVIDER'}->SourceStreamVariables());

    # Iterate the source files for the PDB
    my @srcfiles = `srctool.exe \"$pdb\" -r`;
    foreach (@srcfiles) {
        chomp;
        my $LocalLookupPath = $_;
        # Translate *nix-style paths to DOS-style paths.
        $LocalLookupPath =~ s/\//\\/g;

        # Remove duplicate '\'
        $LocalLookupPath =~ s/\\\\/\\/g;

        # Remove leading ".\" if present
        $LocalLookupPath =~ s/^\.\\//;

        # If the path was a UNC, the above s/// will cause it to
        # be wrong.  Recover from that here.
        if ( $LocalLookupPath =~ /^\\/ ) {
            $LocalLookupPath =~ s/^\\/\\\\/;
        }

        if ( defined $Options{'SRCSRV_OLDROOT'} && defined $Options{'SRCSRV_NEWROOT'}) {
            $LocalLookupPath =~ s/^\Q$Options{'SRCSRV_OLDROOT'}\E/$Options{'SRCSRV_NEWROOT'}/i;
        }

        # Ask the provider to give us the extraction information
        my @FileInfo = ${$context}{'SRCSRV_PROVIDER'}->GetFileInfo($LocalLookupPath, $_);

        # If it did, remember the information
        if (@FileInfo && defined $FileInfo[0]) {
            $filecount++;
            # Change all "%" to "%%" so srvsrv.dll knows it is a literal "%"
            $FileInfo[1] =~ s/\%/\%\%/g;
            push(@files, $FileInfo[1]);

            # Push each var into a hash so we end up with a non-duplicate
            # list of file-specific variables.
            my $loop;
            foreach  $loop (keys %{$FileInfo[0]}) {
                $FileVariables{$loop} = ${$FileInfo[0]}{$loop};
            }
        }
    }

    # Add the file-specific variables
    foreach (sort keys %FileVariables) {
        push(@stream, "$_=$FileVariables{$_}");
    }

    # Add the final ini-section information
    push(@stream, "SRCSRVTRG=%$Options{'SRCSRV_SYSTEM'}_extract_target%");

    if ( grep(/$Options{'SRCSRV_SYSTEM'}_extract_cmd/i, @stream) ) {
        push(@stream, "SRCSRVCMD=%$Options{'SRCSRV_SYSTEM'}_extract_cmd%");
    }

    push(@stream, "SRCSRV: source files ---------------------------------------");

    # Add the files we found
    foreach (@files) {
        push(@stream, "$_");
    }

    # Write the end of the stream
    push(@stream, "SRCSRV: end ------------------------------------------------");

    # If any files where found for this pdb, get a temp file, write the stream to it,
    # then push the stream into the pdb and delete the temp file.
    if ( $filecount > 0 ) {
        my ($fh, $filename) = GetTempFile();

        foreach (@stream) { print $fh "$_\n"; }
        close($fh);
        my $result = `pdbstr -w -s:srcsrv -p:\"$pdb\" -i:\"$filename\"`;
        chomp($result);
        if ( $result =~ /error/i ) {
            warn_message("... error writing $filename to $pdb ($result)");
        } else {
            info_message("... wrote $filename to $pdb ...");
        }
        unlink("$filename");
    } else {
        info_message("... zero source files found ...");
    }

    return;
}

# -----------------------------------------------------------------------------
# Return a hash that maps a server string to a token var.
# -----------------------------------------------------------------------------
sub LoadServerVars {
    my $file = shift;
    my $fh;

    my %Mappings;

    if ( ! ($fh = ScalarOpen("$file")) ) {
        fatal_error("Unable to open \"$file\": $!");
    }

    # Skip everything before the '[variables]' block
    my $line;
    while ( defined($line = <$fh>) && $line !~ m/^\s*\[variables\]/i ) { ; }

    # Now, keep reading until we hit EOF or a new section marker
    while ( defined($line = <$fh>) && ($line !~ m/^\s*\[.*\]/i) ) {
        chomp $line;
        # remove comments
        next if ($line =~ m/^\s*;/);
        # skip blank lines
        next if ($line =~ /^\s*$/);
        $line =~ s/^\s*//;
        $line =~ s/\s*$//;

        my ($Token,$Server) = split(/\s*=\s*/, $line, 2);

        if ( defined($Token) && defined($Server) ) {
            # Record Server->Variable pair
            if ( ! defined $Mappings{uc $Server} ) {
                $Mappings{uc $Server} = $Token;
            }
        }
    }

    close($fh);

    return(%Mappings);
}

# -----------------------------------------------------------------------------
# Return a hash that contains filename and extension exclusions.
# -----------------------------------------------------------------------------
sub LoadExcludeInfo {
    my $file = shift;
    my $fh;

    my %Mappings;

    if ( ! ($fh = ScalarOpen("$file")) ) {
        fatal_error("Unable to open \"$file\": $!");
    }

    # Skip everything before the '[SourceControlIndexExcludes]' block
    my $line;
    while ( defined($line = <$fh>) && $line !~ m/^\s*\[SourceControlIndexExcludes\]/i ) { ; }

    # Now, keep reading until we hit EOF or a new section marker
    while ( defined($line = <$fh>) && ($line !~ m/^\s*\[.*\]/i) ) {
        chomp $line;
        # remove comments
        next if ($line =~ m/^\s*;/);
        # skip blank lines
        next if ($line =~ /^\s*$/);
        $line =~ s/^\s*//;
        $line =~ s/\s*$//;

        my ($Type,$Data) = split(/\s*=\s*/, $line, 2);
        my @Entries = split(/:/, $Data);

        foreach (@Entries) {
            push(@{$Mappings{uc $Type}}, lc $_);
        }
    }

    close($fh);

    return(%Mappings);
}

# -----------------------------------------------------------------------------
# Walk a dir tree calling $Function for each entry (except . and ..) found.
# -----------------------------------------------------------------------------
sub RecurseDirectoryTree {
    my $TreeRoot = shift;
    $TreeRoot    =~ s/\\$//;  # cut possible trailing '\'

    my $Function = shift;
    my $Context = shift;

    my $file;
    my $hDIR = ScalarOpenDir("$TreeRoot");

    if ( ! $hDIR ) {
        warn_message("Failed to open directory \"$TreeRoot\". Skipping it.");
        return(0);
    }

    # Process every entry
    while ( $file = readdir($hDIR) ) {
        next if ($file =~ m/^\.{1,2}$/); # skip . and ..
        $file = "$TreeRoot\\$file";      # full path and file
        &$Function("$file", $Context);   # call routine

        # recurse if it's a directory
        RecurseDirectoryTree("$file", \&$Function, $Context) if (-d "$file");
    }
    closedir($hDIR); # done

    return; # no return value expected
}

# -----------------------------------------------------------------------------
# Display script help
# -----------------------------------------------------------------------------
sub Usage {
    # Don't need/want special SIG handlers for displaying this usage
    $SIG{__DIE__}  = 'Default';
    $SIG{__WARN__} = 'Default';

    my $VerboseHelp  = shift;
    my $SourceSystem = shift;

    if ( defined $SourceSystem ) {
        eval { require "$ScriptPath\\${SourceSystem}.pm"; };
        fatal_error("No \"$SourceSystem\" module found.") if ($@);
    }

    print '-'x80,"\n";
    print("$ScriptName [/option=<value> [...]] [ModuleOptions] [/Debug] [/DieOnError]\n");
    print("\n");
    print("General source server settings:\n");
    print("\n");
    print("     NAME              SWITCH      ENV. VAR        Default\n");
    print("  -----------------------------------------------------------------------------\n");
    print("  1) srcsrv.ini        Ini         SRCSRV_INI      .\\srcsrv.ini\n");
    print("  2) Source root       Source      SRCSRV_SOURCE   .\n");
    print("  3) Symbols root      Symbols     SRCSRV_SYMBOLS  .\n");
    print("  4) Control system    System      SRCSRV_SYSTEM   <N/A>\n");
    print("  5) Save File (opt.)  Save        SRCSRV_SAVE     <N/A>\n");
    print("  6) Load File (opt.)  Load        SRCSRV_LOAD     <N/A>\n");
    print("\n");

    if ( defined $SourceSystem ) {
        if ( $SourceSystem->can( "SimpleUsage" ) ) {
            $SourceSystem->SimpleUsage();
        } else {
            print("No module specific information available for $SourceSystem.\n")
        }
        print("\n");

    } else {
        print("Use \"$ScriptName -?\" with SRCSRV_SYSTEM set to see module-".
              "specific\noptions.\n\n");
    }

    print("Precedence is: Default, environment, cmdline switch. (ie. env overrides default,\n");
    print("switch overrides env).\n");
    print("\n");

    print("Using '/debug' will turn on verbose output.\n");
    print("\n");

    print("Using '/dieonerror' will abort the run on all otherwise non-fatal errors and warnings.\n");
    print("\n");

    if ( ! $VerboseHelp ) {
        print("Use \"$ScriptName -??\" for verbose help information.\n\n");
    } else {
        print("(1)  SrcSrv.ini - points to an .INI file that must contain a [variables] section.\n");
        print("     The [variables] section must contain a token<->server mapping for the VSS\n");
        print("     dir being used.  The script will also automatically check for a srcsrv.ini\n");
        print("     in the same directory as $ScriptName if (1) it default value\n");
        print("     hasn't been overridden and (2) srcsrv.ini isn't found in the current\n");
        print("     directory.\n");
        print("\n");
        print("(2)  Source root - the directory to start gathering source information from.  The\n");
        print("     script will attempt to correlate each file in and below this directory with\n");
        print("     a file on the VSS server.\n");
        print("\n");
        print("(3)  Symbols root - the root directory that contains the PDB files to index. All\n");
        print("     PDBs in and below this directory will have a source stream added if at least\n");
        print("     one source file from the PDB is found on the VSS server.\n");
        print("\n");
        print("(4)  Source Control System - the type of source control system the files are\n");
        print("     are stored in.  This parameter must be set in the environment or from the\n");
        print("     command line.\n");
        print("\n");
        print("(5)  Save File - Save file information after gather source information (and\n");
        print("     before processing symbol files). Optional and may not be supported by all.\n");
        print("     source-control-system specific modules.\n");
        print("\n");
        print("(6)  Load Files - Load previously saved file information. Optional and may not\n");
        print( "    be supported by all source-control-system specific modules.\n");
        print("\n");

        if ( defined $SourceSystem ) {
            if ( $SourceSystem->can( "VerboseUsage" ) ) {
                $SourceSystem->VerboseUsage();
            } else {
                print("No module specific information available for $SourceSystem.\n");
            }
            print("\n");
        }
    }

    print("See the Source Server documentation for more information.\n");
    print '-'x80,"\n";
}

###############################################################################
#
# Misc support routines
#
###############################################################################

#
# set the debug output level
#
sub set_debug_level {
    my $setting = shift;

    if ( $setting =~ m/^-DEBUG:(-?\d+)$/i ) {
        $Options{'SRCSRV_DEBUG'} = $1;
    } elsif ( $setting =~ m/^-QUIET$/i ) {
        $Options{'SRCSRV_DEBUG'} = -1;
    } else {
        $Options{'SRCSRV_DEBUG'} = 99;
    }
}

#
# message routines - display formatted output
#
sub fatal_error {
    my @params = @_;

    # If die() was called by a loaded module, display
    # the module's name.
    my $pkg    = (caller())[0];
    $pkg = "" if ($pkg eq "main");

    printf("$ScriptName [ERROR ] $pkg: ");
    chomp $params[0] if (defined $params[0]);
    printf(@params);
    printf("\n");
    exit(1);
}

sub info_message {
    my @params = @_;
    if ( defined $Options{'SRCSRV_DEBUG'} &&
             int($Options{'SRCSRV_DEBUG'}) > 3 ) {

        # If info_message() was called by a loaded module, display
        # the module's name.
        my $pkg    = (caller())[0];
        $pkg = "" if ($pkg eq "main");

        printf("$ScriptName [INFO  ] $pkg: ");
        chomp $params[0] if (defined $params[0]);
        printf(@params);
        printf("\n");
    }
}

sub status_message {
    my @params = @_;

    # If status_message() always omits package name

    printf("$ScriptName [STATUS] : ");
    chomp $params[0] if (defined $params[0]);
    printf(@params);
    printf("\n");
}

sub warn_message {
    my @params = @_;

    # If warn() was called by a loaded module, display
    # the module's name.
    my $pkg    = (caller())[0];
    $pkg = "" if ($pkg eq "main");

    my $msg = "$pkg: ";

    if ( defined $params[0] ) {
        # This block needed because sprintf() doesn't
        # treat a single array parameter the same way
        # that printf() treats it.  Specifically, it
        # always treats it as a scalar.
        my $fmt = shift @params;
        chomp $fmt;
        $msg .= sprintf($fmt, @params)
    } else {
        $msg .= "";
    }

    if ( $Options{'SRCSRV_DIEONERROR'} ) {
        fatal_error("$msg");
        exit();
    } else {
        if ( defined $Options{'SRCSRV_DEBUG'}  &&
                 int($Options{'SRCSRV_DEBUG'}) >  1 ) {
            printf("$ScriptName [WARN  ] $msg");
            printf("\n");
        }
    }
}

# Very simplistic version mkstemps(). File::Temp is better, but
# not everyone has it installed.
sub GetTempFile {
    my ($fh, $path);

    my $Range       = 104976; # 4 hex digits
    my $RetryCount  = 0;

    do {
        my $digits = sprintf("%X", (rand($Range)));
        $path = "$ENV{'TEMP'}\\index${digits}.stream";
        $fh = ScalarSysOpen($path, O_RDWR|O_CREAT|O_EXCL);
    } while ( ! $fh && ($RetryCount++ < 10 ) );

    if ( $RetryCount >= 10 ) {
        die("Failed to get temp file for processing.\n");
    }

    return ($fh, $path);
}

sub ScalarOpen {
   local *Fhandle;
   my    $file = shift;
   open(\*Fhandle,  "$file") or return 0;
   return(*Fhandle);
}

sub ScalarOpenDir {
   local *Dhandle;
   my    $dir = shift;
   opendir(\*Dhandle,  "$dir") or return 0;
   return(*Dhandle);
}

sub ScalarSysOpen {
   local *Fhandle;
   my    $file = shift;
   my    $flags= shift;
   sysopen(\*Fhandle,  "$file", $flags) or return 0;
   return(*Fhandle);
}

__END__
