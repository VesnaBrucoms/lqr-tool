using System;
using static System.IO.File;

namespace lqr_tool
{
    class LqrTool
    {
        static void Main(string[] args)
        {
            byte[] bytes = ReadAllBytes("C:\\Users\\etste\\source\\repos\\lqr-tool\\testData\\Database\\Clans.lqr");

            Console.WriteLine(BitConverter.ToInt32(bytes, 0));
            Console.WriteLine(BitConverter.ToInt32(bytes, 4));
            Console.WriteLine(BitConverter.ToInt64(bytes, 8)); // entries offset offset?
            long afterOffset = BitConverter.ToInt64(bytes, 16);
            Console.WriteLine("afterOffset: " + afterOffset); // offset -> after entries
            long unknownOffset1 = BitConverter.ToInt64(bytes, 24);
            Console.WriteLine("unknownOffset1: " + unknownOffset1); // offset -> unknown
            long unknownOffset2 = BitConverter.ToInt64(bytes, 32);
            Console.WriteLine("unknownOffset2: " + unknownOffset2); // offset -> labels? column headings
            long entriesOffset = BitConverter.ToInt64(bytes, 40);
            int entriesPointer = (int)entriesOffset;
            Console.WriteLine("entriesOffset: " + entriesOffset);

            // 5 fields that inform how to read the table?
            int numberOfColumns = BitConverter.ToInt32(bytes, 48);
            Console.WriteLine("numberOfColumns: " + BitConverter.ToInt32(bytes, 48));
            int[] columnTypes = new int[numberOfColumns];
            for (int i = 0; i < numberOfColumns; i++)
            {
                columnTypes[i] = BitConverter.ToInt32(bytes, 52 + (4 * i));
                Console.WriteLine("type for col " + i + ": " + columnTypes[i]);

                // TYPES:
                // 0 = int32? (4 bytes)
                // 1 = float? (4 bytes)
                // 2 = string? (x bytes) so far seen for in-game text, there's an extra byte after the terminator
                // 3 = uint32?   (4 bytes)
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

                foreach (int column in columnTypes)
                {
                    if (column == 0)
                    {
                        Console.WriteLine("col type 0: " + BitConverter.ToInt32(bytes, entriesPointer));
                        entriesPointer += 4;
                    } else if (column == 1)
                    {
                        Console.WriteLine("col type 1: " + BitConverter.ToSingle(bytes, entriesPointer));
                        entriesPointer += 4;
                    }
                    else if (column == 2)
                    {
                        int charsInFollowingString2 = BitConverter.ToInt32(bytes, entriesPointer);
                        entriesPointer += 4;
                        Console.WriteLine("characterCount: " + charsInFollowingString2);
                        byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                        Array.Copy(bytes, entriesPointer, fileName2, 0, charsInFollowingString2 * 2);
                        Console.WriteLine(GetUnicode(fileName2));
                        entriesPointer += charsInFollowingString2 * 2;
                    }
                    else if (column == 3)
                    {
                        Console.WriteLine("col type 3: " + BitConverter.ToInt32(bytes, entriesPointer));
                        entriesPointer += 4;
                    }
                    else if (column == 4)
                    {
                        int charsInFollowingString2 = BitConverter.ToInt32(bytes, entriesPointer);
                        entriesPointer += 4;
                        Console.WriteLine("characterCount: " + charsInFollowingString2);
                        byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                        Array.Copy(bytes, entriesPointer, fileName2, 0, charsInFollowingString2 * 2);
                        Console.WriteLine(GetUnicode(fileName2));
                        entriesPointer += charsInFollowingString2 * 2;
                    }
                }
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
