using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;

namespace Ranks
{
    public class RanksUtils
    {
        public string ID = "ID";
        public string UserID = "UserID";
        public string Username = "Name";
        public string RankName = "Rank";

        internal RanksUtils()
        {
            new I18N.West.CP1250();
            MySqlConnection connection = CreateConnection();
            try
            {
                connection.Open();
                connection.Close();

                CreateCheckSchema();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1042)
                {
                    Logger.LogError("Ranks >> Can't connect to MySQL host.");
                }
                else
                {
                    Logger.LogException(ex);
                }
                Ranks.Instance.UnloadPlugin();
            }
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (Ranks.Instance.Configuration.Instance.RanksDatabase.DatabasePort == 0) Ranks.Instance.Configuration.Instance.RanksDatabase.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseAddress, Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseName, Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseUsername, Ranks.Instance.Configuration.Instance.RanksDatabase.DatabasePassword, Ranks.Instance.Configuration.Instance.RanksDatabase.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public string[] GetAccountBySteamID(string SteamID)
        {
            string[] output = new string[3];
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("SELECT * FROM " + Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseTableName + " WHERE " + UserID + " = @UserID ORDER BY " + ID + " DESC LIMIT 1", connection);
                command.Parameters.AddWithValue("@UserID", SteamID);
                connection.Open();
                MySqlDataReader dataReader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                while (dataReader.Read())
                {
                    // This only supports one rank :( (looking for a better way)
                    output[0] = Convert.ToString(dataReader[UserID]);
                    output[1] = Convert.ToString(dataReader[RankName]);
                }
                dataReader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }
        public void AddRank(string SteamID, string Name, string Rank)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("INSERT INTO " + Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseTableName + "(" + UserID + ", " + Username + ", " + RankName + ") VALUES (@SteamID, @Name, @Rank);", connection);
                command.Parameters.AddWithValue("@SteamID", SteamID);
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Rank", Rank);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        
        public void RemoveRank(string SteamID)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = new MySqlCommand("DELETE FROM " + Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseTableName + " WHERE " + UserID + "=@SteamID;", connection);
                command.Parameters.AddWithValue("@SteamID", SteamID);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        internal void CreateCheckSchema()
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                connection.Open();
                command.CommandText = "SHOW TABLES LIKE '" + Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseTableName + "';";

                object test = command.ExecuteScalar();
                if (test == null)
                {
                    if (Ranks.Instance.Configuration.Instance.RanksDatabase.DebugMode == true)
                    {
                        Logger.Log("Tables not found, creating!");
                    }
                    command.CommandText = "CREATE TABLE `" + Ranks.Instance.Configuration.Instance.RanksDatabase.DatabaseTableName + "` ( `ID` int(11) NOT NULL AUTO_INCREMENT, `UserID` varchar(255) NOT NULL, `Name` varchar(255) NOT NULL DEFAULT '', `Rank` varchar(255) NOT NULL, PRIMARY KEY (`ID`) ) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin1;";

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}