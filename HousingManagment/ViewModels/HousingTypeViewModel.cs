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

public class HousingTypeViewModel: ViewModelBase
{
    private static readonly string ConnectionString = DatabaseManagerConnectionString.ConnectionString;

    private AvaloniaList<HousingType> GetHousingTypesFromDb()
    {
        AvaloniaList<HousingType> housingTypes = new AvaloniaList<HousingType>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                string selectAllHousingTypes = "SELECT * FROM HousingType";
                MySqlCommand cmd = new MySqlCommand(selectAllHousingTypes, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    HousingType housingTypesItem = new HousingType();
                    if (!reader.IsDBNull(reader.GetOrdinal("ID")))
                    {
                        housingTypesItem.ID = reader.GetInt32("ID");
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                    {
                        housingTypesItem.Name = reader.GetString("Name");
                    }

                    housingTypes.Add(housingTypesItem);
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

        return housingTypes;
    }
    
    private AvaloniaList<HousingType> _housingType;

    public AvaloniaList<HousingType> HousingType
    {
        get => _housingType;
        set => this.RaiseAndSetIfChanged(ref _housingType, value);
    }

    public HousingTypeViewModel()
    {
        HousingType = GetHousingTypesFromDb();
    }

    public void OnNew(HousingType housingType) {
        HousingType.Add(housingType);
    }

    private HousingType _housingTypeSelectedItem;

    public HousingType HousingTypeSelectedItem {
        get => _housingTypeSelectedItem;
        set => this.RaiseAndSetIfChanged(ref _housingTypeSelectedItem, value);
    }
    
    public void OnDelete(HousingType housingType) {
        HousingType.Remove(housingType);
    }
    
    public void OnEdit(HousingType housingType) {
        HousingType.Replace(HousingTypeSelectedItem, housingType);
    }

    private AvaloniaList<HousingType> _housingTypesPreSearch;

    public AvaloniaList<HousingType> HousingTypesPreSearch
    {
        get => _housingTypesPreSearch;
        set => this.RaiseAndSetIfChanged(ref _housingTypesPreSearch, value);
    }

    public void AddHousingTypeToDB()
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
            OnNew(i);
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

    public void EditHousingTypeInDB()
    {
        var db = new DatabaseManagerEdit();
        int housingTypeId = HousingTypeSelectedItem.ID;
        var edit = ReactiveCommand.Create<HousingType>((i) =>
        {
            db.EditData(
                "HousingType",
                housingTypeId,
                new MySqlParameter("@Name", MySqlDbType.String)
                {
                    Value = i.Name
                }
            );
            OnEdit(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = new HousingType()
            {
                ID = HousingTypeSelectedItem.ID,
                Name = HousingTypeSelectedItem.Name
            },
            Children =
            {
                new TextBox()
                {
                    [!TextBox.TextProperty] = new Binding("Name")
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

    public void DeleteHousingTypeFromDB()
    {
        if (HousingTypeSelectedItem is null)
        {
            return;
        }

        var db = new DatabaseManagerDelete();
        int housingTypeId = HousingTypeSelectedItem.ID;
        var delete = ReactiveCommand.Create<HousingType>((i) =>
        {
            db.DeleteData(
                "HousingType",
                housingTypeId
            );
            OnDelete(i);
            InteractiveContainer.CloseDialog();
        });

        InteractiveContainer.ShowDialog(new StackPanel()
        {
            DataContext = HousingTypeSelectedItem,
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