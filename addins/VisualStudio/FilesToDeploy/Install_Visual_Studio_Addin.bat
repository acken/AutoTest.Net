@echo off

SET AddInDestinationfolder=%HOMEDRIVE%%HOMEPATH%\My Documents
SET DotNetFolder=%SystemRoot%\Microsoft.net\framework\v2.0.50727
SET CURRENTDIR=%CD%

:: Register addin assembly
echo Registering addin assembly
%DotNetFolder%\regasm.exe /CodeBase AutoTest.VSAddin.dll

SET FILE=%CURRENTDIR%\AutoTest.VSAddin.AddIn
SET FIND_VERSION=9.0
SET FIND_PATH=C:\Wherever_you_have_your_autotest_binaries\AutoTest.VSAddin.dll
SET REPLACE_PATH=%CURRENTDIR%\AutoTest.VSAddin.dll

:: Create Visual Studio 2008 addin
echo Registering addin within Visual Studio 2008
SET REPLACE_VERSION=9.0
SET DESTINATION="%AddInDestinationfolder%\Visual Studio 2008\Addins\AutoTest.VSAddin.AddIn"
if Not Exist "%AddInDestinationfolder%\Visual Studio 2008\Addins" mkdir "%AddInDestinationfolder%\Visual Studio 2008\Addins"

if Exist "%DESTINATION%" del "%DESTINATION%"
for /f "tokens=1,* delims=]" %%A in ('"type %FILE%|find /n /v """') do (
	set "line=%%B"
	if defined line (
		call set "line=echo.%%line:%FIND_VERSION%=%REPLACE_VERSION%%%"
		call set "line=%%line:%FIND_PATH%=%REPLACE_PATH%%%"
		for /f "delims=" %%X in ('"echo."%%line%%""') do %%~X
	) >> %DESTINATION% ELSE echo.>>%DESTINATION%
)

:: Create Visual Studio 2010 addin
echo Registering addin within Visual Studio 2010
SET REPLACE_VERSION=10.0
SET DESTINATION="%AddInDestinationfolder%\Visual Studio 2010\Addins\AutoTest.VSAddin.AddIn"
if Not Exist "%AddInDestinationfolder%\Visual Studio 2010\Addins" mkdir "%AddInDestinationfolder%\Visual Studio 2010\Addins"

if Exist "%DESTINATION%" del "%DESTINATION%"
for /f "tokens=1,* delims=]" %%A in ('"type %FILE%|find /n /v """') do (
	set "line=%%B"
	if defined line (
		call set "line=echo.%%line:%FIND_VERSION%=%REPLACE_VERSION%%%"
		call set "line=%%line:%FIND_PATH%=%REPLACE_PATH%%%"
		for /f "delims=" %%X in ('"echo."%%line%%""') do %%~X
	) >> %DESTINATION% ELSE echo.>>%DESTINATION%
)