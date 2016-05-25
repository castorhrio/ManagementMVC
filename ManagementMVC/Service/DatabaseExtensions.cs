using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 自定义Connection对象
        /// </summary>
        private static IDbConnection DefaultConnection
        {
            get { return Domain.MyConfig.DefaultConnection; }
        }

        /// <summary>
        /// 自定义数据库连接字符串，与EF连接模式一致
        /// </summary>
        private static string DefaultConnectionString
        {
            get { return Domain.MyConfig.DefaultConnectionString; }
        }

        /// <summary>
        /// 动态查询主方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable SqlQueryForDynamic(this Database db, string sql,params object[] parameters)
        {
            IDbConnection defaultConn = DefaultConnection;

            db.Connection.ConnectionString = DefaultConnectionString;

            return SqlQueryForDynamicOtherDB(db, sql, defaultConn, parameters);
        }

        private static IEnumerable SqlQueryForDynamicOtherDB(this Database db, string sql, IDbConnection conn,
            params object[] parameters)
        {
            conn.ConnectionString = db.Connection.ConnectionString;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }

            using (IDataReader dataReader = cmd.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    return null;
                }

                #region 构建动态字段
                TypeBuilder builder = DatabaseExtensions.CreateTypeBuilder(
                    "EF_DynamicModelAssembly",
                    "DynamicModule",
                    "DynamicType");

                int fieldCount = dataReader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    Type t = dataReader.GetFieldType(i);
                    switch (t.Name.ToLower())
                    {
                        case "decimal":
                            t = typeof (Decimal?);
                            break;
                        case "double":
                            t = typeof (Double?);
                            break;
                        case "datetime":
                            t = typeof (DateTime?);
                            break;
                        case "single":
                            t = typeof (float?);
                            break;
                        case "int16":
                            t = typeof (int?);
                            break;
                        case "int32":
                            t = typeof (int?);
                            break;
                        case "int64":
                            t = typeof (int?);
                            break;
                        default:
                            break;
                    }

                    DatabaseExtensions.CreateAutoImplementedProperty(
                        builder,
                        dataReader.GetName(i),
                        t);
                }
                #endregion

                cmd.Parameters.Clear();
                dataReader.Close();
                dataReader.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();

                Type returnType = builder.CreateType();
                if (parameters != null)
                {
                    return db.SqlQuery(returnType, sql, parameters);
                }
                else
                {
                    return db.SqlQuery(returnType, sql);
                }
            }
        }

        private static TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName),
                AssemblyBuilderAccess.Run).DefineDynamicModule(moduleName).DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private static void CreateAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            //Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private);

            //Property getter and setter attributes.
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName,
                System.Reflection.PropertyAttributes.HasDefault,
                propertyType, null);

            //Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(
                    GetterPrefix, propertyName),
                propertyMethodAttributes,
                propertyType, Type.EmptyTypes);

            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            //Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(
                    SetterPrefix, propertyName),
                propertyMethodAttributes,
                null,
                new Type[] {propertyType});

            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetGetMethod(setterMethod);
        }

        public static dynamic SqlFunctionForDynamic(this Database db, string sql, params object[] parameters)
        {
            IDbConnection conn = DefaultConnection;

            conn.ConnectionString = DefaultConnectionString;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }

            //DataReader查询数据
            using (IDataReader dataReader = cmd.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    return null;
                }

                //DataReader转换Json
                string jsonstr = Common.JsonConverter.ToJson(dataReader);
                dataReader.Close();
                dataReader.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();

                //Json转换动态类
                dynamic dyna = Common.JsonConverter.ConvertJson(jsonstr);
                return dyna;
            }
        }

        /// <summary>
        /// 对可空类型进行判断转换(*要不然会报错)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
            {
                if (value == null)
                    return null;
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 判断指定对象是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }
    }
}
