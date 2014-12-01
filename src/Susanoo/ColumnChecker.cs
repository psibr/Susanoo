#region

using System.Collections.Generic;
using System.Data;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Tracks available fields in return results to allow for efficient column existence checks.
    /// </summary>
    public class ColumnChecker
    {
        private readonly Dictionary<string, int> _fields = new Dictionary<string, int>();
        private bool _isInit;

        /// <summary>
        ///     Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public int HasColumn(IDataRecord record, string name)
        {
            if (_isInit)
            {
                int value;
                return _fields.TryGetValue(name, out value) ? value : -1;
            }

            for (var i = 0; i < record.FieldCount; i++)
                _fields.Add(record.GetName(i), i);

            _isInit = true;

            int value1;
            return _fields.TryGetValue(name, out value1) ? value1 : -1;
        }

        /// <summary>
        /// Exports a dictionary showing mapped columns and indexes.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Int32&gt;.</returns>
        public Dictionary<string, int> ExportReport()
        {
            return new Dictionary<string, int>(_fields);
        }
    }
}