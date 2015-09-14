using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// All possible compare methods for dynamic where clauses.
    /// </summary>
    public enum CompareMethod
    {
        /// <summary>
        /// Remove the property from the comparison.
        /// </summary>
        Ignore = 0,
        /// <summary>
        /// Provide a hand-coded comparison.
        /// </summary>
        Override = 1,
        /// <summary>
        /// Values must equal.
        /// </summary>
        Equal = 2,
        /// <summary>
        /// Column value must be less than parameter value.
        /// </summary>
        LessThan = 3,
        /// <summary>
        /// Column value must be less than or equal parameter value.
        /// </summary>
        LessThanOrEqual = 4,
        /// <summary>
        /// Column value must be greater than parameter value.
        /// </summary>
        GreaterThan = 5,
        /// <summary>
        /// Column value must be greater than or equal parameter value.
        /// </summary>
        GreaterThanOrEqual = 6,
        /// <summary>
        /// Values must NOT equal.
        /// </summary>
        NotEqual = 7,
        /// <summary>
        /// Column value must start with parameter value.
        /// </summary>
        StartsWith = 8,
        /// <summary>
        /// Column value must end with parameter value.
        /// </summary>
        EndsWith = 9,
        /// <summary>
        /// Column value must contain parameter value.
        /// </summary>
        Contains = 10
    }
}
