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
    public interface IResultSetReader 
        : IEnumerable<IEnumerable<object>>
    {
    }
}
