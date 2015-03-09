using System;
using System.Collections.Generic;
using System.Data;

namespace Susanoo.UnitOfWork.Tests
{
    public class BranchRepository
        : Repository<int?>
    {
        public BranchRepository(IRepository<int?> parentRepository)
            : base(parentRepository)
        {
            _leaf = new Lazy<LeafRepository>(() => new LeafRepository(this));
        }

        private readonly Lazy<LeafRepository> _leaf;

        public LeafRepository Leaf
        {
            get { return _leaf.Value; }
        }

        protected static ICommandProcessor<dynamic, KeyValuePair<int, string>> KeyValuePairFromValuesCommand =
            DefineCommand("SELECT Int, String FROM ( VALUES (1, 'varchar')) AS DataTable(Int, String);",
                CommandType.Text)
                .DefineResults<KeyValuePair<int, string>>()
                .ForResults(expression =>
                {
                    expression.ForProperty(pair => pair.Key, configuration => configuration.UseAlias("Int"));
                    expression.ForProperty(pair => pair.Value, configuration => configuration.UseAlias("String"));
                })
                .Realize();

        public IEnumerable<KeyValuePair<int, string>> KeyValuePairFromValues()
        {
            return KeyValuePairFromValuesCommand
                .Execute(DatabaseManager);
        }
    }
}
