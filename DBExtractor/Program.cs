using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extraction
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                var inputFile = args.First();
                var classNamespace = args[1];
                var outputFile = args.Last();

                var serialize = args.Contains("-s");
                var validate = args.Contains("-v");
                var ropsql = args.Contains("-r");
                var wcf = args.Contains("-w");
                var json = args.Contains("-j");

                var controller = args.Contains("-c");
                var securController = args.Contains("-ca");

                var resultArray = new DBScriptExtractor(inputFile, classNamespace).ExtractModelClass(serialize, validate, ropsql, wcf, json);
                var resultModel = resultArray.First();
                var modelName = resultArray.Last();

                if (controller || securController)
                {
                    var resultController = new DBScriptExtractor(classNamespace).ExtractController(modelName, securController);
                }
            }
            else
                printHelp();
        }

        static void printHelp()
        {
            Console.Clear();
            Console.WriteLine("Invalid argument.");
            Console.WriteLine();
            Console.WriteLine("Use :");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("DBExtractor [inputfile] [modelname] [outputfile] parameters");
            Console.WriteLine();
            Console.WriteLine("Parameters :");
            Console.WriteLine("-s : Extract with serialization enable");
            Console.WriteLine("-v : Extract with auto-validation enable");
            Console.WriteLine("-r : Extract with RopSql mapping enable");
            Console.WriteLine("-w : Extract with WCF contract enable");
            Console.WriteLine("-j : Extract with JSON minimification enable");
            Console.WriteLine();
            Console.Read();
        }

    }
}
