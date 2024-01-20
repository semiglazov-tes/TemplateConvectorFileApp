using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;



namespace TemplateWorkApp
{
    internal static class SqlLiteClient
    {
        private static string _connectionString = "Data Source=templateLogDb.db";
        public static void SaveDataAboutTheOperationВeingPerformed(string operationBeingPerformed, DateTime startOfOperation, DateTime endOfOperation, TimeSpan operationExecutionTime)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"INSERT INTO AppLog (operation_being_performed, start_of_operation, end_of_operation, operation_execution_time )
                                        VALUES (@operationBeingPerformed, @startOfOperation, @endOfOperation, @operationExecutionTime )";
                    command.Parameters.AddWithValue("@operationBeingPerformed", operationBeingPerformed);
                    command.Parameters.AddWithValue("@startOfOperation", startOfOperation.ToString());
                    command.Parameters.AddWithValue("@endOfOperation", endOfOperation.ToString());
                    command.Parameters.AddWithValue("@operationExecutionTime", operationExecutionTime.TotalMilliseconds.ToString("0 мс"));
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
        public static int CreateUser(string email, string password)
        {
            int userId = -1;

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"INSERT INTO Users (mail, password) VALUES (@mail, @password)";
                    command.Parameters.AddWithValue("@mail", email);
                    command.Parameters.AddWithValue("@password", password);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT last_insert_rowid() FROM Users";
                    command.CommandType = CommandType.Text;
                    userId = (int)(long)command.ExecuteScalar();
                }
            }
            return userId;
        }

        public static int GetUserID(string email, string password)
        {
            int userId = -1;
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT ID FROM Users WHERE mail==@mail AND password==@password";
                    command.Parameters.AddWithValue("@mail", email);
                    command.Parameters.AddWithValue("@password", password);
                    command.CommandType = CommandType.Text;
                    if (command.ExecuteScalar() == null) 
                    {
                        return userId;
                    }
                    userId = (int)(long)command.ExecuteScalar();
                }
            }
            return userId;
        }

        public static Tuple<string,string>GetUserMailInfo( int userID)
        {
            string mail;
            string password;
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT mail, password FROM Users WHERE ID = @userID";
                    command.Parameters.AddWithValue("@userID", userID);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            mail = reader.GetString(0);
                            password = reader.GetString(1);
                            return Tuple.Create(mail, password);
                        }
                    }
                }
            }
            return null;
        }
    }
}
