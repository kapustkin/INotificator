#!/bin/bash
chcp.com 65001

echo "Publish started"

dotnet publish -c Release -r debian-arm
RESULT=$?

if [ $RESULT -eq 0 ]; then
    echo "Publish succeeded"
else
    echo "Publish failed"
    exit -1
fi


echo "-------Started copy-------"
scp INotificator/bin/Release/net5.0/debian-arm/* pi@192.168.1.200:/home/pi/inotificator
echo "-------Copy complete-------"

sleep 60
