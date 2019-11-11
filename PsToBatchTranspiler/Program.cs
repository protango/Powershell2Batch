using System;
using System.IO;

namespace PsToBatchTranspiler {
    class Program { 
        static void Main(string[] args) {
            try {
                ValidateArgs(args);
            }
            catch (ArgumentException e) {
                Console.Error.WriteLine(e.Message);
                ShowHelp();
                return;
            }


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
                "USAGE: " +
                "    ps2cmd <file path> [output path]\n" +
                "PARAMS:\n" +
                "    <file path>   specifies the input powershell file that will be transpiled\n" +
                "    [output path] specifies the output batch file path. If omitted, the batch file will be placed in the same directory as the input file"
            );

        }
    }
}
