using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class ResidentialPropertyView : UserControl
{
    public ResidentialPropertyView()
    {
        InitializeComponent();
    }
    
    public ResidentialPropertyViewModel ViewModel => (DataContext as ResidentialPropertyViewModel)!;

    private async void ResidentialPropertySearch_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!ResidentialPropertyWP.IsVisible)
        {
            await StartLoadingAsync();
        }
        
        if (ViewModel.ResidentialPropertiesPreSearch is null)
        {
            ViewModel.ResidentialPropertiesPreSearch = ViewModel.ResidentialProperty;
        }

        if (ResidentialPropertySearch.Text is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ResidentialPropertySearch.Text))
        {
            ResidentialPropertyGrid.ItemsSource = ViewModel.ResidentialPropertiesPreSearch;
            return;
        }

        Filter();
    }

    private async Task StartLoadingAsync()
    {
        ResidentialPropertyWP.Value = 0;
        ResidentialPropertyGrid.IsVisible = false;
        ResidentialPropertyWP.IsVisible = true;

        Random random = new Random();
        HashSet<int> generatedNumbers = new HashSet<int>();

        int totalUniqueValues = 101;

        for (int nextNumber = 0; nextNumber < 100; nextNumber = random.Next(nextNumber, totalUniqueValues))
        {
            await Task.Delay(1000);
            generatedNumbers.Add(nextNumber);
            ResidentialPropertyWP.Value = nextNumber;
            Console.WriteLine($"Прогресс: {nextNumber}%");
        }
    }

    private void Filter()
    {
        if (ResidentialPropertySearch.Text is null)
        {
            return;
        }
        else
        {
            if (ResidentalPropertyFilter.SelectedIndex == 0)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch.Where(
                    it => it.ID.ToString().Contains(ResidentialPropertySearch.Text)
                          || it.Address.ToLower().Contains(ResidentialPropertySearch.Text)
                          || it.Square.ToString().Contains(ResidentialPropertySearch.Text)
                          || it.NumberOfRooms.ToString().Contains(ResidentialPropertySearch.Text)
                          || it.HousingName.ToLower().Contains(ResidentialPropertySearch.Text)
                          || it.PaymentMethod.ToString().Contains(ResidentialPropertySearch.Text)
                          || it.WorkDescription.ToLower().Contains(ResidentialPropertySearch.Text)
                ).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 1)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.ID.ToString().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(id => id.ID).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 2)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.Address.ToLower().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(address => address.Address).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 3)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.Square.ToString().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(square => square.Square).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 4)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.NumberOfRooms.ToString().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(numberofrooms => numberofrooms.NumberOfRooms).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 5)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.HousingName.ToLower().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(housingname => housingname.HousingName).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 6)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.PaymentMethod.ToString().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(paymentmethod => paymentmethod.PaymentMethod).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
            else if (ResidentalPropertyFilter.SelectedIndex == 7)
            {
                var filtered = ViewModel.ResidentialPropertiesPreSearch
                    .Where(it => it.WorkDescription.ToLower().Contains(ResidentialPropertySearch.Text)).ToList();
                filtered = filtered.OrderBy(workdescription => workdescription.WorkDescription).ToList();
                ResidentialPropertyGrid.ItemsSource = filtered;
            }
        }
    }

    private void ResidentalPropertyFilter_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Filter();
}