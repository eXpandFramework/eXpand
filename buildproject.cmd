if exist %2 goto build
echo Not found %2 - Skipped! 
goto end

:build

echo Building %1...
%gacutil% -u %1,processorArchitecture=msil /silent
%msbuild% /nologo /t:Rebuild /verbosity:quiet /p:Configuration=%configuration% %2
rem %gacutil% -i DevExpress.DLL\%1.dll /silent
echo Done %1

:end