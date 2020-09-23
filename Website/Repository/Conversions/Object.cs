﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace professionaltranslator.net.Repository.Conversions
{
    internal class Object
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        internal static void ForEach<T>(IEnumerable<T> enumeration, Action<T> action)
        {

            foreach (var item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convert  list to Data Table
        /// </summary>
        /// <typeparam name="T">Target Class</typeparam>
        /// <param name="varlist">list you want to convert it to Data Table</param>
        /// <param name="fn">Delegate Function to Create Row</param>
        /// <returns>Data Table That Represent List data</returns>
        internal static DataTable ToDataTable<T>(IEnumerable<T> varlist, CreateRowDelegate<T> fn)
        {
            var toReturn = new DataTable();

            // Could add a check to verify that there is an element 0
            var topRec = varlist.ElementAtOrDefault(0);

            if (topRec == null)
                return toReturn;

            // Use reflection to get property names, to create table
            // column names

            var oProps = ((Type)topRec.GetType()).GetProperties();

            foreach (var pi in oProps)
            {
                var pt = pi.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = System.Nullable.GetUnderlyingType(pt);
                toReturn.Columns.Add(pi.Name, pt);
            }

            foreach (var rec in varlist)
            {
                var dr = toReturn.NewRow();
                foreach (var pi in oProps)
                {
                    var o = pi.GetValue(rec, null);
                    if (o == null)
                        dr[pi.Name] = DBNull.Value;
                    else
                        dr[pi.Name] = o;
                }
                toReturn.Rows.Add(dr);
            }

            return toReturn;
        }

        /// <summary>
        /// Convert  list to Data Table
        /// </summary>
        /// <typeparam name="T">Target Class</typeparam>
        /// <param name="varlist">list you want to convert it to Data Table</param>
        /// <returns>Data Table That Represent List data</returns>
        internal static DataTable ToDataTable<T>(IEnumerable<T> varlist)
        {
            var toReturn = new DataTable();

            // Could add a check to verify that there is an element 0
            var TopRec = varlist.ElementAtOrDefault(0);

            if (TopRec == null)
                return toReturn;

            // Use reflection to get property names, to create table
            // column names

            var oProps = ((Type)TopRec.GetType()).GetProperties();

            foreach (var pi in oProps)
            {
                var pt = pi.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = System.Nullable.GetUnderlyingType(pt);
                toReturn.Columns.Add(pi.Name, pt);
            }

            foreach (var rec in varlist)
            {
                var dr = toReturn.NewRow();
                foreach (var pi in oProps)
                {
                    var o = pi.GetValue(rec, null);

                    if (o == null)
                        dr[pi.Name] = DBNull.Value;
                    else
                        dr[pi.Name] = o;
                }
                toReturn.Rows.Add(dr);
            }

            return toReturn;
        }

        /// <summary>
        /// Convert Data Table To List of Type T
        /// </summary>
        /// <typeparam name="T">Target Class to convert data table to List of T </typeparam>
        /// <param name="dataTable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        internal static List<T> ToList<T>(DataTable dataTable) where T : new()
        {
            var temp = new List<T>();
            try
            {
                var columnsNames = (from DataColumn dataColumn in dataTable.Columns select dataColumn.ColumnName).ToList();

                temp = dataTable.AsEnumerable().ToList().ConvertAll<T>(row => GetObject<T>(row, columnsNames));
                return temp;
            }
            catch { return temp; }
        }

        internal static T GetObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            var obj = new T();
            try
            {
                var properties = typeof(T).GetProperties();
                foreach (var objProperty in properties)
                {
                    var columnName = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (string.IsNullOrEmpty(columnName)) continue;
                    var value = row[columnName].ToString();
                    if (string.IsNullOrEmpty(value)) continue;
                    if (System.Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                    {
                        value = row[columnName].ToString()?.Replace("$", "").Replace(",", "");
                        objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(System.Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                    }
                    else
                    {
                        value = row[columnName].ToString()?.Replace("%", "");
                        objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                    }
                }
                return obj;
            }
            catch { return obj; }
        }

        public delegate object[] CreateRowDelegate<T>(T t);

        internal static byte[] ToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            var outItem = memoryStream.ToArray();

            return outItem;
        }

        internal static List<T> ToList<T>(byte[] bytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var outList = (List<T>)binForm.Deserialize(memStream);
            return outList;
        }
        internal static T GetObject<T>(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var outItem = (T)binForm.Deserialize(memStream);
            return outItem;
        }

        protected static List<T> ConvertDataTable<T>(DbDataAdapter dataAdapter)
        {
            var output = new List<T>();

            using var dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            return dataTable.Rows.Count <= 0 ? output : ConvertDataTable<T>(dataTable);
        }

        protected static List<T> ConvertDataTable<T>(DataTable dt)
        {
            return (from DataRow row in dt.Rows select GetItem<T>(row)).ToList();
        }

        protected static T GetItem<T>(DbDataAdapter dataAdapter)
        {
            using var dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            if (dataTable.Rows.Count == 0) return default;
            var dataRow = dataTable.Rows[0];
            return dataRow == null ? default : GetItem<T>(dataRow);
        }

        protected static T GetItem<T>(DataRow dr)
        {
            var temp = typeof(T);
            var obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (var pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    }
                }
            }
            return obj;
        }
    }
}
