using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Collections;
using Avalonia.Logging;
using HousingManagment.Models;
using MySqlConnector;

namespace HousingManagment.ViewModels;

public class HousingTypeViewModel: ViewModelBase
{
    private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    //private const string _connectionString = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public AvaloniaList<HousingType> GetHousingTypesFromDb()
    {
        AvaloniaList<HousingType> housingTypes = new AvaloniaList<HousingType>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string selectAllHousingtypes = "SELECT * FROM HousingType";
                MySqlCommand cmd = new MySqlCommand(selectAllHousingtypes, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    HousingType housingTypesItem = new HousingType();
                    if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                    {
                        housingTypesItem.ID = reader.GetInt32("ID");
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                    {
                        housingTypesItem.Name = reader.GetString("Name");
                    }

                    housingTypes.Add(housingTypesItem);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Ошибка подключения к БД: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        return housingTypes;
    }
    
    private AvaloniaList<HousingType> _housingType;

    public AvaloniaList<HousingType> HousingType
    {
        get => _housingType;
        set => SetField(ref _housingType, value);
    }

    public HousingTypeViewModel()
    {
        HousingType = GetHousingTypesFromDb();
    }
}