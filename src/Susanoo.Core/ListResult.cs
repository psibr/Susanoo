#region

using System.Collections.Generic;

#endregion

namespace Susanoo
{
    /// <summary>
    /// A list that implements IResultSet
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public class ListResult<TResult> : List<TResult>, IResultSet
    {
        private Dictionary<string, int> _queryResultColumnInfo;
        internal ColumnChecker ColumnReport { get; private set; }

        /// <summary>
        /// Gets the available columns in the result set.
        /// </summary>
        /// <value>The available columns.</value>
        public Dictionary<string, int> AvailableColumns
        {
            get
            {
                if (_queryResultColumnInfo == null)
                    _queryResultColumnInfo = ColumnReport.ExportReport();

                return new Dictionary<string, int>(_queryResultColumnInfo);
            }
        }

        /// <summary>
        /// Provides the information to build a mapping report.
        /// </summary>
        /// <param name="checker">The checker.</param>
        public void BuildReport(ColumnChecker checker)
        {
            ColumnReport = checker;
        }
    }

    /// <summary>
    /// A result object that returns columns that were available.
    /// </summary>
    public interface IResultSet
    {
        /// <summary>
        ///     Gets the available columns in the result set.
        /// </summary>
        /// <value>The available columns.</value>
        Dictionary<string, int> AvailableColumns { get; }
    }
}