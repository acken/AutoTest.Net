#!/bin/bash
# stty -echo

function useage
{
	echo ""
	echo "Usage: build"
	exit
}

function displayUsage
{
	case $1 in
		"/?"|"-?"|"?"|"/help") usage ;;
	esac
}

displayUsage $1

mono ./lib/NAnt/NAnt.exe $1 /f:/home/ack/src/AutoTest.Net/build/default.build -D:build.config.settings=/home/ack/src/AutoTest.Net/Settings/UppercuT.config
