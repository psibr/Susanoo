using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Susanoo.Processing;

namespace Susanoo.Deserialization
{
    /// <summary>
    /// Represents values from an IDataRecord object.
    /// </summary>
    public class DynamicRow : IDynamicMetaObjectProvider
    {
        private readonly object[] _values;
        private readonly ColumnChecker _columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicRow"/> class.
        /// </summary>
        /// <param name="columns">The columns Susanoo has discovered and will map to properties.</param>
        public DynamicRow(ColumnChecker columns)
        {
            _columns = columns ?? new ColumnChecker();
            _values = new object[_columns.Count];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicRow"/> class.
        /// </summary>
        public DynamicRow()
        {
            _columns = new ColumnChecker();
            _values = new object[_columns.Count];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicRow" /> class.
        /// </summary>
        /// <param name="columns">The columns Susanoo has discovered and will map to properties.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Thrown if column count and value count do not match.
        /// </exception>
        public DynamicRow(ColumnChecker columns, object[] values)
        {
            if (columns.Count != values.Length)
                throw new IndexOutOfRangeException();

            _columns = columns;
            _values = values;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" />
        ///  responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicRowMeta(parameter, BindingRestrictions.Empty, this, _columns);
        }

        /// <summary>
        /// Gets the value at a specified index.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>System.Object.</returns>
        public object GetValue(int ordinal)
        {
            return _values[ordinal];
        }

        /// <summary>
        /// Gets the value for a column name.
        /// </summary>
        public object GetValue(string columnName)
        {
            int ordinal;
            if (!_columns.TryGetValue(columnName, out ordinal))
                throw new KeyNotFoundException(columnName + " is not available.");

            return _values[ordinal];
        }

        /// <summary>
        /// Sets a value at a specified index.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        public void SetValue(int ordinal, object value)
        {
            _values[ordinal] = value;
        }

        /// <summary>
        /// Sets a value to a column.
        /// </summary>
        /// <param name="columnName">The name of the column in SQL.</param>
        /// <param name="value">The value of the column in this record.</param>
        public void SetValue(string columnName, object value)
        {
            int ordinal;
            if (!_columns.TryGetValue(columnName, out ordinal))
                throw new KeyNotFoundException(columnName + " is not available.");

            _values[ordinal] = value;
        }

        /// <summary>
        /// Gets the length of the values array.
        /// </summary>
        /// <value>The length.</value>
        public int Length => 
            _values.Length;

        /// <summary>
        /// Returns the array of values in the row.
        /// </summary>
        /// <returns>System.Object[].</returns>
        public object[] ToArray()
        {
            return _values;
        }

        /// <summary>
        /// Converts to a dictionary (Only named columns)
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            var items = new List<KeyValuePair<string, object>>();

            for (var i = 0; i < _values.Length; i++)
            {
                string columnName;
                if (_columns.TryGetValue(i, out columnName))
                    items.Add(new KeyValuePair<string, object>(columnName, _values[i]));
            }

            return items.ToDictionary(k => k.Key, v => v.Value);


        }
    }

    /// <summary>
    /// Overrides for Dynamic that uses common methods for properties rather than reflection binding for each property.
    /// </summary>
    public class DynamicRowMeta : DynamicMetaObject
    {
        private readonly ColumnChecker _columns;
        static readonly MethodInfo GetValueMethod = typeof(DynamicRow).GetMethod("GetValue", new[] { typeof(string) });
        static readonly MethodInfo SetValueMethod = typeof(DynamicRow).GetMethod("SetValue", new[] { typeof(string), typeof(object) });

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Dynamic.DynamicMetaObject" /> class.
        /// </summary>
        /// <param name="expression">
        /// The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.
        /// </param>
        /// <param name="restrictions">The set of binding restrictions under which the binding is valid.</param>
        /// <param name="columns">The columns.</param>
        public DynamicRowMeta(Expression expression, BindingRestrictions restrictions, ColumnChecker columns)
            : base(expression, restrictions)
        {
            _columns = columns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Dynamic.DynamicMetaObject" /> class.
        /// </summary>
        /// <param name="expression">
        /// The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.
        /// </param>
        /// <param name="restrictions">The set of binding restrictions under which the binding is valid.</param>
        /// <param name="value">The runtime value represented by the <see cref="T:System.Dynamic.DynamicMetaObject" />.</param>
        /// <param name="columns">The columns.</param>
        public DynamicRowMeta(Expression expression, BindingRestrictions restrictions, object value, ColumnChecker columns)
            : base(expression, restrictions, value)
        {
            _columns = columns;
        }

        DynamicMetaObject CallMethod(MethodInfo method, Expression[] parameters)
        {
            return new DynamicMetaObject(
                Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    method,
                    parameters),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));


        }

        /// <summary>
        /// Performs the binding of the dynamic invoke member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="T:System.Dynamic.InvokeMemberBinder" /> that represents the details of the dynamic operation.
        /// </param>
        /// <param name="args">
        /// An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - arguments to the invoke member operation.
        /// </param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            Expression[] parameters = { Expression.Constant(binder.Name) };

            var callMethod = CallMethod(GetValueMethod, parameters);

            return callMethod;
        }

        /// <summary>
        /// Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="T:System.Dynamic.GetMemberBinder" /> that represents the details of the dynamic operation.
        /// </param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            Expression[] parameters = { Expression.Constant(binder.Name) };

            var callMethod = CallMethod(GetValueMethod, parameters);

            return callMethod;
        }

        /// <summary>
        /// Performs the binding of the dynamic set member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="T:System.Dynamic.SetMemberBinder" /> that represents the details of the dynamic operation.
        /// </param>
        /// <param name="value">
        /// The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the value for the set member operation.
        /// </param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            Expression[] parameters = 
                {
                    Expression.Constant(binder.Name),
                    value.Expression
                };

            var callMethod = CallMethod(SetValueMethod, parameters);

            return callMethod;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>The list of dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _columns.ExportReport()
                .Select(i => i.Key);
        }

        /// <summary>
        /// Performs the binding of the dynamic get index operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="T:System.Dynamic.GetIndexBinder" /> that represents the details of the dynamic operation.
        /// </param>
        /// <param name="indexes">
        /// An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - indexes for the get index operation.
        /// </param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            Expression[] parameters = { Expression.Constant(indexes.First(ix => ix.LimitType == typeof(string)).Value) };

            var callMethod = CallMethod(GetValueMethod, parameters);

            return callMethod;
        }

        /// <summary>
        /// Performs the binding of the dynamic set index operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the <see cref="T:System.Dynamic.SetIndexBinder" /> that represents the details of the dynamic operation.
        /// </param>
        /// <param name="indexes">
        /// An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - indexes for the set index operation.
        /// </param>
        /// <param name="value">The <see cref="T:System.Dynamic.DynamicMetaObject" /> 
        /// representing the value for the set index operation.
        /// </param>
        /// <returns>
        /// The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.
        /// </returns>
        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            Expression[] parameters = { Expression.Constant(indexes.First().Value) };

            var callMethod = CallMethod(SetValueMethod, parameters);

            return callMethod;
        }
    }

}