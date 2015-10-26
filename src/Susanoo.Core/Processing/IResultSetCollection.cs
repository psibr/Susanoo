using System.Collections.Generic;

namespace Susanoo.Processing
{
    /// <summary>
    /// Provides an enumeration and clear structure for retrieving multiple results.
    /// </summary>
    public interface IResultSetCollection
        : IEnumerable<IEnumerable<object>>
    {
    }
}
