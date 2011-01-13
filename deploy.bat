@echo off

SET DIR=%~d0%~p0%
SET BINARYDIR="%DIR%build_output\AutoTest.NET"
SET DEPLOYDIR="%DIR%ReleaseBinaries"
SET CASTLEDIR="%DIR%lib\Castle.Windsor"
SET VSADDINDIR="%DIR%addins\VisualStudio\FilesToDeploy"
SET RESOURCES="%DIR%src\Resources"

IF NOT EXIST %DEPLOYDIR% (
  mkdir %DEPLOYDIR%
  mkdir %DEPLOYDIR%\Icons
  mkdir %DEPLOYDIR%\TestRunners
) ELSE (
  IF NOT EXIST %DEPLOYDIR%\Icons (
	mkdir %DEPLOYDIR%\Icons
  ) ELSE (
	del %DEPLOYDIR%\Icons\* /Q
  )
  IF NOT EXIST %DEPLOYDIR%\TestRunners (
	mkdir %DEPLOYDIR%\TestRunners
  ) ELSE (
	del %DEPLOYDIR%\TestRunners\* /Q
  )
  del  %DEPLOYDIR%\* /Q
)

copy %BINARYDIR%\AutoTest.Messages.dll %DEPLOYDIR%\AutoTest.Messages.dll
copy %BINARYDIR%\AutoTest.Core.dll %DEPLOYDIR%\AutoTest.Core.dll
copy %BINARYDIR%\AutoTest.Console.exe %DEPLOYDIR%\AutoTest.Console.exe
copy %BINARYDIR%\AutoTest.WinForms.exe %DEPLOYDIR%\AutoTest.WinForms.exe
copy %BINARYDIR%\AutoTest.config.template %DEPLOYDIR%\AutoTest.config
copy %DIR%README %DEPLOYDIR%\README
copy %DIR%LICENSE %DEPLOYDIR%\AutoTest.License.txt

copy %BINARYDIR%\AutoTest.TestRunner.exe %DEPLOYDIR%\AutoTest.TestRunner.exe
copy %BINARYDIR%\AutoTest.TestRunners.Shared.dll %DEPLOYDIR%\AutoTest.TestRunners.Shared.dll
copy %BINARYDIR%\AutoTest.TestRunners.NUnit.dll %DEPLOYDIR%\TestRunners\AutoTest.TestRunners.NUnit.dll
copy %BINARYDIR%\nunit.core.dll %DEPLOYDIR%\TestRunners\nunit.core.dll
copy %BINARYDIR%\nunit.core.interfaces.dll %DEPLOYDIR%\TestRunners\nunit.core.interfaces.dll
copy %BINARYDIR%\nunit.util.dll %DEPLOYDIR%\TestRunners\nunit.util.dll

copy %BINARYDIR%\Castle.Core.dll %DEPLOYDIR%\Castle.Core.dll
copy %BINARYDIR%\Castle.Facilities.Logging.dll %DEPLOYDIR%\Castle.Facilities.Logging.dll
copy %CASTLEDIR%\Castle.license.txt %DEPLOYDIR%\Castle.license.txt
copy %BINARYDIR%\Castle.Windsor.dll %DEPLOYDIR%\Castle.Windsor.dll
copy %BINARYDIR%\Mono.Cecil.dll %DEPLOYDIR%\Mono.Cecil.dll

copy %RESOURCES%\* %DEPLOYDIR%\Icons
