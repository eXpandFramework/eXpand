setlocal
set LOCALDIR=%CD:\=\\%
echo Registering Xpand assembly folder
echo Windows Registry Editor Version 5.00 >> Xpand_vs.reg
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\Xpand] >> Xpand_vs.reg
echo @="%LOCALDIR%\\Xpand.DLL" >> Xpand_vs.reg
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\Xpand3rdParty] >> Xpand_vs.reg
echo @="%LOCALDIR%\\_third_party_assemblies" >> Xpand_vs.reg
regedit Xpand_vs.reg
del Xpand_vs.reg