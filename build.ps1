$sources=@("https://api.nuget.org/v3/index.json","https://xpandnugetserver.azurewebsites.net/nuget",(Get-Feed -DX))   
& $PSScriptRoot\support\build\go.ps1 -taskList @("Release") -packageSources $sources -msbuildArgs @("/p:Configuration=Release","/WarnAsError","/v:m") -version "18.2.704.0" 
exit $LastExitCode


