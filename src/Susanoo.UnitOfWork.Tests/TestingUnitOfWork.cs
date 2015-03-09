using System;

namespace Susanoo.UnitOfWork.Tests
{
    public class TestingUnitOfWork
        : UnitOfWork<int?>
    {
        public TestingUnitOfWork(int? info = null, UnitOfWorkDbManagerMode managerMode = UnitOfWorkDbManagerMode.PerTransaction) 
            : base(info, managerMode)
        {
            _branch = new Lazy<BranchRepository>(() => new BranchRepository(this));
        }

        private readonly Lazy<BranchRepository> _branch;

        public BranchRepository Branch 
        {
            get { return _branch.Value; }
        }

        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        protected override string ConnectionStringName {
            get { return "Susanoo"; }
        }
    }
}
