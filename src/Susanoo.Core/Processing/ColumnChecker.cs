#region

using System.Collections.Generic;
using System.Data;

#endregion

namespace Susanoo.Processing
{
    /// <summary>
    /// Tracks available fields in return results to allow for efficient column existence checks.
    /// </summary>
    public class ColumnChecker
    {
        private bool _isInit;

        private Map<int, string> _fieldMap;

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if contains key, <c>false</c> otherwise.</returns>
        public bool TryGetValue(int key, out string value)
        {
            return _fieldMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if contains key, <c>false</c> otherwise.</returns>
        public bool TryGetValue(string key, out int value)
        {
            return _fieldMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the count of columns mapped.
        /// </summary>
        /// <value>The count.</value>
        public int Count => 
            _fieldMap.Forward.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnChecker"/> class.
        /// </summary>
        public ColumnChecker(int? capacity = null)
        {
            _fieldMap = capacity != null ? new Map<int, string>(capacity.Value) : new Map<int, string>();
        }

        /// <summary>
        ///  Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public int HasColumn(IDataRecord record, string name)
        {
            if (!_isInit)
            {

                /* Cant use GetOrdinal here since not all fields will be present and
                 * exception handling is slower than a one time read */
                for (var i = 0; i < record.FieldCount; i++)
                    _fieldMap.Add(record.GetName(i), i);

                _isInit = true;
            }

            int value;
            return _fieldMap.TryGetValue(name, out value) ? value : -1;
        }

        /// <summary>
        /// Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public string HasColumn(IDataRecord record, int index)
        {
            string value;
            if (_isInit)
            {
                if (_fieldMap.TryGetValue(index, out value))
                    return value;
            }

            value = record.GetName(index);
            _fieldMap.Add(index, value);

            _isInit = true;

            return value;
        }

        /// <summary>
        /// Exports a dictionary showing mapped columns and indexes.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Int32&gt;.</returns>
        public Dictionary<string, int> ExportReport()
        {
            return _fieldMap.Reverse.ToDictionary();
        }
    }
}