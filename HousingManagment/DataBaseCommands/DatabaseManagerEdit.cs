﻿using System;
using System.Linq;
using MySqlConnector;

namespace HousingManagment.DataBaseCommands;

public class DatabaseManagerEdit
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public void EditData(string tableName, int id, params MySqlParameter[] parameters) {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using MySqlCommand command = connection.CreateCommand();
        var paramString = string.Join(',', parameters.Select(x => $"{x.ParameterName.Replace("@", "")} = {x.ParameterName}"));
        command.CommandText = $"UPDATE {tableName} SET {paramString} WHERE ID = @Id;";
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddRange(parameters);
        command.ExecuteNonQuery();
    }
}