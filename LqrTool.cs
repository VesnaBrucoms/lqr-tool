using lqr_tool.Models;
using System;
using static System.IO.File;

namespace lqr_tool
{
    class LqrTool
    {
        static void Main(string[] args)
        {
            byte[] bytes = ReadAllBytes("C:\\Users\\etste\\source\\repos\\lqr-tool\\testData\\Database\\Levels.lqr");

            int pointer = 0;

            Header header = ReadHeader(bytes);
            Console.WriteLine(header.ToString());

            // COLUMN DEFINITION
            pointer = (int)header.ColumnOffset + 8;
            ColumnDefinition columns = ReadColumnDefinitions(pointer, bytes);
            Console.WriteLine("\n" + columns.ToString());

            // HEADINGS
            pointer = (int)header.HeadingOffset;
            Headings headings = ReadHeadings(pointer, bytes);
            Console.WriteLine("\n" + headings.ToString());

            // UI WILL GO HERE

            // TEXT
            pointer = (int)header.TextOffset;
            string[] tableText = ReadTableText(pointer, bytes);

            // ROWS
            int rowsPointer = (int)header.RowsOffset;

            int usedEntries = BitConverter.ToInt32(bytes, rowsPointer);
            Console.WriteLine("\nusedEntries: " + usedEntries);
            rowsPointer += 4;

            int totalEntries = BitConverter.ToInt32(bytes, rowsPointer);
            Console.WriteLine("totalEntries: " + totalEntries);
            rowsPointer += 4;

            int posAdj = 0;
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
                    posAdj += 1;
                    continue;
                }

                Console.WriteLine(tableText[headings.HeadingIndices[i - posAdj]]);
                for (int j = 0; j < columns.ColumnDataTypes.Length; j++)
                {
                    if (columns.ColumnDataTypes[j] == 0)
                    {
                        Console.WriteLine(tableText[headings.GetColumnHeading(j)] + ": " + BitConverter.ToInt32(bytes, rowsPointer));
                        rowsPointer += 4;
                    }
                    else if (columns.ColumnDataTypes[j] == 1)
                    {
                        Console.WriteLine(tableText[headings.GetColumnHeading(j)] + ": " + BitConverter.ToSingle(bytes, rowsPointer));
                        rowsPointer += 4;
                    }
                    else if (columns.ColumnDataTypes[j] == 2)
                    {
                        byte unknownByte = bytes[rowsPointer];
                        //Console.WriteLine("unknown spacer?: " + unknownByte);
                        rowsPointer += 1;
                        if (unknownByte > 0)
                        {
                            Console.WriteLine("UNKNOWN IS: " + unknownByte);
                        }

                        int charsInFollowingString2 = BitConverter.ToInt32(bytes, rowsPointer);
                        rowsPointer += 4;
                        //Console.WriteLine("characterCount: " + charsInFollowingString2);
                        byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                        Array.Copy(bytes, rowsPointer, fileName2, 0, charsInFollowingString2 * 2);
                        Console.WriteLine(tableText[headings.GetColumnHeading(j)] + ": " + GetUnicode(fileName2));
                        rowsPointer += charsInFollowingString2 * 2;
                    }
                    else if (columns.ColumnDataTypes[j] == 3)
                    {
                        Console.WriteLine(tableText[headings.GetColumnHeading(j)] + ": " + BitConverter.ToInt32(bytes, rowsPointer));
                        rowsPointer += 4;
                    }
                    else if (columns.ColumnDataTypes[j] == 4)
                    {
                        byte unknownByte = bytes[rowsPointer];
                        //Console.WriteLine("unknown spacer?: " + unknownByte);
                        rowsPointer += 1;
                        if (unknownByte > 0)
                        {
                            Console.WriteLine("UNKNOWN IS: " + unknownByte);
                        }

                        int charsInFollowingString2 = BitConverter.ToInt32(bytes, rowsPointer);
                        rowsPointer += 4;
                        //Console.WriteLine("characterCount: " + charsInFollowingString2);
                        byte[] fileName2 = new byte[charsInFollowingString2 * 2];
                        Array.Copy(bytes, rowsPointer, fileName2, 0, charsInFollowingString2 * 2);
                        Console.WriteLine(tableText[headings.GetColumnHeading(j)] + ": " + GetUnicode(fileName2));
                        rowsPointer += charsInFollowingString2 * 2;
                    }
                }
            }

