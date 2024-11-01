using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GIIS21.SqlEngine;
using POS20.Attributes;
using POS20.Interface;
using POS20.SqlHelper;

namespace POS20.Objects
{
    public enum FilterType
    {
        Int32 = 1,
        Int64 = 2,
        Decimal = 3,
        DateTime = 4,
        Table = 5
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Filter : SqlAttribute
    {
        // name (имя таблицы для данных)        
        FilterType type;

        // от-до
        Boolean isMax;
        Boolean isMin;

        // таличный тип
        String value;
        String description;
        String tableName;


        public FilterType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public bool IsMax
        {
            get
            {
                return isMax;
            }

            set
            {
                isMax = value;
            }
        }

        public bool IsMin
        {
            get
            {
                return isMin;
            }

            set
            {
                isMin = value;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public string TableName
        {
            get
            {
                return tableName;
            }

            set
            {
                tableName = value;
            }
        }

        public Filter()
        {
            Type = FilterType.Int32;
            IsMin = false;
            IsMax = false;

            Value = "ID";
            Description = "Description";
            TableName = "";
        }
    }
}
