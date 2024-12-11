using Recipes.Database;
using Recipes.ViewModel;
using System.Windows;

namespace Recipes;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

    }
}
