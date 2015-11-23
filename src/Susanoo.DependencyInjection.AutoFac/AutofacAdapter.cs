using System;
using Autofac;
using Susanoo.Exceptions;

namespace Susanoo.DependencyInjection.Autofac
{
    public class AutofacAdapter
        : IComponentContainer
    {
        private readonly IContainer _container;

        public AutofacAdapter(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        /// <exception cref="SusanooDependencyResolutionException">
        /// An error occurred resolving a type.
        /// </exception>
        public T Resolve<T>(string name = null) where T : class
        {
            try
            {
                return name == null
                    ? _container.Resolve<T>()
                    : _container.ResolveNamed<T>(name);
            }
            catch (Exception ex)
            {
                throw new SusanooDependencyResolutionException(
                    "An error occurred resolving a type.",
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
            var builder = new ContainerBuilder();

            var registration = builder
                .RegisterInstance(instance);

            if (name == null)
                registration.As<T>();
            else
                registration.Named<T>(name);


            builder.Update(_container);
        }

        /// <summary>
        /// Registers the specified resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="name">The name.</param>
        public void Register<T>(Func<IComponentContainer, T> resolver,
            string name = null) where T : class
        {
            var builder = new ContainerBuilder();

            var registration = builder
                .Register(context => resolver(this));

            if (name == null)
                registration.As<T>();
            else
                registration.Named<T>(name);


            builder.Update(_container);
        }
    }
}
