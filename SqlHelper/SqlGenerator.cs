using POS20.Attributes;
using POS20.Helper;
using POS20.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace POS20.SqlHelper
{
    public static class SqlGenerator 
    {
        static ConvertData convertData = new ConvertData();

        public class SqlSaveData
        {
            private List<SqlParameter> sqlParameters;
            private String storeProcedure;
            private String table;

            public SqlSaveData()
            {
                sqlParameters = new List<SqlParameter>();
                storeProcedure = "";
                Table = "";
            }

            public string StoreProcedure
            {
                get
                {
                    return storeProcedure;
                }
            }

            public List<SqlParameter> SqlParameters
            {
                get
                {
                    return sqlParameters;
                }
            }

            public string Table
            {
                get
                {
                    return table;
                }

                set
                {
                    table = value;
                    storeProcedure = $"xp_Save{value}";
                }
            }

            public void AddParameters(SqlParameter sqlParameter)
            {
                sqlParameters.Add(sqlParameter);
            }

            public void AddParameters(string parameterName, SqlDbType sqlDbType, object value)
            {
                SqlParameter sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = parameterName;
                sqlParameter.SqlDbType = sqlDbType;
                sqlParameter.Value = value;
                sqlParameters.Add(sqlParameter);
            }
        }

        static String PrepareTableData(Type dataType, SqlAttribute attribute)
        { 
            String ret = "";
            if (String.IsNullOrEmpty(attribute.Name))
            {
                ret = $"{dataType.Name}";
            }
            else
            {
                ret = $"{attribute.Name}";
            }
            return ret; 
        }

        static String PrepareName(PropertyInfo propertyInfo, SqlAttribute attribute)
        {
            String ret = "";
            if (String.IsNullOrEmpty(attribute.Name))
            {
                ret = $"[{propertyInfo.Name}]";
            }
            else
            {
                ret = $"[{attribute.Name}]";
            }
            return ret;
        }

        public static SqlDbType ConvertTypeToSql(Type inputType)
        {
            var typeMap = new Dictionary<Type, SqlDbType>();
            typeMap[typeof(string)] = SqlDbType.NVarChar;
            typeMap[typeof(char[])] = SqlDbType.NVarChar;
            typeMap[typeof(int)] = SqlDbType.Int;
            typeMap[typeof(Int32)] = SqlDbType.Int;
            typeMap[typeof(Int16)] = SqlDbType.SmallInt;
            typeMap[typeof(Int64)] = SqlDbType.BigInt;
            typeMap[typeof(Byte[])] = SqlDbType.VarBinary;
            typeMap[typeof(Boolean)] = SqlDbType.Bit;
            typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
            typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
            typeMap[typeof(Decimal)] = SqlDbType.Decimal;
            typeMap[typeof(Double)] = SqlDbType.Float;
            typeMap[typeof(Decimal)] = SqlDbType.Money;
            typeMap[typeof(Byte)] = SqlDbType.TinyInt;
            typeMap[typeof(TimeSpan)] = SqlDbType.Time;
            return typeMap[(inputType)];
        }

        public static Type ConvertTypeFromSql(SqlDbType inputType)
        {
            var typeMap = new Dictionary<SqlDbType, Type>();
            typeMap[SqlDbType.NVarChar] = typeof(string);
            typeMap[SqlDbType.NVarChar] = typeof(char[]);
            typeMap[SqlDbType.Int] = typeof(int);
            typeMap[SqlDbType.Int] = typeof(Int32);
            typeMap[SqlDbType.SmallInt] = typeof(Int16);
            typeMap[SqlDbType.BigInt] = typeof(Int64);
            typeMap[SqlDbType.VarBinary] = typeof(Byte[]);
            typeMap[SqlDbType.Bit] = typeof(Boolean);
            typeMap[SqlDbType.DateTime2] = typeof(DateTime);
            typeMap[SqlDbType.DateTimeOffset] = typeof(DateTimeOffset);
            typeMap[SqlDbType.Decimal] = typeof(Decimal);
            typeMap[SqlDbType.Float] = typeof(Double);
            typeMap[SqlDbType.Money] = typeof(Decimal);
            typeMap[SqlDbType.TinyInt] = typeof(Byte);
            typeMap[SqlDbType.Time] = typeof(TimeSpan);
            return typeMap[(inputType)];
        }

        static SqlDbType PrepareTypeData(PropertyInfo propertyInfo, SqlOutputColumnAttribute sqlOutputColumnAttribute)
        {
            SqlDbType ret = SqlDbType.NVarChar;
            Type type;
            if (sqlOutputColumnAttribute.Type == null)
            {
                type = propertyInfo.PropertyType;
            }
            else
            {
                type = sqlOutputColumnAttribute.Type;
            }

            SqlDbType typeToSql = ConvertTypeToSql(type);
            Type typeFromSql = ConvertTypeFromSql(typeToSql);

            //switch (type.ToString().ToLower())
            //{
            //    case "string":
            //        ret = SqlDbType.NVarChar;
            //        break;
            //    case "Int64":
            //        ret = SqlDbType.BigInt;
            //        break;

            //    default:
            //        ret = SqlDbType.NVarChar;
            //        break;            
            //}

            return ret;
        }
        static SqlParameter PrepareColumnData(PropertyInfo propertyInfo, SqlOutputColumnAttribute sqlOutputColumnAttribute, BaseObject data)
        {
            SqlParameter ret = new SqlParameter();
            ret.SqlDbType = PrepareTypeData(propertyInfo, sqlOutputColumnAttribute);
            ret.ParameterName = PrepareName(propertyInfo, sqlOutputColumnAttribute);

            if (ret.SqlDbType == SqlDbType.NVarChar)
            {
                ret.Value = $"'{propertyInfo.GetValue(data)}',";
            }
            else
            {
                ret.Value = $"{propertyInfo.GetValue(data)},";    
            }
            return ret;
        }

        public static SqlSaveData Save(BaseObject data)
        {
            SqlSaveData ret = new SqlSaveData();

            // таблица(и соответственно имя хранимки для сохраниния)
            Type dataType = data.GetType();            
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();            
            ret.Table = PrepareTableData(dataType, tableAttribute);

            // данные(и соответственно параметры для сохранения)
            var properties = dataType.GetProperties();
            foreach (var property in properties)
            {
                SqlOutputColumnAttribute exportParameter = property.GetCustomAttribute<SqlOutputColumnAttribute>();
                if (exportParameter == null) 
                    continue;
                ret.AddParameters(PrepareColumnData(property, exportParameter, data));
            }
            return ret;
        }        

        public static String Select(BaseObject data)
        {
            // пока простой запрос
            String ret = "";
            string table;            

            List<String> names = new List<String>();
            List<String> joinNames = new List<String>();
            List<String> joinTables = new List<String>();

            Type dataType = data.GetType();
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();
            if (tableAttribute != null && !String.IsNullOrEmpty(tableAttribute.Name))
            {
                table = $"[{tableAttribute.Name}]";
            }
            else
            {
                table = $"[{dataType.Name}]";
            }

            IEnumerable<SqlJoinAttribute> sqlJoinAttributes = dataType.GetCustomAttributes<SqlJoinAttribute>();
            foreach (SqlJoinAttribute sqlJoinAttribute in sqlJoinAttributes)
            {
                if (sqlJoinAttribute != null && !String.IsNullOrEmpty(sqlJoinAttribute.TableName))
                {
                    string joinTable = "";
                    String joinType = "";
                    if (String.IsNullOrEmpty(sqlJoinAttribute.JoinType))
                    {
                        joinType = " INNER JOIN";
                    }
                    else
                    {
                        joinType = $" {sqlJoinAttribute.JoinType} JOIN";
                    }
                    joinTable = $"{joinType} [{sqlJoinAttribute.TableName}] {sqlJoinAttribute.Name} ON {sqlJoinAttribute.Name}.{sqlJoinAttribute.FieldTarget} = {table}.{sqlJoinAttribute.FieldSource}";
                    joinTables.Add(joinTable);
                }
            }

            var properties = dataType.GetProperties();
            foreach (var property in properties)
            {
                SqlInputColumnAttribute exportParameter = property.GetCustomAttribute<SqlInputColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                Type type;

                if (exportParameter.Type == null)
                {
                    type = property.PropertyType;
                }
                else
                {
                    type = exportParameter.Type;
                }

                if (String.IsNullOrEmpty(exportParameter.Name))
                {
                    name = property.Name;
                }
                else
                {
                    name = exportParameter.Name;
                }

                names.Add(name);
            }

            foreach (var property in properties)
            {
                SqlInputJoinColumnAttribute exportParameter = property.GetCustomAttribute<SqlInputJoinColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";

                if (!String.IsNullOrEmpty(exportParameter.TableName))
                {
                    if (String.IsNullOrEmpty(exportParameter.Name))
                    {
                        name = $"{exportParameter.TableName}.{property.Name}";
                    }
                    else
                    {
                        name = $"{exportParameter.TableName}.{exportParameter.Name}";
                    }

                    joinNames.Add(name);
                }                
            }

            ret += $"SELECT ";
            foreach (String name in names)
            {
                ret += $"{table}.{name},";
            }

            foreach (String name in joinNames)
            {
                ret += $"{name},";
            }
            ret = ret.TrimEnd(',');

            ret += $" FROM {table} {table}";
            foreach (String joinTable in joinTables)
            {
                ret += joinTable;
            }
            
            ret += $" ORDER BY {table}.{data.Sort} {data.SqlDirection} ";
            
            // из всех записей выберем пачку. 
            ret += $"OFFSET {data.From} ROWS FETCH NEXT {data.To} ROWS ONLY";

            return ret;
        }

        public static String SelectSummary(BaseObject data)
        {
            // пока простой запрос
            String ret = "";
            string table;

            List<String> names = new List<String>();
            List<String> joinNames = new List<String>();
            List<String> joinTables = new List<String>();

            Type dataType = data.GetType();
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();
            if (tableAttribute != null && !String.IsNullOrEmpty(tableAttribute.Name))
            {
                table = $"[{tableAttribute.Name}]";
            }
            else
            {
                table = $"[{dataType.Name}]";
            }

            IEnumerable<SqlJoinAttribute> sqlJoinAttributes = dataType.GetCustomAttributes<SqlJoinAttribute>();
            foreach (SqlJoinAttribute sqlJoinAttribute in sqlJoinAttributes)
            {
                if (sqlJoinAttribute != null && !String.IsNullOrEmpty(sqlJoinAttribute.TableName))
                {
                    string joinTable = "";
                    String joinType = "";
                    if (String.IsNullOrEmpty(sqlJoinAttribute.JoinType))
                    {
                        joinType = " INNER JOIN";
                    }
                    else
                    {
                        joinType = $" {sqlJoinAttribute.JoinType} JOIN";
                    }
                    joinTable = $"{joinType} [{sqlJoinAttribute.TableName}] {sqlJoinAttribute.Name} ON {sqlJoinAttribute.Name}.{sqlJoinAttribute.FieldTarget} = {table}.{sqlJoinAttribute.FieldSource}";
                    joinTables.Add(joinTable);
                }
            }

            var properties = dataType.GetProperties();
            foreach (var property in properties)
            {
                SqlInputColumnAttribute exportParameter = property.GetCustomAttribute<SqlInputColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                Type type;

                if (exportParameter.Type == null)
                {
                    type = property.PropertyType;
                }
                else
                {
                    type = exportParameter.Type;
                }

                if (String.IsNullOrEmpty(exportParameter.Name))
                {
                    name = property.Name;
                }
                else
                {
                    name = exportParameter.Name;
                }

                names.Add(name);
            }

            foreach (var property in properties)
            {
                SqlInputJoinColumnAttribute exportParameter = property.GetCustomAttribute<SqlInputJoinColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";

                if (!String.IsNullOrEmpty(exportParameter.TableName))
                {
                    if (String.IsNullOrEmpty(exportParameter.Name))
                    {
                        name = $"{exportParameter.TableName}.{property.Name}";
                    }
                    else
                    {
                        name = $"{exportParameter.TableName}.{exportParameter.Name}";
                    }

                    joinNames.Add(name);
                }
            }

            ret += $"SELECT COUNT(*) AS COUNT";
            ret += $" FROM {table} {table}";
            foreach (String joinTable in joinTables)
            {
                ret += joinTable;
            }

            return ret;
        }

        public static void SelectFilter(BaseFilter data, String connectionString)
        {
            String request = "";
            string name;
            string tableoriginal;
            string table;
            string joinTable;            

            // пока простой запрос
            Type dataType = data.GetType();
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();
            if (tableAttribute != null && !String.IsNullOrEmpty(tableAttribute.Name))
            {
                tableoriginal = $"{tableAttribute.Name}";
                table = $"[{tableAttribute.Name}]";
            }
            else
            {
                tableoriginal = $"{dataType.Name}";
                table = $"[{dataType.Name}]";
            }
            var properties = dataType.GetProperties();
            foreach (var property in properties)
            {
                request = "";
                Filter exportParameter = property.GetCustomAttribute<Filter>();
                if (exportParameter != null)
                {
                    joinTable = exportParameter.TableName;
                    if (String.IsNullOrEmpty(exportParameter.Name))
                    {
                        name = property.Name;
                    }
                    else
                    {
                        name = exportParameter.Name;
                    }
                    switch (exportParameter.Type)
                    {
                        case FilterType.Int32:
                        case FilterType.Int64:
                        case FilterType.Decimal:
                        case FilterType.DateTime:
                            if (exportParameter.IsMin)
                            {
                                request += $"DECLARE @p_{tableoriginal}_{name}Min int ";
                                request += $"SELECT @p_{tableoriginal}_{name}Min = MIN({table}.{name}) from {table} ";
                                request += $"SELECT @p_{tableoriginal}_{name}Min AS Value ";
                            }

                            if (exportParameter.IsMax)
                            {
                                request += $"DECLARE @p_{tableoriginal}_{name}Max int ";
                                request += $"SELECT @p_{tableoriginal}_{name}Max = MAX({table}.{name}) from {table} ";
                                request += $"SELECT @p_{tableoriginal}_{name}Max AS Value ";
                            }
                            break;
                        case FilterType.Table:
                            request +=  $"SELECT DISTINCT {table}.{name}, {joinTable}.{exportParameter.Description} FROM {table} AS {table} LEFT JOIN [{joinTable}] AS {joinTable} ON {table}.{name} = { joinTable}.{exportParameter.Value}";
                            break;

                        default:
                            break;
                    }                   

                    try
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                SqlCommand command = new SqlCommand(request, connection);
                                command.CommandType = CommandType.Text;
                                SqlDataReader reader = command.ExecuteReader();
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                reader.Close();

                                ConvertData convertData = new ConvertData(dataTable.Rows[0]);
                                switch (exportParameter.Type)
                                {
                                    case FilterType.Int32:
                                        property.SetValue(data, convertData.ConvertDataInt32("Value"));
                                        break;
                                    case FilterType.Int64:
                                        property.SetValue(data, convertData.ConvertDataInt64("Value"));
                                        break;
                                    case FilterType.Decimal:
                                        property.SetValue(data, convertData.ConvertDataDecimal("Value"));
                                        break;
                                    case FilterType.DateTime:
                                        property.SetValue(data, convertData.ConvertDataDateTime("Value"));
                                        break;                                        
                                    case FilterType.Table:
                                        property.SetValue(data, dataTable);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            scope.Complete();
                        }
                    }
                    catch (TransactionAbortedException ex)
                    {                        
                    }                    
                }
            } 
        }

        public static T Convert<T>(DataRow data)
        {
            T ret = (T)Activator.CreateInstance(typeof(T));
            ConvertData convertData = new ConvertData(data);
            // пока простой запрос            

            Type dataType = typeof(T);
            var properties = dataType.GetProperties();
            foreach (var property in properties)
            {
                SqlInputColumnAttribute exportParameter = property.GetCustomAttribute<SqlInputColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                Type type;

                if (exportParameter.Type == null)
                {
                    type = property.PropertyType;
                }
                else
                {
                    type = exportParameter.Type;
                }

                if (String.IsNullOrEmpty(exportParameter.Name))
                {
                    name = property.Name;
                }
                else
                {
                    name = exportParameter.Name;
                }

                switch (type.Name.ToString().ToLower())
                {
                    case "string":
                        property.SetValue(ret, convertData.ConvertDataString(name));
                        break;

                    case "int32":
                        property.SetValue(ret, convertData.ConvertDataInt32(name));
                        break;

                    case "int64":
                        property.SetValue(ret, convertData.ConvertDataInt64(name));
                        break;

                    case "decimal":
                        property.SetValue(ret, convertData.ConvertDataDecimal(name));
                        break;

                    case "datetime":
                        property.SetValue(ret, convertData.ConvertDataDateTime(name));
                        break;

                    case "boolean":
                        property.SetValue(ret, convertData.ConvertDataBoolean(name));
                        break;

                    case "byte[]":
                        property.SetValue(ret, data[name]);
                        break;

                    default:
                        break;
                }
            }
            return ret;
        }
    }
}
