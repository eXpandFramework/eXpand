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

Task Release -depends Clean,InstallDX, Init,Version,RestoreNuget, CompileModules,CompileDemos,VSIX ,IndexSources, Finalize,PackNuget,Installer
Task Lab -depends Clean,InstallDX, Init,Version,RestoreNuget, CompileModules

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

Task CreateNuspec{
    InvokeScript{
        & "$PSScriptRoot\CreateNuspec.ps1"
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
        if ($UseAllPackageSources){
            $sources=$($sources+$(Get-PackageSource|select-object -ExpandProperty Location -Unique))|Select-Object -Unique
        }
        & "$PSScriptRoot\Restore-Nuget.ps1" -packageSources $sources 
    }   
}

Task IndexSources -precondition {$repository}{
    Get-ChildItem $root\Xpand.dll Xpand*.pdb|
    Update-XSymbols -TargetRoot "https://raw.githubusercontent.com/eXpandFramework/$repository/$branch" -sourcesRoot $root
}

Task EasyTest{
    InvokeScript  {
        $xpandDll="$root\Xpand.Dll"
        $thirdPartPath="$root\Support\_third_party_assemblies\"
        [xml]$xml = get-content "$PSScriptRoot\Xpand.projects"
        $group=$xml.Project.ItemGroup
        . "$PSScriptRoot\Utils.ps1"
        $projects=($group.DemoSolutions|GetProjects)+($group.DemoTesterProjects|GetProjects)
    
        Write-Host "Compiling other projects..." -f "Blue"
        $otherProjects=$projects|Where-Object{!"$_".Contains("Web") -and !"$_".Contains("Win")}
        BuildProjects $otherProjects
    
        Write-Host "Compiling Win projects..." -f "Blue"
        $winProjects=$projects|Where-Object{"$_".Contains("Win")}
        BuildProjects $winProjects

        Write-Host "Compiling Web projects..." -f "Blue"
        $webProjects=$projects|Where-Object{"$_".Contains("Web")}
        BuildProjects $webProjects

        if (Test-path "$PSScriptRoot\easytests.txt"){
            Remove-Item "$PSScriptRoot\easytests.txt" -Force
        }
        
        get-childitem "$root\Demos\" -Filter "*.ets" -Recurse|Select-Object -First 1 -ExpandProperty FullName |Set-Content  "$xpandDll\easytests.txt"
        $reqs="$xpandDll\Xpand.utils.dll;$xpandDll\Xpand.ExpressApp.EasyTest.WinAdapter.dll;$xpandDll\Xpand.ExpressApp.EasyTest.WebAdapter.dll;$xpandDll\Xpand.EasyTest.dll;$xpandDll\Fasterflect.dll;$xpandDll\Aforge*.dll;"+
        "$xpanddll\Xpand.ExpressApp.EasyTest.WinAdapter.pdb;$xpanddll\Xpand.ExpressApp.EasyTest.WebAdapter.pdb;$xpanddll\Xpand.EasyTest.pdb;$xpanddll\Xpand.utils.pdb;"+
        "$xpandDll\psexec.exe;$xpandDll\CommandLine.dll;$xpandDll\executorwrapper.exe;$xpandDll\RDClient.exe;$(Get-DXPath $version)\Tools\eXpressAppFramework\EasyTest\TestExecutor.v$(Get-DXVersion $version).exe;$(Get-DXPath $version)\Tools\eXpressAppFramework\EasyTest\TestExecutor.v$(Get-DXVersion $version).exe.config;$thirdPartPath\AxInterop.MSTSCLib.dll;$thirdPartPath\Interop.MSTSCLib.dll;"
        $easyTests=[System.IO.File]::ReadLines("$xpandDll\easytests.txt") 
        $easyTests| ForEach-Object {
            $easyTest=$_
            if (Test-path $easyTest -ErrorAction SilentlyContinue){
                $reqs.split(';') |ForEach-Object{
                    $directory=$(Get-Item $easyTest).DirectoryName
                    if ($_){
                        if (Test-Path $_){
                            Copy-Item $_ $directory
                        }
                    }
                }
            }
        }
        Set-Location $xpandDll
        
        $r=New-Command "EasyTest" "$xpandDll\XpandTestExecutor.Win.exe" "$XpandDll\easytests.txt"
        if ($r.ExitCode){
            throw $r.stderr
        }
        get-childitem $root TestsLog.xml -Recurse|foreach{
            $xml=[xml](Get-Content $_.FullName)
            $fails=$xml.SelectNodes("/Tests/Test[@Result='Warning' or @Result='Failed']")|foreach{
                "app=$($_.ApplicationName)"
                "msg=$($_.InnerText)`r`n"
            }
            if ($fails.count -gt 0){
                $fails
                throw 
            }
        }
    }
}

