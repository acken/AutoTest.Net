@ECHO OFF

SET DIR=%~d0%~p0%
SET BIDDIR="%DIR%build_output"
SET DEPDIR="%DIR%ReleaseBinaries"
SET VSADDINDIR="%DIR%addins\VisualStudio"

copy %BIDDIR%\AutoTest.VSAddin.dll %DEPDIR%\AutoTest.VSAddin.dll
copy %VSADDINDIR%\AutoTest.VSAddin\AutoTest.VSAddin2010.AddIn %DEPDIR%\AutoTest.VSAddin2010.AddIn
copy %VSADDINDIR%\AutoTest.VSAddin\AutoTest.VSAddin2008.AddIn %DEPDIR%\AutoTest.VSAddin2008.AddIn