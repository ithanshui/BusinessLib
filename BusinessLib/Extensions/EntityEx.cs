using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace BusinessLib.Extensions
{
    unsafe public static class EntityEx
    {
        //把DataRow转换为对象的委托声明
        private delegate T Load<T>(DataRow dataRecord);

        //用于构造Emit的DataRow中获取字段的方法信息
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });

        //用于构造Emit的DataRow中判断是否为空行的方法信息
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });

        //使用字典存储实体的类型以及与之对应的Emit生成的转换方法
        private static Dictionary<Type, Delegate> rowMapMethods = new Dictionary<Type, Delegate>();

        unsafe public static List<T> ToList<T>(this DataTable dt)
        {
            var list = new List<T>();
            if (dt == null) { return list; }
            //声明 委托Load<T>的一个实例rowMap
            Load<T> rowMap = null;

            //从rowMapMethods查找当前T类对应的转换方法，没有则使用Emit构造一个。
            if (!rowMapMethods.ContainsKey(typeof(T)))
            {
                var method = new DynamicMethod("DynamicCreateEntity_" + typeof(T).Name, typeof(T), new Type[] { typeof(DataRow) }, typeof(T), true);
                var generator = method.GetILGenerator();
                var result = generator.DeclareLocal(typeof(T));
                generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);

                for (int index = 0; index < dt.Columns.Count; index++)
                {
                    var propertyInfo = typeof(T).GetProperty(dt.Columns[index].ColumnName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                    var endIfLabel = generator.DefineLabel();
                    if (null != propertyInfo && null != propertyInfo.GetSetMethod())
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, index);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, index);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);

                //构造完成以后传给rowMap
                rowMap = (Load<T>)method.CreateDelegate(typeof(Load<T>));
            }
            else
            {
                rowMap = (Load<T>)rowMapMethods[typeof(T)];
            }

            //遍历Datatable的rows集合，调用rowMap把DataRow转换为对象（T）
            foreach (DataRow info in dt.Rows) { list.Add(rowMap(info)); }
            return list;
        }

        unsafe public static Common.Data.ADO.Component.DataTable ToDataTable<T>(this IEnumerable<T> entity, System.Data.Common.DbConnection connection = null, System.Data.Common.DbTransaction t = null)
        {
            if (null == entity || 0 == entity.Count()) { return null; }

            var properties = typeof(T).GetProperties();

            var result = null == connection ? new Common.Data.ADO.Component.DataTable(typeof(T).Name) : Common.Data.ADO.SqlHelp.ExecuteTable(connection, string.Format("SELECT * FROM {0}", typeof(T).Name), CommandBehavior.SchemaOnly, t);
            if (null == connection) { foreach (var prop in properties) { result.Columns.Add(prop.Name, prop.PropertyType); };}

            var typeDateTime = typeof(System.DateTime);

            foreach (var item in entity)
            {
                var row = result.NewRow();

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item, null);
                    row[prop.Name] = typeDateTime.Equals(prop.PropertyType) && System.DateTime.MinValue.Equals(value) ? System.DBNull.Value : value;
                }

                result.Rows.Add(row);
            }

            return result;
        }
        unsafe public static Common.Data.ADO.Component.DataTable ToDataTable<T>(this T entity, System.Data.Common.DbConnection connection = null, System.Data.Common.DbTransaction t = null)
        {
            if (null == entity) { return null; }

            var properties = typeof(T).GetProperties();

            var result = null == connection ? new Common.Data.ADO.Component.DataTable(typeof(T).Name) : Common.Data.ADO.SqlHelp.ExecuteTable(connection, string.Format("SELECT * FROM {0}", typeof(T).Name), CommandBehavior.SchemaOnly, t);

            if (null == connection) { foreach (var prop in properties) { result.Columns.Add(prop.Name, prop.PropertyType); };}

            var row = result.NewRow();
            var typeDateTime = typeof(System.DateTime);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity, null);
                row[prop.Name] = typeDateTime.Equals(prop.PropertyType) && System.DateTime.MinValue.Equals(value) ? System.DBNull.Value : value;
            }

            result.Rows.Add(row);

            return result;
        }
    }
}
