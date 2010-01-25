setlocal
set LOCALDIR=%CD:\=\\%
echo Registering expand assembly folder
echo Windows Registry Editor Version 5.00 >> expand_vs.reg
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\eXpand] >> expand_vs.reg
echo @="%LOCALDIR%\\eXpand.DLL" >> expand_vs.reg
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\eXpand3rdParty] >> expand_vs.reg
echo @="%LOCALDIR%\\_third_party_assemblies" >> expand_vs.reg
regedit expand_vs.reg
del expand_vs.reg