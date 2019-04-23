
Param (
    [string]$root=$(get-item "$PSScriptRoot\..\..").FullName,
    [string]$version="18.2.401.0",
    [int]$throttle=(Get-WmiObject -class Win32_ComputerSystem).numberoflogicalprocessors
)


& "$PSScriptRoot\ImportXpandPosh.ps1" 
set-location $root
Get-ChildItem *.csproj -recurse|Where-Object{
    $directoryName=(Get-Item $_).DirectoryName
    Write-Host $directoryName -f Green
    set-location $directoryName
    if (Test-Path "$directoryName\packages.config"){
        [xml]$csproj=Get-Content $_
        [xml]$config=Get-Content ".\packages.config"
        $config.packages.package|Where-Object{$_.id -like "DevExpress*"}|ForEach-Object{
            $id=$_.Id
            Write-Host $_.id -f Blue
            if (!($id -like "*.XAF.*")){
                $_.version=Get-DevExpressVersion $version $true
                $newVersion=$_.version
            }
            else{
                $_.version=$version
                $vtemp=New-Object System.Version $version
                $newVersion="$($vtemp.Major).$($vtemp.Minor).$($vtemp.Build)"
            }
            $csproj.Project.ItemGroup.Reference|ForEach-Object{
                if ($_.HintPath){
                    $_.HintPath = $_.HintPath -creplace "_third_party_assemblies\\Packages\\$id\.(\d{2}\.\d{1}\.\d*)", "_third_party_assemblies\Packages\$id.$newVersion"
                }                
            }
        }
        $csproj.Save($_.FullName)
        $config.Save("$directoryName\packages.config")
    }
}
