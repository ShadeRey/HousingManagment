using System;
using Avalonia.Collections;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;

namespace HousingManagment.ViewModels;

public class MaintenanceWorkViewModel: ViewModelBase
{
    public static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    public AvaloniaList<MaintenanceWork> GetMaintenanceWorksFromDb()
    {
        AvaloniaList<MaintenanceWork> maintenanceWorks = new AvaloniaList<MaintenanceWork>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                string selectAllMaintenanceworks = "SELECT * FROM MaintenanceWork";
                MySqlCommand cmd = new MySqlCommand(selectAllMaintenanceworks, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MaintenanceWork maintenanceWorksItem = new MaintenanceWork();
                    if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                    {
                        maintenanceWorksItem.ID = reader.GetInt32("ID");
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                    {
                        maintenanceWorksItem.Description = reader.GetString("Description");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("StartDate")))
                    {
                        maintenanceWorksItem.StartDate = reader.GetDateTimeOffset("StartDate");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("EndDate")))
                    {
                        maintenanceWorksItem.EndDate = reader.GetDateTimeOffset("EndDate");
                    }
                    
                    if (!reader.IsDBNull(reader.GetOrdinal("Price")))
                    {
                        maintenanceWorksItem.Price = reader.GetDecimal("Price");
                    }

                    maintenanceWorks.Add(maintenanceWorksItem);
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

        return maintenanceWorks;
    }
    
    private AvaloniaList<MaintenanceWork> _maintenanceWork;

    public AvaloniaList<MaintenanceWork> MaintenanceWork
    {
        get => _maintenanceWork;
        set => this.RaiseAndSetIfChanged(ref _maintenanceWork, value);
    }

    public MaintenanceWorkViewModel()
    {
        MaintenanceWork = GetMaintenanceWorksFromDb();
    }
    public void OnNew(MaintenanceWork maintenanceWork) {
        MaintenanceWork.Add(maintenanceWork);
    }
    
    private MaintenanceWork _maintenanceWorkSelectedItem;

    public MaintenanceWork MaintenanceWorkSelectedItem {
        get => _maintenanceWorkSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _maintenanceWorkSelectedItem, value);
    }
    
    public void OnDelete(MaintenanceWork maintenanceWork) {
        MaintenanceWork.Remove(maintenanceWork);
    }
    
    public void OnEdit(MaintenanceWork maintenanceWork) {
        MaintenanceWork.Replace(MaintenanceWorkSelectedItem, maintenanceWork);
    }
}