if _%1==_ goto end

del /q .\eXpand.DLL\%1.dll
del /q .\eXpand.DLL\%1.pdb
rd /s /q %Gac4path%%1

:end