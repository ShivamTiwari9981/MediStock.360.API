using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace MediStock360.Application.Common
{
    public static class GenericProcedureCall
    {
        #region StoredProcedureName
        public static class StoredProcedure
        {
            public const string sp_GenerateMasterCode = "sp_GenerateMasterCode";
            public const string sp_RegisterClientUser = "sp_RegisterClientUser";
            public const string Sp_User_Login = "Sp_User_Login";
            public const string sp_AssignRolePermissions_TVP = "sp_AssignRolePermissions_TVP";
            public const string sp_AssignBulkUserRoles = "sp_AssignBulkUserRoles";
            public const string sp_GetUserRolePermissions = "sp_GetUserRolePermissions";
            public const string sp_AddEmployee = "sp_AddEmployee";
            public const string Sp_EmployeeSalary = "Sp_EmployeeSalary";
            public const string SP_Add_User = "SP_Add_User";
            public const string sp_GetEmployees = "sp_GetEmployees";
            public const string sp_LoadEmployeeDropdown = "sp_LoadEmployeeDropdown";
        }
        #endregion

        #region StoredProcedure
        public static DataSet ExecuteStoredProcedure(string storedProcedureName, IEnumerable<SqlParameter> parameters, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.Transaction = dbTransaction;
                cmd.CommandText = storedProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    var ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }
        public static DataSet ExecuteStoredProcedure(string storedProcedureName, IEnumerable<SqlParameter> parameters, DbConnection dbConnection)
        {
            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = storedProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    var ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }

        public static DataSet ExecuteStoredProcedureWithTransation(
        string storedProcedureName,
        IEnumerable<SqlParameter> parameters,
        DbConnection dbConnection,
        DbTransaction? transaction = null)
        {
            if (dbConnection.State != ConnectionState.Open)
                dbConnection.Open();

            using var cmd = dbConnection.CreateCommand();

            cmd.CommandText = storedProcedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = transaction;

            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            using var da = DbProviderFactories
                .GetFactory(dbConnection)
                .CreateDataAdapter();

            da.SelectCommand = cmd;

            var ds = new DataSet();
            da.Fill(ds);

            return ds;
        }

        public static DataTable ExecuteFunctionProcedure(string functionProcedureName, IEnumerable<SqlParameter> parameters, DbConnection dbConnection)
        {
            var ds = new DataSet();
            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM DBO." + functionProcedureName;
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public static string ExecuteFunctionProcedureScalar(string functionProcedureName, IEnumerable<SqlParameter> parameters, DbConnection dbConnection)
        {
            var ds = new DataSet();
            using (var cmd = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                cmd.CommandText = "SELECT DBO." + functionProcedureName;
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                var result = cmd.ExecuteScalar().ToString();
                dbConnection.Close();
                return result;
            }
        }
        #endregion

        #region AsyncProcedure 
        public static async Task<int> ExecuteStoredProcedureAsync(
    string procedureName,
    List<SqlParameter> parameters,
    DbConnection connection)
        {
            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = procedureName;

                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Any())
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }


        public static async Task<DataSet> ExecuteStoredProcedureDataSetAsync(string procedureName, List<SqlParameter> parameters, DbConnection connection)
        {
            var dataSet = new DataSet();

            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = procedureName;

                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Any())
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var reader = await command.ExecuteReaderAsync();

                do
                {
                    var table = new DataTable();

                    table.Load(reader);

                    dataSet.Tables.Add(table);

                } while (!reader.IsClosed && await reader.NextResultAsync());

                return dataSet;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }
        #endregion
        public static IList<T> ToIList<T>(List<T> t)
        {
            return t;
        }
        #region CommonMethod
        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt) where T : new()
            {
                var properties = typeof(T).GetProperties();

                var columnDictionary = dt.Columns
                    .Cast<DataColumn>()
                    .ToDictionary(c => c.ColumnName.ToLower(), c => c.ColumnName);

                var list = new List<T>();

                foreach (DataRow row in dt.Rows)
                {
                    T obj = new T();

                    foreach (var prop in properties)
                    {
                        if (columnDictionary.TryGetValue(prop.Name.ToLower(), out string columnName))
                        {
                            var value = row[columnName];

                            if (value != DBNull.Value)
                            {
                                try
                                {
                                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType)
                                                     ?? prop.PropertyType;

                                    var safeValue = Convert.ChangeType(value, targetType);

                                    prop.SetValue(obj, safeValue);
                                }
                                catch
                                {
                                    // optional logging
                                }
                            }
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            //public static List<T> ConvertToList<T>(DataTable dt)
            //{
            //    var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            //    var properties = typeof(T).GetProperties();
            //    return dt.AsEnumerable().Select(row =>
            //    {
            //        var objT = Activator.CreateInstance<T>();
            //        foreach (var pro in properties)
            //        {
            //            if (columnNames.Contains(pro.Name.ToLower()))
            //            {
            //                try
            //                {
            //                    pro.SetValue(objT, row[pro.Name]);
            //                }
            //                catch (Exception ex) { }
            //            }
            //        }
            //        return objT;
            //    }).ToList();
            //}
        }
        #endregion
    }
}
