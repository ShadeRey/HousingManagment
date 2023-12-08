using System;
using MySqlConnector;

namespace HousingManagment.DataBaseCommands;

public class DatabaseManagerDelete
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public void DeleteData(string tableName, int id) {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        try
        {
            connection.Open();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM {tableName} WHERE ID = @Id;";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка удаления:" + e.Message);
        }
        finally
        {
            connection.Close();
        }
        
    }
}