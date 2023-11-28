using System;
using Avalonia.Collections;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class ResidentialPropertyViewModel: ViewModelBase
{
    private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    //private const string _connectionString = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public AvaloniaList<ResidentialProperty> GetResidentialPropertiesFromDb()
    {
        AvaloniaList<ResidentialProperty> residentialProperties = new AvaloniaList<ResidentialProperty>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string selectAllResidentialProperties = "SELECT * FROM ResidentialProperty";
                MySqlCommand cmd = new MySqlCommand(selectAllResidentialProperties, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ResidentialProperty residentialPropertiesItem = new ResidentialProperty();
                    if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                    {
                        residentialPropertiesItem.ID = reader.GetInt32("ID");
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Address")))
                    {
                        residentialPropertiesItem.Address = reader.GetString("Address");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Square")))
                    {
                        residentialPropertiesItem.Square = reader.GetDecimal("Square");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("NumberOfRooms")))
                    {
                        residentialPropertiesItem.NumberOfRooms = reader.GetInt32("NumberOfRooms");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("HousingType")))
                    {
                        residentialPropertiesItem.HousingType = reader.GetInt32("HousingType");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Payment")))
                    {
                        residentialPropertiesItem.Payment = reader.GetInt32("Payment");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Work")))
                    {
                        residentialPropertiesItem.Work = reader.GetInt32("Work");
                    }

                    residentialProperties.Add(residentialPropertiesItem);
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

        return residentialProperties;
    }
    
    private AvaloniaList<ResidentialProperty> _residentialProperty;

    public AvaloniaList<ResidentialProperty> ResidentialProperty
    {
        get => _residentialProperty;
        set => this.RaiseAndSetIfChanged(ref _residentialProperty, value);
    }

    public ResidentialPropertyViewModel()
    {
        ResidentialProperty = GetResidentialPropertiesFromDb();
    }
}