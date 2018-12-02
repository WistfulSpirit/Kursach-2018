using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LastBoundary
{
    class DBInteraction
    {
        private static SqlConnection connection = null;//field for SQL connection


        /// <summary>
        /// Connect to database
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static SqlConnection
                GetDBConnection(string datasource = @"(LocalDB)\MSSQLLocalDB", string database = @"D:\VisualStudio\source\repos\LastBoundary\LastBoundary\tvDB.mdf")
        {
            //
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=12345

            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\VisualStudio\source\repos\LastBoundary\LastBoundary\tvDB.mdf;Integrated Security=True;Connect Timeout=30
            string connString = @"Data Source=" + datasource + ";AttachDbFilename="
                        + database + ";Integrated Security=True;Connect Timeout=30;";

            connection = new SqlConnection(connString);

            return connection;
        }

        /// <summary>
        /// Update table Channel in SQL with list of channels in param
        /// </summary>
        /// <param name="Channels">list of channels</param>
        public static void UpdateChannelsTable(List<Channel> Channels)
        {
            //Check for connection to exist or being open
            if (connection == null)
                GetDBConnection();
            if (connection.State == ConnectionState.Open)
                connection.Close();
            connection.Open();

            var query = new SqlCommand("DELETE FROM Channel");//Command to delete all rows from channel table
            query.Connection = connection;
            query.ExecuteNonQuery();
            var cmd = new StringBuilder("INSERT INTO Channel VALUES ");//Command to insert list into table
            foreach (var Chan in Channels) //Creating commmand to insert all in one call to DB
            {
                cmd.Append($"({Chan.Number}, N'{Chan.Name}', N'{Chan.Link}'),");
            }
            query = new SqlCommand(cmd.Remove(cmd.Length - 1, 1).Append(';').ToString());
            query.Connection = connection;
            query.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Insert action log into DB
        /// </summary>
        /// <param name="action">Action type</param>
        /// <param name="description">description, null if not necessary</param>
        public static void InsertLog(Action action, string description)
        {
            //Check for connection to exist or being open
            if (connection == null)
                GetDBConnection();
            if (connection.State == ConnectionState.Open)
                connection.Close();
            connection.Open();

            var cmd = new StringBuilder($"INSERT INTO Logs(actionId, description) VALUES ({(int)action}, '{description ?? ""}')");//Command to insert list into table
            var query = new SqlCommand(cmd.ToString());
            query.Connection = connection;
            query.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Return all Logs ordered by time
        /// </summary>
        public static List<string> GetLogs()
        {
            var list = new List<string>();
            if (connection == null)
                GetDBConnection();
            if (connection.State == ConnectionState.Open)
                connection.Close();
            connection.Close();

            var query = new SqlCommand("SELECT Logs.Id, Action.Name, Logs.description, Logs.Time FROM Logs INNER JOIN Action ON Logs.actionId = Action.Id ORDER BY time");

            connection.Open();
            query.Connection = connection;
            var reader = query.ExecuteReader();
            string str = "";
            while (reader.Read())
            {
                str = String.Format(
                    $"{reader.GetInt32(0)}, " +
                    $"{reader.GetString(1)}, " +
                    $"{reader.GetString(2)}, " +
                    $"{reader.GetDateTime(3)}"
                    );
                list.Add(str);
            }

            connection.Close();
            return list;
        }


        /// <summary>
        /// Get rows from a table
        /// </summary>
        /// <returns>List of channels</returns>
        public static List<Channel> GetChannels()
        {
            var rowList = new List<Channel>();
            if (connection == null)
                GetDBConnection();
            if (connection.State == ConnectionState.Open)
                connection.Close();
            connection.Close();

            var query = new SqlCommand("SELECT * FROM Channel ORDER BY num");

            connection.Open();
            query.Connection = connection;
            var reader = query.ExecuteReader();

            while (reader.Read())
            {
                int num = reader.GetInt32(0);
                string name = reader.GetString(1);
                string link = reader.GetString(2);
                rowList.Add(new Channel(num, name, link));
            }

            connection.Close();
            return rowList;
        }
    }
}
