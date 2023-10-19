#!/bin/bash
host=ahow.tw
name=api_dorey_chrs
path=/x/srv/$name
dotnet publish -c Release -o "./app"
tar -zcvf app.tar.gz app
scp app.tar.gz $host:$path
ssh $host tar zxvf $path/app.tar.gz -C $path
echo -n "重新啟動中..."
ssh $host docker restart $name
