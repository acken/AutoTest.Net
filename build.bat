@echo off

if '%2' NEQ '' goto usage
if '%3' NEQ '' goto usage
if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%~d0%~p0%
SET BINARYDIRx86="%DIR%build_outputx86"
SET BINARYDIR="%DIR%build_output"

IF NOT EXIST %BINARYDIRx86% (
	mkdir %BINARYDIRx86%
) ELSE (
	del %BINARYDIRx86%\* /Q
)
IF NOT EXIST %BINARYDIR% (
	mkdir %BINARYDIR%
) ELSE (
	del %BINARYDIR%\* /Q
)

SET CONFIG=Release
if '%1' == 'debug' SET CONFIG=Debug

%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe AutoTest.TestRunner.sln /property:OutDir=%BINARYDIRx86%\;Configuration=%CONFIG% /target:rebuild

if %ERRORLEVEL% NEQ 0 goto errors

%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe AutoTest.NET.sln /property:OutDir=%BINARYDIR%\;Configuration=%CONFIG% /target:rebuild

if %ERRORLEVEL% NEQ 0 goto errors

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe addins\VisualStudio\AutoTest.VSAddin.sln /property:OutDir=%BINARYDIR%\;Configuration=%CONFIG% /target:rebuild

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:usage
echo.
echo Usage: build.bat
echo.
goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish
