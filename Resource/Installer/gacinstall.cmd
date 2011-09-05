@echo off
set gacutil="Tools\gacutil.exe"
for /r Xpand.DLL %%I in (*) do (
	%gacutil% /i Xpand.DLL\%%~nI.dll
)
