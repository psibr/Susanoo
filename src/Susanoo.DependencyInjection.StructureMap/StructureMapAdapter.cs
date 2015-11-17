using System;
using StructureMap;
using Susanoo.Exceptions;

namespace Susanoo.DependencyInjection.StructureMap
{
    public class StructureMapAdapter
        : IComponentContainer
    {
        private readonly IContainer _container;

        public StructureMapAdapter(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        /// <exception cref="SusanooDependencyResolutionException">An error occurred resolving a type.</exception>
        public T Resolve<T>(string name = null) where T : class
        {
            try
            {
                return name == null
                    ? _container.GetInstance<T>()
                    : _container.GetInstance<T>(name);
            }
            catch (Exception ex)
            {
                throw new SusanooDependencyResolutionException("An error occurred resolving a type.",
                    ex);
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
            _container.Configure(_ =>
            {
                if (name == null)
                    _.For<T>().Use(instance);
                else
                    _.For<T>().Use(instance).Named(name);
            });
        }

        /// <summary>
        /// Registers the specified resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="name">The name.</param>
        public void Register<T>(Func<IComponentContainer, T> resolver, string name = null)
            where T : class
        {
            _container.Configure(_ =>
            {
                if (name == null)
                    _.For<T>().Use(() => resolver(this));
                else
                    _.For<T>().Use(() => resolver(this));
            });
        }
    }
}
