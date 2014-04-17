using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.Data.Extraction
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                var inputFile = args.First();
                var classNamespace = args[1];

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

                var modelOutputFile = string.Concat(modelName, ".cs");
                writeFile(modelOutputFile, resultModel);

                var controllerOutputFile = string.Empty;
                if (controller || securController)
                {
                    var resultController = new DBScriptExtractor(classNamespace).ExtractController(modelName, securController);
                    controllerOutputFile = string.Concat(modelName, "Controller.cs");
                    writeFile(controllerOutputFile, resultController);
                }

                Console.WriteLine(string.Concat("Model Class successfully extracted on ", modelOutputFile));

                if (controller || securController)
                    Console.WriteLine(string.Concat(Environment.NewLine, "Controller Class successfully extracted on ", controllerOutputFile));

                Console.WriteLine();
                Console.Read();
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
            Console.WriteLine("-c  : Extract with RopSql based Controller");
            Console.WriteLine("-ca : Extract with RopSql based Controller");
            Console.WriteLine("      and InMemProfile Access Control");
            Console.Read();
        }

        static void writeFile(string output, string content)
        {
            var outFile = File.CreateText(output);
            
            outFile.Write(content);
            outFile.Close();
        }
    }
}
