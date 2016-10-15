using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NEF.Library.Entities.CrmEntities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CrmFieldName : Attribute
    {
        private string _fieldName;
        public CrmFieldName(string fieldName)
        {
            this._fieldName = fieldName;
        }

        public string FieldName
        {
            get
            {
                return _fieldName;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CrmFieldDataType : Attribute
    {
        private CrmDataType _dataType;
        public CrmFieldDataType(CrmDataType dataType)
        {
            this._dataType = dataType;
        }

        public CrmDataType CrmDataType
        {
            get
            {
                return _dataType;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CrmSchemaName : Attribute
    {
        private string _schemaName;
        public CrmSchemaName(string schemaName)
        {
            this._schemaName = schemaName;
        }

        public string SchemaName
        {
            get
            {
                return _schemaName;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PersonalData : Attribute
    {
        private string _fieldName;
        public PersonalData(string fieldName)
        {
            this._fieldName = fieldName;
        }

        public string FieldName
        {
            get { return _fieldName; }
        }
    }

    public enum CrmDataType
    {
        UNIQUEIDENTIFIER,
        STRING,
        INT,
        DATETIME,
        ENTITYREFERENCE,
        OPTIONSETVALUE,
        MONEY,
        DECIMAL,
        BOOL,
        ACTIVITYPARTY,
        DOUBLE,
        SMALLINT
    }
}
