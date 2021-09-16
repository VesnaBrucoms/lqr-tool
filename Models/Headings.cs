using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lqr_tool.Models
{
    class Headings
    {
        private byte _flag1;
        private byte _flag2;
        private byte _flag3;
        private byte _flag4;
        private byte _flag5;
        private int _blankIndex;
        private int _prefixIndex;
        private int _nameIndex;
        private int[] _headingIndices;
        private int _columnCount;

        public byte Flag1
        {
            get { return _flag1; }
            set { _flag1 = value; }
        }
        public byte Flag2
        {
            get { return _flag2; }
            set { _flag2 = value; }
        }
        public byte Flag3
        {
            get { return _flag3; }
            set { _flag3 = value; }
        }
        public byte Flag4
        {
            get { return _flag4; }
            set { _flag4 = value; }
        }
        public byte Flag5
        {
            get { return _flag5; }
            set { _flag5 = value; }
        }

        public int BlankIndex
        {
            get { return _blankIndex; }
            set { _blankIndex = value; }
        }

        public int PrefixIndex
        {
            get { return _prefixIndex; }
            set { _prefixIndex = value; }
        }

        public int NameIndex
        {
            get { return _nameIndex; }
            set { _nameIndex = value; }
        }

        public int[] HeadingIndices
        {
            get { return _headingIndices; }
        }

        public void SetHeadingCount(int rowCounts, int columnCount)
        {
            _headingIndices = new int[rowCounts + columnCount];
            _columnCount = columnCount;
        }

        public int GetColumnHeading(int columnEntry)
        {
            int columnIndex = _headingIndices.Length - (_columnCount - columnEntry);
            return _headingIndices[columnIndex];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("flag1: {0}", _flag1);
            sb.AppendFormat("\nflag2: {0}", _flag2);
            sb.AppendFormat("\nflag3: {0}", _flag3);
            sb.AppendFormat("\nflag4: {0}", _flag4);
            sb.AppendFormat("\nflag5: {0}", _flag5);
            sb.AppendFormat("\nblankIndex: {0}", _blankIndex);
            sb.AppendFormat("\nprefixIndex: {0}", _prefixIndex);
            sb.AppendFormat("\nnameIndex: {0}", _nameIndex);
            foreach (int index in _headingIndices)
            {
                sb.AppendFormat("\n{0}", index);
            }
            return sb.ToString();
        }
    }
}
