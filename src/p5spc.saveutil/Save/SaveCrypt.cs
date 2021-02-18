using System;
using System.Buffers.Binary;
using System.IO;

namespace P5SPCSaveUtil.Save
{
    public static class SaveCrypt
    {
        public static bool IsEncrypted(Span<byte> data)
        {
            var slot = BinaryPrimitives.ReadUInt32LittleEndian(data[4..]);
            return slot > 9;
        }

        public static void CryptFile(string path, string path_out, long key)
        {
            var data = File.ReadAllBytes(path).AsSpan();
            var before = IsEncrypted(data);

            if (before)
                Console.WriteLine("Decrypting...");
            else
                Console.WriteLine("Encrypting...");

            Crypt(data, key);

            if (before && IsEncrypted(data))
            {
                Console.WriteLine("Decryption failed, maybe wrong SteamID64?");
                return;
            }

            if (string.IsNullOrWhiteSpace(path_out))
                path_out = $"{path}_crypt";

            Console.WriteLine($"Writing to {path_out}...");
            File.WriteAllBytes(path_out, data.ToArray());

            Console.WriteLine("Done.");
        }

        public static void Crypt(byte[] data, long key)
            => Crypt(new Span<byte>(data), key);

        public static void Crypt(Span<byte> data, long key)
        {
            var size = data.Length - 4;
            byte checksum = 0;
            uint state = (uint)key ^ 0x20090501;

            if (size >= 2)
            {
                for (int i = 0; i < size; i += 1)
                {
                    state = 0x41C64E6D * state + 12345;
                    checksum += data[i];
                    data[i] ^= (byte)(state >> 16);
                }
            }

            data[size] = checksum;
        }
    }
}
