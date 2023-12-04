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

public partial class ResidentialPropertyView : UserControl
{
    public ResidentialPropertyView()
    {
        InitializeComponent();
    }
    
    public ResidentialPropertyViewModel ViewModel => (DataContext as ResidentialPropertyViewModel)!;
    
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
            ViewModel.OnNew(i);
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
                    Watermark = "Square(m²):",
                    [!TextBox.TextProperty] = new Binding("Square"),
                    Classes = { "Prefix" }
                },
                new TextBox()
                {
                    Watermark = "Rooms:",
                    [!TextBox.TextProperty] = new Binding("NumberOfRooms"),
                    Classes = { "Prefix" }
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

    

    private void ResidentialPropertyEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        var db = new DatabaseManagerEdit();
        int residentialPropertyId = ViewModel.ResidentialPropertySelectedItem.ID;
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
        
        var edit = ReactiveCommand.Create<ResidentialProperty>((i) =>
        {
            i.WorkDescription = works.FirstOrDefault(x => x.ID == i.Work).Description;
            i.HousingName = types.FirstOrDefault(x => x.ID == i.HousingType).Name;
            i.PaymentMethod = payments.FirstOrDefault(x => x.ID == i.Payment).Sum;
            db.EditData(
                "ResidentialProperty",
                residentialPropertyId,
                new MySqlParameter("@Address", MySqlDbType.String)
                {
                    Value = i.Address
                },
                new MySqlParameter("Square", MySqlDbType.Decimal)
                {
                    Value = i.Square
                },
                new MySqlParameter("NumberOfRooms", MySqlDbType.Int32)
                {
                    Value = i.NumberOfRooms
                },
                new MySqlParameter("HousingType", MySqlDbType.Int32)
                {
                    Value = i.HousingType
                },
                new MySqlParameter("Payment", MySqlDbType.Int32)
                {
                    Value = i.Payment
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
            DataContext = new ResidentialProperty()
            {
                ID = ViewModel.ResidentialPropertySelectedItem.ID,
                Address = ViewModel.ResidentialPropertySelectedItem.Address,
                Square = ViewModel.ResidentialPropertySelectedItem.Square,
                NumberOfRooms = ViewModel.ResidentialPropertySelectedItem.NumberOfRooms,
                HousingType = ViewModel.ResidentialPropertySelectedItem.HousingType,
                Payment = ViewModel.ResidentialPropertySelectedItem.Payment,
                Work = ViewModel.ResidentialPropertySelectedItem.Work
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Address")
                },
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Square")
                },
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("NumberOfRooms")
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

    private void ResidentialPropertyDelete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.ResidentialPropertySelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int residentialPropertyId = ViewModel.ResidentialPropertySelectedItem.ID;
        var delete = ReactiveCommand.Create<ResidentialProperty>((i) =>
        {
            db.DeleteData(
                "ResidentialProperty",
                residentialPropertyId
            );
            ViewModel.OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = ViewModel.ResidentialPropertySelectedItem,
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