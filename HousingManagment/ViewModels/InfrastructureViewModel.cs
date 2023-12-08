using System;
using System.Collections.Generic;
using System.Linq;
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

public class InfrastructureViewModel: ViewModelBase
{
    private static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    private AvaloniaList<Infrastructure> GetInfrastructuresFromDb()
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
    
    private AvaloniaList<Infrastructure> _infrastructuresPreSearch;

    public AvaloniaList<Infrastructure> InfrastructuresPreSearch
    {
        get => _infrastructuresPreSearch;
        set => this.RaiseAndSetIfChanged(ref _infrastructuresPreSearch, value);
    }

    public void AddInfrastructureToDB()
    {
        var db = new DatabaseManagerAdd();

        var infrastructure = new Infrastructure();

        var works = new List<MaintenanceWork>();
        {
            using var connection = new MySqlConnection(DatabaseManagerAdd.ConnectionString);
            connection.Open();
            using var cmd = new MySqlCommand("""
                                             select * from MaintenanceWork;
                                             """, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read() && reader.HasRows)
            {
                var item = new MaintenanceWork()
                {
                    ID = reader.GetInt32("ID"),
                    Description = reader.GetString("Description")
                };
                works.Add(item);
            }
        }
        
        var add = ReactiveCommand.Create((Infrastructure i) =>
        {
            var newId = db.InsertData(
                "Infrastructure",
                new MySqlParameter("@Type", MySqlDbType.String)
                {
                    Value = i.Type
                },
                new MySqlParameter("@State", MySqlDbType.String)
                {
                    Value = i.State
                },
                new MySqlParameter("@Work", MySqlDbType.Int32)
                {
                    Value = i.Work
                }
            );
            i.ID = newId;
            infrastructure.WorkDescription = works.FirstOrDefault(x => x.ID == i.Work).Description;
            OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = infrastructure,
            Children =
            {
                new TextBox()
                {
                    Watermark = "Type",
                    [!TextBox.TextProperty] = new Binding("Type")
                },
                new TextBox()
                {
                    Watermark = "State",
                    [!TextBox.TextProperty] = new Binding("State")
                },
                new ComboBox()
                {
                    PlaceholderText = "Maintenance work",
                    ItemsSource = works,
                    Name = "WorkBox",
                    DisplayMemberBinding = new Binding("Description"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Work"),
                    SelectedValueBinding = new Binding("ID")
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

    public void EditInfrastructureInDB()
    {
        var db = new DatabaseManagerEdit();
        int infrastructureId = InfrastructureSelectedItem.ID;
        var works = new List<MaintenanceWork>();
        {
            using var connection = new MySqlConnection(DatabaseManagerAdd.ConnectionString);
            connection.Open();
            using var cmd = new MySqlCommand("""
                                             select * from MaintenanceWork;
                                             """, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read() && reader.HasRows)
            {
                var item = new MaintenanceWork()
                {
                    ID = reader.GetInt32("ID"),
                    Description = reader.GetString("Description")
                };
                works.Add(item);
            }
        }
        var edit = ReactiveCommand.Create<Infrastructure>((i) =>
        {
            i.WorkDescription = works.FirstOrDefault(x => x.ID == i.Work).Description;
            db.EditData(
                "Infrastructure",
                infrastructureId,
                new MySqlParameter("@Type", MySqlDbType.String)
                {
                    Value = i.Type
                },
                new MySqlParameter("State", MySqlDbType.String)
                {
                    Value = i.State
                },
                new MySqlParameter("Work", MySqlDbType.Int32)
                {
                    Value = i.Work
                }
            );
            OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new Infrastructure()
            {
                ID = InfrastructureSelectedItem.ID,
                Type = InfrastructureSelectedItem.Type,
                State = InfrastructureSelectedItem.State,
                Work = InfrastructureSelectedItem.Work
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Type")
                },
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("State")
                },
                new ComboBox()
                {
                    PlaceholderText = "Maintenance work",
                    ItemsSource = works,
                    Name = "WorkBox",
                    DisplayMemberBinding = new Binding("Description"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Work"),
                    SelectedValueBinding = new Binding("ID")
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

    public void DeleteInfrastructureFromDB()
    {
        if (InfrastructureSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int infrastructureId = InfrastructureSelectedItem.ID;
        var delete = ReactiveCommand.Create<Infrastructure>((i) =>
        {
            db.DeleteData(
                "Infrastructure",
                infrastructureId
            );
            OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = InfrastructureSelectedItem,
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