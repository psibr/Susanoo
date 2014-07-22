using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;

namespace Susanoo
{
    /// <summary>
    /// A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultMappingExpression<TFilter, TResult>
        : IResultMappingExpression<TFilter, TResult>
        where TResult : new()
    {
        private readonly IResultMappingImplementor<TFilter, TResult> _Implementor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultMappingExpression{TFilter, TResult}"/> class.
        /// </summary>
        public ResultMappingExpression()
        {
            this._Implementor = new ResultMappingImplementor<TFilter, TResult>();
        }

        public virtual BigInteger CacheHash
        {
            get
            {
                List<BigInteger> hashCombinations = new List<BigInteger>();

                StringBuilder hashText = new StringBuilder(typeof(TResult).FullName);
                foreach (KeyValuePair<string, IPropertyMapping> item in this.Export())
                {
                    hashText.Append(item.Key);
                    hashCombinations.Add(item.Value.CacheHash);
                }

                BigInteger initialHash = FnvHash.GetHash(hashText.ToString(), 64);

                foreach (BigInteger hash in hashCombinations)
                {
                    initialHash = (initialHash * 31) ^ hash;
                }

                return initialHash;
            }
        }

        /// <summary>
        /// Gets the implementor this is the Bridge design pattern.
        /// </summary>
        /// <value>The implementor.</value>
        protected IResultMappingImplementor<TFilter, TResult> Implementor
        {
            get { return this._Implementor; }
        }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> ClearMappings()
        {
            this.Implementor.ClearMappings();

            return this;
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual IResultMappingExpression<TFilter, TResult> ForProperty(
            Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options)
        {
            return ForProperty(propertyExpression.GetPropertyName(), options);
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration> options)
        {
            this.Implementor.ForProperty(propertyName, options);

            return this;
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IDictionary<string, IPropertyMapping> Export()
        {
            return this.Implementor.Export();
        }
    }
}