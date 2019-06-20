Framework "4.6"

properties {
    $version = $null
    $msbuild = $null
    $root = (Get-Item "$PSScriptRoot\..\..").FullName
    $clean=$false
    $configuration=$null
    $verbosity=$null
    $msbuildArgs=$null
    $packageSources=$null
    $dxPath=$null
    $nugetApiKey=$null
    $UseAllPackageSources=$true
    $Repository=$null
    $Brannch=$null
}

Task Release -depends Clean,InstallDX, Init,Version,RestoreNuget, CompileModules,CompileDemos,VSIX ,IndexSources, Finalize,CreateNuGets,Installer
Task Lab -depends Clean,InstallDX, Init,Version,RestoreNuget, CompileModules,CreateNuGets

Task InstallDX{
    InvokeScript{
        $version
        $dxversion=$(Get-XDevExpressVersion -Version $version -build)
        Install-XDevExpress -binPath "$PSScriptRoot\..\..\Xpand.dll" -dxSources $packageSources -sourcePath $root -dxVersion $dxversion
    }
}
Task Init  {
    InvokeScript{
        Write-Host "Remove directories"
        "$root\Xpand.dll","$root\Build","$root\Build\Temp","$root\Support\_third_party_assemblies\Packages\" | ForEach-Object{
            New-Item $_ -ItemType Directory -Force |Out-Null
        }
        Write-Host "Copying 3rd party assemlies"
        Get-ChildItem "$root\support\_third_party_assemblies\" *.dll  | ForEach-Object{
            Copy-Item $_.FullName "$root\Xpand.dll\$($_.FileName)" -Force
        }
        
        Write-Host "Update Code Analysis"
        Get-ChildItem "$root" "*.csproj" -Recurse|ForEach-Object {
            [xml]$xml=Get-Content $_.FullName
            $xml.Project.PropertyGroup|ForEach-Object{
                if ($_.CodeAnalysisRuleSet){
                    $_.ChildNodes|Where-Object{$_.Name -eq "CodeAnalysisRuleSet"}|ForEach-Object{
                        $_.ParentNode.RemoveChild($_)
                    }
                }
            }
            $xml.Project.Import|ForEach-Object{
                if ("$($_.Project)".EndsWith("Nuget.Targets")){
                    $_.ParentNode.RemoveChild($_)|Out-Null
                }
            }
            $xml.Save($_.FullName)
        }
        & "$PSScriptRoot\UpdateProjects.ps1"    
    }
}

Task Finalize {
    InvokeScript{
        Get-ChildItem "$root\Xpand.dll\" -Exclude "*.locked" | ForEach-Object{
            Copy-item $_ "$root\Build\Temp\$($_.FileName)" -Force
        }
        Copy-Item "$root\Xpand\Xpand.key\Xpand.snk" "$root\build\Xpand.snk"
    } 
}

Task CreateNuGets{
    InvokeScript{
        & "$PSScriptRoot\CreateXpandNugets.ps1" $root $version
    }
}
Task PackNuget{
    InvokeScript{
        & "$PSScriptRoot\PackNuget.ps1"  
    }
}

Task VSIX{
    InvokeScript{
        & "$PSScriptRoot\buildVSIX.ps1" "$root" $msbuild $version
    }  
}

Task Version{
    InvokeScript{
        & "$PSScriptRoot\changeversion.ps1" $root $version  
    }
}

Task RestoreNuget{
    InvokeScript {
        $sources=$packageSources
        # if ($UseAllPackageSources){
        #     $sources=$($sources+$(Get-PackageSource|select-object -ExpandProperty Location -Unique))|Select-Object -Unique
        # }
        # & "$PSScriptRoot\Restore-Nuget.ps1" -packageSources $sources 
    }   
}

Task IndexSources -precondition {$repository}{
    Get-ChildItem $root\Xpand.dll Xpand*.pdb|
    Update-XSymbols -TargetRoot "https://raw.githubusercontent.com/eXpandFramework/$repository/$branch" -sourcesRoot $root
}


