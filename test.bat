@echo off
echo 正在复制...
if exist %1 goto _COPY
GOTO _DONE
:_COPY
for /r %1 %%i in (*.*)do (
echo %%i已拷贝完成
copy "%%i" %2 )
if exist %1\readme.txt goto _START 
goto _DONE
:_START
::set "var=true"
if "%3"=="true" (start Notepad++ %1\readme.txt)
:_DONE
