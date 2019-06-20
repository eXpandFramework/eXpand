Param (
    [string]$root = (Get-Item "$PSScriptRoot\..\..").FullName,
    [string]$version = "19.1.303.7",
    [switch]$Release 
)
import-module XpandPwsh -prefix X -force
Use-XMonoCecil 
Write-Host "root=$root"
$projects = Get-ChildItem "$PSScriptRoot\..\..\Xpand" *.csproj -Exclude "*Xpand.Test*" -Recurse
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
function Update-NuspecDependencies {
    [CmdletBinding()]
    param (
        [parameter(Mandatory)]
        [string]$Nuspecpath,
        [parameter(Mandatory)]
        [string]$CsProjPath
    )
    
    begin {
    }
    
    process {
        [xml]$project = Get-Content $CsProjPath
        if ($project.project.ItemGroup.None.Include | Where-Object { $_ -eq "packages.config" }) {
            $directory = (Get-Item $CsProjPath).DirectoryName
            $outputPath = $project.project.propertygroup | Where-Object { $_.Condition -match "Release" } | ForEach-Object { $_.OutputPath } | Select-Object -First 1
            $assemblyName = $project.project.propertygroup.AssemblyName | Select-Object -First 1
            [xml]$packageContent = Get-Content "$directory\packages.config"
            $dependencies = Resolve-AssemblyDependencies "$directory\$outputPath\$assemblyName.dll" -ErrorAction SilentlyContinue | ForEach-Object {
                $_.GetName().Name
            }
            
            [xml]$nuspecpathContent = Get-Content $Nuspecpath
            $metadata = $nuspecpathContent.package.metadata
            $metadata.version = $version
            if ($metadata.dependencies) {
                $metadata.dependencies.RemoveAll()
            }
            $packageContent.packages.package | Where-Object { $_ } | where-Object { !$_.developmentDependency -and $_.Id -in $dependencies } | ForEach-Object {
                AddDependency $_.Id $nuspecpathContent $_.Version
            }
            $metadata.dependencies.dependency
            $nuspecpathContent.Save($nuspecpath)
        }
        else {
            
            [xml]$nuspecpathContent = Get-Content $Nuspecpath
            $metadata = $nuspecpathContent.package.metadata
            $metadata.version = $version
            if ($metadata.dependencies) {
                $metadata.dependencies.RemoveAll()
            }
            if ($nuspecpathContent.package.files) {
                $nuspecpathContent.package.files.RemoveAll()
            }

            $nuspecpathContent.Save($nuspecpath)

            $uArgs = @{
                NuspecFilename           = $Nuspecpath
                ProjectFileName          = $CsProjPath
                ReferenceToPackageFilter = "Xpand.*"
                PublishedSource          = (Get-PackageFeed -Xpand)
                Release                  = $Release
                ReadMe                   = $true
            }
            Update-Nuspec @uArgs
        }
    }
    
    end {
    }
}

function UpdateNuspec {
    param($nuspecpathFile)
    
    if ($nuspecpathFile.BaseName -match "lib") {
        $base = $projects | Where-Object { ($_.BaseName -match "persistent.base") } | Select-Object -First 1
        Update-NuspecDependencies $nuspecpathFile $base.FullName
        
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
        } | ForEach-Object {
            Update-NuspecDependencies $nuspecpathFile $_.FullName
        }
    }
    
}

$nuget = "$(Get-XNugetPath)"


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

function PackNuspec($Nuspecpath, $ReadMe = $true) {
    [xml]$nuspecpathContent = Get-Content $Nuspecpath
    
    if ($ReadMe) {
        $moduleName = GetModuleName $nuspecpathContent
        Write-host $moduleName
        Remove-Item "$root\Xpand.DLL\Readme.txt" -Force -ErrorAction SilentlyContinue
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
        Set-Content "$root\Xpand.DLL\Readme.txt" $message
        AddFile "ReadMe.txt" "" $nuspecpathContent
    }
    
    & $Nuget Pack $Nuspecpath -version ($Version) -OutputDirectory "$root\Build\Nuget" -BasePath "$root\Xpand.DLL"
}

Get-ChildItem "$PSScriptRoot\..\Nuspec" -Exclude "ALL_*" | ForEach-Object {
    Write-Host "Updating $($_.BaseName).nuspec" -f Blue
    UpdateNuspec $_
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

$nuspecpathFile = "$nuspecpathsPath\All_Agnostic.nuspec"
AddAllDependency $nuspecpathFile (Get-ChildItem "$nuspecpathsPath" -Exclude "*Win*", "*Web*")

"Win", "Web" | ForEach-Object {
    $nuspecpaths = (Get-ChildItem "$nuspecpathsPath" "*$_*")
    $nuspecpathFile = "$nuspecpathsPath\All_$_.nuspec"
    AddAllDependency $nuspecpathFile $nuspecpaths
}
Get-ChildItem "$root\Support\Nuspec" *.nuspec | ForEach-Object {
    $file = $_.FullName
    $readMe = ($file -notlike "*EasyTest*" -and $file -notlike "*All_*")
    PackNuspec $file $readMe
}
$ErrorActionPreference = "stop"
$packageDir = "$root\Build\_package\$Version"
New-Item $packageDir -ItemType Directory -Force | Out-Null
Compress-Archive -DestinationPath "$packageDir\Nupkg-$Version.zip" -path "$root\Build\Nuget\*" -Force