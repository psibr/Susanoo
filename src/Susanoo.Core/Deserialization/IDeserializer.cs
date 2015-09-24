using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Susanoo.Processing;

namespace Susanoo.Deserialization
{
    public interface IDeserializer
    {
        Type DeserializationType { get; }

        IEnumerable Deserialize(IDataReader reader, ColumnChecker columnReport);
    }

    public interface IDeserializer<out TResult>
    {
        Type DeserializationType { get; }

        IEnumerable<TResult> Deserialize(IDataReader reader, ColumnChecker columnReport);
    }
}