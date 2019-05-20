Param (
    [string]$root = $(get-item "$PSScriptRoot\..\..").FullName,
    [string]$version = "19.1.201.0"
)

$ErrorActionPreference = "stop"
set-location $PSScriptRoot
$projects = Get-ChildItem "..\..\Xpand" *.csproj -Exclude "*Xpand.Test*" -Recurse
function AddDependency {
    param($id, $nuspecContent, $packageVersion)
    if (!$id) {
        return
    }
    if (!$packageVersion) {
        $packageVersion = $version
    }
    $ns = [System.xml.xmlnamespacemanager]::new($nuspecContent.NameTable)
    $ns.AddNamespace("ns", $nuspecContent.DocumentElement.NamespaceURI)
    $dependencies = $nuspecContent.SelectSingleNode("//ns:dependencies", $ns)
    if (!($dependencies.ChildNodes.Id | Where-Object { $_ -eq $id })) {
        Write-Host "Adding dependency to $id $packageVersion"
        $dependency = $nuspecContent.CreateElement("dependency", $nuspecContent.DocumentElement.NamespaceURI)
        $dependency.SetAttribute("id", $id)
        $dependency.SetAttribute("version", $packageVersion)
        $dependencies.AppendChild($dependency) | Out-Null
    }
    
}
function IsLib {
    param($id)
    "*persistent.base", "*persistent.baseimpl", "*xpo*", "*utils*" | where-Object {
        $id -like $_
    } 
}
function Update-Nuspec {
    [CmdletBinding()]
    param (
        [parameter(Mandatory)]
        [string]$Nuspec,
        [parameter(Mandatory)]
        [string]$CsProj
    )
    
    begin {
    }
    
    process {
        [xml]$project = Get-Content $CsProj
        if ($project.project.ItemGroup.None.Include | Where-Object { $_ -eq "packages.config" }) {
            $directory = (Get-Item $CsProj).DirectoryName
            $outputPath = $project.project.propertygroup | Where-Object { $_.Condition -match "Release" } | ForEach-Object { $_.OutputPath } | Select-Object -First 1
            $assemblyName = $project.project.propertygroup.AssemblyName | Select-Object -First 1
            [xml]$packageContent = Get-Content "$directory\packages.config"
            $dependencies = Resolve-AssemblyDependencies "$directory\$outputPath\$assemblyName.dll" -ErrorAction SilentlyContinue | ForEach-Object {
                $_.GetName().Name
            }
            
            [xml]$nuspecContent = Get-Content $Nuspec
            $metadata = $nuspecContent.package.metadata
            if ($metadata.dependencies) {
                $metadata.dependencies.RemoveAll()
            }
            $packageContent.packages.package | Where-Object { $_ } | where-Object { !$_.developmentDependency -and $_.Id -in $dependencies } | ForEach-Object {
                AddDependency $_.Id $nuspecContent $_.Version
            }
            $metadata.dependencies.dependency
            $nuspecContent.Save($nuspec)
        }
    }
    
    end {
    }
}

function AddVersionConverterDependency {
    param(
        $CsProj
    )
    [xml]$project = Get-Content $CsProj
    if ($project.project.ItemGroup.None.Include | Where-Object { $_ -eq "packages.config" }) {
        $directory = (Get-Item $CsProj).DirectoryName
        [xml]$packageContent = Get-Content "$directory\packages.config"
        $versionConverter = $packageContent.packages.package | Where-Object { $_.id -match "VersionConverter" } | Select-Object -First 1
        if ($versionConverter) {
            [xml]$nuspecContent = Get-Content $nuspecFile
            AddDependency $versionConverter.id $nuspecContent $versionConverter.Version
            $nuspecContent.Save($nuspecFile)
        }
    }
}

function UpdateNuspec {
    param($nuspecFile)
    
    if ($nuspecFile.BaseName -match "lib") {
        $base = $projects | Where-Object { ($_.BaseName -match "persistent.base") } | Select-Object -First 1
        Update-Nuspec $nuspecFile $base.FullName
        AddVersionConverterDependency $base.FullName
    }
    else {
        $projects | Where-Object {
            $name = $_.BaseName
            if ($nuspecFile.BaseName -like "System") {
                $name -eq "Xpand.ExpressApp"
            }
            elseif ($nuspecFile.BaseName -like "System.Win") {
                $name -eq "Xpand.ExpressApp.Win"
            }
            elseif ($nuspecFile.BaseName -like "System.Web") {
                $name -eq "Xpand.ExpressApp.Web"
            }
            else {
                $name -like "*.$($nuspecFile.BaseName)" -and $nuspecFile.BaseName -notlike "Lib"
            }
        } | ForEach-Object {
            [xml]$csprojContent = Get-Content $_.FullName
            Update-Nuspec $nuspecFile $_.FullName
            AddVersionConverterDependency $_.FullName
            [xml]$nuspecContent = Get-Content $nuspecFile
            $csprojContent.Project.ItemGroup.Reference | Where-Object { 
                if ($_.Include -like "Xpand*") {
                    $_.Include -notlike "Xpand.Vers*" -and $_.Include -notlike "Xpand.XAF*"
                }
            } | ForEach-Object {
                $id = $_.Include.Replace("Xpand.ExpressApp", "eXpand").Replace(".", "")
                if ($id -eq "eXpand") {
                    $id = "eXpandSystem"
                }
                elseif ($id -eq "eXpandWin") {
                    $id = "eXpandSystemWin"
                }
                elseif ($id -eq "eXpandWeb") {
                    $id = "eXpandSystemWeb"
                }
                elseif (IsLib $_.Include ) {
                    $id = "eXpandLib"
                }
                if ($id) {
                    $id
                }
            } | Get-Unique | ForEach-Object {
                AddDependency $_ $nuspecContent
            }
            $nuspecContent.Save($nuspecFile)
        }
    }
    
}
Get-ChildItem "..\Nuspec" -Exclude "ALL_*" | ForEach-Object {
    Write-Host "Updating $($_.BaseName).nuspec" -f Blue
    UpdateNuspec $_
}

function AddAllDependency($file, $nuspecs) {
    [xml]$nuspec = Get-Content $file
    $metadata = $nuspec.package.metadata
    if ($metadata.dependencies) {
        $metadata.dependencies.RemoveAll()
    }
    $nuspecs | ForEach-Object {
        AddDependency $_.BaseName $nuspec $Version
    }
    $nuspec.Save($file)
}

AddAllDependency "$PSScriptRoot\..\Nuspec\All_Agnostic.nuspec" (Get-ChildItem "$PSScriptRoot\..\Nuspec" -Exclude "*Win*", "*Web*")
"Win", "Web" | ForEach-Object {
    $nuspecs = (Get-ChildItem "$PSScriptRoot\..\Nuspec" "*$_*")
    AddAllDependency "$PSScriptRoot\..\Nuspec\All_$_.nuspec" $nuspecs
}