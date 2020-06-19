using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace Ranks
{
    public class MySQLUtils
    {
        internal MySQLUtils()
        {
            MySqlConnection connection = CreateConnection();
            try
            {
                connection.Open();
                connection.Close();

                CreateCheckSchema();
            }
            catch (MySqlException ex)
            {
                Logger.LogException(ex);
                Main.Instance.UnloadPlugin();
            }
        }

        private static MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection($"SERVER={Main.Config.DatabaseAddress};DATABASE={Main.Config.DatabaseName};UID={Main.Config.DatabaseUsername};PASSWORD={Main.Config.DatabasePassword};PORT={Main.Config.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        private void CreateCheckSchema()
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SHOW TABLES LIKE '" + Main.Config.DatabaseTableName + "';";

                    object test = command.ExecuteScalar();
                    if (test == null)
                    {
                        Logger.Log("Tables not found, creating!");
                        command.CommandText =
                            $@"CREATE TABLE `{Main.Config.DatabaseTableName}`
                            (
                                `steam64` BIGINT(20) DEFAULT NULL,
                                `name` TEXT DEFAULT NULL,
                                `ranks` TEXT DEFAULT NULL
                            ) COLLATE = 'utf8_general_ci' ENGINE = InnoDB;";
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

        public async Task<bool> CheckExists(string id)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"SELECT EXISTS(SELECT 1 FROM `{Main.Config.DatabaseTableName}`
                        WHERE `steam64` = @steam64);", connection
                    );

                    command.Parameters.AddWithValue("@steam64", id);
                    await connection.OpenAsync();

                    var status = Convert.ToInt32(await command.ExecuteScalarAsync());

                    await connection.CloseAsync();
                    return status > 0;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    return false;
                }
            }
        }

        public async Task<List<string>> GetRanks(string steam64)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                string output = "";

                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"SELECT * FROM {Main.Config.DatabaseTableName}
                        WHERE steam64 = @Steam64", connection
                    );

                    command.Parameters.AddWithValue("@Steam64", steam64);
                    await connection.OpenAsync();
                    var dataReader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow);

                    while (await dataReader.ReadAsync())
                    {
                        output = Convert.ToString(dataReader["ranks"]);
                    }
                    dataReader.Close();
                    await connection.CloseAsync();

                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                var list = output.Split(',').ToList();
                return list;
            }
        }

        public async Task AddRanks(string steam64, string name, string ranks)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"INSERT INTO {Main.Config.DatabaseTableName} (steam64, name, ranks)
                        VALUES (@Steam64, @Name, @Ranks);", connection
                    );

                    command.Parameters.AddWithValue("@Steam64", steam64);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Ranks", ranks);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public async Task RemoveRanks(string steam64)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"DELETE FROM {Main.Config.DatabaseTableName}
                    	WHERE steam64=@Steam64;", connection
                    );

                    command.Parameters.AddWithValue("@Steam64", steam64);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public async Task UpdateRanks(string steam64, string ranks)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = new MySqlCommand
                    (
                        $@"UPDATE {Main.Config.DatabaseTableName}
                        SET `ranks` = @Ranks WHERE `steam64` = @Steam64", connection
                    );

                    command.Parameters.AddWithValue("@Steam64", steam64);
                    command.Parameters.AddWithValue("@Ranks", ranks);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
    }
}
