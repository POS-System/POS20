using POS20.Attributes;
using POS20.Objects;
using POS20.SqlHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;

namespace GIIS21.SqlEngine
{
    public static class DataBaseData
    {
        public static String _database = "JDB_POS_2";
        public static String _servername = "serv40";
        public static String _serverlogin = "tamuz";
        public static String _serverpassword = "effy1";
    }

    public class SqlConnectionExtended
    {
        // соединение с базой
        private String _connectionString = "";

        public SqlConnectionExtended()
        {            
            _connectionString = string.Concat("Data Source=", DataBaseData._servername, ";Initial Catalog=", DataBaseData._database, ";Integrated Security=SSPI");
        }

        public SqlConnectionExtended(Boolean fullMode)
        {
            if(fullMode)
                _connectionString = string.Concat("Data Source=", DataBaseData._servername, ";Initial Catalog=", DataBaseData._database, ";PersistSecurityInfo=True;User ID=", DataBaseData._serverlogin, ";Password=", DataBaseData._serverpassword);
        }

        public SqlConnectionExtended(string _dataSource, string _initialCatalog)
        {            
            _connectionString = string.Concat("Data Source=", _dataSource, ";Initial Catalog=", _initialCatalog, ";Integrated Security=SSPI");            
        }

        public SqlConnectionExtended(string _dataSource, string _initialCatalog, string _userID, string _Password)
        {
            _connectionString = string.Concat("Data Source=", _dataSource, ";Initial Catalog=", _initialCatalog, ";PersistSecurityInfo=True;User ID=", _userID, ";=", _Password);
        }

        public Boolean CheckConnection()
        {
            using (SqlConnection con_msql = new SqlConnection(_connectionString))
            {
                try
                {
                    con_msql.Open();
                    con_msql.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }         

        public DataTable SimpleRequest(SqlConnection con_msql, string sq)
        {
            SqlCommand command = new SqlCommand(sq, con_msql);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(reader);
            reader.Close();
            return data;
        }

        public DataTable Request(SqlConnection con_msql, string sq)
        {
            SqlCommand command = new SqlCommand(sq, con_msql);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(reader);
            reader.Close();
            return data;
        }

        public Int32 Save(BaseObject baseObject)
        {
            Int32 ret = 0;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        String request = "";
                        if (baseObject.ID == 0)
                        {
                            request = SqlGenerator.Insert(baseObject);
                        }

                        if (baseObject.ID > 0)
                        {
                            request = SqlGenerator.Update(baseObject);
                        }

                        if (baseObject.ID < 0)
                        {
                            request = SqlGenerator.Delete(baseObject);
                        }

                        SqlCommand command = new SqlCommand(request, connection);
                        command.CommandType = CommandType.Text;
                        SqlDataReader reader = command.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);
                        if (baseObject.ID == 0)
                        {
                            Int32.TryParse(data.Rows[0][0].ToString(), out ret);
                            baseObject.ID = ret;
                        }
                        if (baseObject.ID > 0)
                        {
                            ret = baseObject.ID;
                        }
                        reader.Close();

                        // основной объект прошел сохранение() вставка
                        if (ret > 0)
                        {
                            // если есть дети - то запустим детей. Иначе конец сейва
                            Type dataType = baseObject.GetType();
                            var properties = dataType.GetProperties();
                            foreach (var property in properties)
                            {
                                ChildAttribute сhildAttribute = property.GetCustomAttribute<ChildAttribute>();
                                if (сhildAttribute == null) continue;

                                IEnumerable<ChildBaseObject> childs = property.GetValue(baseObject) as IEnumerable<ChildBaseObject>;
                                if (childs != null)
                                {
                                    foreach (ChildBaseObject child in childs)
                                    {
                                        child.ParentID = ret;
                                        if (baseObject.ID == 0)
                                        {
                                            request = SqlGenerator.Insert(child);
                                        }

                                        if (baseObject.ID > 0)
                                        {
                                            request = SqlGenerator.Update(child);
                                        }                                       

                                        command = new SqlCommand(request, connection);
                                        command.CommandType = CommandType.Text;
                                        reader = command.ExecuteReader();
                                        data = new DataTable();
                                        data.Load(reader);
                                        if (baseObject.ID == 0)
                                        {
                                            Int32 retChild;
                                            Int32.TryParse(data.Rows[0][0].ToString(), out retChild);
                                            child.ID = retChild;
                                        }
                                        reader.Close();
                                    }
                                }                                
                            }
                        }
                        
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                if (baseObject.ID == 0)
                {
                    baseObject.ID = 0;
                    Type dataType = baseObject.GetType();
                    var properties = dataType.GetProperties();
                    foreach (var property in properties)
                    {
                        ChildAttribute сhildAttribute = property.GetCustomAttribute<ChildAttribute>();
                        if (сhildAttribute == null) continue;

                        IEnumerable<ChildBaseObject> childs = property.GetValue(baseObject) as IEnumerable<ChildBaseObject>;
                        if (childs != null)
                        {
                            foreach (ChildBaseObject child in childs)
                            {
                                child.ParentID = 0;
                                child.ID = 0;
                            }
                        }
                    }
                }
                ret = -1;
            }
            return ret;                      
        }

        public List<T> Select<T>(BaseObject baseObject)
        {
            List<T> ret = new List<T>();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        String request = SqlGenerator.Select(baseObject);
                        SqlCommand command = new SqlCommand(request, connection);
                        command.CommandType = CommandType.Text;
                        SqlDataReader reader = command.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);                        
                        reader.Close();
                        foreach (DataRow dataRow in data.Rows)
                        {                            
                            T currentBaseObject =  SqlGenerator.Convert<T>(dataRow);
                            ret.Add(currentBaseObject);
                        }
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {               
                ret = null;
            }            
            return ret;
        }
    }
}
