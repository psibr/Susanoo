using System;

namespace Susanoo.Tests.Static
{
    public class TypeTestModel
    {
        public bool Bit { get; set; }

        public byte TinyInt { get; set; }

        public Int16 SmallInt { get; set; }

        public int Int { get; set; }

        public Int64 BigInt { get; set; }

        public decimal SmallMoney { get; set; }

        public decimal Money { get; set; }

        public decimal Numeric { get; set; }

        public decimal Decimal { get; set; }

        public string Character { get; set; }

        public string String { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public DateTime SmallDateTime { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime DateTime2 { get; set; }

        public TimeSpan Time { get; set; }

        public Guid Guid { get; set; }

        public string UnusedProperty { get; set; }
    }
}
