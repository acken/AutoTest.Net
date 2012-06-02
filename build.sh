#!/bin/bash
# stty -echo

# Break on errors
set -e

DIR=$PWD
BINARYDIR=$DIR"/build_output"
BINARYDIRx86=$DIR"/build_outputx86"

if [ ! -d $BINARYDIR ]; then
	mkdir $BINARYDIR
else
	rm -rf $BINARYDIR/*
fi

if [ ! -d $BINARYDIRx86 ]; then
	mkdir $BINARYDIRx86
else
	rm -rf $BINARYDIRx86/*
fi

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

xbuild AutoTest.TestRunner.sln /property:OutDir=$BINARYDIRx86/;Configuration=Release
xbuild  AutoTest.NET.sln /property:OutDir=$BINARYDIR/;Configuration=Release
