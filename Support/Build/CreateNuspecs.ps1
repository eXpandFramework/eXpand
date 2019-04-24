Param (
    [string]$root = $(get-item "$PSScriptRoot\..\..").FullName,
    [string]$version = "19.1.201.0"
)
$ErrorActionPreference = "stop"
set-location $PSScriptRoot
$projects = Get-ChildItem "..\..\Xpand" *.csproj -Exclude "*Xpand.Test*" -Recurse
function AddDependency {
    param($id, $nuspecContent, $packageVersion)
    if (!$id){
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
function AddPackages {
    param($packageFile, $nuspecContent)
    if (Test-Path $packageFile) {
        [xml]$packageContent = Get-Content $packageFile
        $packageContent.packages.package | ForEach-Object {
            AddDependency $_.Id $nuspecContent $_.Version
        }
    }
}
function UpdateNuspec {
    param($metadata, $projectName)
    if ($metadata.dependencies) {
        $metadata.dependencies.RemoveAll()
    }
    if ($projectName -match "lib") {
        $projects | Where-Object { (IsLib $_.BaseName) } | ForEach-Object { 
            AddPackages "$($_.DirectoryName)\packages.config" $nuspecContent
        }
    }
    else {
        $projects | Where-Object {
            $name = $_.BaseName
            if ($projectName -like "System") {
                $name -eq "Xpand.ExpressApp"
            }
            elseif ($projectName -like "System.Win") {
                $name -eq "Xpand.ExpressApp.Win"
            }
            elseif ($projectName -like "System.Web") {
                $name -eq "Xpand.ExpressApp.Web"
            }
            else {
                $name -like "*.$projectName" -and $projectName -notlike "Lib"
            }
        } | ForEach-Object {
            [xml]$csprojContent = Get-Content $_.FullName
        
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
    
            $packageFile = "$($_.DirectoryName)\packages.config"
            AddPackages $packageFile $nuspecContent
        }
    }
}
Get-ChildItem "..\Nuspec" -Exclude "ALL_*" | ForEach-Object {
    $projectName = $_.BaseName
    
    Write-Host "Updating $projectName.nuspec"
    [xml]$nuspecContent = Get-Content $_.FullName
    $metadata = $nuspecContent.package.metadata
    UpdateNuspec $metadata $projectName
    $nuspecContent.Save($_.FullName)
}

function AddAllDependency($file,$nuspecs) {
    [xml]$nuspec=Get-Content $file
    $nuspecs | ForEach-Object {
        AddDependency $_.BaseName $nuspec $Version
    }
    $nuspec.Save($file)
}

AddAllDependency "..\Nuspec\All_Agnostic.nuspec" (Get-ChildItem "..\Nuspec" -Exclude "*Win*", "*Web*")
"Win","Web"|ForEach-Object{
    $nuspecs=(Get-ChildItem "..\Nuspec" "*$_*")
    AddAllDependency "..\Nuspec\All_$_.nuspec" $nuspecs
}