Task Installer{
    InvokeScript{
        & "$PSScriptRoot\Installer.ps1" $root $version
    }
}

Task CompileModules{
    InvokeScript{
        [xml]$xml = get-content "$PSScriptRoot\Xpand.projects"
        $group=$xml.Project.ItemGroup
        $projects=($group.CoreProjects|GetProjects)+ ($group.ModuleProjects|GetProjects)
        $projects|ForEach-Object{
            $fileName=(Get-Item $_).Name
            write-host "Building $fileName..." -f "Blue"
            "packageSources=$packageSources"
            & dotnet build "$_" --output $root\Xpand.dll --configuration Release --source ($packageSources -join ";")
            if ($LASTEXITCODE) {
                throw
            }
        }

        
        Write-Host "Compiling helper projects..." -f "Blue"
        $helperProjects=($group.HelperProjects|GetProjects)
        "helperProjects=$helperProjects"
        BuildProjects $helperProjects
        $vsAddons=($group.VSAddons|GetProjects)
        "vsAddons=$vsAddons"
        BuildProjects $vsAddons

        Write-Host "Compiling Agnostic EasyTest projects..." -f "Blue"
        $agnosticEasytest=(($group.EasyTestProjects|GetProjects)|Where-Object{!("$_".Contains("Win"))  -and !("$_".Contains("Web"))}) 
        "agnosticEasytest=$agnosticEasytest"
        BuildProjects $agnosticEasytest
        
        Write-Host "Compiling Win EasyTest projects..." -f "Blue"
        $winEasyTest=(($group.EasyTestProjects|GetProjects)|Where-Object{"$_".Contains("Win")}) 
        "winEasyTest=$winEasyTest"
        BuildProjects $winEasyTest
        
        Write-Host "Compiling Web EasyTest projects..." -f "Blue"
        $webEasyTest=(($group.EasyTestProjects|GetProjects)|Where-Object{"$_".Contains("Web")}) 
        "webEasyTest=$webEasyTest"
        BuildProjects $webEasyTest
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


task CompileDemos {
    InvokeScript{
        
        [xml]$xml = get-content "$PSScriptRoot\Xpand.projects"
        $group=$xml.Project.ItemGroup
        $projects= ($group.DemoWinSolutions|GetProjects)
        
        Write-Host "Compiling win demos..." -f "Blue"
        BuildProjects $projects
        
        $projects= ($group.DemoWebSolutions|GetProjects)
        
        # Write-Host "Compiling web demos..." -f "Blue"
        # BuildProjects $projects $true
    }
}

function BuildProjects($projects,$useMsBuild ){
    $projects|ForEach-Object {
        $bargs=(@("$_","/p:OutputPath=$root\Xpand.dll\")+$msbuildArgs.Split(";"))
        if (!$useMsBuild){
            "packageSources=$packageSources"
            $o=& dotnet build "$_" --output $root\Xpand.dll --configuration Release --source ($packageSources -join ";")
        }
        else {
            $o=& $msbuild $bargs
        }
        
        if ($LASTEXITCODE){
            throw $o
        }
        Write-Output $o
    }
}



task Clean -precondition {return $clean} {
    exec {
        Set-Location $root
        if (Test-path $root\Build){
            Remove-Item $root\Build -Recurse -Force
        }
        if (Test-path $root\Xpand.dll){
            Remove-Item $root\Xpand.dll -Recurse -Force
        }
        Clear-ProjectDirectories
    }
}

task ? -Description "Helper to display task info" {
    Write-Documentation
}

function InvokeScript($sb,$maxRetries=0){
    try {
        exec $sb -maxRetries $maxRetries
    }
    catch {
        Write-Error ($_.Exception | Format-List -Force | Out-String) -ErrorAction Continue
        Write-Error ($_.InvocationInfo | Format-List -Force | Out-String) -ErrorAction Continue
        exit 1
    }
}
