using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicData;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using MySqlConnector;
using ReactiveUI;
using SukiUI.Controls;

namespace HousingManagment.ViewModels;

public class MaintenanceWorkViewModel: ViewModelBase
{
    private static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    private AvaloniaList<MaintenanceWork> GetMaintenanceWorksFromDb()
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
    
    private AvaloniaList<MaintenanceWork> _maintenanceWorksPreSearch;

    public AvaloniaList<MaintenanceWork> MaintenanceWorksPreSearch
    {
        get => _maintenanceWorksPreSearch;
        set => this.RaiseAndSetIfChanged(ref _maintenanceWorksPreSearch, value);
    }

    public void AddMaintenanceWorkToDB()
    {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<MaintenanceWork>((i) =>
        {
            var newId = db.InsertData(
                "MaintenanceWork",
                new MySqlParameter("@Description", MySqlDbType.String)
                {
                    Value = i.Description
                },
                new MySqlParameter("@StartDate", MySqlDbType.Date)
                {
                    Value = i.StartDate
                },
                new MySqlParameter("@EndDate", MySqlDbType.Date)
                {
                    Value = i.EndDate
                },
                new MySqlParameter("@Price", MySqlDbType.Decimal)
                {
                    Value = i.Price
                }
            );
            i.ID = newId;
            OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new MaintenanceWork(),
            Children =
            {
                new TextBox()
                {
                    Watermark = "Description",
                    [!TextBox.TextProperty] = new Binding("Description")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("StartDate"),
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("EndDate"),
                },
                new TextBox()
                {
                    Watermark = "Price($):",
                    [!TextBox.TextProperty] = new Binding("Price"),
                    Classes = { "Prefix" }
                },
                new Button()
                {
                    Content = "Добавить",
                    Classes = { "Primary" },
                    Command = add,
                    Foreground = Brushes.White,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button()
                {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }

    public void EditMaintenanceWorkInDB()
    {
        var db = new DatabaseManagerEdit();
        int maintenanceWorkId = MaintenanceWorkSelectedItem.ID;
        var edit = ReactiveCommand.Create<MaintenanceWork>((i) =>
        {
            db.EditData(
                "MaintenanceWork",
                maintenanceWorkId,
                new MySqlParameter("@Description", MySqlDbType.String)
                {
                    Value = i.Description
                },
                new MySqlParameter("StartDate", MySqlDbType.DateTime)
                {
                    Value = i.StartDate
                },
                new MySqlParameter("EndDate", MySqlDbType.DateTime)
                {
                    Value = i.EndDate
                },
                new MySqlParameter("Price", MySqlDbType.Decimal)
                {
                    Value = i.Price
                }
            );
            OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new MaintenanceWork()
            {
                ID = MaintenanceWorkSelectedItem.ID,
                Description = MaintenanceWorkSelectedItem.Description,
                StartDate = MaintenanceWorkSelectedItem.StartDate,
                EndDate = MaintenanceWorkSelectedItem.EndDate,
                Price = MaintenanceWorkSelectedItem.Price
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Description")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("StartDate")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("EndDate")
                },
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Price")
                },
                new Button()
                {
                    Content = "Обновить",
                    Classes = { "Primary" },
                    Command = edit,
                    Foreground = Brushes.White,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button()
                {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }

    public void DeleteMaintenanceWorkFromDB()
    {
        if (MaintenanceWorkSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int maintenanceWorkId = MaintenanceWorkSelectedItem.ID;
        var delete = ReactiveCommand.Create<MaintenanceWork>((i) =>
        {
            db.DeleteData(
                "MaintenanceWork",
                maintenanceWorkId
            );
            OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = MaintenanceWorkSelectedItem,
            Children =
            {
                new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Classes = { "h2" },
                    Text = "Удалить?"
                },
                new Button()
                {
                    Content = "Да",
                    Classes = { "Primary" },
                    Command = delete,
                    Foreground = Brushes.White,
                    [!Button.CommandParameterProperty] = new Binding(".")
                },
                new Button()
                {
                    Content = "Закрыть",
                    Command = ReactiveCommand.Create(InteractiveContainer.CloseDialog)
                }
            }
        });
    }
}