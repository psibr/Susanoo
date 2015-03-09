namespace Susanoo.UnitOfWork
{

    /// <summary>
    /// Repository pattern with a link back to the UnitOfWork that created the instance.
    /// </summary>
    /// <typeparam name="TInfo">The type of the shared information.</typeparam>
    public interface IRepository<TInfo>
    {
        /// <summary>
        /// Gets the context specific information.
        /// </summary>
        /// <value>The information.</value>
        TInfo Info { get; }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        DatabaseManager DatabaseManager { get; }

        /// <summary>
        /// Gets the unit of work to which the repository instance belongs.
        /// </summary>
        UnitOfWork<TInfo> UnitOfWork { get; }}
}