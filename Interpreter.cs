using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DSL_Interpreter
{
    // 解释器类
    internal class Interpreter
    {
        // 构造函数
        public Interpreter(string fileName, string userName, string dataName)
        {
            try
            {
                this.userName = userName;
                using (StreamReader dataSr = new StreamReader(dataName))
                {
                    while (!dataSr.EndOfStream)
                    {
                        string line = dataSr.ReadLine();
                        string[] strs = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        if (strs.Length == 2 && strs[0] == userName)
                        {
                            amount = decimal.Parse(strs[1]);
                            break;
                        }
                    }
                    dataSr.Close();
                }
                using StreamReader sr = new StreamReader(fileName);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    lines++;
                    Parser(line);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("脚本解析失败！");
                Console.WriteLine($"脚本文件名：{fileName}");
                Console.WriteLine($"用户名：{userName}");
                Console.WriteLine($"数据文件名：{dataName}");
                Console.Write($"出错行数：{lines}");

            }

        }

        // 记录当前行数
        private readonly int lines = 0;

        // 用户名
        private readonly string userName;

        // 账单
        private readonly decimal amount = 0.00m;

        // 记录当前分支，用于构建程序
        private List<Fun> lastList;

        // 记录下列语句是否有效，为false时语句不执行
        private bool Continue { get; set; }

        // 当前listen内容是否为投诉
        private bool isComplain = false;

        // listen结果
        private string listenStr;

        // 处理Speak
        private void Speak(string str)
        {
            Console.WriteLine(ParseSpeak(str));
        }

        // 处理Speak的内容
        private string ParseSpeak(string originalStr)
        {
            StringBuilder sb = new StringBuilder();
            int @case = 0;
            for (int i = 0; i < originalStr.Length; i++)
            {
                switch (@case)
                {
                    //init
                    case 0:
                        if (originalStr[i] == 'S')
                        {
                            @case = 1;
                        }

                        break;
                    //S
                    case 1:
                        if (originalStr[i] == 'p')
                        {
                            @case = 2;
                        }
                        else
                        {
                            @case = 0;
                        }

                        break;
                    //p
                    case 2:
                        if (originalStr[i] == 'e')
                        {
                            @case = 3;
                        }
                        else
                        {
                            @case = 0;
                        }

                        break;
                    //e
                    case 3:
                        if (originalStr[i] == 'a')
                        {
                            @case = 4;
                        }
                        else
                        {
                            @case = 0;
                        }

                        break;
                    //a
                    case 4:
                        if (originalStr[i] == 'k')
                        {
                            @case = 6;
                        }
                        else
                        {
                            @case = 0;
                        }

                        break;

                    //content-init
                    case 6:
                        switch (originalStr[i])
                        {
                            case '$':
                                @case = 7;
                                break;
                            case '\"':
                                @case = 8;
                                break;
                        }
                        break;
                    //$
                    case 7:
                        switch (originalStr[i])
                        {
                            case 'n':
                                @case = 10;
                                break;
                            case 'a':
                                @case = 14;
                                break;
                        }
                        break;
                    //"
                    case 8:
                        if (originalStr[i] == '"')
                        {
                            @case = 13;
                        }
                        else
                        {
                            sb.Append(originalStr[i]);
                        }

                        break;
                    //+
                    case 9:
                        switch (originalStr[i])
                        {
                            case '$':
                                @case = 7;
                                break;
                            case '\"':
                                @case = 8;
                                break;
                        }
                        break;
                    //n
                    case 10:
                        if (originalStr[i] == 'a')
                        {
                            @case = 11;
                        }

                        break;
                    //a
                    case 11:
                        if (originalStr[i] == 'm')
                        {
                            @case = 12;
                        }

                        break;
                    //m
                    case 12:
                        if (originalStr[i] == 'e')
                        {
                            sb.Append(userName);
                            @case = 13;
                        }
                        break;
                    //2-cinit
                    case 13:
                        switch (originalStr[i])
                        {
                            case '$':
                                @case = 7;
                                break;
                            case '\"':
                                @case = 8;
                                break;
                            case '+':
                                @case = 9;
                                break;
                        }
                        break;
                    //a(amount)
                    case 14:
                        if (originalStr[i] == 'm')
                        {
                            @case = 15;
                        }

                        break;
                    //m(amount)
                    case 15:
                        if (originalStr[i] == 'o')
                        {
                            @case = 16;
                        }

                        break;
                    //o(amount)
                    case 16:
                        if (originalStr[i] == 'u')
                        {
                            @case = 17;
                        }

                        break;
                    //u(amount)
                    case 17:
                        if (originalStr[i] == 'n')
                        {
                            @case = 13;
                            sb.Append(amount);
                        }
                        break;

                }
            }

            return sb.ToString();
        }

        // 处理Listen
        private void Listen(string timeout)
        {
            Continue = true;
            Console.Write(">>> ");
            listenStr = Console.ReadLine();
            if (isComplain)
            {
                using (StreamWriter sw = new StreamWriter($"Complain of {userName}.txt"))
                {
                    sw.WriteLine(listenStr);
                    sw.Close();
                }
                isComplain = false;
            }
        }

        // 处理Branch
        private void Branch(string originalStr)
        {
            if (!Continue)
            {
                return;
            }

            string[] strs = originalStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Match match = Regex.Match(originalStr, @"""(?<keyword>[\s\S]*)""");
            if (match.Success)
            {
                string keyword = match.Groups["keyword"].Value;
                if (Regex.Match(listenStr, keyword).Success)
                {
                    isComplain = strs[^1] == "complainProc";
                    step.Run(strs[^1]);
                    Continue = false;
                }
            }

        }

        // 处理Silence
        private void Silence(string @case)
        {
            if (!Continue)
            {
                return;
            }

            if (listenStr == "")
            {
                step.Run(@case);
                Continue = false;
            }
        }

        // 处理Default
        private void Default(string @case)
        {
            if (!Continue)
            {
                return;
            }

            step.Run(@case);
            Continue = false;

        }

        // 解析脚本中的一行
        private void Parser(string line)
        {
            string[] strs = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 0)
            {
                return;
            }

            switch (strs[0])
            {
                case "Step":
                    step.Add(strs[1], new List<Fun>());
                    lastList = step[strs[1]];
                    break;
                case "Speak":
                    lastList.Add(new Fun() { Arg = line, Act = Speak });
                    break;
                case "Listen":
                    lastList.Add(new Fun() { Arg = strs[^1], Act = Listen });
                    break;
                case "Branch":
                    lastList.Add(new Fun() { Arg = line, Act = Branch });
                    break;
                case "Silence":
                    lastList.Add(new Fun() { Arg = strs[1], Act = Silence });
                    break;
                case "Default":
                    lastList.Add(new Fun() { Arg = strs[1], Act = Default });
                    break;
            }
        }


        // 解析程序的外部接口，开始运行
        public void Run()
        {
            step.Run();
        }

        // 保存每一个分支的语句
        private readonly Dictionary<string, List<Fun>> step = new Dictionary<string, List<Fun>>();
    }

    // 保存语句和参数
    internal class Fun
    {
        public Action<string> Act { get; set; }
        public string Arg { get; set; }

    }

    // 扩展类
    internal static class Extension
    {
        // 扩展Dictionary的功能
        public static void Run(this Dictionary<string, List<Fun>> step, string key = "welcome")
        {
            foreach (Fun fun in step[key])
            {
                fun.Act(fun.Arg);
            }
        }


    }
}
