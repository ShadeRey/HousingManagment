using System;
using Avalonia.Collections;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class InfrastructureViewModel: ViewModelBase
{
    // private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    private const string _connectionString = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public AvaloniaList<Infrastructure> GetInfrastructuresFromDb()
    {
        AvaloniaList<Infrastructure> infrastructures = new AvaloniaList<Infrastructure>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string selectAllInfrastructures = """
                                                  SELECT Infrastructure.ID, Type, State, Description
                                                  From Infrastructure
                                                  join MaintenanceWork on Infrastructure.ID = MaintenanceWork.ID;
                                                  """;
                MySqlCommand cmd = new MySqlCommand(selectAllInfrastructures, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Infrastructure infrastructuresItem = new Infrastructure();
                    if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                    {
                        infrastructuresItem.ID = reader.GetInt32("ID");
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Type")))
                    {
                        infrastructuresItem.Type = reader.GetString("Type");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("State")))
                    {
                        infrastructuresItem.State = reader.GetString("State");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                    {
                        infrastructuresItem.WorkDescription = reader.GetString("Description");
                    }

                    infrastructures.Add(infrastructuresItem);
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

        return infrastructures;
    }
    
    private AvaloniaList<Infrastructure> _infrastructure;

    public AvaloniaList<Infrastructure> Infrastructure
    {
        get => _infrastructure;
        set => this.RaiseAndSetIfChanged(ref _infrastructure, value);
    }

    public InfrastructureViewModel()
    {
        Infrastructure = GetInfrastructuresFromDb();
    }
}