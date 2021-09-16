using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lqr_tool.Models
{
    class ColumnDefinition
    {
        private int[] _columnDataTypes;

        public int[] ColumnDataTypes
        {
            get { return _columnDataTypes; }
        }

        public ColumnDefinition(int columnCount)
        {
            _columnDataTypes = new int[columnCount];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("columnCount: {0}", _columnDataTypes.Length);
            int count = 0;
            foreach (int type in _columnDataTypes)
            {
                sb.AppendFormat("\ncolumn {0}: {1}", count, type);
                count += 1;
            }
            return sb.ToString();
        }
    }
}
