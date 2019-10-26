param(
    $Nuspecpath = "$PSScriptRoot\..\Nuspec", 
    $ReadMe=$true, 
    $nuget, $version, $root
)
"$nuspecPath"
function AddFile {
    param(
        $src,
        $target, 
        $nuspecpathContent 
    )
    $ns = [System.xml.xmlnamespacemanager]::new($nuspecpathContent.NameTable)
    $ns.AddNamespace("ns", $nuspecpathContent.DocumentElement.NamespaceURI)
    $files = $nuspecpathContent.SelectSingleNode("//ns:files", $ns)
    if (!($files.ChildNodes.src | Where-Object { $_ -eq $src })) {
        Write-Host "Adding file $src with target $target"
        $fileElement = $nuspecpathContent.CreateElement("file", $nuspecpathContent.DocumentElement.NamespaceURI)
        $fileElement.SetAttribute("src", $src)
        $fileElement.SetAttribute("target", "$target")
        $files.AppendChild($fileElement) | Out-Null
    }
    
}

[xml]$nuspecpathContent = Get-Content $Nuspecpath
$file = $nuspecpathContent.package.files.file | Where-Object { $_.src -match "readme.txt" } | Select-Object -First 1
if ($file) {
    $file.ParentNode.RemoveChild($file)
    $nuspecpathContent.Save($Nuspecpath)
}

if ($ReadMe) {
    $moduleName = GetModuleName $nuspecpathContent
    Write-Host $moduleName
    $readMePath = "$root\Xpand.DLL\Readme.txt"
    Remove-Item $readMePath -Force -ErrorAction SilentlyContinue
    $nuspecpathContent.package.files.file | Where-Object { $_.src -match "readme.txt" } | ForEach-Object {
        $_.ParentNode.RemoveChild($_)
    }
    $message = @"
        
        ➤ ​̲𝗣​̲𝗟​̲𝗘​̲𝗔​̲𝗦​̲𝗘​̲ ​̲𝗦​̲𝗨​̲𝗦​̲𝗧​̲𝗔​̲𝗜​̲𝗡​̲ ​̲𝗢​̲𝗨​̲𝗥​̲ ​̲𝗔​̲𝗖​̲𝗧​̲𝗜​̲𝗩​̲𝗜​̲𝗧​̲𝗜​̲𝗘​̲𝗦

            ☞  Iғ ᴏᴜʀ ᴘᴀᴄᴋᴀɢᴇs ᴀʀᴇ ʜᴇʟᴘɪɴɢ ʏᴏᴜʀ ʙᴜsɪɴᴇss ᴀɴᴅ ʏᴏᴜ ᴡᴀɴᴛ ᴛᴏ ɢɪᴠᴇ ʙᴀᴄᴋ ᴄᴏɴsɪᴅᴇʀ ʙᴇᴄᴏᴍɪɴɢ ᴀ SPONSOR ᴏʀ ᴀ BACKER.
                https://opencollective.com/expand
                
            ☞  ɪғ ʏᴏᴜ ʟɪᴋᴇ ᴏᴜʀ ᴡᴏʀᴋ ᴘʟᴇᴀsᴇ ᴄᴏɴsɪᴅᴇʀ ᴛᴏ ɢɪᴠᴇ ᴜs ᴀ STAR.
                https://github.com/eXpandFramework/eXpand/stargazers 

        ➤ ​​̲𝗣​̲𝗮​̲𝗰​̲𝗸​̲𝗮​̲𝗴​̲𝗲​̲ ​̲𝗻​̲𝗼​̲𝘁​̲𝗲​̲𝘀

            ☞ Build the project before opening the model editor.
            
            ☞ The package only adds the required references. To install $moduleName add the next line in the constructor of your XAF module.
                RequiredModuleTypes.Add(typeof($moduleName));
"@
    Set-Content $readMePath $message
    AddFile "Readme.txt" "" $nuspecpathContent
    $nuspecpathContent.Save($Nuspecpath)
}
    
& $Nuget Pack $Nuspecpath -version ($Version) -OutputDirectory "$root\Build\Nuget" -BasePath "$root\Xpand.DLL"

if ($LASTEXITCODE) {
    throw
}

