@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

SET DIR=%~d0%~p0%

SET build.config.settings="%DIR%Settings\UppercuT.config"
"%DIR%lib\Nant\nant.exe" %1 /f:.\build\package.step -D:build.config.settings=%build.config.settings%
