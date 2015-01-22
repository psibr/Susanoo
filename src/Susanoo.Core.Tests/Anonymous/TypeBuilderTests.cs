using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Susanoo.Tests.Anonymous
{
    [TestFixture]
    public class TypeBuilderTests
    {
        [Test]
        public void ConstructionTest()
        {
            var fields = new Dictionary<string, Type>
            {
                { "one", typeof(int) },
                { "two", typeof(int) }
            };

            Type X = AnonymousTypeBuilder.BuildType(fields);

            Assert.IsNotNull(X);
        }

    }
}
