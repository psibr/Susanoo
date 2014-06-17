using System;

namespace Susanoo
{
    /// <summary>
    /// Describes the casting functionality used to read values from an IDataRecord.
    /// </summary>
    public interface IDataRecordValueProcessor
    {
        /// <summary>
        /// Processes a value returned from a IDataRecord.
        /// </summary>
        /// <param name="newType">The new type.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Object.</returns>
        object ProcessValue(Type newType, object value, object defaultValue);
    }
}