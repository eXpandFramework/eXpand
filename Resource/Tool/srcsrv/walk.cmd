  @echo off
  setlocal 
  rem ENABLEDELAYEDEXPANSION

  rem walk.cmd
  rem walk through a directory tree and execute the passed command
  
  if "%2" == "" (
    echo walk.cmd: You must specify a file mask and command to execute.
    goto end
  )
  
  for /f %%a in ('dir /s/b/a-d %1') do call %2 %%a %3 %4 %5 %6 %7 %8 %9

 :end
