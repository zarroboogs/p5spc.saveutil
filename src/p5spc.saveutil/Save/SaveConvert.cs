using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;

namespace P5SPCSaveUtil.Save
{
    public enum SaveFmt
    {
        Switch_JP,
        Switch_EN,
        PC,
        PS4_JP,
        PS4_EN,
    }

    public record SaveFmtDesc(SaveFmt Format, int Size, int NameLength);

    public class SaveConvert
    {
        static readonly SaveFmtDesc[] Formats =
        {
            new (SaveFmt.Switch_JP, 0x600000, 13),
            new (SaveFmt.Switch_EN, 0x600000, 33),
            new (SaveFmt.PC,        0x55DEA0, 33),
            new (SaveFmt.PS4_JP,    0x55DB2C, 13),
            new (SaveFmt.PS4_EN,    0x55DCBC, 33),
        };

        public static SaveFmtDesc DetectPlatform(Span<byte> data)
        {
            foreach (var fmt in Formats)
            {
                var lang = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x1C + 0x88DF4 + fmt.NameLength * 2 + 0x938, 4)) == 0x0036EE7F;
                var sz = fmt.Size == data.Length;

                if (lang && sz) return fmt;
            }

            return null;
        }

        public static void ConvertFile(string path, string pathOut, SaveFmt dstFmt)
        {
            var data = File.ReadAllBytes(path);
            var srcFormat = DetectPlatform(data);

            if (srcFormat == null)
            {
                Console.WriteLine("Unknown save format, aborting...");
                return;
            }

            Console.WriteLine($"Detected format <{srcFormat.Format}>...");

            var dstFormat = Formats.Where(x => x.Format == dstFmt).FirstOrDefault();
            Console.WriteLine($"Converting to <{dstFormat.Format}> format...");

            data = Convert(data, srcFormat, dstFormat);

            if (string.IsNullOrWhiteSpace(pathOut))
                pathOut = $"{path}_conv";

            Console.WriteLine($"Writing to {pathOut}...");
            File.WriteAllBytes(pathOut, data);

            Console.WriteLine("Done.");
        }

        public static byte[] Convert(byte[] data, SaveFmtDesc inFormat, SaveFmtDesc outFormat)
        {
            var headerSize = 0x1C; // relative to save file
            var baseSize = 0x88DF4; // relative to game save, size without name
            var nameOffset = 0x87842; // relative to game save

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
