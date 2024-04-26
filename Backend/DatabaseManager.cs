using System.Data;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace Managers
{
    public record User(int id, string name, string password);
    public record TodoTask(int id, int user_id, string header, string description, bool done);
    public class DBManager
    {
        private SqliteConnection dbConnection;

        public DBManager(string dbName)
        {
            dbConnection = new SqliteConnection("Data Source=" + dbName + ".db");
            dbConnection.Open();
            dbConnection.Close();
        }

        public bool NewTable(string tableName, List<(string, string)> columns)
        {
            if (tableName == "") return false;

            string command = "CREATE TABLE IF NOT EXISTS " + tableName + " (";

            foreach ((string, string) column in columns)
            {
                command += column.Item1 + " " + column.Item2;

                if (columns.IndexOf(column) != columns.Count - 1)
                {
                    command += ",";
                }
            }

            command += ")";

            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = command;
            dbConnection.Open();
            sqlcommand.ExecuteNonQuery();
            dbConnection.Close();

            return true;
        }

        public void NewUser(string username, string password)
        {   
            string command = @"INSERT INTO Users (Username,Password) VALUES ($Username,$Password)";
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = command;

            var parameterName = sqlcommand.CreateParameter();
            parameterName.ParameterName = "$Username";
            parameterName.Value = username;
            sqlcommand.Parameters.Add(parameterName);
            var parameterPassword = sqlcommand.CreateParameter();
            parameterPassword.ParameterName = "$Password";
            parameterPassword.Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            sqlcommand.Parameters.Add(parameterPassword);
        
            dbConnection.Open();
            sqlcommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public void NewTask(int userId, string header, string description)
        {   
            string command = @"INSERT INTO Tasks (User_Id,Header,Description,Done) VALUES ($User_Id,$Header,$Description,$Done)";
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = command;

            var parameterUserId = sqlcommand.CreateParameter();
            parameterUserId.ParameterName = "$User_Id";
            parameterUserId.Value = userId;
            sqlcommand.Parameters.Add(parameterUserId);
            var parameterHeader = sqlcommand.CreateParameter();
            parameterHeader.ParameterName = "$Header";
            parameterHeader.Value = header;
            sqlcommand.Parameters.Add(parameterHeader);
            var parameterDescription = sqlcommand.CreateParameter();
            parameterDescription.ParameterName = "$Description";
            parameterDescription.Value = description;
            sqlcommand.Parameters.Add(parameterDescription);
            var parameterDone = sqlcommand.CreateParameter();
            parameterDone.ParameterName = "$Done";
            parameterDone.Value = 0;
            sqlcommand.Parameters.Add(parameterDone);
            
        
            dbConnection.Open();
            sqlcommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public int SignIn(string username, string password)
        {   
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = @"SELECT id,password FROM Users WHERE Username=$Username";

            var parameterName = sqlcommand.CreateParameter();
            parameterName.ParameterName = "$Username";
            parameterName.Value = username;
            sqlcommand.Parameters.Add(parameterName);
        
            dbConnection.Open();
            SqliteDataReader response = sqlcommand.ExecuteReader();

            int ret = -1;
            if (response == null)
            {
                return -1;
            }
            else
            {
                while(response.Read())
                {
                    if (response.GetString(1) == System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)))
                    {
                        ret = response.GetInt32(0);
                        break;
                    }
                }
            }
            dbConnection.Close();

            return ret;
        }

        public List<TodoTask> GetTasksFromUserId(int userId)
        {
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = @"SELECT Id,User_Id,Header,Description,Done FROM Tasks WHERE $UserId=User_Id";

            var parametertId = sqlcommand.CreateParameter();
            parametertId.ParameterName = "$UserId";
            parametertId.Value = userId;
            sqlcommand.Parameters.Add(parametertId);
        
            dbConnection.Open();
            SqliteDataReader response = sqlcommand.ExecuteReader();

            List<TodoTask> tasks = new List<TodoTask>();
            while(response.Read())
            {
                tasks.Add(new TodoTask(Convert.ToInt32(response.GetString(0)), Convert.ToInt32(response.GetString(1)), response.GetString(2), response.GetString(3), (response.GetString(4) == "1") ? true : false));
            }
            dbConnection.Close();

            return tasks;
        }

        public TodoTask? GetTaskDataFromId(int taskId)
        {
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = @"SELECT User_Id,Header,Description,Done FROM Tasks WHERE Id=$TaskId";

            var parametertId = sqlcommand.CreateParameter();
            parametertId.ParameterName = "$TaskId";
            parametertId.Value = taskId;
            sqlcommand.Parameters.Add(parametertId);
        
            dbConnection.Open();
            SqliteDataReader response = sqlcommand.ExecuteReader();
            while (response.Read())
            {
                TodoTask task = new TodoTask(taskId, Convert.ToInt32(response.GetString(0)), response.GetString(1), response.GetString(2), (response.GetInt32(3) == 1) ? true : false);
                return task;
            }
            dbConnection.Close();
            return null;
        }

        public void DeleteTaskOfId(int taskId)
        {
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = @"DELETE FROM Tasks WHERE Id=$TaskId";

            var parametertId = sqlcommand.CreateParameter();
            parametertId.ParameterName = "$TaskId";
            parametertId.Value = taskId;
            sqlcommand.Parameters.Add(parametertId);
        
            dbConnection.Open();
            SqliteDataReader response = sqlcommand.ExecuteReader();
            dbConnection.Close();
        }

        public void UpdateTask(string newHeader, string newDesc, bool done, int taskId)
        {
            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = @"UPDATE Tasks SET Header=$newHeader,Description=$newDesc,Done=$IsDone WHERE Id=$TaskId";

            var parameterHeader = sqlcommand.CreateParameter();
            parameterHeader.ParameterName = "$newHeader";
            parameterHeader.Value = newHeader;
            sqlcommand.Parameters.Add(parameterHeader);
            var parameterDesc = sqlcommand.CreateParameter();
            parameterDesc.ParameterName = "$newDesc";
            parameterDesc.Value = newDesc;
            sqlcommand.Parameters.Add(parameterDesc);
            var parameterId = sqlcommand.CreateParameter();
            parameterId.ParameterName = "$TaskId";
            parameterId.Value = taskId;
            sqlcommand.Parameters.Add(parameterId);
            var parameterDone = sqlcommand.CreateParameter();
            parameterDone.ParameterName = "$IsDone";
            parameterDone.Value = done ? 1 : 0;
            sqlcommand.Parameters.Add(parameterDone);
        
            dbConnection.Open();
            sqlcommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public List<string> GetUsernames()
        {
            List<string> usernames = new List<string>();

            var sqlcommand = dbConnection.CreateCommand();
            sqlcommand.CommandText = @"SELECT Username FROM Users";

            dbConnection.Open();
            SqliteDataReader response = sqlcommand.ExecuteReader();
            while (response.Read())
            {
                usernames.Add(response.GetString(0));
            }
            dbConnection.Close();

            return usernames;
        }
    }
}