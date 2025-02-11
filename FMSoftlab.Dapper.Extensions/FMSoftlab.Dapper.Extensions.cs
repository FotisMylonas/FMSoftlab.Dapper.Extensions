using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Dapper;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient.Server;


namespace FMSoftlab.Dapper.Extensions
{

    public class DBAttributeDecimal : System.Attribute
    {
        public byte Precision { get; }
        public byte Scale { get; }
        public DBAttributeDecimal(byte precision, byte scale)
        {
            this.Precision = precision;
            this.Scale = scale;
        }
    }

    public class DBAttributeMoney : System.Attribute
    {

    }

    public class DBAttributeMaxLen : System.Attribute
    {
        public int MaxLength;
        public DBAttributeMaxLen(int maxLength)
        {
            this.MaxLength = maxLength;
        }
    }
    public class DBAttributeNVarcharMax : System.Attribute
    {
        public DBAttributeNVarcharMax()
        {

        }
    }
    public static class TypeToDbType
    {
        //https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings

        public static readonly Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>()
        {

            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
            //,[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        };

        public static readonly Dictionary<Type, SqlDbType> sqldbtypeMap = new Dictionary<Type, SqlDbType>()
        {
            [typeof(byte)] = SqlDbType.TinyInt,
            [typeof(sbyte)] = SqlDbType.TinyInt,
            [typeof(short)] = SqlDbType.SmallInt,
            [typeof(ushort)] = SqlDbType.SmallInt,
            [typeof(int)] = SqlDbType.Int,
            [typeof(uint)] = SqlDbType.Int,
            [typeof(long)] = SqlDbType.BigInt,
            [typeof(ulong)] = SqlDbType.BigInt,
            [typeof(float)] = SqlDbType.Float,
            [typeof(double)] = SqlDbType.Decimal,
            [typeof(decimal)] = SqlDbType.Decimal,
            [typeof(bool)] = SqlDbType.Bit,
            [typeof(string)] = SqlDbType.NVarChar,
            [typeof(char[])] = SqlDbType.NVarChar,
            [typeof(char)] = SqlDbType.Char,
            [typeof(Guid)] = SqlDbType.UniqueIdentifier,
            [typeof(DateTime)] = SqlDbType.DateTime,
            [typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset,
            [typeof(byte[])] = SqlDbType.VarBinary,
            [typeof(byte?)] = SqlDbType.TinyInt,
            [typeof(sbyte?)] = SqlDbType.SmallInt,
            [typeof(short?)] = SqlDbType.SmallInt,
            [typeof(ushort?)] = SqlDbType.SmallInt,
            [typeof(int?)] = SqlDbType.Int,
            [typeof(uint?)] = SqlDbType.Int,
            [typeof(long?)] = SqlDbType.BigInt,
            [typeof(ulong?)] = SqlDbType.BigInt,
            [typeof(float?)] = SqlDbType.Float,
            [typeof(double?)] = SqlDbType.Decimal,
            [typeof(decimal?)] = SqlDbType.Decimal,
            [typeof(bool?)] = SqlDbType.Bit,
            [typeof(char?)] = SqlDbType.Char,
            [typeof(Guid?)] = SqlDbType.UniqueIdentifier,
            [typeof(DateTime?)] = SqlDbType.DateTime,
            [typeof(DateTimeOffset?)] = SqlDbType.DateTimeOffset
            //,[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        };

        public static DbType GetDBTypeFromType(Type T)
        {
            return typeMap[T];
        }

        public static SqlDbType GetSqlDbTypeFromType(Type T)
        {
            return sqldbtypeMap[T];
        }
    }

    public static class SqlDataRecordsExtensions
    {
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, Int32 Value)
        {
            sdr.SetInt32(sdr.GetOrdinal(FieldName), Value);
        }
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, Int32? Value)
        {
            if (Value != null)
            {

                sdr.SetInt32(sdr.GetOrdinal(FieldName), Value.Value);
            }
            else
            {
                sdr.SetDBNull(sdr.GetOrdinal(FieldName));
            }
        }
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, string Value)
        {
            if (Value != null)
            {
                sdr.SetString(sdr.GetOrdinal(FieldName), Value);
            }
            else
            {
                sdr.SetDBNull(sdr.GetOrdinal(FieldName));
            }
        }
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, DateTime Value)
        {
            sdr.SetDateTime(sdr.GetOrdinal(FieldName), Value);
        }
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, DateTime? Value)
        {
            if (Value != null)
            {
                sdr.SetDateTime(sdr.GetOrdinal(FieldName), Value.Value);
            }
            else
            {
                sdr.SetDBNull(sdr.GetOrdinal(FieldName));
            }
        }
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, Guid Value)
        {
            sdr.SetGuid(sdr.GetOrdinal(FieldName), Value);
        }
        public static void SetFieldValue(this SqlDataRecord sdr, string FieldName, Boolean Value)
        {
            sdr.SetBoolean(sdr.GetOrdinal(FieldName), Value);
        }
    }
    public static class DapperExtensions
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static SqlMetaData[] GetSQLClassMetaData(Type type)
        {
            DynamicParameters d;
            List<SqlMetaData> res = new List<SqlMetaData>();
            PropertyInfo[] pinfo = type.GetProperties();
            foreach (var prop in pinfo)
            {
                if (!IsNonStringEnumerable(prop.PropertyType)
                    || prop.PropertyType == typeof(byte[])
                    )
                {
                    int maxlen = 0;
                    bool isnvarcharmax = false;
                    byte precision = 0;
                    byte scale = 0;
                    bool isMoney = false;
                    foreach (Attribute a in prop.GetCustomAttributes(false))
                    {
                        if (a is MaxLengthAttribute)
                        {
                            MaxLengthAttribute dba = (MaxLengthAttribute)a;
                            maxlen = dba.Length;
                            if (maxlen <= 0 || maxlen == int.MaxValue)
                            {
                                isnvarcharmax = true;
                            }
                        }
                        if (a is DBAttributeMaxLen)
                        {
                            DBAttributeMaxLen dba = (DBAttributeMaxLen)a;
                            maxlen = dba.MaxLength;
                            if (maxlen <= 0 || maxlen == int.MaxValue)
                            {
                                isnvarcharmax = true;
                            }
                        }
                        if (a is DBAttributeNVarcharMax)
                        {
                            DBAttributeNVarcharMax dba = (DBAttributeNVarcharMax)a;
                            isnvarcharmax = true;
                        }
                        if (a is DBAttributeDecimal)
                        {
                            DBAttributeDecimal dbd = (DBAttributeDecimal)a;
                            precision = dbd.Precision;
                            scale = dbd.Scale;
                        }
                        if (a is DBAttributeMoney)
                        {
                            isMoney = true;
                        }
                    }
                    SqlDbType t;
                    if (isMoney)
                    {
                        t = SqlDbType.Money;
                    }
                    else
                    {
                        t = TypeToDbType.GetSqlDbTypeFromType(prop.PropertyType);
                    }
                    SqlMetaData md;
                    if (
                        ((t==SqlDbType.NVarChar)
                        || (t==SqlDbType.VarChar)
                        || (t==SqlDbType.NChar)
                        || (t==SqlDbType.Char)
                        )&& ((maxlen > 0) || (isnvarcharmax)))
                    {
                        md = new SqlMetaData(prop.Name, t, (isnvarcharmax) ? SqlMetaData.Max : maxlen);
                    }
                    else if (t==SqlDbType.Decimal || t==SqlDbType.Float)
                    {
                        md = new SqlMetaData(prop.Name, dbType: t, precision: precision, scale: scale);
                    }
                    else
                    {
                        md = new SqlMetaData(prop.Name, t);
                    }
                    res.Add(md);
                }
            }
            return res.ToArray();
        }
        public static SqlDataRecord GetEmptySQLRecord(Type type)
        {
            return new SqlDataRecord(GetSQLClassMetaData(type));
        }
        private static bool IsNonStringEnumerable(Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type) || typeof(ICollection).IsAssignableFrom(type);
        }

        private static void SetValues(PropertyInfo[] pinfo, SqlDataRecord sdr, object instance)
        {
            foreach (var prop in pinfo)
            {
                if (!IsNonStringEnumerable(prop.PropertyType)
                    || prop.PropertyType == typeof(byte[])
                    )
                {
                    sdr.SetValue(sdr.GetOrdinal(prop.Name), prop.GetValue(instance));
                }
            }
        }
        private static List<SqlDataRecord> GetSqlDataRecords<T>(IEnumerable<T> data)
        {
            PropertyInfo[] pinfo = typeof(T).GetProperties();
            SqlMetaData[] md = GetSQLClassMetaData(typeof(T));
            List<SqlDataRecord> drl = new List<SqlDataRecord>();
            if ((data != null) && (data.Count() > 0))
            {
                foreach (T instance in data)
                {
                    SqlDataRecord res = new SqlDataRecord(md);
                    SetValues(pinfo, res, instance);
                    drl.Add(res);
                }
            }
            return drl;
        }
        private static SqlMapper.ICustomQueryParameter GetTableValuedParam<T>(string typeName, IEnumerable<T> data)
        {
            SqlMapper.ICustomQueryParameter res = null;
            if (data != null)
            {
                var sqlDataRecords = GetSqlDataRecords(data);
                if (sqlDataRecords?.Count() > 0)
                {
                    res = sqlDataRecords.AsTableValuedParameter(typeName);
                }
            }
            return res;
        }

        public static SqlMapper.ICustomQueryParameter GetTableValuedParam<T>(this IEnumerable<T> data, string typeName)
        {
            return GetTableValuedParam(typeName, data);
        }
        public static void AddTableValuedParam<T>(this DynamicParameters dyn, string paramName, string typeName, IEnumerable<T> data)
        {
            if (data != null)
            {
                SqlMapper.ICustomQueryParameter param = data.GetTableValuedParam(typeName);
                dyn.Add(name: paramName, value: param, direction: ParameterDirection.Input);
            }
        }
    }
}