            // NEXT BIT 2
            // Most of this section is telling the program where to get the text for certain bits of the UI e.g. form heading
            // Seems to start with the columns
            // Then unknown
            int unknown1Pointer = (int)header.UiOffset;

            byte hasConverter = bytes[unknown1Pointer];
            Console.WriteLine("\nhasConverter: " + hasConverter); // External tool converter flag
            unknown1Pointer += 1;

            while (unknown1Pointer != header.TextOffset)
            {
                int unknown = BitConverter.ToInt32(bytes, unknown1Pointer);
                Console.WriteLine(unknown);
                if (unknown == -1)
                {
                    Console.Write("\n");
                }
                unknown1Pointer += 4;
            }

            Console.ReadKey();
        }

        static Header ReadHeader(byte[] file)
        {
            Header header = new Header();
            header.Unknown1 = BitConverter.ToInt32(file, 0); // always 106
            header.Unknown2 = BitConverter.ToInt32(file, 4); // checksum?
            header.ColumnOffset = BitConverter.ToInt64(file, 8);
            header.HeadingOffset = BitConverter.ToInt64(file, 16);
            header.UiOffset = BitConverter.ToInt64(file, 24);
            header.TextOffset = BitConverter.ToInt64(file, 32);
            header.RowsOffset = BitConverter.ToInt64(file, 40);
            return header;
        }

        static ColumnDefinition ReadColumnDefinitions(int pointer, byte[] file)
        {
            int columnCount = BitConverter.ToInt32(file, pointer);
            pointer += 4;
            ColumnDefinition columns = new ColumnDefinition(columnCount);
            for (int i = 0; i < columnCount; i++)
            {
                columns.ColumnDataTypes[i] = BitConverter.ToInt32(file, pointer + (4 * i));
            }
            return columns;
        }

        static Headings ReadHeadings(int pointer, byte[] file)
        {
            Headings headings = new Headings();
            // block of five flags? typically 0 or 1
            headings.Flag1 = file[pointer];
            pointer += 1;
            headings.Flag2 = file[pointer];
            pointer += 1;
            headings.Flag3 = file[pointer];
            pointer += 1;
            headings.Flag4 = file[pointer];
            pointer += 1;
            headings.Flag5 = file[pointer];
            pointer += 1;

            headings.BlankIndex = BitConverter.ToInt32(file, pointer);
            pointer += 4;
            headings.PrefixIndex = BitConverter.ToInt32(file, pointer);
            pointer += 4;
            headings.NameIndex = BitConverter.ToInt32(file, pointer);
            pointer += 4;

            int usedRows = BitConverter.ToInt32(file, pointer);
            pointer += 4;
            int columnCount = BitConverter.ToInt32(file, pointer);
            pointer += 4;
            headings.SetHeadingCount(usedRows, columnCount);
            for (int i = 0; i < headings.HeadingIndices.Length; i++)
            {
                headings.HeadingIndices[i] = BitConverter.ToInt32(file, pointer);
                pointer += 4;
            }
            return headings;
        }

        static string[] ReadTableText(int pointer, byte[] file)
        {
            int textCount = BitConverter.ToInt32(file, pointer);
            pointer += 4;
            int byteCount = BitConverter.ToInt32(file, pointer);
            pointer += 4;

            string[] tableText = new string[textCount];
            for (int i = 0; i < textCount; i++)
            {
                string newString = "";
                bool isTerminator = false;
                while (!isTerminator)
                {
                    char character = BitConverter.ToChar(file, pointer);
                    pointer += 2;
                    if (character == '\0')
                    {
                        isTerminator = true;
                        tableText[i] = newString;
                        continue;
                    }
                    else
                    {
                        newString += character;
                    }
                }
            }
            return tableText;
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
