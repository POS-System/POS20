using POS20.Attributes;
using POS20.Objects;
using System;
using System.Collections.Generic;
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
                SqlColumnAttribute exportParameter = property.GetCustomAttribute<SqlColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                String value = "";
                Type type;

                if (exportParameter.Type == null)
                {
                    type = property.GetType();
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
                SqlColumnAttribute exportParameter = property.GetCustomAttribute<SqlColumnAttribute>();
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
                    type = property.GetType();
                }
                else
                {
                    type = exportParameter.Type;
                }

                if (type == typeof(String))
                {
                    value = $"'{property.GetValue(data)}'";
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
            
            ret += $" WHERE ID = {data.ID}";            
            return ret;
        }

        public static String UpdateChild(ChildBaseObject data)
        {
            String ret = "";
            String value = "";            
            Type type;            

            Dictionary<String, String> fieldsValues = new Dictionary<string, string>();
            Dictionary<String, String> parentValues = new Dictionary<string, string>();
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
                SqlColumnAttribute exportParameter = property.GetCustomAttribute<SqlColumnAttribute>();
                if (exportParameter == null) continue;
                String name = "";
                if (!String.IsNullOrEmpty(exportParameter.Name))
                {
                    name = property.Name;
                }
                else
                {
                    name = exportParameter.Name;
                }

                if (exportParameter.Type == null)
                {
                    type = property.GetType();
                }
                else
                {
                    type = exportParameter.Type;
                }

                if (type == typeof(String))
                {
                    value = $"'{property.GetValue(data)}'";
                }
                else
                {
                    value = $"{property.GetValue(data)},";
                }

                fieldsValues.Add($"{name}", $"{value}");
            }

            ret += $"UPDATE {table} SET ";

            foreach (KeyValuePair<String, String> fieldsValue in fieldsValues)
            {
                ret += $"{fieldsValue.Key} = {fieldsValue.Value}";                
            }

            ret += ret.TrimEnd(',');
            ret += $" WHERE ParentID = {data.ParentID}";
            return ret;
        }
    }
}
