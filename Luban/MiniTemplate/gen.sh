#!/bin/bash

WORKSPACE=..
LUBAN_DLL=$WORKSPACE/Tools/Luban/Luban.dll
CONF_ROOT=.
Luban_Pos=../../Assets/Scripts/Luban
dotnet $LUBAN_DLL \
    -t client \
    -c cs-simple-json \
    -d json \
    --conf $CONF_ROOT/luban.conf \
    -x outputCodeDir=$Luban_Pos/code \
    -x outputDataDir=$Luban_Pos/data


# dotnet $LUBAN_DLL \
#     -t server \
#     -c go-json \
#     -d json \
#     --conf $CONF_ROOT/luban.conf \
#     -x outputCodeDir=output/server/code \
#     -x outputDataDir=output/server/data