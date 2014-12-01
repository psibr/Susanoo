using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// A linked list that implements IResultSet
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public class LinkedListResult<TResult> : LinkedList<TResult>, IResultSet
    {
        private  Dictionary<string, int> _QueryResultColumnInfo;
        private ColumnChecker _Checker;

        /// <summary>
        /// Gets the available columns in the result set.
        /// </summary>
        /// <value>The available columns.</value>
        public Dictionary<string, int> AvailableColumns
        {
            get 
            {
                if(_QueryResultColumnInfo == null)
                    _QueryResultColumnInfo = _Checker.ExportReport();

                return new Dictionary<string, int>(_QueryResultColumnInfo); 
            }
        }

        /// <summary>
        /// Provides the information to build a mapping report.
        /// </summary>
        /// <param name="checker">The checker.</param>
        public void BuildReport(ColumnChecker checker)
        {
            _Checker = checker;
        }
    }

    /// <summary>
    /// A result object that returns columns that were available.
    /// </summary>
    public interface IResultSet
    {
        /// <summary>
        /// Gets the available columns in the result set.
        /// </summary>
        /// <value>The available columns.</value>
        Dictionary<string, int> AvailableColumns { get; }
    }
}
