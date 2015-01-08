#region

using System.Collections.Generic;
using System.Data;
using System.Linq;

#endregion

namespace Susanoo
{
    /// <summary>
    /// Tracks available fields in return results to allow for efficient column existence checks.
    /// </summary>
    public class ColumnChecker
    {
        private bool _isInit;
        private bool _isStringBased;
        private readonly Dictionary<int, string> _intKeyFields = new Dictionary<int, string>();
        private readonly Dictionary<string, int> _stringKeyFields = new Dictionary<string, int>();

        private ColumnChecker(IDictionary<int, string> intKey, IDictionary<string, int> stringKey, bool isStringBased)
        {
            _isInit = true;
            _intKeyFields = new Dictionary<int, string>(intKey);
            _stringKeyFields = new Dictionary<string, int>(stringKey);
            _isStringBased = isStringBased;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnChecker"/> class.
        /// </summary>
        public ColumnChecker() { }

        /// <summary>
        ///  Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public int HasColumn(IDataRecord record, string name)
        {
            if (_isInit)
            {
                int value;
                return _stringKeyFields.TryGetValue(name, out value) ? value : -1;
            }
            _isStringBased = true;
            // Could be possible to speed things up by using GetOrdinal.
            for (var i = 0; i < record.FieldCount; i++)
                _stringKeyFields.Add(record.GetName(i), i);

            _isInit = true;

            int value1;
            return _stringKeyFields.TryGetValue(name, out value1) ? value1 : -1;
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
                if (_intKeyFields.TryGetValue(index, out value))
                    return value;
            }

            value = record.GetName(index);
            _intKeyFields.Add(index, value);

            _isInit = true;

            return value;
        }

        /// <summary>
        /// Exports a dictionary showing mapped columns and indexes.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Int32&gt;.</returns>
        public Dictionary<string, int> ExportReport()
        {
            return _isStringBased
                ? new Dictionary<string, int>(_stringKeyFields)
                : _intKeyFields.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>ColumnChecker.</returns>
        public ColumnChecker Copy()
        {
            return new ColumnChecker(_intKeyFields, _stringKeyFields, _isStringBased);
        }
    }
}