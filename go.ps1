if (!(Get-Module -ListAvailable -Name VSSetup)) {
    Write-Host "Module exists"
    Set-PSRepository -Name "PSGallery" -InstallationPolicy Trusted
    Install-Module VSSetup
}
$path=Get-VSSetupInstance  | Select-VSSetupInstance -Product Microsoft.VisualStudio.Product.BuildTools -Latest |Select-Object -ExpandProperty InstallationPath
$path=join-path $path MSBuild\15.0\Bin\MSBuild.exe
$assemblyInfo="Xpand\Xpand.Utils\Properties\XpandAssemblyInfo.cs"
$version=(Get-Content $assemblyInfo -ErrorAction Stop | Select-String 'public const string Version = \"([^\"]*)')[0].Matches.Groups[1].Value
Invoke-Expression "'$path' Xpand.Build  /fl /t:Release /p:Version=$version;Configuration=$configuration"
