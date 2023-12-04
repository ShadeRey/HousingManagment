using System;
using Avalonia.Collections;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class InfrastructureViewModel: ViewModelBase
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public AvaloniaList<Infrastructure> GetInfrastructuresFromDb()
    {
        AvaloniaList<Infrastructure> infrastructures = new AvaloniaList<Infrastructure>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                string selectAllInfrastructures = """
                                                  SELECT Infrastructure.ID, Type, State, Description
                                                  From Infrastructure
                                                  join MaintenanceWork on Infrastructure.Work = MaintenanceWork.ID;
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
    
    public void OnNew(Infrastructure infrastructure) {
        Infrastructure.Add(infrastructure);
    }
    
    private Infrastructure _infrastructureSelectedItem;

    public Infrastructure InfrastructureSelectedItem {
        get => _infrastructureSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _infrastructureSelectedItem, value);
    }
    
    public void OnDelete(Infrastructure infrastructure) {
        Infrastructure.Remove(infrastructure);
    }
    
    public void OnEdit(Infrastructure infrastructure) {
        Infrastructure.Replace(InfrastructureSelectedItem, infrastructure);
    }
}