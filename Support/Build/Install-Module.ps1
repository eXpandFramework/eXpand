param(
    $psObj
)
"Getting module $($psObj.Name)"
$module=Get-module $psObj.Name -ListAvailable|Where-Object{$_.Version -eq $psObj.Version}
if (!$module){
    "Installing module $($psObj.Name) $($psObj.Version)"
    Install-Module $psObj.Name -RequiredVersion $psObj.Version -Scope CurrentUser -AllowClobber -Force
}
"Importing module $($psObj.Name)"
Import-Module $psObj.Name -Global -Prefix X -RequiredVersion $psObj.Version -Force 

