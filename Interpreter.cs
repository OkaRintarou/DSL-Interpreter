using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DSL_Interpreter
{
    internal class Interpreter
    {
        public Interpreter(string fileName, string userName)
        {
            try
            {
                this.userName = userName;
                using StreamReader sr = new StreamReader(fileName);
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
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
                Console.Write($"出错行数：{lines}");

            }

        }

        private int lines = 0;

        private string userName;

        private decimal amount = 0.00m;

        private List<Fun> lastList;

        private bool Continue { get; set; }

        private string listenStr;


        private void Speak(string str)
        {
            Console.WriteLine(ParseSpeak(str));
        }

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
                            @case = 1;
                        break;
                    //S
                    case 1:
                        if (originalStr[i] == 'p')
                            @case = 2;
                        else
                            @case = 0;
                        break;
                    //p
                    case 2:
                        if (originalStr[i] == 'e')
                            @case = 3;
                        else
                            @case = 0;
                        break;
                    //e
                    case 3:
                        if (originalStr[i] == 'a')
                            @case = 4;
                        else
                            @case = 0;
                        break;
                    //a
                    case 4:
                        if (originalStr[i] == 'k')
                            @case = 6;
                        else
                            @case = 0;
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
                            @case = 13;
                        else
                            sb.Append(originalStr[i]);
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
                            @case = 11;
                        break;
                    //a
                    case 11:
                        if (originalStr[i] == 'm')
                            @case = 12;
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
                            @case = 15;
                        break;
                    //m(amount)
                    case 15:
                        if (originalStr[i] == 'o')
                            @case = 16;
                        break;
                    //o(amount)
                    case 16:
                        if (originalStr[i] == 'u')
                            @case = 17;
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


        private void Listen(string timeout)
        {
            Continue = true;
            Console.Write(">>> ");
            listenStr = Console.ReadLine();
        }

        private void Branch(string originalStr)
        {
            if (!Continue)
                return;
            var strs = originalStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var match = Regex.Match(originalStr, @"""(?<keyword>[\s\S]*)""");
            if (match.Success)
            {
                var keyword = match.Groups["keyword"].Value;
                if (Regex.Match(listenStr, keyword).Success)
                {
                    Continue = false;
                    step.Run(strs[^1]);
                }
            }

        }

        private void Silence(string @case)
        {
            if (!Continue)
                return;
            if (listenStr == "")
                step.Run(@case);
        }

        private void Default(string @case)
        {
            if (!Continue)
                return;
            step.Run(@case);

        }


        private void Parser(string line)
        {
            var strs = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 0)
                return;
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



        public void Run()
        {
            step.Run();
        }


        private Dictionary<string, List<Fun>> step = new Dictionary<string, List<Fun>>();
    }

    class Fun
    {
        public Action<string> Act { get; set; }
        public string Arg { get; set; }

    }

    static class Extension
    {

        public static void Run(this Dictionary<string, List<Fun>> step, string key = "welcome")
        {
            foreach (var fun in step[key])
            {
                fun.Act(fun.Arg);
            }
        }


    }
}
