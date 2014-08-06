# -----------------------------------------------------------------------------
# tfs.pm
#
# Copyright (c) 2006  Microsoft Corporation
#
# Source Server indexing module for Team Foundation Server
# -----------------------------------------------------------------------------

package TFS;

require Exporter;
use strict;

my %EXPORT_TAGS = ( 'all' => [ qw() ] );
my @EXPORT_OK   = ( @{ $EXPORT_TAGS{'all'} } );
my @EXPORT      = qw();
my $VERSION     = '0.1';

# -----------------------------------------------------------------------------
# Simple subs to make it clear when we're testing for BOOL values
# -----------------------------------------------------------------------------

sub TRUE   {return(1);} # BOOLEAN TRUE
sub FALSE  {return(0);} # BOOLEAN FALSE

# -----------------------------------------------------------------------------
# Create a new blessed reference that will maintain state for this instance of
# indexing
# -----------------------------------------------------------------------------

sub new
{
    my $proto = shift;
    my $class = ref($proto) || $proto;
    my $self  = {};
    bless($self, $class);

    #
    # The command to use for talking to the server.  We don't allow this
    # to be overridden at the command line.
    #
    if ( defined $ENV{'TFS_CMD'} ) {
        $$self{'TFS_CMD'} = $ENV{'TFS_CMD'};
    } else {
        $$self{'TFS_CMD'} = "tf.exe";
    }

    $$self{'TFS_LABEL'}     = $ENV{'TFS_LABEL'}
        if ( defined $ENV{'TFS_LABEL'}     );

    $$self{'TFS_DEBUGMODE'} = 0;

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
                        $$self{'TFS_CMD'}     = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "CMD");
                        $$self{'TFS_LABEL'}   = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "LABEL");
                        $$self{'TFS_NEWROOT'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "NEWROOT");
                        $$self{'TFS_OLDROOT'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "OLDROOT");
                        # Remember this was unused
                        push(@unused_opts, $_);
                        1;
                    }

                # options that are just flags
                } else {
                    block: {
                        $$self{'SHOWCMDS'}   = TRUE, last
                            if ( uc substr($opt[0], 1) eq "SHOWCMDS");
                        $$self{'TFS_VERSION2PLUS'}   = TRUE, last
                            if ( uc substr($opt[0], 1) eq "V2+");
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

    return($self);
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
# Execute a TFS command and return a handle to the output.
# -----------------------------------------------------------------------------

sub ExecTFSCmd
{
    my $self = shift;
    my $dir = shift;
    my $cmd = shift;

    return ExecCmd($self, $dir, "$$self{'TFS_CMD'} $cmd");
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

sub DisplayVariableInfo
{
    my $self = shift;

    ::status_message("%-15s: %s\n",
                     "TFS program name",
                     $$self{'TFS_CMD'});

    ::status_message("%-15s: %s\n",
                    "TFS Label",
                    $$self{'TFS_LABEL'} ? $$self{'TFS_LABEL'}      : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "Old path root",
                     $$self{'TFS_OLDROOT'} ? $$self{'TFS_OLDROOT'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "New path root",
                     $$self{'TFS_NEWROOT'} ? $$self{'TFS_NEWROOT'} : "<N/A>");

}

# -----------------------------------------------------------------------------
# Given our init data and a local source path, create a lookup table that can
# return individual stream data for each source file.
# -----------------------------------------------------------------------------

sub GatherFileInformation
{
    my $self       = shift;
    my $SourceRoot = shift;
    my $ServerRefs = shift;
    my $ExcludesRef= shift;
    my %FileLookup;
    my $hProc;

    #::status_message("self = $self\n");
    #::status_message("SourceRoot = $SourceRoot\n");
    #::status_message("ServerRefs = $ServerRefs\n");

    # Remember the current directory

    my $PrevDir    = `cd`;
    chomp $PrevDir;

    chdir($SourceRoot);

    my ($Server, $Root, $PreferredAlias, $PreferredServer);

    # Get the TFS database info

    $hProc = ExecTFSCmd($self, $SourceRoot, "workfold");

    while (<$hProc>)
    {
        chomp;
        if (m/Server\s+:\s+(.*)/i)
        {
           $PreferredServer = $1;
        }
    }

    close($hProc);

    # Confirm the results.

    if ( defined $PreferredServer )
    {
        $Server = $PreferredServer;
    }

    if ( ! defined $Server )
    {
        ::status_message("Server name not returned by $$self{'TFS_CMD'} info for $SourceRoot.".
                         "Skipping all files in the working folder.");
        return;
    }

    if ( ! defined $$ServerRefs{uc $Server} )
    {
        ::status_message("$Server not found in srcsrv.ini. Skipping all files in database.");
        return;
    }

    # Get the file list for all the files in this directory and below.
    # Get the file name, database location, and current revision.

    # This is what the format of the input data looks like...

    #Local information:
    #  Local path : E:\dd\tsadt_1\src\debugger\AutoAttach\AutoAttach.rc
    #  Server path: $/Orcas/PU/tsadt/debugger/AutoAttach/AutoAttach.rc
    #  Changeset  : 60522
    #  Change     : none
    #  Type       : file
    #Server information:
    #  Server path  : $/Orcas/PU/tsadt/debugger/AutoAttach/AutoAttach.rc
    #  Changeset    : 60522
    #  Deletion ID  : 0
    #  Lock         : none
    #  Lock owner   :
    #  Last modified: Monday, May 01, 2006 8:46:59 PM
    #  Type         : file
    #  File type    : Windows-1252
    #  Size         : 458

    # The information we want begins with the section labled, "Local Information"
    # and ends with the section labeled, "Server Information".

    $hProc = ExecTFSCmd($self, $SourceRoot, "properties . /r");

    my $local;
    my $remote;
    my $revision;
    my $active = 0;

    while (<$hProc>)
    {
        # Is this a new item record?  If so, initialize it and
        # indicate that we are processing an active block of data.

        if (m/^Local\sInformation:/i)
        {
            $active = 1;
            $local = undef;
            $remote = undef;
            $revision = undef;
            next;
        }

        # If this is not an active block, go to the next line.

        next if ($active != 1);

        # Check to see if we have reached the end of the active block.

        if (m/^Server\sinformation:/i)
        {
            $active = 0;
            next;
        }

        # Look for and read in the data for the local file name,
        # server file name, and file revision.

        chomp;
        if (m/\s+Local\spath\s*:\s+(.*)/i)
        {
            $local = $1;
        }
        if (m/\s+Server\spath\s*:\s+\$(.*)/i)
        {
            $remote = $1;
        }
        if (m/\s+Changeset\s*:\s+(.*)/i)
        {
            $revision = $1;
        }

        # If we are looking at type field, it's time to process the file.

        if (m/\s+Type\s*:\s+(.*)/i)
        {
            # If this is not a file, nothing to do.

            next if ($1 ne "file");

            # Debug spew.
            # ::status_message("$local\n");
            # ::status_message("$remote\n");
            # ::status_message("$revision\n\n");

            # If a label was specified, overwrite the revsion with the label.

            if (defined $$self{'TFS_LABEL'} )
            {
                $revision = $$self{'TFS_LABEL'};
            }

            # If we don't have all the data, bail and go to the next item.

            next if (!defined $local);
            next if (!defined $remote);
            next if (!defined $revision);

            next if ::ExcludeFile($remote, $ExcludesRef);

            # Add the information for this file to the FILE_LOOKUP_TABLE that
            # will be referenced when SSIndex calls out GetFileInfo() function.

            @{$$self{'FILE_LOOKUP_TABLE'}{lc $local}} =
                    # First element is a hash of the variables used in this line
                    ( {"$$ServerRefs{uc $Server}" => "$Server"},
                    # Second element is the data for extracting this file
                    # var 1 will be prepended by GetFileInfo()
                    "$$ServerRefs{uc $Server}*". # var 2
                    "$remote*".                  # var 3
                    "$revision");                # var 4
        }

        next;
    }

    close($hProc);
    chdir($PrevDir);

    #DumpFileLookupTable($self);
}

# -----------------------------------------------------------------------------
# Return ths SRCSRV stream data for a single file.
# -----------------------------------------------------------------------------

sub GetFileInfo
{
    my $self        = shift;
    my $file        = shift;
    my $name_in_pdb = shift;

    # Debug spew
    # ::status_message("GetFileInfo: $file\n");
    # ::status_message("             $name_in_pdb\n");

    if ( ! defined $name_in_pdb )
    {
        $name_in_pdb = $file;
    }

    # We stored the necessary information when GatherFileInformation() was
    # called so we just need to return that information.

    if ( defined $$self{'FILE_LOOKUP_TABLE'}{lc $file} )
    {
        #::status_message("GetFileInfo returns something for $file\n");
        return( ${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[0],
                "$name_in_pdb*${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[1]" );
    }
    else
    {
        return(undef);
    }
}

# -----------------------------------------------------------------------------
# The long name that should be written the SRCSRV stream to describe
# the source control system being indexed.
# -----------------------------------------------------------------------------

sub LongName
{
    return("Team Foundation Server");
}

# -----------------------------------------------------------------------------
# Set the debug level for output.
# -----------------------------------------------------------------------------

sub SetDebugMode
{
    my $self = shift;
    $$self{'TFS_DEBUGMODE'} = shift;
}

# -----------------------------------------------------------------------------
# Return the SCS specific stream variables.
# -----------------------------------------------------------------------------

sub SourceStreamVariables
{
    my $self = shift;
    my @stream;
    my $cmd;
    my $verprefix = "";

    # The extraction command varies based on whether or not we're using
    # a label and whether or not we are using the 1.0 version of TFS.

    if ( defined $$self{'TFS_LABEL'} )
    {
        $verprefix = "L";
    }

# NOTE: This code does not support spaces in labels.
    $cmd = "view ".
           "/version:$verprefix%var4% ".
           "/noprompt ".
           "\"\$%var3%\" ".
           "/server:%fnvar%(%var2%)";
# NOTE: Suggest replacing that line with the following commented
#       out line.
#           
#   $cmd = "view ".
#          "/version:$verprefix\"%var4%\" ".
#          "/noprompt ".
#          "\"\$%var3%\" ".
#           "/server:%fnvar%(%var2%)";

    if ( defined $$self{'TFS_VERSION2PLUS'} )
    {
        $cmd = "TFS_EXTRACT_CMD=tf.exe $cmd /output:%srcsrvtrg%";
    }
    else
    {
        $cmd = "TFS_EXTRACT_CMD=tf.exe $cmd /console >%srcsrvtrg%";
    }
    push(@stream, $cmd);

    push(@stream, "TFS_EXTRACT_TARGET=".
                  "%targ%\\%var2%%fnbksl%(%var3%)\\%var4%".
                  "\\%fnfile%(%var1%)");

    push(@stream, "SRCSRVVERCTRL=tfs");

    push(@stream, "SRCSRVERRDESC=access");
    push(@stream, "SRCSRVERRVAR=var2");

    # Error messages for reference purposes

    #Dead server:
    #Team Foundation Server http://asdf:8080 does not exist or is not accessible at this time.
    #Technical information (for administrator):
    #  The remote name could not be resolved: 'asdf'
    #
    #No privs:
    #TF30063: You are not authorized to access http://tkbgitvstfat01:8080.

    return(@stream);
}

# -----------------------------------------------------------------------------
# Loads previously saved file information.
# -----------------------------------------------------------------------------

sub LoadFileInfo
{
    my $self = shift;
    my $dir  = shift;

    my $file = "$dir\\tfs_files.dat";
    if ( -e  $file ) {
        do $file;
        $$self{'FILE_LOOKUP_TABLE'} = $TFS_FILES::FileData1;
    } else {
        ::status_message("No TFS information saved in $dir.\n");
    }

    return();
}

# -----------------------------------------------------------------------------
# Saves current file information.
# -----------------------------------------------------------------------------

sub SaveFileInfo
{
    my $self = shift;
    my $dir  = shift;
    my $FileLookupRef = $$self{'FILE_LOOKUP_TABLE'};

    my $fh;
    if ( defined $FileLookupRef ) {
        $fh = ::ScalarOpen(">$dir\\tfs_files.dat");

        if ( $fh ) {
            # Use this so simulate Data::Dumper instead of using
            # Data::Dumper directly since this doesn't require
            # in-memory expansion of the entire hash structure.
            print($fh "\$TFS_FILES::FileData1 = {");
            my $key;

            foreach $key (sort keys %{$FileLookupRef} ) {
                printf($fh "'%s' => [ {", $key);
                foreach (sort keys %{$FileLookupRef->{$key}->[0]}) {
                    printf($fh
                           "'%s' => '%s',",
                           $_,
                           ${$FileLookupRef->{$key}->[0]}{$_});
                }
                printf($fh "}, '%s'", $FileLookupRef->{$key}->[1] );
                printf($fh "],\n                        ");
            }

            print($fh "};");
            close($fh);
        } else {
            ::status_message("Failed to save data to $dir.\n");
        }
    }

    return();
}

# -----------------------------------------------------------------------------
# Returns the version of this script.
# -----------------------------------------------------------------------------

sub GetScriptVersion
{
    my $self = shift;

    if ( defined $$self{'TFS_VERSION2PLUS'} )
    {
        $_ = 1;
    }
    else
    {
        $_ = 3;
    }
}

# -----------------------------------------------------------------------------
# Simple usage ('-?')
# -----------------------------------------------------------------------------

sub SimpleUsage
{
print<<TFS_SIMPLE_USAGE;
Team Foundation Server specific settings:

     NAME            SWITCH      ENV. VAR        Default
  -----------------------------------------------------------------------------
  A) tfs command    CMD         TFS_CMD          tf.exe
  B) label          Label       TFS_LABEL        <n/a>
  C) old root       OldRoot     <n/a>            <n/a>
  D) new root       NewRoot     <n/a>            <n/a>
TFS_SIMPLE_USAGE
}

# -----------------------------------------------------------------------------
# Verbose usage ('-??')
# -----------------------------------------------------------------------------

sub VerboseUsage
{
print<<TFS_VERBOSE_USAGE;
(A)  TFS Command - The name of the executable to run to issue commands
     against the TFS database.  The executable named here must support the same
     options as tf.exe in order for the script to work correctly.

(B)  Label - Use the given text as the file revision label to extract source
     using instead of extracting the source using the numeric file revision.

(C)  Old Root and New Root - Allows the source indexing of symbols that were
     built on another machine.  If these are set, every source path that is
     prefixed with Old Root with have that prefix replaced with the value in New
     Root prior to attempting to resolve the local path and filename to a server
     path and filename. Both Old Root and New Root must be specified to use this
     feature.

(D)  New Root - See Old Root above.
TFS_VERBOSE_USAGE
}

1;
__END__
   9) depot map      DepotMap    TFS_DEPOTMAP     <n/a>
  10) Source files   FileTable   TFS_FILETABLE    <n/A>

(9)  Depot Map and Source Files - See the Source Server documentation for a full
     description of these settings.

(10) Source Files - See Depot Map above.
