using System;
using Susanoo.UnitOfWork;

namespace Susanoo.Example.Web.Data
{
    public class UnitOfWork : UnitOfWork<int?>
    {
        public UnitOfWork(int? userId = null)
            : base(userId, UnitOfWorkDbManagerMode.PerTransaction)
        {
            
        }

        protected override string ConnectionStringName
        {
            get { throw new NotImplementedException(); }
        }
    }
}