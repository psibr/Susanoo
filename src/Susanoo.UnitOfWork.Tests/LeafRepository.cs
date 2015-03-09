namespace Susanoo.UnitOfWork.Tests
{
    public class LeafRepository
        : Repository<int?>
    {
        public LeafRepository(IRepository<int?> parentRepository)
            : base(parentRepository)
        {
        }
    }
}
