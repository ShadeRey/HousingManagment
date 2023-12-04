using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using HousingManagment.ViewModels;
using MySqlConnector;
using ReactiveUI;
using SukiUI.Controls;

namespace HousingManagment.Views;

public partial class InfrastructureView : UserControl
{
    public InfrastructureView()
    {
        InitializeComponent();
    }
    
    public InfrastructureViewModel ViewModel => (DataContext as InfrastructureViewModel)!;
    
    private void InfrastructureAdd_OnClick(object? sender, RoutedEventArgs e)
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
            ViewModel.OnNew(i);
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
    
    private void InfrastructureEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerEdit();
        int infrastructureId = ViewModel.InfrastructureSelectedItem.ID;
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
            ViewModel.OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new Infrastructure()
            {
                ID = ViewModel.InfrastructureSelectedItem.ID,
                Type = ViewModel.InfrastructureSelectedItem.Type,
                State = ViewModel.InfrastructureSelectedItem.State,
                Work = ViewModel.InfrastructureSelectedItem.Work
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

    private void InfrastructureDelete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.InfrastructureSelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int infrastructureId = ViewModel.InfrastructureSelectedItem.ID;
        var delete = ReactiveCommand.Create<Infrastructure>((i) =>
        {
            db.DeleteData(
                "Infrastructure",
                infrastructureId
            );
            ViewModel.OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = ViewModel.InfrastructureSelectedItem,
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