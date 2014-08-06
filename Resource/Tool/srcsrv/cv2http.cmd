  @echo off
  setlocal ENABLEDELAYEDEXPANSION

  rem cv2http_f.cmd
  rem convert a pdb's srcsrv stream to http
  
  if "%3" == "" (
    echo cv2http.cmd: pdb  alias  url
    echo              you must specify a pdb file, 
    echo              a logical http site alias
    echo              and the URL of the site
    goto end        
  )
  
  srctool -c %1
  if not "!errorlevel!" == "-1" (
    pdbstr -r -s:srcsrv -p:%1 >%1.stream
    if exist %1.stream (
      echo %1.stream
      perl -f %~dp0\cv2http.pl %2 %3 < %1.stream > %1.stream.fixed
      if exist %1.stream.fixed (
        fc %1.stream %1.stream.fixed >NUL
        if not "!errorlevel!" == "0" (
          pdbstr -w -i:%1.stream.fixed -s:srcsrv -p:%1
          del /f %1.stream.fixed
        )
      )
      del /f %1.stream
    )
  )

 :end