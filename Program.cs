using System;

namespace DSL_Interpreter
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string fileName = "script.txt";
            string userName = "Demo";
            int fileNum = Array.FindIndex(args, (s) => s == "-f");
            if (fileNum != -1 && fileNum + 1 < args.Length)
                fileName = args[fileNum + 1];
            int userNum = Array.FindIndex(args, (s) => s == "-u");
            if (userNum != -1 && userNum + 1 < args.Length)
                userName = args[userNum + 1];


            Interpreter interpreter = new Interpreter(fileName, userName);
            interpreter.Run();
        }
    }
}
