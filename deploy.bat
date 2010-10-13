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
) ELSE (
  IF NOT EXIST %DEPLOYDIR%\Icons (
	mkdir %DEPLOYDIR%\Icons
  ) ELSE (
	del %DEPLOYDIR%\Icons\* /Q
  )
  del  %DEPLOYDIR%\* /Q
)

copy %BINARYDIR%\AutoTest.Core.dll %DEPLOYDIR%\AutoTest.Core.dll
copy %BINARYDIR%\AutoTest.Console.exe %DEPLOYDIR%\AutoTest.Console.exe
copy %BINARYDIR%\AutoTest.WinForms.exe %DEPLOYDIR%\AutoTest.WinForms.exe
copy %BINARYDIR%\AutoTest.config.template %DEPLOYDIR%\AutoTest.config
copy %DIR%README %DEPLOYDIR%\README
copy %DIR%LICENSE %DEPLOYDIR%\AutoTest.License.txt

copy %BINARYDIR%\Castle.Core.dll %DEPLOYDIR%\Castle.Core.dll
copy %BINARYDIR%\Castle.Dynamicproxy2.dll %DEPLOYDIR%\Castle.Dynamicproxy2.dll
copy %BINARYDIR%\Castle.Facilities.Logging.dll %DEPLOYDIR%\Castle.Facilities.Logging.dll
copy %CASTLEDIR%\Castle.license.txt %DEPLOYDIR%\Castle.license.txt
copy %BINARYDIR%\Castle.MicroKernel.dll %DEPLOYDIR%\Castle.MicroKernel.dll
copy %BINARYDIR%\Castle.Windsor.dll %DEPLOYDIR%\Castle.Windsor.dll

copy %VSADDINDIR%\AutoTest.VSAddin.AddIn %DEPLOYDIR%\AutoTest.VSAddin.AddIn
copy %VSADDINDIR%\AutoTest.VSAddin.dll %DEPLOYDIR%\AutoTest.VSAddin.dll
copy %VSADDINDIR%\Install_Visual_Studio_Addin.bat %DEPLOYDIR%\Install_Visual_Studio_Addin.bat


copy %RESOURCES%\* %DEPLOYDIR%\Icons
