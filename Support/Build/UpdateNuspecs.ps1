param($nuspecpathFile, $version, $release, $root,$ResolveNugetDependecies)
if ($release -eq "true") {
    $release = $true
}
else {
    $release = $false
}
[xml]$xpandProjects = Get-Content "$root\Support\Build\Xpand.projects"
$group = $xpandProjects.Project.Itemgroup

$projects = ("$($group.CoreProjects.include);$($group.EasyTestProjects.Include);$($group.ModuleProjects.Include)".split(";")) | Where-Object { $_ } | Sort-Object | ForEach-Object {
    Get-item "$root\$($_.Trim())"
}
$nuspecpathFile
function Update-NuspecDependencies {
    [CmdletBinding()]
    param (
        [parameter(Mandatory)]
        [string]$Nuspecpath,
        [parameter(Mandatory)]
        [string]$CsProjPath,
        [parameter(Mandatory)]
        [string]$Version

    )
    
    begin {
    }
    
    process {    
        [xml]$nuspecpathContent = Get-Content $Nuspecpath
        $metadata = $nuspecpathContent.package.metadata
        $metadata.version = $version

        $nuspecpathContent.Save($nuspecpath)

        $uArgs = @{
            NuspecFilename           = $Nuspecpath
            ProjectFileName          = $CsProjPath
            ReferenceToPackageFilter = "Xpand.*"
            PublishedSource          = (Get-PackageFeed -Xpand)
            Release                  = $Release
            ReadMe                   = $true
            CustomPackageLinks       = @{
                "Xpand.Persistent.Base" = "eXpandLib" 
                "Xpand.Persistent.BaseImpl" = "eXpandLib" 
                "Xpand.Utils" = "eXpandLib" 
                "Xpand.Xpo" = "eXpandLib" 
                "Xpand.ExpressApp" ="eXpandSystem"
                "Xpand.ExpressApp.Win" ="eXpandSystemWin"
                "Xpand.ExpressApp.Web" ="eXpandSystemWeb"
            }
            NuspecMatchPattern="(?ix)Xpand\.ExpressApp\.|Xpand\."
            ResolveNugetDependecies=$ResolveNugetDependecies
            AllProjects=$projects|Get-Item|Select-Object -ExpandProperty BaseName
        }
        $uArgs|Format-Table -AutoSize | Write-Output
        if ($Release) {
            $uArgs.PublishedSource = (Get-PackageFeed -Nuget)
        }
        Update-Nuspec @uArgs
        
    }
    
    end {
    }
}
$nuspecpathFile = Get-Item $nuspecpathFile
if ($nuspecpathFile.BaseName -match "lib") {
    $base = $projects | Where-Object { ($_.BaseName -eq "xpand.persistent.base") } | Select-Object -First 1
    Update-NuspecDependencies -Nuspecpath $nuspecpathFile -CsProjPath $base.FullName -Version $version 
        
}
else {
    $projects | Where-Object {
        $name = $_.BaseName
        if ($nuspecpathFile.BaseName -like "System") {
            $name -eq "Xpand.ExpressApp"
        }
        elseif ($nuspecpathFile.BaseName -like "System.Win") {
            $name -eq "Xpand.ExpressApp.Win"
        }
        elseif ($nuspecpathFile.BaseName -like "System.Web") {
            $name -eq "Xpand.ExpressApp.Web"
        }
        else {
            $name -like "*.$($nuspecpathFile.BaseName)" -and $nuspecpathFile.BaseName -notlike "Lib"
        }
    } | Select-Object -First 1 | ForEach-Object {
        Update-NuspecDependencies -Nuspecpath $nuspecpathFile -CsProjPath $_.FullName -Version $version 
    }
}

