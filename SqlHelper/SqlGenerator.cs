using POS20.Attributes;
using POS20.Helper;
using POS20.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POS20.SqlHelper
{
    public static class SqlGenerator
    {
        public static String Insert(BaseObject data)
        {
            String ret = "";
            String fields = "";
            String values = "";
            string table;

            Type dataType = data.GetType();
            
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();
            
            if (String.IsNullOrEmpty(tableAttribute.Name))
            {
                table = dataType.Name;
            }
            else
            {
                table = tableAttribute.Name;
            }

            var properties = dataType.GetProperties(); 
            foreach (var property in properties)
            {
                SqlOutputColumnAttribute exportParameter = property.GetCustomAttribute<SqlOutputColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                String value = "";
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
                fields += $"{name},";
                 
                if (type == typeof(String))
                {
                    value = $"'{property.GetValue(data)}',";
                }
                else
                {
                    value = $"{property.GetValue(data)},";
                }                
                values += value;
            }

            if (fields.Length > 0)
                fields = fields.TrimEnd(',');
            if (values.Length > 0)
                values = values.TrimEnd(',');
            ret += $"INSERT INTO {table} ({fields}) VALUES ({values})";
            ret += Environment.NewLine;
            ret += "SELECT SCOPE_IDENTITY()";
            return ret;
        }

        public static String Delete(BaseObject data)
        {
            String ret = "";
            string table;

            Type dataType = data.GetType();
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();            
            if (String.IsNullOrEmpty(tableAttribute.Name))
            {
                table = dataType.Name;
            }
            else
            {
                table = tableAttribute.Name;
            }

            ret += $"DELETE FROM {table} WHERE ID = {data.ID}";
            return ret;
        }

        public static String Update(BaseObject data)
        {
            String ret = "";
            String value = "";
            Type type;

            Dictionary<String, String> fieldsValues = new Dictionary<string, string>();            
            string table;

            Type dataType = data.GetType();
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();
            if (String.IsNullOrEmpty(tableAttribute.Name))
            {
                table = dataType.Name;
            }
            else
            {
                table = tableAttribute.Name;
            }
            var properties = dataType.GetProperties();
            foreach (var property in properties)
            {
                SqlOutputColumnAttribute exportParameter = property.GetCustomAttribute<SqlOutputColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                if (String.IsNullOrEmpty(exportParameter.Name))
                {
                    name = property.Name;
                }
                else
                {
                    name = exportParameter.Name;
                }

                if (exportParameter.Type == null)
                {
                    type = property.PropertyType;
                }
                else
                {
                    type = exportParameter.Type;
                }

                if (type == typeof(String))
                {
                    value = $"'{property.GetValue(data)}',";
                }
                else
                {
                    value = $"{property.GetValue(data)},";
                }

                fieldsValues.Add($"{name}", $"{value}");                
            }
            
            ret += $"UPDATE {table} SET ";

            foreach(KeyValuePair<String, String> fieldsValue in fieldsValues)
            {
                ret += $"{fieldsValue.Key} = {fieldsValue.Value}";
            }
           
            ret = ret.TrimEnd(',');
            ret += $" WHERE ID = {data.ID}";
            ret += Environment.NewLine;
            ret += "SELECT SCOPE_IDENTITY()";
            return ret;
        }

        public static String Select(BaseObject data)
        {
            // пока простой запрос
            String ret = "";
            string table;
            List<String> names = new List<String>();

            Type dataType = data.GetType();
            SqlTableAttribute tableAttribute = dataType.GetCustomAttribute<SqlTableAttribute>();

            if (String.IsNullOrEmpty(tableAttribute.Name))
            {
                table = dataType.Name;
            }
            else
            {
                table = tableAttribute.Name;
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

            ret += $"SELECT ";
            foreach (String name in names)
            {
                ret += $"{name},";
            }
            ret = ret.TrimEnd(',');
            ret += $" FROM {table}";
            return ret;
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

                    default:
                        break;
                }
            }
            return ret;
        }
    }
}
