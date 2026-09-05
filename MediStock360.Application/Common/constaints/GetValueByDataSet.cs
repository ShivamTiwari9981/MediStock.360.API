using System;
using System.Collections.Generic;
using System.Data;

namespace MediStock360.Application.Common.constaints
{
    public static class GetValueByDataSet
    {
        public static T? GetValue<T>(DataSet dataSet,string tableName,string columnName)
        {
            if (dataSet == null ||
                !dataSet.Tables.Contains(tableName))
            {
                return default;
            }

            var table = dataSet.Tables[tableName];

            if (table == null ||
                !table.Columns.Contains(columnName) ||
                table.Rows.Count == 0)
            {
                return default;
            }

            var value = table.Rows[0][columnName];

            if (value == DBNull.Value)
                return default;

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
