using CommandLine;
using P5SPCSaveUtil.Save;

namespace P5SPCSaveUtil.CommandLine
{
    [Verb("convert", HelpText = "Convert P5S save data.")]
    public class ConvertOptions
    {
        [Option('i', "input", Required = true, HelpText = "Path to P5S save file.")]
        public string PathIn { get; set; }

        [Option('o', "output", Required = false)]
        public string PathOut { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target format.")]
        public SaveFmt Target { get; set; }
    }
}
