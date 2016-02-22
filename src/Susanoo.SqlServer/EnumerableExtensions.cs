using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo.SqlServer
{
#if FULLFX
    internal static class EnumerableExtensions
    {
        private static readonly Dictionary<string, DelegateInfo> CompiledFuncs =
            new Dictionary<string, DelegateInfo>();

        private static readonly MethodInfo SetValueMethodInfo = typeof(SqlDataRecord).GetMethod("SetValue");
        private static readonly ConstructorInfo SqlDataRecordConstructorInfo =
            typeof(SqlDataRecord).GetConstructor(new[] { typeof(SqlMetaData[]) });

        /// <summary>
        /// Converts an IEnumerable to appropriate SqlDataRecords for TVP.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>IEnumerable&lt;SqlDataRecord&gt;.</returns>
        public static IEnumerable<SqlDataRecord> ToDataRecords(this IEnumerable items)
        {
            if (items == null)
                return null;

            var enumerable = (items as IEnumerable<object> ?? items.Cast<object>());
            return !enumerable.Any() ? null : ToDataRecordsInternal(enumerable);
        }

        private static IEnumerable<SqlDataRecord> ToDataRecordsInternal(IEnumerable<object> items)
        {
            var enumerator = items.GetEnumerator();

            enumerator.MoveNext();

            var itemType = enumerator.Current.GetType();

            if (!CompiledFuncs.ContainsKey(itemType.AssemblyQualifiedName))
            {
                AddCompilationInfo(itemType);
            }

            var info = CompiledFuncs[itemType.AssemblyQualifiedName];

            var buildSqlDataRecord = info.Func;

            return items
                .Select(item => buildSqlDataRecord(info.MetaData, item));
        }

        private static void AddCompilationInfo(Type itemType)
        {
            var members = itemType.GetMembers();

            var props =
                members.Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                    .ToArray();

            var coercionParameter = new SqlParameter();

            var metaData = new SqlMetaData[props.Length];

            for (int index = 0; index < props.Length; index++)
            {
                var prop = props[index];

                if (index > 0)
                {
                    coercionParameter.ResetDbType();
                    coercionParameter.ResetSqlDbType();
                }

                switch (props[index].MemberType)
                {
                    case MemberTypes.Property:
                        coercionParameter.DbType = CommandManager.GetDbType(((PropertyInfo)prop).PropertyType) ??
                                                   DbType.Object;
                        break;
                    case MemberTypes.Field:
                        coercionParameter.DbType = CommandManager.GetDbType(((FieldInfo)prop).FieldType) ??
                                                   DbType.Object;
                        break;
                }

                if(coercionParameter.SqlDbType == SqlDbType.NVarChar)
                    metaData[index] = new SqlMetaData(prop.Name, coercionParameter.SqlDbType, -1);
                else
                    metaData[index] = new SqlMetaData(prop.Name, coercionParameter.SqlDbType);
            }

            var bodyExpressions = new List<Expression>();

            var metaDataParameter = Expression.Parameter(typeof(SqlMetaData[]), "metaData");
            var itemParameter = Expression.Parameter(typeof(object), "item");
            var castedItemParameter = Expression.Variable(itemType, "castedItem");
            var recordVariable = Expression.Variable(typeof(SqlDataRecord), "record");

            bodyExpressions.Add(Expression.Assign(recordVariable,
                Expression.New(SqlDataRecordConstructorInfo, metaDataParameter)));

            bodyExpressions.Add(Expression.Assign(castedItemParameter,
                Expression.Convert(itemParameter, itemType)));

            for (int index = 0; index < props.Length; index++)
            {
                var mi = props[index];
                switch (mi.MemberType)
                {
                    case MemberTypes.Property:
                        bodyExpressions.Add(Expression.Call(recordVariable, SetValueMethodInfo,
                            new Expression[]
                                {
                                    Expression.Constant(index, typeof (int)),
                                    Expression.Convert(Expression.Property(castedItemParameter, itemType, mi.Name),
                                        typeof (object))
                                }));

                        break;
                    case MemberTypes.Field:
                        bodyExpressions.Add(Expression.Call(recordVariable, SetValueMethodInfo,
                            new Expression[]
                                {
                                    Expression.Constant(index, typeof (int)),
                                    Expression.Convert(Expression.Field(castedItemParameter, itemType, mi.Name),
                                        typeof (object))
                                }));
                        break;
                }
            }

            bodyExpressions.Add(recordVariable);

            var body = Expression.Block(new[] { recordVariable, castedItemParameter }, bodyExpressions);
            var lambda = Expression.Lambda<Func<SqlMetaData[], object, SqlDataRecord>>(body, metaDataParameter,
                itemParameter);

            CompiledFuncs.Add(itemType.AssemblyQualifiedName, new DelegateInfo { Func = lambda.Compile(), MetaData = metaData });
        }

        private class DelegateInfo
        {
            public SqlMetaData[] MetaData { get; set; }
            public Func<SqlMetaData[], object, SqlDataRecord> Func { get; set; }
        }
    }

#endif
}
