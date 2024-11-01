using POS20.Attributes;
using POS20.Helper;
using POS20.Objects;
using POS20.SqlHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Windows;
using static POS20.SqlHelper.SqlGenerator;

namespace GIIS21.SqlEngine
{
    public static class DataBaseData
    {
        public static String _database = "JDB_POS_2";
        public static String _servername = "mos834\\SQLEXPRESS";
        public static String _serverlogin = "tamuz";
        public static String _serverpassword = "effy1";
    }

    public class Answer
    {       
        private String _errorMessage;
        private DataTable datatable;
        
        public Answer()
        {
            Datatable = new DataTable();
            _errorMessage = "";
        }

        public void SetError(String errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public DataTable Datatable
        {
            get
            {
                return datatable;
            }

            set
            {
                datatable = value;
            }
        }

        public Boolean IsError
        {
            get
            {
                return String.IsNullOrEmpty(_errorMessage);
            }            
        }
    }

    public class SqlConnectionExtended
    {
        // соединение с базой
        private String _connectionString = "";
        ConvertData convertData;

        public int ConnectionTimeOut
        {
            get
            {
                Int32 _connectTimeout = 30;                
                return _connectTimeout;
            }
        }

        public SqlConnectionExtended()
        {            
            _connectionString = string.Concat("Data Source=", DataBaseData._servername, ";Initial Catalog=", DataBaseData._database, ";Integrated Security=SSPI");
            convertData = new ConvertData();
        }

        public SqlConnectionExtended(Boolean fullMode)
        {
            if(fullMode)
                _connectionString = string.Concat("Data Source=", DataBaseData._servername, ";Initial Catalog=", DataBaseData._database, ";PersistSecurityInfo=True;User ID=", DataBaseData._serverlogin, ";Password=", DataBaseData._serverpassword);
            convertData = new ConvertData();
        }

        public SqlConnectionExtended(string _dataSource, string _initialCatalog)
        {            
            _connectionString = string.Concat("Data Source=", _dataSource, ";Initial Catalog=", _initialCatalog, ";Integrated Security=SSPI");
            convertData = new ConvertData();
        }

        public SqlConnectionExtended(string _dataSource, string _initialCatalog, string _userID, string _Password)
        {
            _connectionString = string.Concat("Data Source=", _dataSource, ";Initial Catalog=", _initialCatalog, ";PersistSecurityInfo=True;User ID=", _userID, ";=", _Password);
            convertData = new ConvertData();
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
            Byte[] timeStamp;
            Int32 ID = baseObject.ID;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        /*connection.Open();
                        String request = "";
                        SqlSaveData sqlSaveData = SqlGenerator.Save(baseObject);  

                        SqlCommand command = new SqlCommand(request, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = command.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);
                        if (ID == 0)
                        {
                            convertData = new ConvertData(data.Rows[0]);
                            timeStamp = data.Rows[0]["TimeStamp"] as Byte[];
                            ret = convertData.ConvertDataInt32("ID");
                            if (ret != 0)
                            {
                                baseObject.ID = ret;
                            }
                            else 
                            {
                                throw new TransactionAbortedException("Вставить объект невозможно.");
                            }                            
                            if (timeStamp != null)
                            {
                                baseObject.TimeStamp = timeStamp;
                            }
                            else
                            {
                                throw new TransactionAbortedException("Вставить объект невозможно.");
                            }
                        }
                        if (ID > 0)
                        {
                            convertData = new ConvertData(data.Rows[0]);
                            timeStamp = data.Rows[0]["TimeStamp"] as Byte[];
                            ret = convertData.ConvertDataInt32("COUNT");
                            if (ret == 0)
                            {
                                throw new TransactionAbortedException("Состояние объекта изменилось. Перезагрузите экран");
                            }
                            if (timeStamp != null)
                            {
                                baseObject.TimeStamp = timeStamp;
                            }
                            else
                            {
                                throw new TransactionAbortedException("Состояние объекта изменилось. Перезагрузите экран");
                            }
                        }
                        reader.Close();

                        // основной объект прошел сохранение() вставка
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

                                    if (baseObject.ID > 0)
                                    {
                                        Int32.TryParse(data.Rows[0][0].ToString(), out ret);
                                        if (ret != baseObject.ID)
                                        {
                                            MessageBox.Show("Состояние объекта изменилось. Перезагрузите экран");
                                            throw new TransactionAbortedException();
                                        }
                                        ret = baseObject.ID;
                                    }
                                    reader.Close();
                                }
                            }
                        }*/
                        Answer answer = new Answer();
                        try
                        {
                            SqlSaveData sqlSaveData = SqlGenerator.Save(baseObject);
                            SqlCommand sqlCmd = new SqlCommand(sqlSaveData.StoreProcedure, connection);
                            sqlCmd.CommandType =  CommandType.StoredProcedure;
                            sqlCmd.CommandTimeout = ConnectionTimeOut;

                            foreach (SqlParameter parametr in sqlSaveData.SqlParameters)
                            {
                                if (!sqlCmd.Parameters.Contains(parametr))
                                {
                                    sqlCmd.Parameters.Add(parametr.ParameterName, parametr.SqlDbType);
                                    sqlCmd.Parameters[parametr.ParameterName].Value = parametr.Value;
                                }
                            }

                            connection.Open();
                            SqlDataReader requestanswer = sqlCmd.ExecuteReader();
                            if (requestanswer != null)
                            {                                
                                answer.Datatable.Load(requestanswer);                                
                            }
                            connection.Close();
                        }
                        catch (Exception e)
                        {
                            answer.SetError(e.ToString());
                            //LogSqlError(sq, _sqlAnswer.AnswerText);                            
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
                MessageBox.Show(ex.Message);
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

        public Int32 SelectSummary<T>(BaseObject baseObject)
        {
            Int32 ret = 0;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        String request = SqlGenerator.SelectSummary(baseObject);
                        SqlCommand command = new SqlCommand(request, connection);
                        command.CommandType = CommandType.Text;
                        SqlDataReader reader = command.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);
                        reader.Close();
                        foreach (DataRow dataRow in data.Rows)
                        {
                            convertData = new ConvertData(data.Rows[0]);
                            ret = convertData.ConvertDataInt32("COUNT");
                        }
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                ret = 0;
            }
            return ret;
        }

        public void CollectFilter(BaseFilter filter)
        {
            SqlGenerator.SelectFilter(filter, _connectionString);            
        }
    }
}
