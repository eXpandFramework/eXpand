Param (
    [string]$root = (Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$version = "19.1.402.0",
    [bool]$ResolveNugetDependecies,
    [bool]$Release ,
    [switch]$DotSourcing,
    [System.IO.FileInfo[]]$projects
)

Use-MonoCecil | Out-Null

if (!$projects) {
    $projects = Get-ChildItem "$PSScriptRoot\..\..\Xpand" *.csproj -Exclude "*Xpand.Test*" -Recurse
}
$nuget = "$(Get-NugetPath)"
$nuspecpathsPath = "$PSScriptRoot\..\Nuspec"
function AddDependency {
    param($id, $nuspecpathContent, $packageVersion)
    if (!$id) {
        return
    }
    if (!$packageVersion) {
        $packageVersion = $version
    }
    $ns = [System.xml.xmlnamespacemanager]::new($nuspecpathContent.NameTable)
    $ns.AddNamespace("ns", $nuspecpathContent.DocumentElement.NamespaceURI)
    $dependencies = $nuspecpathContent.SelectSingleNode("//ns:dependencies", $ns)
    if (!($dependencies.ChildNodes.Id | Where-Object { $_ -eq $id })) {
        Write-Host "Adding dependency to $id $packageVersion"
        $dependency = $nuspecpathContent.CreateElement("dependency", $nuspecpathContent.DocumentElement.NamespaceURI)
        $dependency.SetAttribute("id", $id)
        $dependency.SetAttribute("version", $packageVersion)
        $dependencies.AppendChild($dependency) | Out-Null
    }   
}

function AddFile {
    param(
        $src,
        $target, 
        $nuspecpathContent 
    )
    $ns = [System.xml.xmlnamespacemanager]::new($nuspecpathContent.NameTable)
    $ns.AddNamespace("ns", $nuspecpathContent.DocumentElement.NamespaceURI)
    $files = $nuspecpathContent.SelectSingleNode("//ns:files", $ns)
    if (!($files.ChildNodes.src | Where-Object { $_ -eq $src })) {
        Write-Host "Adding file $src with target $target"
        $fileElement = $nuspecpathContent.CreateElement("file", $nuspecpathContent.DocumentElement.NamespaceURI)
        $fileElement.SetAttribute("src", $src)
        $fileElement.SetAttribute("target", "$target")
        $files.AppendChild($fileElement) | Out-Null
    }
    
}
function IsLib {
    param($id)
    "*persistent.base", "*persistent.baseimpl", "*xpo*", "*utils*" | where-Object {
        $id -like $_
    } 
}


function GetModuleName {
    param(
        $nuspecpathContent
    )

    $nuspecpathContent.package.files.file.src | Where-Object { $_ -like "Xpand.ExpressApp*.dll" } | ForEach-Object {        
        $asm = [Mono.Cecil.AssemblyDefinition]::ReadAssembly("$root\Xpand.DLL\$_")
        $m = $asm.MainModule.Types | Where-Object { $_ -like "*Module" } | Select-Object -First 1
        if (!$m) {
            throw
        }
        $m
    }
}


$scriptPath = $MyInvocation.MyCommand.path
function AddAllDependency($file, $nuspecpaths) {
    [xml]$nuspecpath = Get-Content $file
    $metadata = $nuspecpath.package.metadata
    if ($metadata.dependencies) {
        $metadata.dependencies.RemoveAll()
    }
    $metadata.version = $version
    $nuspecpaths | ForEach-Object {
        AddDependency $_.BaseName $nuspecpath $Version
    }
    $nuspecpath.Save($file)
}
if ($DotSourcing) {
    return
}

$processorCount = [System.Environment]::ProcessorCount

$pArgs = @{
    scriptPath              = $scriptPath
    Version                 = $version
    Release                 = $Release
    Root                    = $root
    ResolveNugetDependecies = $ResolveNugetDependecies
}
# Get-ChildItem "$PSScriptRoot\..\Nuspec" -Exclude "ALL_*" | Invoke-Parallel -LimitConcurrency $processorCount -ActivityName "Update Nuspec" -VariablesToImport @("pArgs", "scriptPath") -Script {   
Get-ChildItem "$PSScriptRoot\..\Nuspec" -Exclude "ALL_*" | foreach {   
    Write-host "Updating $($_.BaseName)" -f Blue
    $dir = (Get-Item $scriptPath).DirectoryName
    & "$dir\UpdateNuspecs.ps1" -nuspecpathFile $($_.Fullname) @pArgs
}

$libNuspecPath = [System.io.path]::GetFullPath("$root\Support\Nuspec\Lib.nuspec")
[xml]$libNuspec = Get-Content $libNuspecPath
[xml]$libCsproj = Get-Content "$PSScriptRoot\..\..\Xpand\Xpand.Persistent\Xpand.Persistent.Base\Xpand.Persistent.Base.csproj"
$libTargetFramework = $libCsproj.project.propertygroup.targetFramework
$ns = New-Object System.Xml.XmlNamespaceManager($libNuspec.NameTable)
$ns.AddNamespace("ns", $libNuspec.DocumentElement.NamespaceURI)
"dll", "pdb" | ForEach-Object {
    $ext = $_
    "Xpand.xpo", "Xpand.Utils", "Xpand.Persistent.BaseImpl" | ForEach-Object {
        $id = "$_.$ext"
        $file = $libNuspec.CreateElement("file", $libNuspec.DocumentElement.NamespaceURI)
        $file.SetAttribute("src", $id)
        $file.SetAttribute("target", "lib\$libTargetFramework\$id")
        $libNuspec.SelectSingleNode("//ns:files", $ns).AppendChild($file)
    }
}
$libNuspec.Save($libNuspecPath)

$nuspecpathFile = "$nuspecpathsPath\All_Agnostic.nuspec"
AddAllDependency $nuspecpathFile (Get-ChildItem "$nuspecpathsPath" -Exclude "*Win*", "*Web*")

"Win", "Web" | ForEach-Object {
    $nuspecpaths = (Get-ChildItem "$nuspecpathsPath" "*$_*")
    $nuspecpathFile = "$nuspecpathsPath\All_$_.nuspec"
    AddAllDependency $nuspecpathFile $nuspecpaths
}

Get-ChildItem "$root\Support\Nuspec" *.nuspec|ForEach-Object{
    [xml]$nuspec=Get-Content $_.FullName
    $nuspec.package.metaData.dependencies.dependency|Where-Object{$_.Id -like "DevExpress*"}|ForEach-Object{
        $_.ParentNode.RemoveChild($_)
    }
    $nuspec.Save($_.FullName)
}
Get-ChildItem "$root\Support\Nuspec" *.nuspec | foreach {
    $file = $_.FullName
    $readMe = ($file -notlike "*EasyTest*" -and $file -notlike "*All_*")
    $dir = (Get-Item $scriptPath).DirectoryName
    & "$dir\PackNugets.ps1" $file $readMe  $nuget $version $root
}

$ErrorActionPreference = "stop"
$packageDir = "$root\Build\_package\$Version"
New-Item $packageDir -ItemType Directory -Force | Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$Version.zip" -path "$root\Build\Nuget\*" -Force