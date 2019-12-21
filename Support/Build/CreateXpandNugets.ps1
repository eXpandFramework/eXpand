Param (
    [string]$root = (Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$version = "19.2.304.7",
    [bool]$ResolveNugetDependecies,
    [bool]$Release 
)
$ErrorActionPreference = "Stop"
Use-MonoCecil | Out-Null

if (!$projects) {
    $projects = Get-ChildItem "$PSScriptRoot\..\..\Xpand" *.csproj -Exclude "*Xpand.Test*" -Recurse
}
$nuget = "$(Get-NugetPath)"
$nuspecpathsPath = "$PSScriptRoot\..\Nuspec"
$scriptPath = $MyInvocation.MyCommand.path
function AddAllDependency($file, $nuspecpaths) {
    [xml]$nuspecpath = Get-Content $file
    $metadata = $nuspecpath.package.metadata
    if ($metadata.dependencies) {
        $metadata.dependencies.RemoveAll()
    }
    $metadata.version = $version
    $nuspecpaths | ForEach-Object {
        [xml]$package = Get-Content $_.Fullname
        Add-NuspecDependency $package.package.metaData.Id $Version $nuspecpath
    }
    $nuspecpath.Save($file)
}

Set-Location $root
$nuspecs = Get-ChildItem "$PSScriptRoot\..\Nuspec" -Exclude "ALL_*" -recurse | ForEach-Object {
    [PSCustomObject]@{
        FileInfo = $_
        Content  = [xml](Get-Content $_)
    }
}

$nuspecs | Where-Object { $_ -like "*lib*" } | ForEach-Object {   
    # $nuspecs| Invoke-Parallel  -VariablesToImport "nuspecs","projects" -Script {   
    $name = ($_.FileInfo.BaseName)
    Write-Output "--------------------Updating $name-----------------------" 
    $project = $projects | Where-Object { $_.BaseName -eq "Xpand.ExpressApp.$name" }
    if ($name -like "System*") {
        $name = $name.Replace("System", "")
        $project = $projects | Where-Object { $_.BaseName -eq "Xpand.ExpressApp$name" }
    }
    elseif (!$project) {
        if ($name -eq "lib") {
            $project = $projects | Where-Object { $_.BaseName -in "Xpand.Persistent.Base","Xpand.Utils","Xpand.Xpo" }
        }
        else {
            $project = $projects | Where-Object { $_.BaseName -eq "Xpand.$name" }
        }
        
    }
    [xml]$nuspec = $_.Content
    if ($nuspec.package.metaData.dependencies) {
        $nuspec.package.metaData.dependencies.RemoveAll()
    }
    $project | ForEach-Object {
        [xml]$csproj = Get-Content $_.FullName
        
        $xpandMoleReference = $csproj.project.ItemGroup.Reference | Where-Object { $_.include -like "Xpand.ExpressApp*" }
        if ($name -ne "lib" -and $libRefs) {
            $libRefs = $csproj.project.ItemGroup.Reference | Where-Object { $_.include -like "Xpand.*" -and $_ -notin $xpandMoleReference } | Select-Object -First 1
            $assemblyPath = Get-Item (Resolve-Path $libRefs.hintpath)
            $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($assemblyPath.FullName).FileVersion
            Add-NuspecDependency eXpandLib $version $nuspec
        }
        $xpandMoleReference | ForEach-Object {
            $assemblyPath = Get-Item (Resolve-Path $_.hintpath)
            $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($assemblyPath.FullName).FileVersion
            $id = ($nuspecs | Where-Object { $_.Content.package.files.file | Where-Object { $_.src -eq $assemblyPath.Name } }).Content.package.metaData.id
            Add-NuspecDependency $id $version $nuspec
        }
        
        Get-PackageReference $_.FullName | Where-Object { $_.id -notlike "DevExpress*" } | ForEach-Object {
            Add-NuspecDependency $_.id $_.version $nuspec
        }
    }
    
    $sortedDeps = $nuspec.package.metadata.dependencies.dependency | Sort-Object id -Unique
    $nuspec.package.metadata.dependencies.RemoveAll()
    $sortedDeps | Add-NuspecDependency -Nuspec $nuspec
    $nuspec.Save($_.FileInfo.FullName)
    Format-Xml -path $_.FileInfo.FullName
    
}


$libNuspecPath = [System.io.path]::GetFullPath("$root\Support\Nuspec\Lib.nuspec")
[xml]$libNuspec = Get-Content $libNuspecPath
$libNuspec.package.files.RemoveAll()
[xml]$libCsproj = Get-Content "$PSScriptRoot\..\..\Xpand\Xpand.Persistent\Xpand.Persistent.Base\Xpand.Persistent.Base.csproj"
$libTargetFramework = $libCsproj.project.propertygroup.targetFramework
"dll", "pdb" | ForEach-Object {
    $ext = $_
    "Xpand.xpo", "Xpand.Utils", "Xpand.Persistent.BaseImpl" | ForEach-Object {
        $id = "$_.$ext"
        Add-XmlElement $libNuspec "file" "files" @{
            src    = $id
            target = "lib\$libTargetFramework\$id"
        }
    }
}
$libNuspec.Save($libNuspecPath)

$nuspecpathFile = "$nuspecpathsPath\All_Agnostic.nuspec"
AddAllDependency $nuspecpathFile (Get-ChildItem "$nuspecpathsPath" -Exclude "*Win*", "*Web*", "*All*")

"Win", "Web" | ForEach-Object {
    $nuspecpaths = (Get-ChildItem "$nuspecpathsPath" "*$_*" | Where-Object { $_.BaseName -notmatch "All" })
    $nuspecpathFile = "$nuspecpathsPath\All_$_.nuspec"
    AddAllDependency $nuspecpathFile $nuspecpaths
}
Write-HostFormatted "packing nuspecs"
Get-ChildItem "$root\build\Nuget" -ErrorAction SilentlyContinue | Remove-Item -Force -Recurse
# Get-ChildItem "$root\Support\Nuspec" *.nuspec | where{$_ -match "World"}|ForEach-Object {
# Get-ChildItem "$root\Support\Nuspec" *.nuspec| Invoke-Parallel -VariablesToImport "nuget","version","root" -Script {    
Get-ChildItem "$root\Support\Nuspec" *.nuspec | ForEach-Object {    
    & $Nuget Pack $_.FullName -version $Version -OutputDirectory "$root\build\nuget" -BasePath "$root\Xpand.DLL"
}

Write-HostFormatted "Updating ReadMe" -Section
& "$root\support\build\addreadme.ps1"
$ErrorActionPreference = "stop"
$packageDir = "$root\Build\_package\$Version"
New-Item $packageDir -ItemType Directory -Force | Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$Version.zip" -path "$root\Build\Nuget\*" -Force