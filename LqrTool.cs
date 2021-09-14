using System;
using static System.IO.File;

namespace lqr_tool
{
    class LqrTool
    {
        static void Main(string[] args)
        {
            byte[] bytes = ReadAllBytes("C:\\Users\\etste\\source\\repos\\lqr-tool\\testData\\Database\\Levels.lqr");

            Console.WriteLine(BitConverter.ToInt32(bytes, 0)); // always 106
            Console.WriteLine(BitConverter.ToInt32(bytes, 4)); // checksum?
            long columnOffset = BitConverter.ToInt64(bytes, 8);
            Console.WriteLine("columnOffset: " + columnOffset);
            long afterOffset = BitConverter.ToInt64(bytes, 16);
            Console.WriteLine("afterOffset: " + afterOffset); // offset -> after entries
            long unknownOffset1 = BitConverter.ToInt64(bytes, 24);
            Console.WriteLine("unknownOffset1: " + unknownOffset1); // offset -> unknown
            long textOffset = BitConverter.ToInt64(bytes, 32);
            Console.WriteLine("textOffset: " + textOffset);
            long rowsOffset = BitConverter.ToInt64(bytes, 40);
            int rowsPointer = (int)rowsOffset;
            Console.WriteLine("rowsOffset: " + rowsOffset);

            int numberOfColumns = BitConverter.ToInt32(bytes, 48);
            Console.WriteLine("numberOfColumns: " + BitConverter.ToInt32(bytes, 48));
            int[] columnTypes = new int[numberOfColumns];
            for (int i = 0; i < numberOfColumns; i++)
            {
                columnTypes[i] = BitConverter.ToInt32(bytes, 52 + (4 * i));
                Console.WriteLine("type for col " + i + ": " + columnTypes[i]);
            }

            int usedEntries = BitConverter.ToInt32(bytes, rowsPointer);
            Console.WriteLine("\nusedEntries: " + usedEntries);
            rowsPointer += 4;

            int totalEntries = BitConverter.ToInt32(bytes, rowsPointer);
            Console.WriteLine("totalEntries: " + totalEntries);
            rowsPointer += 4;

            for (int i = 0; i < totalEntries; i++)
            {
                int fileId = BitConverter.ToInt32(bytes, rowsPointer);
                Console.WriteLine("\nfileId: " + fileId);
                rowsPointer += 4;
                Boolean isEnabled = BitConverter.ToBoolean(bytes, rowsPointer);
                Console.WriteLine("isEnabled: " + isEnabled);
                rowsPointer += 1;
                if (!isEnabled)
                {
                    Console.WriteLine("DELETED/UNUSED ENTRY");
                    continue;
                }

                for (int j = 0; j < columnTypes.Length; j++)
                {
                    if (columnTypes[j] == 0)
                    {
                        Console.WriteLine("col type 0: " + BitConverter.ToInt32(bytes, rowsPointer));
                        rowsPointer += 4;
                    }
                    else if (columnTypes[j] == 1)
                    {
                        Console.WriteLine("col type 1: " + BitConverter.ToSingle(bytes, rowsPointer));
                        rowsPointer += 4;
                    }
                    else if (columnTypes[j] == 2)
                    {
                        byte unknownByte = bytes[rowsPointer];
                        Console.WriteLine("unknown spacer?: " + unknownByte);
                        rowsPointer += 1;
                        if (unknownByte > 0)
                        {
                            Console.WriteLine("UNKNOWN IS: " + unknownByte);
                        }

                        int charsInFollowingString2 = BitConverter.ToInt32(bytes, rowsPointer);
                        rowsPointer += 4;
                        Console.WriteLine("characterCount: " + charsInFollowingString2);
                        byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                        Array.Copy(bytes, rowsPointer, fileName2, 0, charsInFollowingString2 * 2);
                        Console.WriteLine(GetUnicode(fileName2));
                        rowsPointer += charsInFollowingString2 * 2;
                    }
                    else if (columnTypes[j] == 3)
                    {
                        Console.WriteLine("col type 3: " + BitConverter.ToInt32(bytes, rowsPointer));
                        rowsPointer += 4;
                    }
                    else if (columnTypes[j] == 4)
                    {
                        byte unknownByte = bytes[rowsPointer];
                        Console.WriteLine("unknown spacer?: " + unknownByte);
                        rowsPointer += 1;
                        if (unknownByte > 0)
                        {
                            Console.WriteLine("UNKNOWN IS: " + unknownByte);
                        }

                        int charsInFollowingString2 = BitConverter.ToInt32(bytes, rowsPointer);
                        rowsPointer += 4;
                        Console.WriteLine("characterCount: " + charsInFollowingString2);
                        byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                        Array.Copy(bytes, rowsPointer, fileName2, 0, charsInFollowingString2 * 2);
                        Console.WriteLine(GetUnicode(fileName2));
                        rowsPointer += charsInFollowingString2 * 2;
                    }
                }
            }

            //NEXT BIT
            int afterPointer = (int)afterOffset;

            // block of five flags? typically 0 or 1
            Console.WriteLine("\n" + bytes[afterPointer]);
            afterPointer += 1;
            Console.WriteLine(bytes[afterPointer]);
            afterPointer += 1;
            Console.WriteLine(bytes[afterPointer]);
            afterPointer += 1;
            Console.WriteLine(bytes[afterPointer]);
            afterPointer += 1;
            Console.WriteLine(bytes[afterPointer]);
            afterPointer += 1;

            Console.WriteLine(BitConverter.ToInt32(bytes, afterPointer));
            afterPointer += 4;
            Console.WriteLine(BitConverter.ToInt32(bytes, afterPointer));
            afterPointer += 4;
            Console.WriteLine(BitConverter.ToInt32(bytes, afterPointer));
            afterPointer += 4;
            int usedEntries2 = BitConverter.ToInt32(bytes, afterPointer);
            Console.WriteLine("usedEntries: " + usedEntries2); // this + no of columns indicates integers to read in this section
            afterPointer += 4;
            int noColumns2 = BitConverter.ToInt32(bytes, afterPointer);
            Console.WriteLine("numberOfColumns: " + noColumns2); // this + used entries indicates integers to read in this section
            afterPointer += 4;
            for (int i = 0; i < (usedEntries2 + noColumns2); i++)
            {
                Console.WriteLine(BitConverter.ToInt32(bytes, afterPointer));
                afterPointer += 4;
            }

            // NEXT BIT 2
            // 1 byte followed by a lot of integers?
            int unknown1Pointer = (int)unknownOffset1;

            Console.WriteLine("\n" + bytes[unknown1Pointer]);
            unknown1Pointer += 1;
            while (unknown1Pointer != textOffset)
            {
                int unknown = BitConverter.ToInt32(bytes, unknown1Pointer);
                Console.WriteLine(unknown);
                if (unknown == 0xFFFF)
                {
                    Console.WriteLine("\n");
                }
                unknown1Pointer += 4;
            }

            // TEXT
            int textPointer = (int)textOffset;

            int textCount = BitConverter.ToInt32(bytes, textPointer);
            Console.WriteLine("\ntextCount: " + textCount);
            textPointer += 4;
            int byteCount = BitConverter.ToInt32(bytes, textPointer);
            Console.WriteLine("byteCount: " + byteCount);
            textPointer += 4;

            string[] strings = new string[textCount];
            for (int i = 0; i < textCount; i++)
            {
                string newString = "";
                bool isTerminator = false;
                while (!isTerminator)
                {
                    char character = BitConverter.ToChar(bytes, textPointer);
                    textPointer += 2;
                    if (character == '\0')
                    {
                        isTerminator = true;
                        strings[i] = newString;
                        continue;
                    }
                    else
                    {
                        newString += character;
                    }
                }
                Console.WriteLine(newString);
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
