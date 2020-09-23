using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using professionaltranslator.net.Repository;
using convert = professionaltranslator.net.Repository.Conversions;

namespace professionaltranslator.net.Models
{
    public class Image
    {
        public Guid? Id { get; set; }
        public string Path { get; set; }

        private Image() {}

        //private static Image Get(DataRow dataRow)
        //{
        //    if (dataRow == null) return null;
        //    var output = GetItem<Image>(dataRow);
        //    return output;
        //}

        //internal static Image Get(DbDataAdapter dataAdapter)
        //{
        //    using var dataTable = new DataTable();
        //    dataAdapter.Fill(dataTable);
        //    if (dataTable.Rows.Count == 0) return null;
        //    var dataRow = dataTable.Rows[0];
        //    return dataRow == null ? null : Get(dataRow);
        //}

        //internal List<Image> List(DbDataAdapter dataAdapter)
        //{
        //    var output = new List<Image>();

        //    using var dataTable = new DataTable();
        //    dataAdapter.Fill(dataTable);
        //    if (dataTable.Rows.Count <= 0) return output;
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        var image = Get(row);
        //        if ((image.Id != null) && (!output.Contains(image)))
        //        {
        //            output.Add(image);
        //        }
        //    }

        //    return output;
        //}
    }
}
