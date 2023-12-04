using System;
using System.Linq;
using MySqlConnector;

namespace HousingManagment.DataBaseCommands;

public class DatabaseManagerDelete
{
    public static readonly string ConnectionString
        = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    // = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public void DeleteData(string tableName, int id) {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {tableName} WHERE ID = @Id;";
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }
}