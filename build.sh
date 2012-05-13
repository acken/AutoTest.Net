#!/bin/bash
# stty -echo

DIR=$PWD
BINARYDIR=$DIR"/build_output"
BINARYDIRx86=$DIR"/build_outputx86"

function usage
{
	echo ""
	echo "Usage: build.sh"
	exit
}

function displayUsage
{
	case $1 in
		"/?"|"-?"|"?"|"/help") usage ;;
	esac
}

displayUsage $1

xbuild AutoTest.TestRunner.sln /property:OutDir=$BINARYDIRx86/;Configuration=Release /target:rebuild
xbuild  AutoTest.NET.sln /property:OutDir=$BINARYDIR/;Configuration=Release /target:rebuild
xbuild addins/VisualStudio/AutoTest.VSAddin.sln /property:OutDir=$BINARYDIR/;Configuration=Release /target:rebuild
