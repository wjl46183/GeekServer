#!/bin/bash

DIR=$(cd $(dirname $0);pwd)
cd $DIR

LUBAN_DLL="../pixelminion/pixelminion-client/tools/luban/Luban.dll"
CONF_ROOT="../pixelminion/pixelminion-client/common/lubanConfig"

dotnet "$LUBAN_DLL" \
    -t all \
    -c cs-lazyload-bin \
    -x codeStyle=none \
    -d bin bin-offsetlength \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputCodeDir=./Geek.Server.HotData/Configs/ExportScript \
    -x outputDataDir=./Geek.Server.HotData/ExportData/bytes \
    -x bin-offsetlength.outputDataDir=./Geek.Server.HotData/ExportData/offset \

# Pause to keep the terminal open (similar to 'pause' in batch script)
read -p "Press Enter to continue..."

