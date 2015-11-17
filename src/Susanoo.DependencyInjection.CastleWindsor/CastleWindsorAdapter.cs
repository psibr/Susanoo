using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Susanoo.Exceptions;

namespace Susanoo.DependencyInjection.CastleWindsor
{
    public class CastleWindsorAdapter
        : IComponentContainer
    {
        private readonly IWindsorContainer _windsorContainer;

        public CastleWindsorAdapter(IWindsorContainer windsorContainer)
        {
            _windsorContainer = windsorContainer;
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        /// <exception cref="SusanooDependencyResolutionException">An error occurred resolving a type.</exception>
        public T Resolve<T>(string name = null)
            where T : class
        {
            try
            {
                return name == null
                    ? _windsorContainer.Resolve<T>()
                    : _windsorContainer.Resolve<T>(name);
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
            var binding = Component.For<T>().Instance(instance).LifeStyle.Transient;

            if (name != null)
                binding.Named(name);

            if (!_windsorContainer.Kernel.HasComponent(typeof(T)))
                binding.IsFallback();
            else
                binding.NamedAutomatically(Guid.NewGuid().ToString());

            _windsorContainer.Register(binding);
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
            var binding = Component.For<T>().UsingFactoryMethod(() => resolver(this)).LifeStyle.Transient;

            if (name != null)
                binding.Named(name);

            if (!_windsorContainer.Kernel.HasComponent(typeof(T)))
                binding.IsFallback();
            else
                binding.NamedAutomatically(Guid.NewGuid().ToString());


            _windsorContainer.Register(binding);
        }
    }
}
