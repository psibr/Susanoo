using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo.Processing
{
    /// <summary>
    /// Provides an enumeration and clear structure for retrieving multiple results.
    /// </summary>
    public class ResultSetReader
        : IResultSetReader
    {
        private readonly IEnumerable<IEnumerable<object>> _resultSets;

        internal ResultSetReader(IEnumerable<IEnumerable> resultSets)
        {
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
            return _resultSets.GetEnumerator();
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
}
