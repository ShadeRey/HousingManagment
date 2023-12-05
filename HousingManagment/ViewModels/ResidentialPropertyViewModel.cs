using System;
using Avalonia.Collections;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;
using Splat;

namespace HousingManagment.ViewModels;

public class ResidentialPropertyViewModel: ViewModelBase, IEnableLogger
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public AvaloniaList<ResidentialProperty> GetResidentialPropertiesFromDb()
    {
        AvaloniaList<ResidentialProperty> residentialProperties = new AvaloniaList<ResidentialProperty>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                string selectAllResidentialProperties = """
                                                        SELECT ResidentialProperty.ID, Address, Square, NumberOfRooms, Name, Sum, Description
                                                        From ResidentialProperty
                                                            join MaintenanceWork on ResidentialProperty.Work = MaintenanceWork.ID
                                                            join HousingType on ResidentialProperty.HousingType = HousingType.ID
                                                            join UtilityPayment on ResidentialProperty.Payment = UtilityPayment.ID;
                                                        """;
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
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                    {
                        residentialPropertiesItem.HousingName = reader.GetString("Name");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Sum")))
                    {
                        residentialPropertiesItem.PaymentMethod = reader.GetDecimal("Sum");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                    {
                        residentialPropertiesItem.WorkDescription = reader.GetString("Description");
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
    
    public void OnNew(ResidentialProperty residentialProperty) {
        ResidentialProperty.Add(residentialProperty);
    }
    
    private ResidentialProperty _residentialPropertySelectedItem;

    public ResidentialProperty ResidentialPropertySelectedItem {
        get => _residentialPropertySelectedItem;
        set => this.RaiseAndSetIfChanged(ref _residentialPropertySelectedItem, value);
    }
    
    public void OnDelete(ResidentialProperty residentialProperty) {
        ResidentialProperty.Remove(residentialProperty);
    }
    
    public void OnEdit(ResidentialProperty residentialProperty) {
        ResidentialProperty.Replace(ResidentialPropertySelectedItem, residentialProperty);
    }
    
    private AvaloniaList<ResidentialProperty> _residentialPropertiesPreSearch;

    public AvaloniaList<ResidentialProperty> ResidentialPropertiesPreSearch
    {
        get => _residentialPropertiesPreSearch;
        set => this.RaiseAndSetIfChanged(ref _residentialPropertiesPreSearch, value);
    }
}