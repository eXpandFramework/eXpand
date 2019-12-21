param(
    $nugetBin = "$PSScriptRoot\..\..\build\Nuget",
    $BuildBin = "$nugetbin\..\Temp"
)

$packages = & (Get-NugetPath) list -source $nugetBin | ConvertTo-PackageObject
$assemblyList = (Get-ChildItem $BuildBin *.dll)
Write-HostFormatted "Extracting Packages" -Section
Get-ChildItem $nugetBin | Where-Object { $_ -is [System.IO.DirectoryInfo] } | Remove-Item -Force -Recurse

$unzipDirs = $packages | Invoke-Parallel -VariablesToImport "nugetBin" -Script {
    
    $baseName = "$($_.Id).$($_.Version)"
    $zip = "$nugetbin\$baseName.zip" 
    $nupkgPath = "$nugetBin\$baseName.nupkg"
    Move-Item $nupkgPath $zip -Force
    $unzipDir = "$nugetBin\$baseName"
    Expand-Archive $zip $unzipDir -Force
    Move-Item $zip $nupkgPath -Force
    [PSCustomObject]@{
        Path    = $unzipDir
        Package = $_
    }
}
$unzipDirs

Write-HostFormatted "Discover XAFModules" -Section
"BuildBin=$BuildBin"
$modules = Get-XAFModule $BuildBin -Include "Xpand.ExpressApp.*" -AssemblyList $assemblyList | ForEach-Object {
    [PSCustomObject]@{
        FullName = $_.FullName
        platform = (Get-AssemblyMetadata $_.Assembly -key platform).value
    }
}
$modules 
Write-HostFormatted "Adding Readme" -Section

$unzipDirs| Invoke-Parallel -VariablesToImport "assemblyList", "modules" -Script {
    $Package=$_.package
    $Directory=$_.Path
    $moduleName = (Get-XAFModule $Directory $assemblyList).Name
    $wikiName = $moduleName
    if (!$wikiName) {
        $wikiName = "Modules"
    }
    $registration = "RequiredModuleTypes.Add(typeof($moduleName));"
    if ($package.Id -like "*all*") {
        $registration = ($modules | Where-Object { $_.platform -eq "Core" -or $package.id -like "*$($_.platform)*" } | ForEach-Object { "RequiredModuleTypes.Add(typeof($($_.FullName)));" }) -join "`r`n                                                "
    }
    $message = @"


    
++++++++++++++++++++++++  ++++++++      ❇️ 🅴🆇🅲🅻🆄🆂🅸🆅🅴 🆂🅴🆁🆅🅸🅲🅴🆂?❇️
++++++++++++++++++++++##  ++++++++          http://apobekiaris.expandframework.com
++++++++++++++++++++++  ++++++++++
++++++++++    ++++++  ++++++++++++       ➤  ɪғ ʏᴏᴜ ʟɪᴋᴇ ᴏᴜʀ ᴡᴏʀᴋ ᴘʟᴇᴀsᴇ ᴄᴏɴsɪᴅᴇʀ ᴛᴏ ɢɪᴠᴇ ᴜs ᴀ STAR.
++++++++++++  ++++++  ++++++++++++          https://github.com/eXpandFramework/eXpand/stargazers
++++++++++++++  ++  ++++++++++++++
++++++++++++++    ++++++++++++++++       ➤ ​​̲𝗣​̲𝗮​̲𝗰​̲𝗸​̲𝗮​̲𝗴​̲𝗲​̲ ​̲𝗻​̲𝗼​̲𝘁​̲𝗲​̲𝘀
++++++++++++++  ++  ++++++++++++++
++++++++++++  ++++    ++++++++++++          ☞ Build the project before opening the model editor.
++++++++++  ++++++++  ++++++++++++          ☞ The package only adds the required references. To install $moduleName add the next line in the constructor of your XAF module.
++++++++++  ++++++++++  ++++++++++              $registration
++++++++  ++++++++++++++++++++++++
++++++  ++++++++++++++++++++++++++        
"@
    $readmePath = "$Directory\ReadMe.txt"
    Set-Content $readmePath $message
    Write-Output "ReadMe for $($_.package) at $readmePath"
}

Write-HostFormatted "Zip- Packages" -Section
$unzipDirs|Invoke-Parallel -ActivityName "ZipPackages" -VariablesToImport "nugetBin" -Script{
# $unzipDirs|foreach{
    $baseName=(Get-Item $_.path).BaseName
    $zip = "$nugetbin\$baseName.zip" 
    $unzipDir = "$nugetBin\$baseName"
    $nupkgPath = "$nugetBin\$baseName.nupkg"
    Compress-Files "$unzipDir" $zip -Force
    Move-Item $zip $nupkgPath -Force
    
    Remove-Item $unzipDir -Force -Recurse
}

