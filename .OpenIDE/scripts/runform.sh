#!/bin/bash 
# First parameter is the execution location of this script instance

if [ "$2" = "get-command-definitions" ]; then
	# Definition format usually represented as a single line:

	# Script description|
	# command1|"Command1 description"
	# 	param|"Param description" end
	# end
	# command2|"Command2 description"
	# 	param|"Param description" end
	# end

	echo "Run the winforms app|[WATCH_FOLDER]|\"Folder/Solution to watch\" end [-p]|\"Runs package.sh\" end"
	exit
fi

set -e

if [[ "$@" == *-p* ]]; then
	./package.sh
fi

sed -i 's/Debugging>false/Debugging>true/g' ReleaseBinaries/AutoTest.config
sed -i 's/<BuildExecutable parameters=\"\">C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe<\/BuildExecutable>/<BuildExecutable>\/usr\/bin\/xbuild<\/BuildExecutable>/g' ReleaseBinaries/AutoTest.config

mono --debug ReleaseBinaries/AutoTest.WinForms.exe $2
