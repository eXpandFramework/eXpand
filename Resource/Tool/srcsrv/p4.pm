# -----------------------------------------------------------------------------
# p4.pm
# 
# Copyright (c) 2005-2006  Microsoft Corporation
#
# Source Server indexing module for Perforce
# -----------------------------------------------------------------------------

package P4;

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
    if ( defined $ENV{'P4_CMD'} ) {
        $$self{'P4_CMD'} = $ENV{'P4_CMD'};
    } else {
        $$self{'P4_CMD'} = "p4.exe";
    }

    $$self{'P4_DEBUGMODE'} = 0;

    # Partial-path matching disabled by default.
    $$self{'P4_PARTIALMATCH'} = FALSE;

    #
    # Allow P4_LABEL to be set via the environment
    #   
    $$self{'P4_LABEL'} = $ENV{'P4_LABEL'} if ( defined $ENV{'P4_LABEL'} );

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
                        $$self{'P4_LABEL'}   = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "LABEL");
                        $$self{'P4_NEWROOT'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "NEWROOT");
                        $$self{'P4_OLDROOT'} = $opt[1], last
                                if ( uc substr($opt[0], 1) eq "OLDROOT");
                        # Remember this was unused
                        push(@unused_opts, $_);
                        1;
                    }
                } else {
                    block: {
                        $$self{'P4_PARTIALMATCH'} = TRUE, last
                            if ( uc substr($_, 1) eq "PARTIALMATCH");

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
# Display module internal option state
# -----------------------------------------------------------------------------
sub DisplayVariableInfo {
    my $self = shift;

    ::status_message("%-15s: %s\n",
                     "P4 program name",
                     $$self{'P4_CMD'});

    ::status_message("%-15s: %s\n",
                    "P4 Label",
                    $$self{'P4_LABEL'} ? $$self{'P4_LABEL'}      : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "Old path root",
                     $$self{'P4_OLDROOT'} ? $$self{'P4_OLDROOT'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "New path root",
                     $$self{'P4_NEWROOT'} ? $$self{'P4_NEWROOT'} : "<N/A>");

    ::status_message("%-15s: %s\n",
                     "Partial match",
                     $$self{'P4_PARTIALMATCH'} ? "Enabled" : "Not enabled");
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

    # Remember the current directory
    my $PrevDir    = `cd`;
    chomp $PrevDir;

    # Move to the source root to keep life simple
    chdir($SourceRoot);

    # Process "p4 info" to get the server string and
    # the base root of the client (which may be a subset of
    # the source root we're processing).
    open($hProc, "$$self{'P4_CMD'} info 2>NUL|");
    my ($Server, $Root);
    while (<$hProc>) {
        chomp;
        if (m/Server\saddress:\s(.*)/i) {
            $Server = $1;
        } elsif (m/Client\sroot:\s(.*)/i) {
            $Root = $1;
        }
    }

    if ( ! defined $Server ) {
        ::status_message("Server name not returned by $$self{'P4_CMD'} info for $SourceRoot.".
                         "Skipping all files in the depot.");
        return;
    }

    if (! defined $$ServerRefs{uc $Server} ) {
        die("$Server not found in srcsrv.ini!\n");
        # ERROR!
    }

    # Run "p4 have ..." to get a list of every file
    # mapped on this machines.
    my @Files = `$$self{'P4_CMD'} have ... 2>NUL`;

    # Go back to our old directory - SSIndex.cmd could rely
    # on it.
    chdir($PrevDir);

    # Process every returned file.
    foreach (@Files) {
        next if     (m/^--/);
        next if     (m/^\s*$/);
        next unless (m/#/);
        chomp;

        # Make sure this line matches the output we expect
        if ( $_ =~ m/^(.*)\#(\d*)\s-\s([A-Z]:\\.*)/i ) {
            # Store away the matching substrings for creating
            # our file reference below.
            my $local_file    = $3;
            my $remote_file   = $1;
            my $file_revision = $2;

            $remote_file =~ s/\/\///;

            # If we're using a label, replace the current file revision
            # with the name of the given label.
            if (defined $$self{'P4_LABEL'} ) {
                $file_revision = $$self{'P4_LABEL'};
            }

            # Add the information for this file to the FILE_LOOKUP_TABLE that
            # will be referenced when SSIndex calls out GetFileInfo() function.
            @{$$self{'FILE_LOOKUP_TABLE'}{lc $local_file}} =
                    # First element is a hash of the variables used in this line
                    ( { "$$ServerRefs{uc $Server}" => "$Server"},
                    # Second element is the data for extracting this file
                    # var1 will be prepended by GetFileInfo()
                    "$$ServerRefs{uc $Server}*". # var 2
                    "$remote_file*".             # var 3
                    "$file_revision");           # var 4
        }
    }
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

        #
        # Exact match found - return it.
        #
        return( ${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[0],
                "$name_in_pdb*${$$self{'FILE_LOOKUP_TABLE'}{lc $file}}[1]" );

    } elsif ( $$self{'P4_PARTIALMATCH'} && $file !~ /^[A-Z]:/i ) {

        #
        # Older compilers may write partial paths.  If this doesn't look like
        # a full path and partial matching is allowed, try to match the current
        # file to the tail of a file that was found.
        #
        my @possible_matches;

        foreach ( sort keys %{$self->{'FILE_LOOKUP_TABLE'}} ) {
            if ( /\\\Q$file\E$/i ) {
                push(@possible_matches,
                    [${$$self{'FILE_LOOKUP_TABLE'}{lc $_}}[0],
                    "$name_in_pdb*${$$self{'FILE_LOOKUP_TABLE'}{lc $_}}[1]"]);
            }
        }

        if ( @possible_matches == 0 ) {
            # No partial matches found, return undef.
            return(undef);
        } elsif ( @possible_matches == 1 ) {
            # Exactly one partial match found, return it.
            return( @{$possible_matches[0]} );
        } else {
            # More than one partial match found. Better to skip the file than
            # use the wrong one, so inform the user and return undef.
            ::status_message("Multiple possibilities found for file $file. Not indexing.");
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
    return("Perforce");
}

# -----------------------------------------------------------------------------
# Set the debug level for output.
# -----------------------------------------------------------------------------
sub SetDebugMode {
    my $self = shift;
    $$self{'P4_DEBUGMODE'} = shift;
}

# -----------------------------------------------------------------------------
# Return the SCS specific stream variables.
# -----------------------------------------------------------------------------
sub SourceStreamVariables {
    my $self = shift;
    my @stream;

    # The extraction command varies based on whether or not we're using
    # a label.
    if ( defined $$self{'P4_LABEL'} ) {
        push(@stream, "P4_EXTRACT_CMD=p4.exe -p %fnvar%(%var2%) print ".
                                            "-o %srcsrvtrg% ".
                                            "-q \"//%var3%@%var4%\"");
    } else {
        push(@stream, "P4_EXTRACT_CMD=p4.exe -p %fnvar%(%var2%) print ".
                                            "-o %srcsrvtrg% ".
                                            "-q \"//%var3%#%var4%\"");
    }

    push(@stream, "P4_EXTRACT_TARGET=".
                  "%targ%\\%var2%\\%fnbksl%(%var3%)\\%var4%\\%fnfile%(%var1%)");

    return(@stream);
}

# -----------------------------------------------------------------------------
# Loads previously saved file information.
# -----------------------------------------------------------------------------
sub LoadFileInfo {
    my $self = shift;
    my $dir  = shift;

    if ( -e "$dir\\p4_files.dat" ) {
        our $FileData1;
        require "$dir\\sd_files.dat";
        $$self{'FILE_LOOKUP_TABLE'} = $FileData1;
    } else {
        ::status_message("No Perforce information saved in $dir.\n");
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
    if ( open($fh, ">$dir\\p4_files.dat") ) {
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
print<<P4_SIMPLE_USAGE;
Perforce specific settings:

     NAME            SWITCH      ENV. VAR        Default
  -----------------------------------------------------------------------------
  A) p4 command     <n/a>       P4_CMD           p4.exe
  B) label          Label       P4_LABEL         <n/a>
  C) old root       OldRoot     <n/a>            <n/a>
  D) new root       NewRoot     <n/a>            <n/a>
P4_SIMPLE_USAGE
}

# -----------------------------------------------------------------------------
# Verbose usage ('-??')
# -----------------------------------------------------------------------------
sub VerboseUsage {
print<<P4_VERBOSE_USAGE;
(A)  P4 Command - The name of the executable to run to issue commands to the
     Perforce server.  The executable named here must support the same
     options as p4.exe in order for the script to work correctly.

(B)  Label - Use the given text as the file revision label to extract source
     using instead of extracting the source using the numeric file revision.

(C)  Old Root and New Root - Allows the source indexing of symbols that were
     built on another machine.  If these are set, every source path that is
     prefixed with Old Root with have that prefix replaced with the value in New
     Root prior to attempting to resolve the local path and filename to a server
     path and filename. Both Old Root and New Root must be specified to use this
     feature.

(D)  New Root - See Old Root above.
P4_VERBOSE_USAGE
}

1;
__END__
