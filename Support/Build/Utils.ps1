
function CloneItem{
    [cmdletbinding()]
    param(
        [parameter(ValueFromPipeline=$True,mandatory=$True)]
        [string]$Path,
        [parameter(mandatory=$True)]
        [string] $TargetDir,
        [parameter(mandatory=$True)]
        [string]$SourceDir
    )
    $targetFile = $TargetDir + $Path.SubString($SourceDir.Length);
    
    if (!((Get-Item $Path) -is [System.IO.DirectoryInfo])){
        $dirName=Split-Path $targetFile -Parent
        New-Item -ItemType Directory $dirName -ErrorAction SilentlyContinue
        Copy-Item $Path -destination $targetFile -Force
        Write-Output $targetFile
    }
}
function GetProjects{
    param(
    [Parameter(ValueFromPipeline)]
    $projects)
    
    ($projects.Include -split ";")|ForEach-Object{
            $item=$_.Trim()
            if ($item -ne ""){
               $project="$PSScriptRoot\..\..\$item"
               if ((Get-Item $project).GetType().Name -eq "FileInfo"){
                   $project
                }
            }
        }|Where-Object{$_ -ne "" -and $_ -ne $null}
}


