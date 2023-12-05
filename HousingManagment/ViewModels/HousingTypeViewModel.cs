using System;
using System.Collections.Generic;
using Avalonia.Collections;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class HousingTypeViewModel: ViewModelBase
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public AvaloniaList<HousingType> GetHousingTypesFromDb()
    {
        AvaloniaList<HousingType> housingTypes = new AvaloniaList<HousingType>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
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
    
    public void OnEdit(HousingType housingType) {
        HousingType.Replace(HousingTypeSelectedItem, housingType);
    }

    private AvaloniaList<HousingType> _housingTypesPreSearch;

    public AvaloniaList<HousingType> HousingTypesPreSearch
    {
        get => _housingTypesPreSearch;
        set => this.RaiseAndSetIfChanged(ref _housingTypesPreSearch, value);
    }
}