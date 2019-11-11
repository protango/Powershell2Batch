using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Powershell2Batch {
    class Program {
        private static readonly IReadOnlyList<string> BatchSpecialChars = new string[] {
            "%", "^", "&", "<", ">", "|"
        };

        static void Main(string[] args) {
            try {
                ValidateArgs(args);
            }
            catch (ArgumentException e) {
                Console.Error.WriteLine(e.Message);
                ShowHelp();
                return;
            }

            FileInfo inputFile = new FileInfo(args[0]), outputFile;
            if (args.Length < 2) {
                outputFile = new FileInfo(Path.ChangeExtension(inputFile.FullName, ".bat"));
            } else {
                outputFile = new FileInfo(args[1]);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream templateStream = assembly.GetManifestResourceStream("Powershell2Batch.Template.bat"))
            using (StreamReader templateReader = new StreamReader(templateStream))
            using (StreamReader inputReader = new StreamReader(inputFile.FullName))
            using (StreamWriter writer = new StreamWriter(outputFile.FullName))
            {
                string templateLine, inputLine;
                while ((templateLine = templateReader.ReadLine()) != "::Payload") {
                    if (templateLine.EndsWith("::ScriptName"))
                        templateLine = templateLine.Replace("::ScriptName", inputFile.Name);
                    writer.WriteLine(templateLine);
                }
                    

                while ((inputLine = inputReader.ReadLine()) != null) {
                    foreach (string c in BatchSpecialChars)
                        inputLine = inputLine.Replace(c, "^" + c);
                    writer.WriteLine($"echo.{inputLine}>%scriptName%");
                }

                while ((templateLine = templateReader.ReadLine()) != null)
                    writer.WriteLine(templateLine);
            }

            Console.WriteLine("Done :)");
        }

        static void ValidateArgs(string[] args) {
            // validate parameters
            if (args.Length > 2) throw new ArgumentException("Too many arguments specified");
            if (args.Length == 0) throw new ArgumentException("No value specified for mandatory parameter \"<file path>\"");

            string inputFilePath = args[0];
            if (!File.Exists(inputFilePath)) throw new ArgumentException("Could not find input file");

            if (args.Length > 1) {
                string outputFilePath = args[1];
                if (!outputFilePath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Output file must end with \".bat\"");
                FileInfo fileInfo = new FileInfo(outputFilePath);
                if (!Directory.Exists(fileInfo.DirectoryName))
                    throw new ArgumentException("Output directory does not exist");
            }
        }

        static void ShowHelp() {
            Console.WriteLine(
                "USAGE:\n" +
                "    ps2cmd <file path> [output path]\n" +
                "PARAMS:\n" +
                "    <file path>   The input powershell file that will be transpiled\n" +
                "    [output path] The output batch file path. \n" +
                "                  If omitted, the output file will be placed alongside the input file"
            );

        }
    }
}
