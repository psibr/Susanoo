using System;
using Ninject;
using Susanoo.Exceptions;

namespace Susanoo.DependencyInjection.Ninject
{
    public class NinjectAdapter
        : IComponentContainer
    {
        private readonly IKernel _kernel;

        public NinjectAdapter(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        /// <exception cref="SusanooDependencyResolutionException">An error occured resolving a type.</exception>
        public T Resolve<T>(string name = null) where T : class
        {
            try
            {
                return name != null
                    ? _kernel.Get<T>(name)
                    : _kernel.Get<T>();
            }
            catch (Exception ex)
            {
                throw new SusanooDependencyResolutionException("An error occured resolving a type.",
                    ex);
            }
        }

        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        public void Register<T>(T instance, string name = null) where T : class
        {
            var binding = _kernel.Rebind<T>().ToConstant(instance);

            if (name != null)
                binding.Named(name);
        }

        /// <summary>
        /// Registers the specified resolver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="name">The name.</param>
        public void Register<T>(Func<IComponentContainer, T> resolver, string name = null) where T : class
        {
            var binding = _kernel.Rebind<T>().ToMethod(context => resolver(this));

            if (name != null)
                binding.Named(name);
        }
    }
}
