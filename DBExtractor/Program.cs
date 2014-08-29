using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Globalization;
using System.Diagnostics;

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
                var excepDepth = args[2];
                var funcGroup = args[3];
                var funcSubGroup = args[4];

                var serialize = args.Contains("-s");
                var validate = args.Contains("-v");
                var valmsg = args.Contains("-vm");
                var ropsql = args.Contains("-r");
                var wcf = args.Contains("-w");
                var json = args.Contains("-j");
                var navmenu = args.Contains("-f");

                var controller = args.Contains("-c");
                var excepSafeController = args.Contains("-ce");
                var securController = args.Contains("-ca");
                var loggedController = args.Contains("-cl");
                var wkflowController = args.Contains("-cw");
                var gzip = args.Contains("-g");

                CultureInfo culture = new CultureInfo(ConfigurationSettings.AppSettings["Culture"]);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                FuncionalityConfig funcConfig = null;

                if (navmenu)
                {
                    funcConfig = new FuncionalityConfig();
                    funcConfig.Group = funcGroup;
                    funcConfig.SubGroup = funcSubGroup;
                }

                var resultArray = new DBScriptExtractor(inputFile, classNamespace).ExtractModelClass(serialize, validate, valmsg, ropsql, gzip, wcf, json, funcConfig);
                var resultModel = resultArray.First();
                var modelName = resultArray.Last();

                var modelOutputFile = string.Concat(modelName, ".cs");
                writeFile(modelOutputFile, resultModel);
                Console.WriteLine(string.Concat("Model Class successfully extracted on ", modelOutputFile));

                var controllerOutputFile = string.Empty;
                if (controller || excepSafeController || securController || loggedController || wkflowController)
                {
                    var exDepth = !string.IsNullOrEmpty(excepDepth) ? int.Parse(excepDepth) : 0;
                    var resultController = new DBScriptExtractor(classNamespace).ExtractController(modelName, 
                                                                                                   excepSafeController, 
                                                                                                   securController, 
                                                                                                   loggedController, 
                                                                                                   wkflowController, 
                                                                                                   gzip, exDepth);
                    controllerOutputFile = string.Concat(modelName, "Controller.cs");
                    writeFile(controllerOutputFile, resultController);
                    Console.WriteLine(string.Concat(Environment.NewLine, "Controller Class successfully extracted on ", controllerOutputFile));
                }

                Process.Start("Notepad.exe", modelOutputFile);
                Process.Start("Notepad.exe", controllerOutputFile);

                Console.Read();
            }
            else
                printHelp();
        }

        static void printHelp()
        {
            Console.Clear();
            Console.WriteLine("Use :");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("DBExtractor [inputfile] [namespace] [exceptiondepth] [funcgroup] [funcsubgroup] parameters");
            Console.WriteLine();
            Console.WriteLine("Parameters :");
            Console.WriteLine("-s  : Extract with serialization enable");
            Console.WriteLine("-v  : Extract with auto-validation enable");
            Console.WriteLine("-vm : Extract with validation message enable");
            Console.WriteLine("-r  : Extract with RopSql mapping enable");
            Console.WriteLine("-w  : Extract with WCF contract enable");
            Console.WriteLine("-j  : Extract with JSON minimification enable");
            Console.WriteLine("-f  : Extract with Navigation Menu enable");
            Console.WriteLine();
            Console.WriteLine("-c  : Extract with RopSql based Controller");
            Console.WriteLine("-ce : Extract with RopSql based Controller");
            Console.WriteLine("      and Exception Manager"); 
            Console.WriteLine("      (Depths : 1 Show, 2 Store, 4 Mail, 8 Ticket)");
            Console.WriteLine("-ca : Extract with RopSql based Controller");
            Console.WriteLine("      Exception Safe and InMemProfile Access Control");
            Console.WriteLine("-cl : Extract with RopSql based Controller,");
            Console.WriteLine("      Exception Safe, Security and Registry Log");
            Console.WriteLine("-cw : Extract with RopSql based Controller,");
            Console.WriteLine("      Exception Safe, Security, Registry Log");
            Console.WriteLine("      and Custom Workflow Support");
            Console.WriteLine("-g  : Extract with GZip result enable");
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
