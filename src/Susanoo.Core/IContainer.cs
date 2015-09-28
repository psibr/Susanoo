using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// A simple container interface for plugging in DI containers.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        T Resolve<T>(string name = null)
            where T : class;

        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        void Register<T>(T instance, string name = null)
            where T : class;

        /// <summary>
        /// Registers the specified resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="name">The name.</param>
        void Register<T>(Func<IContainer, T> resolver, string name = null)
            where T : class;
    }
}
