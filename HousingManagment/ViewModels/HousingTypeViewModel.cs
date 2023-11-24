using System.Collections.Generic;
using System.Collections.ObjectModel;
using HousingManagment.Models;
using MySql.Data.MySqlClient;

namespace HousingManagment.ViewModels;

public class HousingTypeViewModel: ViewModelBase
{
    private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";

    public ObservableCollection<HousingType> GetItems()
    {
        ObservableCollection<HousingType> items = new ObservableCollection<HousingType>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM HousingType";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var housingType = new HousingType()
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        items.Add(housingType);
                    }
                }
            }
        }

        return items;
    }
    private ObservableCollection<HousingType> _housingTypes;

    public ObservableCollection<HousingType> HousingTypes
    {
        get => _housingTypes;
        set => SetField(ref _housingTypes, value);
    }

    public HousingTypeViewModel()
    {
        HousingTypes = new ObservableCollection<HousingType>();
        HousingTypes = GetItems();
    }
}