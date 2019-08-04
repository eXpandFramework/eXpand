
$sources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget","C:\Program Files (x86)\DevExpress 19.1\Components\System\Components\packages")   
& $PSScriptRoot\support\build\go.ps1 -taskList @("Release") -packageSources $sources -msbuildArgs @("/p:Configuration=Release","/WarnAsError","/v:m") -version "19.1.501.0" 
exit $LastExitCode


