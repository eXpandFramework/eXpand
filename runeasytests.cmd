@echo off
call defines.bat
@%msbuild% Xpand.build /t:RunEasyTests /v:m /fl