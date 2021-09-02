using System;
using static System.IO.File;

namespace lqr_tool
{
    class LqrTool
    {
        static void Main(string[] args)
        {
            byte[] bytes = ReadAllBytes("C:\\Users\\etste\\source\\repos\\lqr-tool\\testData\\Database\\Buildings.lqr");

            Console.WriteLine(BitConverter.ToInt32(bytes, 0));
            Console.WriteLine(BitConverter.ToInt32(bytes, 4));
            Console.WriteLine(BitConverter.ToInt64(bytes, 8)); // entries offset offset?
            long afterOffset = BitConverter.ToInt64(bytes, 16);
            Console.WriteLine("afterOffset: " + afterOffset); // offset -> after entries
            long unknownOffset1 = BitConverter.ToInt64(bytes, 24);
            Console.WriteLine("unknownOffset1: " + unknownOffset1); // offset -> unknown
            long unknownOffset2 = BitConverter.ToInt64(bytes, 32);
            Console.WriteLine("unknownOffset2: " + unknownOffset2); // offset -> unknown
            long entriesOffset = BitConverter.ToInt64(bytes, 40);
            int entriesPointer = (int)entriesOffset;
            Console.WriteLine("entriesOffset: " + entriesOffset);

            // 5 fields that inform how to read the table?
            int numberOfColumns = BitConverter.ToInt32(bytes, 48);
            Console.WriteLine("numberOfColumns: " + BitConverter.ToInt32(bytes, 48));
            for (int i = 0; i < numberOfColumns; i++)
            {
                Console.WriteLine("type: " + BitConverter.ToInt32(bytes, 52 + (4 * i)));

                // TYPES:
                // 0 = unknown (4 bytes)
                // 1 = unknown
                // 2 = string? (x bytes) so far seen for in-game text
                // 3 = int32   (4 bytes)
                // 4 = string? (x bytes) so far seen for file names
            }

            int usedEntries = BitConverter.ToInt32(bytes, entriesPointer);
            Console.WriteLine("\nusedEntries: " + usedEntries);
            entriesPointer += 4;

            int totalEntries = BitConverter.ToInt32(bytes, entriesPointer);
            Console.WriteLine("totalEntries: " + totalEntries);
            entriesPointer += 4;

            for (int i = 0; i < totalEntries; i++)
            {
                int fileId = BitConverter.ToInt32(bytes, entriesPointer);
                Console.WriteLine("\nfileId: " + fileId);
                entriesPointer += 4;
                Boolean isEnabled = BitConverter.ToBoolean(bytes, entriesPointer);
                Console.WriteLine("isEnabled: " + isEnabled);
                entriesPointer += 1;
                if (!isEnabled)
                {
                    Console.WriteLine("DELETED/UNUSED ENTRY");
                    continue;
                }
                Boolean unknown = BitConverter.ToBoolean(bytes, entriesPointer);
                Console.WriteLine("unknown: " + unknown);
                entriesPointer += 1;
                int charsInFollowingString2 = BitConverter.ToInt32(bytes, entriesPointer);
                entriesPointer += 4;
                Console.WriteLine("characterCount: " + charsInFollowingString2);
                byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                Array.Copy(bytes, entriesPointer, fileName2, 0, charsInFollowingString2 * 2);
                Console.WriteLine(GetUnicode(fileName2));
                entriesPointer += charsInFollowingString2 * 2;
                Console.WriteLine(BitConverter.ToInt32(bytes, entriesPointer));
                entriesPointer += 4;
                Console.WriteLine(BitConverter.ToInt32(bytes, entriesPointer));
                entriesPointer += 4;
                Console.WriteLine(BitConverter.ToInt32(bytes, entriesPointer));
                entriesPointer += 4;
            }

            Console.ReadKey();
        }

        private static string GetUnicode(byte[] bytes)
        {
            string newString = "";
            for (int i = 0; i < bytes.Length; i += 2)
            {
                char character = BitConverter.ToChar(bytes, i);
                newString += character;
            }
            return newString;
        }
    }
}
