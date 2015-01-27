#region

using System.Collections.Generic;
using System.Data;
using System.Linq;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets.Processing
{
    /// <summary>
    /// Tracks available fields in return results to allow for efficient column existence checks.
    /// </summary>
    public class ColumnChecker
    {
        private bool _isInit;

        private readonly Map<int, string> _fieldMap;

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
        public int Count
        {
            get { return _fieldMap.Forward.Count; }
        }

        private ColumnChecker(IDictionary<int, string> intKey, IDictionary<string, int> stringKey)
        {
            _isInit = true;
            _fieldMap = new Map<int, string>(intKey, stringKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnChecker"/> class.
        /// </summary>
        public ColumnChecker() { _fieldMap = new Map<int, string>(); }

        /// <summary>
        ///  Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public int HasColumn(IDataRecord record, string name)
        {
            int value;
            if (_isInit)
            {
                if (_fieldMap.TryGetValue(name, out value))
                    return value;
            }

            value = record.GetOrdinal(name);
            _fieldMap.Add(name, value);

            _isInit = true;

            return value;
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

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>ColumnChecker.</returns>
        public ColumnChecker Copy()
        {
            return new ColumnChecker(_fieldMap.Forward.ToDictionary(), _fieldMap.Reverse.ToDictionary());
        }
    }
}