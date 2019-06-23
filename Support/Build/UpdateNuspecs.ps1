param($nuspecpathFile, $projects,$version)
function Update-NuspecDependencies {
    [CmdletBinding()]
    param (
        [parameter(Mandatory)]
        [string]$Nuspecpath,
        [parameter(Mandatory)]
        [string]$CsProjPath,
        [parameter(Mandatory)]
        [string]$Version,
        [switch]$Release

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

            $nuspecpathContent.Save($nuspecpath)

            $uArgs = @{
                NuspecFilename           = $Nuspecpath
                ProjectFileName          = $CsProjPath
                ReferenceToPackageFilter = "Xpand.*"
                PublishedSource          = (Get-PackageFeed -Xpand)
                Release                  = $Release
                ReadMe                   = $true
            }
            if ($Release) {
                $uArgs.PublishedSource = (Get-PackageFeed -Nuget)
            }
            Update-Nuspec @uArgs
        }
    }
    
    end {
    }
}

if ($nuspecpathFile.BaseName -match "lib") {
    $base = $projects | Where-Object { ($_.BaseName -match "persistent.base") } | Select-Object -First 1
    Update-NuspecDependencies $nuspecpathFile $base.FullName $version
        
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
        Update-NuspecDependencies $nuspecpathFile $_.FullName $version
    }
}

