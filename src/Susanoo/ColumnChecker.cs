using System.Collections.Generic;
using System.Data;

namespace Susanoo
{
    /// <summary>
    /// Tracks available fields in return results to allow for efficient column existence checks.
    /// </summary>
    public class ColumnChecker
    {
        private bool isInit = false;
        private Dictionary<string, int> fields = new Dictionary<string, int>();

        /// <summary>
        /// Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public int HasColumn(IDataRecord record, string name)
        {
            if (!isInit)
            {
                for (int i = 0; i < record.FieldCount; i++)
                    fields.Add(record.GetName(i), i);

                isInit = true;
            }

            return fields.ContainsKey(name) ? fields[name] : -1;
        }
    }
}