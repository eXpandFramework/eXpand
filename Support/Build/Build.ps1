
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
    $Repository=$null
    $Release=$false
    $ResolveNugetDependecies=$false
}


Task Release -depends Clean, Init,Version, CompileModules,CompileDemos,VSIX ,IndexSources, Finalize,CreateNuGets,Installer
Task Lab -depends Clean,Init,Version,CompileModules,CompileDemos


Task Init  {
    InvokeScript{
        Write-HostFormatted "Remove directories" -Section
        "$root\Xpand.dll","$root\Build","$root\Build\Temp","$root\Support\_third_party_assemblies\Packages\" | ForEach-Object{
            New-Item $_ -ItemType Directory -Force |Out-Null
        }
        Write-HostFormatted "Copying 3rd party assemlies" -Section
        Get-ChildItem "$root\support\_third_party_assemblies\" *.dll  | ForEach-Object{
            Copy-Item $_.FullName "$root\Xpand.dll\$($_.FileName)" -Force
        }
        
        Write-HostFormatted  "Update Code Analysis" -Section
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
        & "$PSScriptRoot\CreateXpandNugets.ps1" $root $version $ResolveNugetDependecies $Release
    }
}
Task PackNuget{
    InvokeScript{
        & "$PSScriptRoot\PackNuget.ps1"  
    }
}

Task VSIX{
    InvokeScript{
        & "$PSScriptRoot\buildVSIX.ps1" "$root" $msbuild $version ($packageSources -join ";")
    }  
}

Task Version{
    InvokeScript{
        & "$PSScriptRoot\changeversion.ps1" $root $version  
    }
}

Task IndexSources -precondition {$repository}{
    $sha=Get-GitLastSha "https://github.com/eXpandFramework/$repository" $branch
    Get-ChildItem $root\Xpand.dll Xpand*.pdb|
    Update-XSymbols -TargetRoot "https://raw.githubusercontent.com/eXpandFramework/$repository/$sha" -sourcesRoot $root
}


Task Installer{
    InvokeScript{
        & "$PSScriptRoot\Installer.ps1" $root $version
    }
}

Task CompileModules{
    dotnet tool restore
    Set-Location $root
    Invoke-PaketRestore -Install -Strict
    InvokeScript -maxRetries 3 {
        [xml]$xml = get-content "$PSScriptRoot\Xpand.projects"
        $group=$xml.Project.ItemGroup
        $compileArgs=$msbuildArgs
        $compileArgs+="/fl"
        $compileArgs+="/bl:$root\Xpand.dll\CompileModules.binlog"
        $compileArgs+="-m"
        dotnet restore "$root\Xpand\Xpand.ExpressApp.Modules\AllModules.sln" --source ($packageSources -join ";") --verbosity m
        dotnet msbuild "$root\Xpand\Xpand.ExpressApp.Modules\AllModules.sln" @compileArgs
        if ($LASTEXITCODE){
            throw
        }
        
        Write-HostFormatted "Compiling helper projects..." -Section
        $helperProjects=($group.HelperProjects|GetProjects)
        Write-HostFormatted "helperProjects=$helperProjects" -ForegroundColor Magenta
        BuildProjects $helperProjects "Helper"
        $vsAddons=($group.VSAddons|GetProjects)
        Write-HostFormatted "vsAddons=$vsAddons" -ForegroundColor Magenta
        Push-Location "$root\Xpand.Plugins"
        Get-Content ".\paket.dependencies" -Raw
        Invoke-PaketRestore -Install -Strict
        BuildProjects $vsAddons "VSIX"
        Pop-Location
        
        Write-HostFormatted "Compiling Agnostic EasyTest projects..." -Section
        $agnosticEasytest=(($group.EasyTestProjects|GetProjects)|Where-Object{!("$_".Contains("Win"))  -and !("$_".Contains("Web"))}) 
        "agnosticEasytest=$agnosticEasytest"
        BuildProjects $agnosticEasytest "EasyTest"
        
        Write-HostFormatted "Compiling Win EasyTest projects..." -Section
        $winEasyTest=(($group.EasyTestProjects|GetProjects)|Where-Object{"$_".Contains("Win")}) 
        "winEasyTest=$winEasyTest"
        BuildProjects $winEasyTest "EasyTest"
        
        Write-HostFormatted "Compiling Web EasyTest projects..." -Section
        $webEasyTest=(($group.EasyTestProjects|GetProjects)|Where-Object{"$_".Contains("Web")}) 
        "webEasyTest=$webEasyTest"
        BuildProjects $webEasyTest "EasyTest"
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
        
        Write-HostFormatted "Compiling win demos..." -Section
        BuildProjects $projects "Demos"
        
        $projects= ($group.DemoWebSolutions|GetProjects)
        
        # Write-Host "Compiling web demos..." -f "Blue"
        # BuildProjects $projects $true
    }
}

function BuildProjects($projects,$buildName ){
    $projects|ForEach-Object {
        $bargs=(@("$_","/p:OutputPath=$root\Xpand.dll\")+$msbuildArgs.Split(";"))
        if (!$useMsBuild){
            "packageSources=$packageSources"
            dotnet restore "$_" --source ($packageSources -join ";")
            $compileArgs=$msbuildArgs
            $compileArgs+="/bl:$root\Xpand.dll\helper.binlog"
            dotnet msbuild "$_" @compileArgs
            # $o=& dotnet build "$_"  --output $root\Xpand.dll --configuration Release --source ($packageSources -join ";") /WarnAserror
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
        Clear-ProjectDirectories $root
        
    }
}

task ? -Description "Helper to display task info" {
    Write-Documentation
}

function InvokeScript($sb,$maxRetries=0){
    try {
        exec $sb -maxRetries $maxRetries
        Approve-LastExitCode
    }
    catch {
        Write-Error ($_.Exception | Format-List -Force | Out-String) -ErrorAction Continue
        Write-Error ($_.InvocationInfo | Format-List -Force | Out-String) -ErrorAction Continue
        exit 1
    }
}
