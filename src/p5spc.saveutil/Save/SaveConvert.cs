using System;
using System.IO;
using System.Linq;

namespace P5SPCSaveUtil.Save
{
    public record SaveFormat(string Name, int Size, int NameLength);

    public class SaveConvert
    {
        static readonly SaveFormat[] _formats = new SaveFormat[]
        {
            new ("switch_jp", 0x600000, 13),
            new ("pc", 0x55DEA0, 33),
        };

        public static void ConvertFile(string path, string path_out)
        {
            var data = File.ReadAllBytes(path);
            var srcFormat = _formats.Where(x => x.Size == data.Length).FirstOrDefault();

            if (srcFormat == null)
            {
                Console.WriteLine("Unknown save format, aborting...");
                return;
            }

            Console.WriteLine($"Detected format <{srcFormat.Name}>...");

            var dstFormat = srcFormat.Name == "switch_jp" ? _formats[1] : _formats[0];
            Console.WriteLine($"Converting to <{dstFormat.Name}> format...");

            data = Convert(data, srcFormat, dstFormat);

            if (string.IsNullOrWhiteSpace(path_out))
                path_out = $"{path}_conv";

            Console.WriteLine($"Writing to {path_out}...");
            File.WriteAllBytes(path_out, data);

            Console.WriteLine("Done.");
        }

        public static byte[] Convert(byte[] data, SaveFormat inFormat, SaveFormat outFormat)
        {
            var headerSize = 0xC; // relative to save file
            var baseSize = 0x88DF4; // relative to game save, size without name
            var nameOffset = 0x87852; // relative to game save

            var saveGameSize = baseSize + outFormat.NameLength * 2;

            var conv = new byte[outFormat.Size];

            var msSrc = new MemoryStream(data);
            var msDst = new MemoryStream(conv);

            var src = new BinaryReader(msSrc);
            var dst = new BinaryWriter(msDst);

            void RWCopy(int n) => dst.Write(src.ReadBytes(n));

            void RWName(int srcLen, int dstLen)
            {
                var nameSrc = src.ReadBytes(srcLen);
                var nameDst = new byte[dstLen];
                Buffer.BlockCopy(nameSrc, 0, nameDst, 0, Math.Min(srcLen, dstLen) - 1);
                dst.Write(nameDst);
            }

            RWCopy(headerSize);

            for (int i = 0; i < 10; i++)
            {
                RWCopy(nameOffset); // copy upto name in game save
                RWName(inFormat.NameLength, outFormat.NameLength); // copy fname
                RWName(inFormat.NameLength, outFormat.NameLength); // copy lname
                RWCopy(saveGameSize - nameOffset - outFormat.NameLength * 2); // copy the rest
            }

            //msDst.Seek(0, 0);
            //dst.Write(0x20012000); // write version

            return conv;
        }
    }
}
