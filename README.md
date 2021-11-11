﻿﻿﻿<div style="text-align:center">
	<h1 >
		DSL Interpreter
	</h1>
    <p>
        作者： 张朕硕 @2019211616
    </p>
</div>


## 功能

识别专用脚本语言并解释执行。并未实现错误恢复，因为解释后即执行，错误恢复没有意义。大多数无用语句以及未定义语句将会被跳过。发生未知错误时将会中断执行，在解析过程发生错误会显示出错行数和文件名。脚本文件可含注释，实现了课件中的脚本识别。

## 编程环境及运行环境

- Visual Studio 2022 Preview
- .NET 6（经测试在.NET core 3.1上也能正确工作，可满足跨平台需求）

## 用法

命令行中输入可执行文件名加参数`-f 脚本文件名 -u 用户名 -d 数据文件名`，缺省参数将会采用默认配置`-f ./script.txt -u Demo -d ./data.txt `，用户名用于在数据文件中查询账单以及系统对用户的称呼。单个参数中有空格请用英文双引号括起来。

## 脚本文件示例

```DSL Script
# 脚本入口
Step welcome
    Speak $name + "您好，请问有什么可以帮您?"
    Listen 5, 10
    Branch "投诉", complainProc
    Branch "账单", billProc
    Silence silenceProc
    Default defaultProc

# 投诉
Step complainProc
    Speak "您的意见是我们改进工作的动力，请问您还有什么补充?"
    Listen 5, 50
    Default thanks

# 致谢
Step thanks
    Speak "感谢您的来电，再见"
    Exit

# 查询账单
Step billProc
    Speak "您的本月账单是" + $amount + "元，感谢您的来电，再见"
    Exit

# 未识别到输入
Step silenceProc
    Speak "听不清，请您大声一点可以吗"
    Listen 5, 3
    Branch "投诉", complainProc
    Branch "账单", billProc
    Silence silenceProc
    Default defaultProc

# 不理解含义
Step defaultProc
    Speak "抱歉，我没能理解您的意思，请重说一遍"
    Listen 5,10
    Branch "投诉", complainProc
    Branch "账单", billProc
    Silence silenceProc
    Default defaultProc

```

该语言对缩进不敏感，要注意单语句单行，注释由`#`开头，但解释器并不会有任何行为，因此只要单行由空格分离的第一个子句不为关键字均可以视为注释，不要使用`\t`，解释器将会有错误行为，使用缩进请用空格代替。

- `Step` 指定分支名，该脚本语言主体为分支，`Step` 后接分支名，直至遇见下一个`Step`均为当前分支内容。
- `Speak` 向控制台输出信息，`""`中为输出字符串，可使用`+`拼接内置变量用户名`$name`和账单`$amount`。
- `Listen` 接收用户输入，由`,`分开的两个参数第一个为最低语音时长，第二个参数为最长语音时长，在本程序由控制台输入模拟，平台受限，.NET core不允许Windows上外部结束未完成线程，因此无法实现定时取消。由用户输入空行模拟用户保持沉默超时。
- `Branch` 分支跳转语句，需要接在`Listen`后，第一个由`""`括起的参数为`Listen`结果的关键词，若匹配成功将会进入`,`后第二个参数指定的`Step`。
- `Silence` 类似`Branch`，需放在`Branch`后，作为接收空串的分支进入，参数即为分支名。
- `Default` 类似`Branch`，一般放在`Silence`后，作为默认分支入口，后接默认分支名，应用中多为无法理解用户含义。
- `Exit` 严格意义上不为关键字，提示用户脚本运行至此结束，解释器不会响应此文本。

## 数据文件示例

```TXT
name,bill
Jack,48.321
Bill,70
Ellis,80

```

该格式为了方便以csv解析，第一行无效，只是为了表示每一行要有姓名，账单两部分组成，解释器并未忽略第一行，因此不要有人名为name（这种现象也不合理）。

## 主体设计

解释器逐行解析，以字典存储分支，键为字符串，表示分支名，值为存储`Fun`类的列表，`Fun`的定义如下：

```C#
class Fun
{
    public Action<string> Act{get;set;} //保存函数，该函数接受一个参数，为字符串
    public string Arg{get;set;}			//作为函数参数传入
}
```

对分支中每一个功能都以一个接受单个字符串为参数的原型存入该类中，运行时从字典取出键值为`welcome`的值，此为默认入口，接下来遍历该列表，执行该分支的语句，若有跳转则从字典查找对应分支，且当前分支的后续语句将不会执行。

该脚本大部分语句较为简单，因此只有关键字和`Speak`分支使用了自动机进行识别。

用户输入当存在`Branch`中关键词即会跳转，不会考虑逻辑含义。输入空串会进入`Silence`分支，输入不能识别分支则会进入`Default`。

## 包含文件

`Program.cs`为程序入口

`Interpreter.cs`为解释器类，包含了对字典类的功能拓展

## 声明

该程序没有参考任何他人成果及第三方库，均为语言自带特性。
