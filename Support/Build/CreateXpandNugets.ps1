Param (
    [string]$root = (Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$version = "19.1.303.7",
    [switch]$Release ,
    [switch]$DotSourcing 
    
)

Use-MonoCecil | Out-Null

if (!$DotSourcing) {
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

function PackNuspec($Nuspecpath, $ReadMe, $nuget,$version) {
    [xml]$nuspecpathContent = Get-Content $Nuspecpath
    $file = $nuspecpathContent.package.files.file | Where-Object { $_.src -match "readme.txt" } | Select-Object -First 1
    if ($file) {
        $file.ParentNode.RemoveChild($file)
        $nuspecpathContent.Save($Nuspecpath)
    }
    if ($ReadMe) {
        $moduleName = GetModuleName $nuspecpathContent
        Write-host $moduleName
        $readMePath = "$env:temp\$moduleName.Readme.txt"
        Remove-Item $readMePath -Force -ErrorAction SilentlyContinue
        $nuspecpathContent.package.files.file | Where-Object { $_.src -match "readme.txt" } | ForEach-Object {
            $_.ParentNode.RemoveChild($_)
        }
        $message = @"
        
        ➤ ​̲𝗣​̲𝗟​̲𝗘​̲𝗔​̲𝗦​̲𝗘​̲ ​̲𝗦​̲𝗨​̲𝗦​̲𝗧​̲𝗔​̲𝗜​̲𝗡​̲ ​̲𝗢​̲𝗨​̲𝗥​̲ ​̲𝗔​̲𝗖​̲𝗧​̲𝗜​̲𝗩​̲𝗜​̲𝗧​̲𝗜​̲𝗘​̲𝗦

            ☞  Iғ ᴏᴜʀ ᴘᴀᴄᴋᴀɢᴇs ᴀʀᴇ ʜᴇʟᴘɪɴɢ ʏᴏᴜʀ ʙᴜsɪɴᴇss ᴀɴᴅ ʏᴏᴜ ᴡᴀɴᴛ ᴛᴏ ɢɪᴠᴇ ʙᴀᴄᴋ ᴄᴏɴsɪᴅᴇʀ ʙᴇᴄᴏᴍɪɴɢ ᴀ SPONSOR ᴏʀ ᴀ BACKER.
                https://opencollective.com/expand
                
            ☞  ɪғ ʏᴏᴜ ʟɪᴋᴇ ᴏᴜʀ ᴡᴏʀᴋ ᴘʟᴇᴀsᴇ ᴄᴏɴsɪᴅᴇʀ ᴛᴏ ɢɪᴠᴇ ᴜs ᴀ STAR.
                https://github.com/eXpandFramework/eXpand/stargazers 

        ➤ ​​̲𝗣​̲𝗮​̲𝗰​̲𝗸​̲𝗮​̲𝗴​̲𝗲​̲ ​̲𝗻​̲𝗼​̲𝘁​̲𝗲​̲𝘀

            ☞ Build the project before opening the model editor.
            
            ☞ The package only adds the required references. To install $moduleName add the next line in the constructor of your XAF module.
                RequiredModuleTypes.Add(typeof($moduleName));
"@
        Set-Content $readMePath $message
        AddFile $readMePath "" $nuspecpathContent
        $nuspecpathContent.Save($Nuspecpath)
    }
    
    & $Nuget Pack $Nuspecpath -version ($Version) -OutputDirectory "$root\Build\Nuget" -BasePath "$root\Xpand.DLL"
    if ($LASTEXITCODE) {
        throw
    }
    if ($ReadMe) {
        $file = $nuspecpathContent.package.files.file | Where-Object { $_.src -match "readme.txt" } | Select-Object -First 1
        $file.src = "Readme.txt"
        $nuspecpathContent.Save($Nuspecpath)
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

Get-ChildItem "$PSScriptRoot\..\Nuspec" -Exclude "ALL_*" |ForEach-Object{
    $dArgs=@{
        DotSourcing=$true
        Version=$version
        Release=$Release
    }
    . $scriptPath @dArgs
    Write-host "Updating $($_.BaseName)" -f Blue
    $sb={
        param($nuspecpathFile, $projects,$scriptPath,$version,$Release)
        & $scriptPath\UpdateNuspecs.ps1 $_ $projects $version $Release
    }
    
    Invoke-Command -ScriptBlock $sb  -ArgumentList @($_,$projects,$scriptPath,$version,$Release) -AsJob -ComputerName $([System.Environment]::MachineName)
}
Get-Job|Wait-Job 

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

Get-ChildItem "$root\Support\Nuspec" *.nuspec | Invoke-Parallel -ActivityName "Packaging" -VariablesToImport @("nuget","scriptPath","version","Release") -LimitConcurrency $processorCount  -Script{
    $dArgs=@{
        DotSourcing=$true
        Version=$version
        Release=$Release
    }
    . $scriptPath @dArgs
    $file = $_.FullName
    $readMe = ($file -notlike "*EasyTest*" -and $file -notlike "*All_*")
    PackNuspec $file $readMe  $nuget $version
}
$ErrorActionPreference = "stop"
$packageDir = "$root\Build\_package\$Version"
New-Item $packageDir -ItemType Directory -Force | Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$Version.zip" -path "$root\Build\Nuget\*" -Force