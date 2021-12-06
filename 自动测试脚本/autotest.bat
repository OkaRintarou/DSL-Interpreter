@echo off
chcp 65001
%视情况保留cd语句，此语句仅为进入可执行文件目录%
cd ../可执行文件/net6.0
%以下对应程序的3个参数，根据需求修改位置%
set script="script1.txt"
set userName="Jack"
set data="data.txt"
%此为模拟用户输入的内容%
set input="test3.txt"
"DSL Interpreter" -f %script% -u %userName% -d %data% < %input%
pause
exit