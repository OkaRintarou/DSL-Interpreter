@echo off
chcp 65001
%视情况保留cd语句，此语句仅为进入可执行文件目录%
cd ../可执行文件/net6.0
%以下对应程序的3个参数，根据需求修改位置%
set script="script1.txt"
set userName="Jack"
set data="data.txt"
%此为模拟用户输入的内容%
set input="test1.txt"
%若要实现多组测试请对各变量直接写为路径，并将下面的运行语句复制多行%
echo test1
"DSL Interpreter" -f %script% -u %userName% -d %data% < %input%
echo test2
"DSL Interpreter" -f %script% -u %userName% -d %data% < "test2.txt"
echo test3
"DSL Interpreter" -f %script% -u %userName% -d %data% < "test3.txt"
pause
exit