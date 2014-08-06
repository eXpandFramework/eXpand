# cv2http.pl
#
# converts a srcsrv stream to one that 
# polls an http site for the source

my $alias;  # logical name of the web site that stores the source files
my $url;    # actual url of the web site that stores the source files

foreach (@ARGV) 
{
    if (defined($alias))
    {
        $url = $_;
    }
    else
    {
        $alias = $_;
    }
}

if (!defined($url))
{
    exit(1);
}

while (@ARGV) 
{
    shift;
}

$/ = "\r\n";

while ( <> ) 
{ 
    chomp;
    s/^VERSION=1/VERSION=2/i;
    s/^SRCSRVERRDESC=.*//i;
    s/^SRCSRVERRVAR=.*//i;
    s/^.*EXTRACT_CMD=.*//i;
    s/^.*EXTRACT_TARGET=.*//i;
    s/^VERCTRL=.*/VERCTRL=http/i;
    s/^SRCSRVVERCTRL=.*/SRCSRVVERCTRL=http/i;
    s/^SRCSRVCMD=.*/SRCSRVCMD=/i;
    s/^SRCSRVTRG=.*/$alias=$url\nHTTP_EXTRACT_TARGET=%$alias%\/%var2%\/%var3%\/%var4%\/%fnfile%(%var1%)\nSRCSRVTRG=%http_extract_target%/i;
    # you can remove other unneeded entries such as server names and the like, if you want
    print "$_\n" if ($_ ne "");
}
