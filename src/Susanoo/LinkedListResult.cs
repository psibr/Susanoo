using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public class LinkedListResult<TResult> : LinkedList<TResult>
    {
        private readonly Dictionary<string, int> _QueryResultColumnInfo;
        Dictionary<string, int> QueryResultColumnInfo
        {
            get { return new Dictionary<string, int>(_QueryResultColumnInfo); }
        }
        public LinkedListResult(ColumnChecker checker)
            : base()
        {
            _QueryResultColumnInfo = checker.ExportReport();
        }
    }
}
