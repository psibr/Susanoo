using System.Data;
using Susanoo.Pipeline.Command;

namespace Susanoo.UnitOfWork
{

    /// <summary>
    /// Repository pattern object with a link back to the UnitOfWork that created the instance.
    /// </summary>
    /// <typeparam name="TInfo">The type of the t information.</typeparam>
    public abstract class Repository<TInfo> : IRepository<TInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TInfo}" /> class.
        /// </summary>
        /// <param name="parentRepository">The parent repository.</param>
        protected Repository(IRepository<TInfo> parentRepository)
        {
            ParentRepository = parentRepository;
            Info = parentRepository.Info;
        }

        public DatabaseManager DatabaseManager
        {
            get { return ParentRepository.DatabaseManager; }
        }

        /// <summary>
        /// Gets the unit of work specific information.
        /// </summary>
        /// <value>The information.</value>
        public TInfo Info { get; private set; }


        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <value>The unit of work.</value>
        public UnitOfWork<TInfo> UnitOfWork { get { return ParentRepository.UnitOfWork; } }

        /// <summary>
        /// Gets or sets the parent repository.
        /// </summary>
        /// <value>
        /// The parent repository.
        /// </value>
        protected IRepository<TInfo> ParentRepository { get; set; }

        public static ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType)
        {
            return CommandManager.DefineCommand(commandText, commandType);
        }

        public static ICommandExpression<TFilter> DefineCommand<TFilter>(string commandText, CommandType commandType)
        {
            return CommandManager.DefineCommand<TFilter>(commandText, commandType);
        }
    }
}