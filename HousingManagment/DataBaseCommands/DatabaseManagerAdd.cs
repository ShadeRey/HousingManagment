using System;
using System.Linq;
using MySqlConnector;

namespace HousingManagment.DataBaseCommands; 

public class DatabaseManagerAdd {
    public static readonly string ConnectionString
        = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
        // = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public int InsertData(string tableName, params MySqlParameter[] parameters) {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using MySqlCommand command = connection.CreateCommand();
        var paramString = string.Join(',', parameters.Select(x => x.ParameterName));
        var columnsString = string.Join(',', parameters.Select(x => x.ParameterName.Replace("@", "")));
        command.CommandText = $"INSERT INTO {tableName}({columnsString}) VALUES ({paramString}); SELECT LAST_INSERT_ID();";
        command.Parameters.AddRange(parameters);
        return Convert.ToInt32(command.ExecuteScalar());
    }
}