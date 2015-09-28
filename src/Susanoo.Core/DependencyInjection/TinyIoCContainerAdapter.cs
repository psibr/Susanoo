using System;
using Susanoo.DependencyInjection.TinyIoC;
using Susanoo.Exceptions;

namespace Susanoo.DependencyInjection
{
    /// <summary>
    /// An adpater for TinyIoC to the shared IContainer interface.
    /// </summary>
    internal class TinyIoCContainerAdapter 
        : IContainer
    {
        private readonly TinyIoCContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyIoCContainerAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        internal TinyIoCContainerAdapter(TinyIoCContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        /// <exception cref="SusanooDependencyResolutionException">An error occured resolving a type.</exception>
        public T Resolve<T>(string name = null)
            where T : class
        {
            try
            {
                return name == null 
                    ? _container.Resolve<T>() 
                    : _container.Resolve<T>(name);
            }
            catch (TinyIoCResolutionException tinyIoCResolutionException)
            {
                throw new SusanooDependencyResolutionException("An error occured resolving a type.",
                    tinyIoCResolutionException);
            }
        }

        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void Register<T>(T instance, string name = null)
            where T : class
        {
            if (name == null)
                _container.Register(instance);
            else
                _container.Register(instance, name);
        }

        /// <summary>
        /// Registers the specified resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="name">The name.</param>
        public void Register<T>(Func<IContainer, T> resolver, string name = null)
            where T : class 
        {
            if (name == null)
                _container.Register<T>((tinyIoC, overloads) => resolver(this));
            else
                _container.Register<T>((tinyIoC, overloads) => resolver(this), name);
        }
    }
}
