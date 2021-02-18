using CommandLine;

namespace P5SPCSaveUtil.CommandLine
{
    [Verb("crypt", HelpText = "Encrypt/decrypt P5S PC save data.")]
    internal class CryptOptions
    {
        [Option('i', "input", Required = true, HelpText = "Path to P5S PC save file.")]
        public string PathIn { get; set; }

        [Option('o', "output", Required = false)]
        public string PathOut { get; set; }

        [Option('s', "steam", Required = true, HelpText = "SteamID64 to encrypt or decrypt with.")]
        public long SteamId { get; set; }
    }
}
