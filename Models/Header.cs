using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lqr_tool.Models
{
    class Header
    {
        private int _unknown1;
        private int _unknown2;
        private long _columnOffset;
        private long _headingsOffset;
        private long _uiOffset;
        private long _textOffset;
        private long _rowsOffset;

        public int Unknown1
        {
            get { return _unknown1; }
            set { _unknown1 = value; }
        }

        public int Unknown2
        {
            get { return _unknown2; }
            set { _unknown2 = value; }
        }

        public long ColumnOffset
        {
            get { return _columnOffset; }
            set { _columnOffset = value; }
        }

        public long HeadingOffset
        {
            get { return _headingsOffset; }
            set { _headingsOffset = value; }
        }

        public long UiOffset
        {
            get { return _uiOffset; }
            set { _uiOffset = value; }
        }

        public long TextOffset
        {
            get { return _textOffset; }
            set { _textOffset = value; }
        }

        public long RowsOffset
        {
            get { return _rowsOffset; }
            set { _rowsOffset = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("unknown1: {0}, ", _unknown1);
            sb.AppendFormat("unknown2: {0}, ", _unknown2);
            sb.AppendFormat("columnOffset: {0}, ", _columnOffset);
            sb.AppendFormat("headingsOffset: {0}, ", _headingsOffset);
            sb.AppendFormat("uiOffset: {0}, ", _uiOffset);
            sb.AppendFormat("textOffset: {0}, ", _textOffset);
            sb.AppendFormat("rowsOffset: {0}", _rowsOffset);
            return sb.ToString();
        }
    }
}
