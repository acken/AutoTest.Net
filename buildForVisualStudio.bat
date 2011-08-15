@echo off

SET DIR=%~d0%~p0%
SET BINARYDIR="%DIR%build_output"

%SystemRoot%\Microsoft.NET\Framework\v3.5\MSBuild.exe addins\VisualStudio\AutoTest.VSAddin.sln /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild

::Run normal builds
build.bat