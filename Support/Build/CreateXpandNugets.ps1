Param (
    [string]$root = (Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$version = "20.2.401.3",
    [bool]$ResolveNugetDependecies,
    [bool]$Release 
)


Write-HostFormatted "Create Nuget for $version"
$ErrorActionPreference = "Stop"
Use-MonoCecil | Out-Null
Write-HostFormatted "Update CopySymbols scripts" -section
$branch = "master"
if (([version]$version).Revision -gt 0) {
    $branch = "lab"
}
$c = [System.Net.WebClient]::new()
"ps1", "targets" | ForEach-Object {
    $script = $c.DownloadString("https://raw.githubusercontent.com/eXpandFramework/DevExpress.XAF/$branch/Build/targets/CopySymbols.$_")
    Set-Content "$PSScriptRoot\CopySymbols.$_" $script
}

if (!$projects) {
    $projects = Get-ChildItem "$PSScriptRoot\..\..\Xpand" *.csproj -Exclude "*Xpand.Test*" -Recurse
}
$nuget = "$(Get-NugetPath)"
$nuspecpathsPath = "$PSScriptRoot\..\Nuspec"


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

# $nuspecs|Where-Object{$_.FileInfo.baseName -eq "System.Win"} | ForEach-Object {   
$nuspecs| Invoke-Parallel  -VariablesToImport "nuspecs","projects" -Script {   
    $name = ($_.FileInfo.BaseName)
    Write-Output "--------------------Updating $name-----------------------" 
    $project = $projects | Where-Object { $_.BaseName -eq "Xpand.ExpressApp.$name" }
    if ($name -like "System*") {
        $name = $name.Replace("System", "")
        $project = $projects | Where-Object { $_.BaseName -eq "Xpand.ExpressApp$name" }
    }
    elseif (!$project) {
        if ($name -eq "lib") {
            $project = $projects | Where-Object { $_.BaseName -in "Xpand.Persistent.Base", "Xpand.Utils", "Xpand.Xpo" }
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
        Push-Location $_.DirectoryName
        [xml]$csproj = Get-Content $_.FullName
        
        $xpandMoleReference = $csproj.project.ItemGroup.Reference | Where-Object { $_.include -like "Xpand.ExpressApp*" }
        if ($name -ne "lib") {
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
        Pop-Location
    }
    
    $sortedDeps = $nuspec.package.metadata.dependencies.dependency | Sort-Object id -Unique
    if ($nuspec.package.metadata.dependencies) {
        $nuspec.package.metadata.dependencies.RemoveAll()
    }
    $sortedDeps | Add-NuspecDependency -Nuspec $nuspec
    if (!($nuspec.Package.Files.File.src | Select-String "CopySymbols")) {
        $a = [ordered]@{
            src    = "..\Support\build\CopySymbols.targets"
            target = "build\$($nuspec.package.metadata.id).targets" 
        }
        Add-XmlElement -Owner $nuspec -elementname "file" -parent "files"-Attributes $a
        $a = [ordered]@{
            src    = "..\Support\build\CopySymbols.ps1"
            target = "build\CopySymbols.ps1" 
        } 
        Add-XmlElement -Owner $nuspec -elementname "file" -parent "files"-Attributes $a
    }
    
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
    "Xpand.xpo", "Xpand.Utils", "Xpand.Persistent.BaseImpl", "Xpand.Persistent.Base" | ForEach-Object {
        $id = "$_.$ext"
        Add-XmlElement -Owner $libNuspec -ElementName "file" -Parent "files" -Attributes ([ordered]@{
                src    = $id
                target = "lib\$libTargetFramework\$id"
            })
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

Write-HostFormatted "Discover XAFModules" -Section
$assemblyList = Get-ChildItem "$root\xpand.dll" *.dll
$modulesJson = "$root\support\build\modules.json"
if (Test-Path $modulesJson) {
    $modules = Get-Content $modulesJson | ConvertFrom-Json
    if ($assemblyList | Where-Object { !$modules.Assembly.Contains($_.BaseName) -and $_.BaseName -like "Xpand.ExpressApp.*" -and $_.BaseName -notlike "*EasyTest*" }) {
        Get-XAFModule "$root\Xpand.dll" -Include "Xpand.ExpressApp.*" -AssemblyList $assemblyList -Verbose | ConvertTo-Json | Set-Content $modulesJson    
        $modules = Get-Content $modulesJson | ConvertFrom-Json | Sort-Object Name
    }
}
else {
    $m = Get-XAFModule "$root\Xpand.dll" -Include "Xpand.ExpressApp.*" -AssemblyList $assemblyList -Verbose | ForEach-Object {
        $_.Assembly = [System.IO.Path]::GetFileNameWithoutExtension($_.Assembly)
        $_
    }
    $m | ConvertTo-Json | Set-Content $modulesJson
    $modules | Sort-Object Name=Get-Content $modulesJson | ConvertFrom-Json | Sort-Object Name
}

$modules 

Write-HostFormatted "packing nuspecs"
Get-ChildItem "$root\build\Nuget" -ErrorAction SilentlyContinue | Remove-Item -Force -Recurse

Get-ChildItem "$root\Support\Nuspec" *.nuspec | Invoke-Parallel -RetryOnError 3 -VariablesToImport @("modules","Nuget","version","root") -Script {    
# $nuspecs = Get-ChildItem "$root\Support\Nuspec" *.nuspec
# $nuspecs | foreach {    
    if (!$Version) {
        throw
    }
    $packageVersion = $Version
    $readmePath = "$($_.DirectoryName)\$($_.BaseName)"
    Write-Output "AddReadme $($_.FullName)"
    [xml]$nuspec = Get-Content $_.FullName
    $nuspec.package.files.file | Where-Object { $_.src -like "*readme.txt" } | ForEach-Object {
        $_.parentnode.removechild($_)
    }
    Add-XmlElement -Owner $nuspec -ElementName "file" -Parent "files" -Attributes ([ordered]@{
            src    = "$readmePath\Readme.txt"
            target = ""
        })
    $nuspec.Save($_.FullName)

    $Package = $_.BaseName
    $module = $modules | Where-Object { $_.assembly.Replace("Xpand.ExpressApp.", "") -eq $Package }
    if ($package -eq "System") {
        $module = $modules | Where-Object { $_.Name -eq "XpandSystemModule" }
    }
    elseif ($package -eq "System.Web") {
        $module = $modules | Where-Object { $_.Name -eq "XpandSystemAspNetModule" }
    }
    elseif ($package -eq "System.Win") {
        $module = $modules | Where-Object { $_.Name -eq "XpandSystemWindowsFormsModule" }
    }
    $moduleName = $module.FullName
    New-Item $readmePath -ItemType Directory -Force
    if (!$moduleName -and $package -notlike "*all*" -and $package -notlike "*easytest*" -and $package -notin @("lib", "Ncarousel")) {
        throw $_
    }
    "moduleName=$moduleName"
    $registration = "RequiredModuleTypes.Add(typeof($moduleName));"
    if ($package -like "*all*") {
        $registration = ($modules | Where-Object { $_.platform -eq "Core" -or $package -like "*$($_.platform)*" } | ForEach-Object { "RequiredModuleTypes.Add(typeof($($_.FullName)));" }) -join "`r`n                                                "
    }
    elseif ("*lib*", "*easytest*" | Where-Object { $package -like $_ }) {
        $registration = $null
    }
    if ($registration) {
        $registrationMessage = "The package only adds the required references. To install $moduleName add the next line in the constructor of your XAF module."
    }
    
    $message = @"


    
++++++++++++++++++++++++  ++++++++       ➤ 𝗘𝗫𝗖𝗟𝗨𝗦𝗜𝗩𝗘 𝗦𝗘𝗥𝗩𝗜𝗖𝗘𝗦 @
++++++++++++++++++++++##  ++++++++          http://apobekiaris.expandframework.com
++++++++++++++++++++++  ++++++++++
++++++++++    ++++++  ++++++++++++       ➤  ɪғ ʏᴏᴜ ʟɪᴋᴇ ᴏᴜʀ ᴡᴏʀᴋ ᴘʟᴇᴀsᴇ ᴄᴏɴsɪᴅᴇʀ ᴛᴏ ɢɪᴠᴇ ᴜs ᴀ STAR.
++++++++++++  ++++++  ++++++++++++          https://github.com/eXpandFramework/eXpand/stargazers
++++++++++++++  ++  ++++++++++++++
++++++++++++++    ++++++++++++++++       ➤ Package Notes
++++++++++++++  ++  ++++++++++++++
++++++++++++  ++++    ++++++++++++          ☞ Build the project before opening the model editor.
++++++++++  ++++++++  ++++++++++++          ☞ $registrationMessage
++++++++++  ++++++++++  ++++++++++              $registration
++++++++  ++++++++++++++++++++++++
++++++  ++++++++++++++++++++++++++        
"@
    
    Set-Content "$readmePath\ReadMe.txt" $message
    "$Nuget Pack $($_.FullName) -version $packageVersion -OutputDirectory $root\build\nuget -BasePath $root\Xpand.DLL"
    $result = & $Nuget Pack $_.FullName -version $packageVersion -OutputDirectory "$root\build\nuget" -BasePath "$root\Xpand.DLL"
    Write-Output $result
    Remove-Item $readmePath -Force -Recurse
    
    if ($nuspec.package.files.file) {
        $file = $nuspec.package.files.file | Where-Object { $_.src -like "*Readme*" }
        if ($file) {
            $nuspec.package.files.removechild($file)
        }
        $nuspec.Save($_.FullName)
    }
    
    
}
if ((Get-ChildItem "$PSScriptRoot\..\Nuspec" *.nuspec).Count -ne (Get-ChildItem "$root\Build\Nuget").count) {
    throw "Nugget count does not match nuspec"
}

$packageDir = "$root\Build\_package\$Version"
New-Item $packageDir -ItemType Directory -Force | Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$Version.zip" -path "$root\Build\Nuget\*" -Force