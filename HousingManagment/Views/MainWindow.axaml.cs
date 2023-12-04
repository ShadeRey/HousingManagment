using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Media;
using HousingManagment.DataBaseCommands;
using HousingManagment.Models;
using HousingManagment.ViewModels;
using MySqlConnector;
using ReactiveUI;
using SukiUI.Controls;

namespace HousingManagment.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void HousingTypeAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<HousingType>((i) =>
        {
            var newId = db.InsertData(
                "HousingType",
                new MySqlParameter("@Name", MySqlDbType.String)
                {
                    Value = i.Name
                }
            );
            i.ID = newId;
            (DataContext as MainWindowViewModel)!.HousingTypeViewModel.OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new HousingType(),
            Children =
            {
                new TextBox()
                {
                    Watermark = "Name",
                    [!TextBox.TextProperty] = new Binding("Name")
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

    private void MaintenanceWorkAdd_OnClick(object? sender, RoutedEventArgs e)
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
            (DataContext as MainWindowViewModel)!.MaintenanceWorkViewModel.OnNew(i);
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
                    Watermark = "$",
                    [!TextBox.TextProperty] = new Binding("Price"),
                    Classes = { "Suffix" }
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

    private void UtilityPaymentAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerAdd();

        var add = ReactiveCommand.Create<UtilityPayment>((i) =>
        {
            var newId = db.InsertData(
                "UtilityPayment",
                new MySqlParameter("@Sum", MySqlDbType.Decimal)
                {
                    Value = i.Sum
                },
                new MySqlParameter("@PaymentDate", MySqlDbType.Date)
                {
                    Value = i.PaymentDate
                },
                new MySqlParameter("@PaymentMethod", MySqlDbType.String)
                {
                    Value = i.PaymentMethod
                }
            );
            i.ID = newId;
            (DataContext as MainWindowViewModel)!.UtilityPaymentViewModel.OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new UtilityPayment(),
            Children =
            {
                new TextBox()
                {
                    Watermark = "Sum",
                    [!TextBox.TextProperty] = new Binding("Sum")
                },
                new DatePicker()
                {
                    [!DatePicker.SelectedDateProperty] = new Binding("PaymentDate")
                },
                new TextBox()
                {
                    Watermark = "PaymentMethod",
                    [!TextBox.TextProperty] = new Binding("PaymentMethod")
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
            (DataContext as MainWindowViewModel)!.InfrastructureViewModel.OnNew(i);
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

    private void ResidentialPropertyAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerAdd();

        var residentialProperty = new ResidentialProperty();

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
        
        var types = new List<HousingType>();
        {
            using var connection = new MySqlConnection(DatabaseManagerAdd.ConnectionString);
            connection.Open();
            using var cmd = new MySqlCommand("""
                                             select * from HousingType;
                                             """, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read() && reader.HasRows)
            {
                var item = new HousingType()
                {
                    ID = reader.GetInt32("ID"),
                    Name = reader.GetString("Name")
                };
                types.Add(item);
            }
        }
        
        var payments = new List<UtilityPayment>();
        {
            using var connection = new MySqlConnection(DatabaseManagerAdd.ConnectionString);
            connection.Open();
            using var cmd = new MySqlCommand("""
                                             select * from UtilityPayment;
                                             """, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read() && reader.HasRows)
            {
                var item = new UtilityPayment()
                {
                    ID = reader.GetInt32("ID"),
                    Sum = reader.GetDecimal("Sum")
                };
                payments.Add(item);
            }
        }
        
        var add = ReactiveCommand.Create((ResidentialProperty i) =>
        {
            var newId = db.InsertData(
                "ResidentialProperty",
                new MySqlParameter("@Address", MySqlDbType.String)
                {
                    Value = i.Address
                },
                new MySqlParameter("@Square", MySqlDbType.Decimal)
                {
                    Value = i.Square
                },
                new MySqlParameter("@NumberOfRooms", MySqlDbType.Int32)
                {
                    Value = i.NumberOfRooms
                },
                new MySqlParameter("@HousingType", MySqlDbType.Int32)
                {
                    Value = i.HousingType
                },
                new MySqlParameter("@Payment", MySqlDbType.Int32)
                {
                    Value = i.Payment
                },
                new MySqlParameter("@Work", MySqlDbType.Int32)
                {
                    Value = i.Work
                }
            );
            i.ID = newId;
            residentialProperty.WorkDescription = works.FirstOrDefault(x => x.ID == i.Work).Description;
            residentialProperty.HousingName = types.FirstOrDefault(x => x.ID == i.HousingType).Name;
            residentialProperty.PaymentMethod = payments.FirstOrDefault(x => x.ID == i.Payment).Sum;
            (DataContext as MainWindowViewModel)!.ResidentialPropertyViewModel.OnNew(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = residentialProperty,
            Children =
            {
                new TextBox()
                {
                    Watermark = "Address",
                    [!TextBox.TextProperty] = new Binding("Address")
                },
                new TextBox()
                {
                    Watermark = "Square",
                    [!TextBox.TextProperty] = new Binding("Square")
                },
                new ComboBox()
                {
                    PlaceholderText = "Housing type",
                    ItemsSource = types,
                    Name = "TypeBox",
                    DisplayMemberBinding = new Binding("Name"),
                    [!ComboBox.SelectedValueProperty] = new Binding("HousingType"),
                    SelectedValueBinding = new Binding("ID")
                },
                new ComboBox()
                {
                    PlaceholderText = "Payment",
                    ItemsSource = payments,
                    Name = "PaymentBox",
                    DisplayMemberBinding = new Binding("Sum"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Payment"),
                    SelectedValueBinding = new Binding("ID")
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

    private void HousingTypeEdit_OnClick(object? sender, RoutedEventArgs e) {
        throw new NotImplementedException();
    }

    private void HousingTypeDelete_OnClick(object? sender, RoutedEventArgs e) {
        var db = new DatabaseManagerDelete();
        int housingTypeID = new HousingTypeViewModel().HousingTypeSelectedItem.ID;
        var delete = ReactiveCommand.Create<HousingType>((i) =>
        {
            db.DeleteData(
                "HousingType",
                housingTypeID
            );
            (DataContext as MainWindowViewModel)!.HousingTypeViewModel.OnDelete(i);
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new HousingType(),
            Children =
            {
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