Task Installer{
    InvokeScript{
        & "$PSScriptRoot\Installer.ps1" $root $version
    }
}

Task CompileModules{
    InvokeScript{
        . "$PSScriptRoot\Utils.ps1"
        [xml]$xml = get-content "$PSScriptRoot\Xpand.projects"
        $group=$xml.Project.ItemGroup
        $projects=($group.CoreProjects|GetProjects)+ ($group.ModuleProjects|GetProjects)
        $projects|ForEach-Object{
            $fileName=(Get-Item $_).Name
            write-host "Building $fileName..." -f "Blue"
            & $msbuild (GetBuildArgs "$_")
            if ($LASTEXITCODE) {
                throw
            }
        }

        
        Write-Host "Compiling helper projects..." -f "Blue"
        
        BuildProjects ($group.HelperProjects|GetProjects)
        BuildProjects ($group.VSAddons|GetProjects)

        Write-Host "Compiling Agnostic EasyTest projects..." -f "Blue"
        BuildProjects (($group.EasyTestProjects|GetProjects)|Where-Object{!("$_".Contains("Win"))  -and !("$_".Contains("Web"))}) 
        
        Write-Host "Compiling Win EasyTest projects..." -f "Blue"
        BuildProjects (($group.EasyTestProjects|GetProjects)|Where-Object{"$_".Contains("Win")}) 
        
        Write-Host "Compiling Web EasyTest projects..." -f "Blue"
        BuildProjects (($group.EasyTestProjects|GetProjects)|Where-Object{"$_".Contains("Web")}) 
    }
}
task CompileDemos {
    InvokeScript{
        [xml]$xml = get-content "$PSScriptRoot\Xpand.projects"
        . $PSScriptRoot\Utils.ps1
        $group=$xml.Project.ItemGroup
        $projects= ($group.DemoSolutions|GetProjects)+($group.DemoTesterProjects|GetProjects)
        
        Write-Host "Compiling agnostic demos..." -f "Blue"
        $otherProjects=$projects|Where-Object{!"$_".Contains("Web") -and !"$_".Contains("Win")}
        BuildProjects $otherProjects $true
        
        Write-Host "Compiling Win demos..." -f "Blue"
        $winProjects=$projects|Where-Object{"$_".Contains("Win")}
        BuildProjects $winProjects $true

        Write-Host "Compiling Web demos..." -f "Blue"
        $webProjects=$projects|Where-Object{"$_".Contains("Web")}
        BuildProjects $webProjects $true

    }

}

function BuildProjects($projects,$clean ){
    # $projects|Invoke-Parallel -ActivityName Building -VariablesToImport @("v","msbuildArgs","root","msbuild") -Script {
    $projects|ForEach-Object {
        $bargs=(@("$_","/p:OutputPath=$root\Xpand.dll\")+$msbuildArgs.Split(";"))
        $o=& $msbuild $bargs
        if ($LASTEXITCODE){
            throw $o
        }
        Write-Output $o
    }
}

function GetBuildArgs($projectPath){
    (@($projectPath,"/p:OutputPath=$root\Xpand.dll\")+$msbuildArgs)
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
