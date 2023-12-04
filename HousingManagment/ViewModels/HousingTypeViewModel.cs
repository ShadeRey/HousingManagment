using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Collections;
using Avalonia.Logging;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class HousingTypeViewModel: ViewModelBase
{
    private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    // private const string _connectionString = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public AvaloniaList<HousingType> GetHousingTypesFromDb()
    {
        AvaloniaList<HousingType> housingTypes = new AvaloniaList<HousingType>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string selectAllHousingTypes = "SELECT * FROM HousingType";
                MySqlCommand cmd = new MySqlCommand(selectAllHousingTypes, connection);
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
        set => this.RaiseAndSetIfChanged(ref _housingType, value);
    }

    public HousingTypeViewModel()
    {
        HousingType = GetHousingTypesFromDb();
    }

    public void OnNew(HousingType housingType) {
        HousingType.Add(housingType);
    }

    private HousingType _housingTypeSelectedItem;

    public HousingType HousingTypeSelectedItem {
        get => _housingTypeSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _housingTypeSelectedItem, value);
    }
    
    public void OnDelete(HousingType housingType) {
        HousingType.Remove(housingType);
    }
}