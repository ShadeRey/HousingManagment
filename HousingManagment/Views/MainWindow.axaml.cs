using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia.Controls;
using HousingManagment.ViewModels;

namespace HousingManagment.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    public MainWindowViewModel ViewModel => (DataContext as MainWindowViewModel)!;
}