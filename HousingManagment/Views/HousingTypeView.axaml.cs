using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class HousingTypeView : UserControl
{
    public HousingTypeView()
    {
        InitializeComponent();
    }

    public HousingTypeViewModel ViewModel => (DataContext as HousingTypeViewModel)!;
    
    private async void HousingTypeSearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!HousingTypeWP.IsVisible)
        {
            await StartLoadingAsync();
        }

        if (ViewModel.HousingTypesPreSearch is null)
        {
            ViewModel.HousingTypesPreSearch = ViewModel.HousingType;
        }

        if (HousingTypeSearch.Text is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(HousingTypeSearch.Text))
        {
            HousingTypeGrid.ItemsSource = ViewModel.HousingTypesPreSearch;
            return;
        }

        Filter();
    }

    private async Task StartLoadingAsync()
    {
        HousingTypeWP.Value = 0;
        HousingTypeGrid.IsVisible = false;
        HousingTypeWP.IsVisible = true;

        Random random = new Random();
        HashSet<int> generatedNumbers = new HashSet<int>();

        int totalUniqueValues = 101;

        for (int nextNumber = 0; nextNumber < 100; nextNumber = random.Next(nextNumber, totalUniqueValues))
        {
            await Task.Delay(1000);
            generatedNumbers.Add(nextNumber);
            HousingTypeWP.Value = nextNumber;
            Console.WriteLine($"Прогресс: {nextNumber}%");
        }


        HousingTypeWP.IsVisible = false;
        HousingTypeGrid.IsVisible = true;
        HousingTypeWP.Value = 0;

        Console.WriteLine("Загрузка завершена.");
    }

    private void Filter()
    {
        if (HousingTypeSearch.Text is null)
        {
            return;
        }
        else
        {
            if (HousingTypeFilter.SelectedIndex == 0)
            {
                var filtered = ViewModel.HousingTypesPreSearch.Where(
                    it => it.ID.ToString().Contains(HousingTypeSearch.Text)
                          || it.Name.ToLower().Contains(HousingTypeSearch.Text)
                ).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                HousingTypeGrid.ItemsSource = filtered;
            }
            else if (HousingTypeFilter.SelectedIndex == 1)
            {
                var filtered = ViewModel.HousingTypesPreSearch
                    .Where(it => it.ID.ToString().Contains(HousingTypeSearch.Text)).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                HousingTypeGrid.ItemsSource = filtered;
            }
            else if (HousingTypeFilter.SelectedIndex == 2)
            {
                var filtered = ViewModel.HousingTypesPreSearch
                    .Where(it => it.Name.ToLower().Contains(HousingTypeSearch.Text)).ToList();
                filtered = filtered.OrderBy(name => name.Name).ToList();
                HousingTypeGrid.ItemsSource = filtered;
            }
        }
    }

    private void HousingTypeFilter_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Filter();
}