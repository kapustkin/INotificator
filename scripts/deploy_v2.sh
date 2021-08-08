#!/bin/bash

# Settings started

LOCAL_PATH=INotificator/bin/Release/net5.0/debian-arm/
REMOTE_IP=192.168.1.200
REMOTE_USER=pi
REMOPTE_PATH=/home/pi/inotificator
REMOTE_SERVICE_NAME=inotificator

# Settings end

chcp.com 65001

cd ..

echo "-------Started tests-------"
dotnet test
RESULT=$?

if [ $RESULT -eq 0 ]; then
    echo "-------Tests succeeded-------"
else
    echo "-------Tests failed-------"
    sleep 10
    exit -1
fi

echo "Deploy ready! For continue need enter password from remote machine"

# Ented Password
echo -n Password: 
read -s password

echo "-------Publish started-------"

dotnet publish -c Release -r debian-arm
RESULT=$?

if [ $RESULT -eq 0 ]; then
    echo "-------Publish succeeded-------"
else
    echo "-------Publish failed-------"
    sleep 10
    exit -1
fi

echo "-------Service stopping-------"
sshpass -p $password ssh $REMOTE_USER@$REMOTE_IP "sudo -u root systemctl stop $REMOTE_SERVICE_NAME"
echo "-------Service stopped-------"

echo "-------Started copy-------"
sshpass -p $password scp $LOCAL_PATH/* $REMOTE_USER@$REMOTE_IP:$REMOPTE_PATH
echo "-------Copy complete-------"

echo "-------Service starting-------"
sshpass -p $password ssh $REMOTE_USER@$REMOTE_IP "sudo -u root systemctl start $REMOTE_SERVICE_NAME"
echo "-------Service started-------"

echo "-------Deploy finished-------"
sleep 30