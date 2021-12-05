using System.Text;

string demoStr = @"Speak $name+""您好，您的账单为""+$amount";
Console.WriteLine("此为对ParseSpeak方法的测试，请输入Speak的字符串。");
Console.WriteLine(@$"例：{demoStr}");
Console.WriteLine(ParseSpeak(demoStr));
Console.WriteLine("输入：（注意：Speak不可省略）");
var str = Console.ReadLine();
Console.WriteLine("输出：");
Console.WriteLine(ParseSpeak(str!));
Console.WriteLine("测试结束，任意键退出。");
Console.ReadKey();

string ParseSpeak(string originalStr)
{
    string userName = "Demo";
    decimal amount = 10.2m;
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
