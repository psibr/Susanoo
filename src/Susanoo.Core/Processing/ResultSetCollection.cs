using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Susanoo.Processing
{
    /// <summary>
    /// Provides an enumeration and clear structure for retrieving multiple results.
    /// </summary>
    public class ResultSetCollection
        : IResultSetCollection
    {
        private readonly IDataReader _reader;
        private readonly IEnumerable<IEnumerable<object>> _resultSets;

        internal ResultSetCollection(IEnumerable<IEnumerable> resultSets, IDataReader reader)
        {
            _reader = reader;
            _resultSets = resultSets.Select(r => r?.Cast<object>());
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IEnumerable<object>> GetEnumerator()
        {
            return new ResultSetEnumerator(_resultSets.GetEnumerator(), _reader);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Manages enumeration of multiple result sets.
    /// </summary>
    public class ResultSetEnumerator
            : IEnumerator<IEnumerable<object>>, IEnumerator
    {
        private readonly IEnumerator<IEnumerable<object>> _baseEnumerator;
        private readonly IDataReader _reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSetEnumerator" /> class.
        /// </summary>
        /// <param name="baseEnumerator">The base enumerator.</param>
        /// <param name="reader">The reader.</param>
        public ResultSetEnumerator(IEnumerator<IEnumerable<object>> baseEnumerator, IDataReader reader)
        {
            _baseEnumerator = baseEnumerator;
            _reader = reader;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public bool MoveNext()
        {
            var moved = true;
            if (_reader != null)
                moved = _reader.NextResult();

            if (moved)
                moved = _baseEnumerator.MoveNext();

            return moved;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public void Reset()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerable<object> Current => _baseEnumerator.Current;

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        object IEnumerator.Current => _baseEnumerator.Current;
    }
}
