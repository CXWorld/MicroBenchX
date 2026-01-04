using System.Windows;
using MicroBenchX.UI.ViewModels;

namespace MicroBenchX.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
