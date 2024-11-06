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

        void SingleSave(BaseObject baseObject, SqlConnection connection)
        {
            Int32 retID;
            Answer answer = new Answer();
            try
            {
                SqlSaveData sqlSaveData = SqlGenerator.Prepare(baseObject);
                SqlCommand sqlCmd = new SqlCommand(sqlSaveData.StoreProcedure, connection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
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
                    convertData = new ConvertData(answer.Datatable.Rows[0]);
                    retID = convertData.ConvertDataInt32("ID");

                    // разбор ответа (вставка)
                    if (baseObject.ID == 0)
                    {
                        if (retID != 0)
                        {
                            baseObject.ID = retID;
                            foreach (BaseObject child in SqlGenerator.PrepareChild(baseObject, retID))
                            {
                                SingleSave(child, connection);
                            }
                        }
                        else
                        {
                            throw new TransactionAbortedException("Вставить объект невозможно.");                            
                        }
                    }

                    if (baseObject.ID > 0)
                    {
                        if (retID != 0 && retID != baseObject.ID)
                        {
                            foreach (BaseObject child in SqlGenerator.PrepareChild(baseObject, retID))
                            {
                                SingleSave(child, connection);
                            }
                        }
                        else
                        {
                            throw new TransactionAbortedException("Состояние объекта изменилось. Перезагрузите экран");
                        }
                    }

                    if (baseObject.ID < 0)
                    {
                        if (retID == 0 && retID != baseObject.ID)
                        {
                            throw new TransactionAbortedException("Удалить объект невозможно");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                answer.SetError(e.ToString());
                //LogSqlError(sq, _sqlAnswer.AnswerText);                            
            }
            finally
            {
                connection.Close();
            }
        }

        public Int32 Save(BaseObject baseObject)
        {                    
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        SingleSave(baseObject, connection);
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                
            }
            return baseObject.ID;
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
