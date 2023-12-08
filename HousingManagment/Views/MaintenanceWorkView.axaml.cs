using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public partial class MaintenanceWorkView : UserControl
{
    public MaintenanceWorkView()
    {
        InitializeComponent();
    }
    
    public MaintenanceWorkViewModel ViewModel => (DataContext as MaintenanceWorkViewModel)!;

    private async void MaintenanceWorkSearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!MaintenanceWorkWP.IsVisible)
        {
            await StartLoadingAsync();
        }
        if (ViewModel.MaintenanceWorksPreSearch is null)
        {
            ViewModel.MaintenanceWorksPreSearch = ViewModel.MaintenanceWork;
        }

        if (MaintenanceWorkSearch.Text is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(MaintenanceWorkSearch.Text))
        {
            MaintenanceWorkGrid.ItemsSource = ViewModel.MaintenanceWorksPreSearch;
            return;
        }

        Filter();
    }

    private void Filter()
    {
        if (MaintenanceWorkSearch.Text is null)
        {
            return;
        }
        else
        {
            if (MaintenanceWorkFilter.SelectedIndex == 0)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch.Where(
                    it => it.ID.ToString().Contains(MaintenanceWorkSearch.Text)
                          || it.Description.ToLower().Contains(MaintenanceWorkSearch.Text)
                          || it.StartDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)
                          || it.EndDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)
                          || it.Price.ToString().Contains(MaintenanceWorkSearch.Text)
                ).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 1)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.ID.ToString().Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 2)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.Description.ToLower().Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(description => description.Description).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 3)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.StartDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(startdate => startdate.StartDate).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 4)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.EndDate.ToString("dd.MM.yyyy").Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(enddate => enddate.EndDate).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
            else if (MaintenanceWorkFilter.SelectedIndex == 5)
            {
                var filtered = ViewModel.MaintenanceWorksPreSearch
                    .Where(it => it.Price.ToString().Contains(MaintenanceWorkSearch.Text)).ToList();
                filtered = filtered.OrderBy(price => price.Price).ToList();
                MaintenanceWorkGrid.ItemsSource = filtered;
            }
        }
    }
    
    private async Task StartLoadingAsync()
    {
        MaintenanceWorkWP.Value = 0;
        MaintenanceWorkGrid.IsVisible = false;
        MaintenanceWorkWP.IsVisible = true;

        Random random = new Random();
        HashSet<int> generatedNumbers = new HashSet<int>();

        int totalUniqueValues = 101;

        for (int nextNumber = 0; nextNumber < 100; nextNumber = random.Next(nextNumber, totalUniqueValues))
        {
            await Task.Delay(1000);
            generatedNumbers.Add(nextNumber);
            MaintenanceWorkWP.Value = nextNumber;
            Console.WriteLine($"Прогресс: {nextNumber}%");
        }


        MaintenanceWorkWP.IsVisible = false;
        MaintenanceWorkGrid.IsVisible = true;
        MaintenanceWorkWP.Value = 0;

        Console.WriteLine("Загрузка завершена.");
    }

    private void MaintenanceWorkFilter_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Filter();
}