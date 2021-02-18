using System;
using System.IO;
using CommandLine;
using CommandLine.Text;
using P5SPCSaveUtil.CommandLine;
using P5SPCSaveUtil.Save;

namespace P5SPCSaveUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<CryptOptions, ConvertOptions>(args);

            try
            {
                parserResult.MapResult(
                        (CryptOptions o) => Crypt(o),
                        (ConvertOptions o) => Convert(o),
                        errors => DisplayHelp(parserResult));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        static int DisplayHelp<T>(ParserResult<T> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "p5spc.saveutil";
                h.Copyright = "";
                h.AutoVersion = false;
                h.AddDashesToOption = true;
                return h;
            },
            e => e,
            verbsIndex: true);
            Console.WriteLine(helpText);
            return -1;
        }

        static int Crypt(CryptOptions o)
        {
            if (!File.Exists(o.PathIn))
            {
                Console.WriteLine("Input path doesn't exist.");
                return 1;
            }

            SaveCrypt.CryptFile(o.PathIn, o.PathOut, o.SteamId);

            return 0;
        }

        static int Convert(ConvertOptions o)
        {
            if (!File.Exists(o.PathIn))
            {
                Console.WriteLine("Input path doesn't exist.");
                return 1;
            }

            SaveConvert.ConvertFile(o.PathIn, o.PathOut);

            return 0;
        }
    }
}
