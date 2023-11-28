using System;
using Avalonia.Collections;
using Avalonia.Logging;
using HousingManagment.Models;
using MySqlConnector;

namespace HousingManagment.ViewModels;

public class MaintenanceWorkViewModel: ViewModelBase
{
    private const string _connectionString = "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_23;";
    //private const string _connectionString = "Server=localhost;Database=UP;User Id=root;Password=sharaga228;";

    public AvaloniaList<MaintenanceWork> GetMaintenanceWorksFromDb()
    {
        AvaloniaList<MaintenanceWork> maintenanceWorks = new AvaloniaList<MaintenanceWork>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
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
        set => SetField(ref _maintenanceWork, value);
    }

    public MaintenanceWorkViewModel()
    {
        MaintenanceWork = GetMaintenanceWorksFromDb();
    }
}