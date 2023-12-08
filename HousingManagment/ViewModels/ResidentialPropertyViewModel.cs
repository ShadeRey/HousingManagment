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
using Splat;
using SukiUI.Controls;

namespace HousingManagment.ViewModels;

public class ResidentialPropertyViewModel: ViewModelBase, IEnableLogger
{
    private static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    private AvaloniaList<ResidentialProperty> GetResidentialPropertiesFromDb()
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

    public void AddResidentialPropertyToDB()
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
            OnNew(i);
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

    public void EditResidentialPropertyInDB()
    {
        var db = new DatabaseManagerEdit();
        int residentialPropertyId = ResidentialPropertySelectedItem.ID;
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
            OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new ResidentialProperty()
            {
                ID = ResidentialPropertySelectedItem.ID,
                Address = ResidentialPropertySelectedItem.Address,
                Square = ResidentialPropertySelectedItem.Square,
                NumberOfRooms = ResidentialPropertySelectedItem.NumberOfRooms,
                HousingType = ResidentialPropertySelectedItem.HousingType,
                Payment = ResidentialPropertySelectedItem.Payment,
                Work = ResidentialPropertySelectedItem.Work
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

    public void DeleteResidentialPropertyFromDB()
    {
        if (ResidentialPropertySelectedItem is null)
        {
            return;
        }
        var db = new DatabaseManagerDelete();
        int residentialPropertyId = ResidentialPropertySelectedItem.ID;
        var delete = ReactiveCommand.Create<ResidentialProperty>((i) =>
        {
            db.DeleteData(
                "ResidentialProperty",
                residentialPropertyId
            );
            OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = ResidentialPropertySelectedItem,
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