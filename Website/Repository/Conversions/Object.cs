using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace professionaltranslator.net.Repository.Conversions
{
    public static class Object
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {

            foreach (T item in enumeration)
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
        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist, CreateRowDelegate<T> fn)
        {
            DataTable toReturn = new DataTable();

            // Could add a check to verify that there is an element 0
            T topRec = varlist.ElementAtOrDefault(0);

            if (topRec == null)
                return toReturn;

            // Use reflection to get property names, to create table
            // column names

            PropertyInfo[] oProps = ((Type)topRec.GetType()).GetProperties();

            foreach (PropertyInfo pi in oProps)
            {
                Type pt = pi.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = System.Nullable.GetUnderlyingType(pt);
                toReturn.Columns.Add(pi.Name, pt);
            }

            foreach (T rec in varlist)
            {
                DataRow dr = toReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    object o = pi.GetValue(rec, null);
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
        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist)
        {
            DataTable toReturn = new DataTable();

            // Could add a check to verify that there is an element 0
            T TopRec = varlist.ElementAtOrDefault(0);

            if (TopRec == null)
                return toReturn;

            // Use reflection to get property names, to create table
            // column names

            PropertyInfo[] oProps = ((Type)TopRec.GetType()).GetProperties();

            foreach (PropertyInfo pi in oProps)
            {
                Type pt = pi.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = System.Nullable.GetUnderlyingType(pt);
                toReturn.Columns.Add(pi.Name, pt);
            }

            foreach (T rec in varlist)
            {
                DataRow dr = toReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    object o = pi.GetValue(rec, null);

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
        /// <param name="datatable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        public static List<T> ToList<T>(this DataTable datatable) where T : new()
        {
            List<T> temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn dataColumn in datatable.Columns)
                {
                    columnsNames.Add(dataColumn.ColumnName);
                }

                temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => GetObject<T>(row, columnsNames));
                return temp;
            }
            catch { return temp; }
        }

        public static T GetObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in properties)
                {
                    string columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        string value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (System.Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(System.Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch { return obj; }
        }

        public delegate object[] CreateRowDelegate<T>(T t);

        public static byte[] ToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            byte[] outItem;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                outItem = memoryStream.ToArray();
            }

            return outItem;
        }

        public static List<T> ToList<T>(this byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            List<T> outList = (List<T>)binForm.Deserialize(memStream);
            return outList;
        }
        public static T GetObject<T>(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T outItem = (T)binForm.Deserialize(memStream);
            return outItem;
        }
    }
}
