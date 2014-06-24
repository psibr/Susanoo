using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public class ColumnChecker
    {
        private List<string> fields = new List<string>();
        private int lastIndex = 0;

        /// <summary>
        /// Determines whether the specified record has a column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified record has column; otherwise, <c>false</c>.</returns>
        public bool HasColumn(IDataRecord record, string name)
        {
            bool map = false;

            if (fields.Contains(name))
            {
                map = true;
            }
            else
            {
                for (int i = lastIndex; i < record.FieldCount; i++)
                {
                    lastIndex = i;
                    var fieldName = record.GetName(i);
                    fields.Add(fieldName);
                    if (fieldName == name)
                    {
                        map = true;
                        lastIndex++;
                        break;
                    }
                }
            }

            return map;
        }
    }
}
