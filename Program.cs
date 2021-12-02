using System;

namespace DSL_Interpreter
{
    // 提供程序入口的类
    internal class Program
    {
        // 接收命令行参数
        // -f 脚本文件名
        // -u 用户名
        // -d 数据文件名（用于存放账单）
        // 当给出参数有误视情况可能采用默认配置
        private static void Main(string[] args)
        {
            string fileName = "script.txt";
            string userName = "Demo";
            string dataName = "data.txt";
            int fileNum = Array.FindIndex(args, (s) => s == "-f");
            if (fileNum != -1 && fileNum + 1 < args.Length)
            {
                fileName = args[fileNum + 1];
            }

            int userNum = Array.FindIndex(args, (s) => s == "-u");
            if (userNum != -1 && userNum + 1 < args.Length)
            {
                userName = args[userNum + 1];
            }

            int dataNum = Array.FindIndex(args, (s) => s == "-d");
            if (dataNum != -1 && dataNum + 1 < args.Length)
            {
                dataName = args[dataNum + 1];
            }
            try
            {
                Interpreter interpreter = new Interpreter(fileName, userName, dataName);
                interpreter.Run();
            }
            catch (MyException ex)
            {

                ex.ShowMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("程序运行结束，按任意键退出");
            Console.ReadKey();
        }
    }
}
