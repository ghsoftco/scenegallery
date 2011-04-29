#!/bin/bash

if [ $# -lt 3 ]
then
	echo "Arguments: inputPngDirectory outputDirectory j|p"
fi

case "$3" in

j)	echo "Outputting jpg..."
	find "$1" -name "*.png" | xargs sudo mogrify -format jpg -resize 512x512\> -path "$2"
	;;
p)	echo "Outputting png..."
	find "$1" -name "*.png" | xargs sudo mogrify -format png -resize 512x512\> -path "$2"
	;;
*)	echo "Use j,p as 3rd argument for jpg,png output respectively."
	;;
esac
