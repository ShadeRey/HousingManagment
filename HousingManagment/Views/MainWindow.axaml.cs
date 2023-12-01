using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
}