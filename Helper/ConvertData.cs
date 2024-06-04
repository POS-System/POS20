using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text;

namespace POS20.Helper
{
    public class ConvertData
    {
        public enum ConvertDataTypes
        {
            Int32 = 0,
            Int64 = 1,
            Decimal = 2,
            DateTime = 3,
            String = 4,
            Boolean = 5
        }

        DataRow dataRow;       

        public ConvertData()
        {
        }

        public ConvertData(DataRow _row)
        {
            dataRow = _row;
        }

        public Int32 ConvertDataInt32(string _columnname)
        {
            Int32 i = 0;
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (Int32.TryParse(dataRow[_columnname].ToString(), out i))
                    return i;
                else
                    return 0;
            }
            return 0;
        } 

        public Int64 ConvertDataInt64(string _columnname)
        {
            Int64 i = 0;
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (Int64.TryParse(dataRow[_columnname].ToString(), out i))
                    return i;
                else
                    return 0;
            }
            return 0;
        }
        public Double ConvertDataDouble(string _columnname)
        {
            Double i = 0.0;
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (Double.TryParse(dataRow[_columnname].ToString(), out i))
                    return i;
                else
                    return 0.0;
            }
            return 0.0;
        }

        public Decimal ConvertDataDecimal(string _columnname)
        {
            Decimal i = 0;
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (Decimal.TryParse(dataRow[_columnname].ToString(), out i))
                    return i;
                else
                    return 0;
            }
            return 0;
        }

        public byte[] ConvertDataByteArray(string _columnname)
        {
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (dataRow[_columnname] as byte[] != null)
                    return dataRow[_columnname] as byte[];                
                else
                    return null;
            }
            return null;
        }

        public DateTime? ConvertDataDateTime(string _columnname)
        {
            DateTime i = new DateTime();
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (DateTime.TryParse(dataRow[_columnname].ToString(), out i))
                    return i;
                else
                    return null;
            }
            return null;
        }

        public TimeSpan ConverDataToTimeSpan(string _columnname)
        {
            TimeSpan i = new TimeSpan();
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                if (TimeSpan.TryParse(dataRow[_columnname].ToString(), out i))
                    return i;
                else
                    return new TimeSpan();
            }
            return new TimeSpan();
        }

        public Boolean ConvertDataBoolean(string _columnname)
        {
            Boolean i;
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                String convertdata;
                convertdata = dataRow[_columnname].ToString();
                if (dataRow[_columnname].ToString() == "0")
                {
                    convertdata = "False";
                }
                if (dataRow[_columnname].ToString() == "1")
                {
                    convertdata = "True";
                }

                if (Boolean.TryParse(convertdata, out i))
                    return i;
                else
                    return false;
            }
            return false;
        }

        public Int64 ConvertDataByteArrayToInt64(string _columnname)
        {
            Int64 i = new Int64();
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                Byte[] _timeStamp = System.Text.Encoding.UTF8.GetBytes(dataRow[_columnname].ToString());
                i  = BitConverter.ToInt64(_timeStamp, 0);
                return i;
            }
            return 0;
        }

        public String ConvertDataString(string _columnname)
        {
            String i = "";
            if (_columnname != null && dataRow.Table.Columns.Contains(_columnname) && dataRow[_columnname].ToString() != null && !String.IsNullOrEmpty(dataRow[_columnname].ToString()))
            {
                i = dataRow[_columnname].ToString().Trim();
            }
            return i;
        }     
    }
}
