@ECHO OFF
REM First parameter is the execution location of this script instance

if "%2" == "get-command-definitions" (
	REM Definition format usually represented as a single line:

	REM Script description|
	REM command1|"Command1 description"
	REM 	param|"Param description" end
	REM end
	REM command2|"Command2 description"
	REM 	param|"Param description" end
	REM end

	ECHO "Deploys and launches the AutoTest.Net testrunner (AutoTest.TestRunner.exe). Available commands when running are all, single and ext"
	GOTO end
)

SET RUNDIR=%~1\src\AutoTest.TestRunner\AutoTest.TestRunners\bin\RunTests
SET ATDIR=%~1\src\AutoTest.TestRunner\AutoTest.TestRunners\bin\AutoTest.Net
SET CONSOLEDIR=%~1\src\AutoTest.TestRunner\AutoTest.TestConsole\bin\AutoTest.Net

SET NUNIT_DIR=%~1\src\AutoTest.TestRunner\Plugins\AutoTest.TestRunners.NUnit\bin\AutoTest.Net
SET NUNIT_TESTS_DIR=%~1\src\AutoTest.TestRunner\Plugins\AutoTest.TestRunners.NUnit.Tests\bin\AutoTest.Net

IF NOT EXIST %RUNDIR% (
	mkdir %RUNDIR%
)
del %RUNDIR%\*.* /Q /S
mkdir %RUNDIR%\TestAssembly
mkdir %RUNDIR%\TestRunners
mkdir %RUNDIR%\TestRunners\NUnit

copy %NUNIT_DIR%\AutoTest.TestRunners.NUnit*.* %RUNDIR%\TestRunners\NUnit
copy %NUNIT_TESTS_DIR%\*.* %RUNDIR%\TestAssembly
copy %NUNIT_DIR%\nunit*.* %RUNDIR%\TestRunners\NUnit
copy %CONSOLEDIR%\AutoTest.TestConsole.*.* %RUNDIR%
copy %ATDIR%\*.* %RUNDIR%

del %RUNDIR%\*.mm.dll
del %RUNDIR%\*.mm.exe
del %RUNDIR%\*.exe.config
mv %RUNDIR%\App_3.5.config %RUNDIR%\AutoTest.TestRunner.exe.config
del %RUNDIR%\TestRunners\NUnit\*.mm.dll

rem cd %RUNDIR%\TestAssembly
cd %RUNDIR%

@ECHO ON
rem %RUNDIR%\AutoTest.TestConsole.exe %RUNDIR%\TestAssembly\AutoTest.TestRunners.NUnit.Tests.dll

ECHO ^<?xml version="1.0" encoding="utf-8" ?^>^<run^>^<runner id="NUnit"^>^<test_assembly name="%RUNDIR%\TestAssembly\AutoTest.TestRunners.NUnit.Tests.dll" /^>^</runner^>^</run^> > %RUNDIR%\input.xml

start %RUNDIR%\AutoTest.TestRunner.exe --input=%RUNDIR%\input.xml --output=%RUNDIR%\output.xml --port=8090 --logging

sleep 0.1

%RUNDIR%\AutoTest.TestRunner.exe "127.0.0.1" 8090 "%RUNDIR%\TestAssembly\AutoTest.TestRunners.NUnit.Tests.dll|nunit:load-assembly"

%RUNDIR%\AutoTest.TestRunner.exe "127.0.0.1" 8090 "%RUNDIR%\TestAssembly\AutoTest.TestRunners.NUnit.Tests.dll|nunit:run-all"

%RUNDIR%\AutoTest.TestRunner.exe "127.0.0.1" 8090 exit

:end
