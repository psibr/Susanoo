#region

using System;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Mapping;
using Susanoo.Processing;

#endregion

namespace Susanoo.ResultSets
{
    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandResultExpression<TFilter, TResult> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultExpression(ICommandBuilderInfo<TFilter> command,
            ICommandResultImplementor<TFilter> implementor)
            : base(command, implementor)
        {
        }



        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash =>
            (base.CacheHash * 31)
                ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                ^ GetType().AssemblyQualifiedName.GetHashCode();

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> ForResults(
            Action<IResultMappingExpression<TFilter, TResult>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> Realize(string name = null)
        {
            var processor = BuildOrRegenCommandProcessor(this, name);

            return processor;
        }

        /// <summary>
        ///     Builds the or regenerates a CommandBuilder processor from cache.
        /// </summary>
        /// <param name="commandResultInfo">The CommandBuilder result information.</param>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public static ICommandProcessor<TFilter, TResult> BuildOrRegenCommandProcessor(
            ICommandResultInfo<TFilter> commandResultInfo, string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor<TFilter, TResult> result = null;

            if (name == null)
            {
                var hash = (commandResultInfo.CacheHash * 31) ^
                           GetTypeArgumentHashCode(typeof(ICommandProcessor<TFilter, TResult>));

                if (CommandManager.Instance.TryGetCommandProcessor(hash, out instance))
                    result = (ICommandProcessor<TFilter, TResult>)instance;
            }
            else
            {
                if (CommandManager.Instance.TryGetCommandProcessor(name, out instance))
                    result = (ICommandProcessor<TFilter, TResult>)instance;
            }

            return result ??
                    CommandManager.Instance.Bootstrapper
                        .ResolveDependency<ISingleResultSetCommandProcessorFactory>()
                        .BuildCommandProcessor<TFilter, TResult>(commandResultInfo, name);
        }

        /// <summary>
        ///     To a single result.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public override ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
        {
            return new CommandResultExpression<TFilter, TSingle>(Command);
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor<TFilter, TResult1, TResult2> result;

            if (CommandManager.Instance.TryGetCommandProcessor(CacheHash, out instance))
                result = (ICommandProcessor<TFilter, TResult1, TResult2>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2>(this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3}" />
        ///     class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3> result;

            if (CommandManager.Instance.TryGetCommandProcessor(CacheHash, out instance))
                result = (ICommandProcessor<TFilter, TResult1, TResult2, TResult3>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>(this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> result;

            if (CommandManager.Instance.TryGetCommandProcessor(CacheHash, out instance))
                result = (ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>(this,
                    name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultsOfType
            <TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> result;

            if (CommandManager.Instance.TryGetCommandProcessor(CacheHash, out instance))
                result =
                    (ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>)
                        instance;
            else
                result =
                    new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(
                        this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Realize(
            string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
                result;

            if (CommandManager.Instance.TryGetCommandProcessor(CacheHash, out instance))
                result =
                    (
                        ICommandProcessor
                            <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>)instance;
            else
                result =
                    new MultipleResultSetCommandProcessor
                        <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    /// <typeparam name="TResult7">The type of the 7th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}" />
        ///     class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Realize(
            string name = null)
        {
            ICommandProcessorWithResults instance;
            ICommandProcessor
                <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> result;

            if (CommandManager.Instance.TryGetCommandProcessor(CacheHash, out instance))
                result =
                    (
                        ICommandProcessor
                            <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>)instance;
            else
                result =
                    new MultipleResultSetCommandProcessor
                        <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this, name);

            return result;
        }
    }
}