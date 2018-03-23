@echo off
call defines.bat
%msbuild% Xpand.build /fl /p:configuration=Release;BuildVSIXCondition=false