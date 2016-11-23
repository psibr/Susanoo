using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo.Tests.Static
{
    public class User
    {
        public StringValueType UserName { get; set; }
        public DateValueType DoB { get; set; }
    }

    public class StringValueType
    {
        private readonly string _value;
        private StringValueType(string value)
        {
            _value = value;
        }
        public StringValueType() {}
        public override string ToString() => (_value);
        public static implicit operator StringValueType(string value) => (new StringValueType(value));
        public static implicit operator string(StringValueType value) => (value._value);
        public static StringValueType TryParse(string value) => (new StringValueType(value));
    }
    public class DateValueType
    {
        private readonly DateTime _value;
        private DateValueType(DateTime value)
        {
            _value = value;
        }
        public DateValueType() { }
        public override string ToString() => (_value.ToString());
        public static implicit operator DateValueType(DateTime value) => (new DateValueType(value));
        public static implicit operator DateTime(DateValueType value) => (value._value);
        public static DateValueType TryParse(DateTime value) => (new DateValueType(value));
    }
}
