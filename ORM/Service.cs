using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ORM
{
    public class Service
    {
        private readonly IType cType;
        private readonly string tableName;

        public Service(IType type)
        {
            this.cType = type;
            this.tableName = this.cType.GetType().GetCustomAttribute<TableAttribute>().Name;            
        }

        private string CreateInsertSql(List<DataObject> list)
        {
            int i = 0;
            string sqlFields = string.Empty;
            string sqlValues = string.Empty;
            string sql = string.Empty;

            if (list.Count > 0)
            {
                foreach (var l in list)
                {
                    if (i == 0)
                    {
                        sqlFields += l.FieldName;
                        sqlValues += "@" + l.FieldName;
                    }
                    else
                    {
                        sqlFields += "," + l.FieldName;
                        sqlValues += ",@" + l.FieldName;
                    }

                    i++;
                }

                sql = "INSERT INTO " + this.tableName + " (" + sqlFields + ") VALUES (" + sqlValues + ")";
            }

            return sql;
        }

        private string CreateSelectSql(List<DataObject> list)
        {
            string sql = string.Empty;

            if (list.Count > 0)
            {
                sql += " SELECT ";
                sql += string.Join(", ", list.Select(s => s.FieldName));
                sql += " FROM " + this.tableName;
            }

            return sql;
        }

        private string CreateSelectSqlById(List<DataObject> list)
        {
            string sql = string.Empty;
            int i = 0;

            if (list.Count > 0)
            {
                sql += " SELECT ";
                sql += string.Join(", ", list.Select(s => s.FieldName));
                sql += " FROM " + this.tableName;

                foreach (var l in list.Where(s => s.FieldKey == true))
                {
                    if (i == 0)
                    {
                        sql += " WHERE " + l.FieldName + " = @" + l.FieldName;
                    }
                    else
                    {
                        sql += " AND " + l.FieldName + " = @" + l.FieldName;
                    }

                    i++;
                }
            }

            return sql;
        }

        private string CreateSelectSqlFilter(List<DataObject> list, bool like)
        {
            string sql = string.Empty;
            string filter = string.Empty;
            int i = 0;

            if (list.Count > 0)
            {
                sql += " SELECT ";
                sql += string.Join(", ", list.Select(s => s.FieldName));
                sql += " FROM " + this.tableName;

                filter = like ? " LIKE " : " = ";

                var listFilter = list.Where(s => s.FieldKey == false)
                    .Where(s => s.FieldValue != null).ToList();

                foreach (var l in listFilter)
                {
                    if (i == 0)
                    {
                        sql += " WHERE " + l.FieldName + " " + filter + " @" + l.FieldName;                        
                    }
                    else
                    {
                        sql += " AND " + l.FieldName + " " + filter + " @" + l.FieldName;                        
                    }

                    i++;
                }
            }

            return sql;
        }

        private string CreateUpdateSql(List<DataObject> list)
        {
            string sql = string.Empty;
            int i = 0;

            if (list.Count > 0)
            {
                sql += " UPDATE " + this.tableName + " SET ";

                foreach (var l in list.Where(s => s.FieldKey == false))
                {
                    if (i == 0)
                    {
                        sql += " " + l.FieldName + " = @" + l.FieldName;
                    }
                    else
                    {
                        sql += " ," + l.FieldName + " = @" + l.FieldName;
                    }
                    i++;
                }
                
                i = 0;

                foreach (var l in list.Where(s => s.FieldKey == true))
                {
                    if (i == 0)
                    {                        
                        sql += " WHERE " + l.FieldName + " = @" + l.FieldName;
                    }
                    else
                    {
                        sql += " AND " + l.FieldName + " = @" + l.FieldName;
                    }

                    i++;
                }   
            }

            return sql;
        }

        private string CreateDeleteSql(List<DataObject> list)
        {
            string sql = string.Empty;
            int i = 0;

            if (list.Count > 0)
            {
                sql += " DELETE FROM " + this.tableName;

                foreach(var l in list.Where(s => s.FieldKey == true))
                {
                    if(i == 0)
                    {
                        sql += " WHERE " + l.FieldName + " = @" + l.FieldName;
                    }
                    else
                    {
                        sql += " AND " + l.FieldName + " = @" + l.FieldName;
                    }

                    i++;
                }                
            }

            return sql;
        }

        private SqlDbType GetDbType(object value)
        {
            var result = SqlDbType.VarChar;

            try
            {
                if (value != null)
                {
                    Type type = value.GetType();

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Empty:
                            break;
                        case TypeCode.DBNull:
                            break;
                        case TypeCode.Object:
                            result = SqlDbType.Variant;
                            break;
                        case TypeCode.Boolean:
                            result = SqlDbType.Bit;
                            break;
                        case TypeCode.Char:
                            result = SqlDbType.NChar;
                            break;
                        case TypeCode.SByte:
                            result = SqlDbType.SmallInt;
                            break;
                        case TypeCode.Byte:
                            result = SqlDbType.TinyInt;
                            break;
                        case TypeCode.Int16:
                            result = SqlDbType.SmallInt;
                            break;
                        case TypeCode.UInt16:
                            result = SqlDbType.Int;
                            break;
                        case TypeCode.Int32:
                            result = SqlDbType.Int;
                            break;
                        case TypeCode.UInt32:
                            result = SqlDbType.BigInt;
                            break;
                        case TypeCode.Int64:
                            result = SqlDbType.BigInt;
                            break;
                        case TypeCode.UInt64:
                            result = SqlDbType.Decimal;
                            break;
                        case TypeCode.Single:
                            result = SqlDbType.Real;
                            break;
                        case TypeCode.Double:
                            result = SqlDbType.Float;
                            break;
                        case TypeCode.Decimal:
                            result = SqlDbType.Money;
                            break;
                        case TypeCode.DateTime:
                            result = SqlDbType.DateTime;
                            break;
                        case TypeCode.String:
                            result = SqlDbType.VarChar;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        private List<DataObject> GetDataObject<T>(T item)
        {
            var list = new List<DataObject>();
            object value;
            SqlDbType sqlDbtype;

            try
            {
                Type getType = item.GetType();

                PropertyInfo[] properties = getType.GetProperties();
                foreach (var propertie in properties)
                {
                    value = propertie.GetValue(item);
                    sqlDbtype = GetDbType(value);

                    foreach (var customAttribute in propertie.GetCustomAttributes())
                    {
                        var dataObject = new DataObject
                        {
                            FieldKey = ((TableAttribute)customAttribute).Key,
                            FieldName = propertie.Name.ToLower(),
                            FieldType = sqlDbtype,
                            FieldValue = value
                        };

                        list.Add(dataObject);
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return list;
        }

        public bool Insert()
        {
            List<DataObject> list;
            string sql;
            bool resultado = false;

            try
            {
                list = GetDataObject(this.cType).Where(s => s.FieldKey == false).ToList();

                sql = CreateInsertSql(list);

                if (!string.IsNullOrEmpty(sql) && list.Count > 0)
                {
                    using SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString);
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        foreach (var l in list)
                        {
                            SqlParameter parameter = new SqlParameter("@" + l.FieldName, l.FieldType)
                            {
                                Value = l.FieldValue
                            };
                            command.Parameters.Add(parameter);

                        }

                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                    resultado = true;
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return resultado;
        }

        public bool Update()
        {
            List<DataObject> list;
            string sql;
            bool resultado = false;

            try
            {
                list = GetDataObject(this.cType);

                sql = CreateUpdateSql(list);

                if (!string.IsNullOrEmpty(sql) && list.Count > 0)
                {
                    using SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString);
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        foreach (var l in list)
                        {
                            SqlParameter parameter = new SqlParameter("@" + l.FieldName, l.FieldType)
                            {
                                Value = l.FieldValue
                            };
                            command.Parameters.Add(parameter);

                        }

                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                    resultado = true;
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return resultado;
        }

        public bool Delete()
        {
            List<DataObject> list;
            string sql;
            bool resultado = false;

            try
            {
                list = GetDataObject(this.cType);

                sql = CreateDeleteSql(list);

                if (!string.IsNullOrEmpty(sql) && list.Count > 0)
                {
                    using SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString);
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        foreach (var l in list.Where(s => s.FieldKey == true).ToList())
                        {
                            SqlParameter parameter = new SqlParameter("@" + l.FieldName, l.FieldType)
                            {
                                Value = l.FieldValue
                            };
                            command.Parameters.Add(parameter);

                        }

                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                    resultado = true;
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return resultado;
        }

        private IEnumerable<T> ConvertDataTableToListGeneric<T>(DataTable table)
        {
            List<T> list = null;

            try
            {
                if (table.Rows.Count > 0)
                {
                    list = new List<T>();

                    foreach (DataRow row in table.Rows)
                    {
                        T item = (T)Activator.CreateInstance(typeof(T), row.ItemArray);

                        list.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return list;
        }

        public List<T> GetAll<T>() where T : class
        {
            List<T> result = null;
            DataTable table;
            List<DataObject> list;
            string sql;

            try
            {
                list = GetDataObject(this.cType);
                sql = CreateSelectSql(list);
                table = new DataTable();

                using SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString);

                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(table);

                }

                connection.Close();

                if (table.Rows.Count > 0)
                    result = ConvertDataTableToListGeneric<T>(table).ToList();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            
            return result;
        }

        public List<T> GetById<T>(T item) where T : class
        {
            List<T> result = null;
            DataTable table;
            List<DataObject> list;
            string sql;

            try
            {
                list = GetDataObject(item);
                sql = CreateSelectSqlById(list);
                table = new DataTable();

                using SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString);

                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    foreach (var l in list.Where(s => s.FieldKey == true).ToList())
                    {
                        SqlParameter parameter = new SqlParameter("@" + l.FieldName, l.FieldType)
                        {
                            Value = l.FieldValue
                        };

                        command.Parameters.Add(parameter);
                    }

                    using SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(table);

                }

                connection.Close();

                if (table.Rows.Count > 0)
                    result = ConvertDataTableToListGeneric<T>(table).ToList();

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        public List<T> GetByFilter<T>(T item, bool like) where T : class
        {
            List<T> result = null;
            DataTable table;
            List<DataObject> list;
            string sql;

            try
            {
                list = GetDataObject(item);
                sql = CreateSelectSqlFilter(list, like);
                table = new DataTable();

                using SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString);

                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    var listFilter = list.Where(s => s.FieldKey == false)
                        .Where(s => s.FieldValue != null).ToList();

                    foreach (var l in listFilter)
                    {
                        SqlParameter parameter = new SqlParameter("@" + l.FieldName, l.FieldType)
                        {
                            Value = like ? "%" + l.FieldValue + "%" : l.FieldValue
                        };

                        command.Parameters.Add(parameter);
                    }

                    using SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(table);

                }

                connection.Close();

                if (table.Rows.Count > 0)
                    result = ConvertDataTableToListGeneric<T>(table).ToList();

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        public void Destroy()
        {

        }

        public void Get()
        {

        }
    }
}
