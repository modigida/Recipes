using Microsoft.Extensions.DependencyInjection;
using Recipes.Database;
using Recipes.Services;
using Recipes.ViewModel;
using System.Windows;

namespace Recipes;
public partial class App : Application
{
    public IServiceProvider Services { get; }
    public App()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>();
        serviceCollection.AddSingleton<GetStaticListDataService>();
        serviceCollection.AddTransient<MainWindow>();

        Services = serviceCollection.BuildServiceProvider();
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new MainWindow();

        mainWindow.DataContext = new MainWindowViewModel(); 
    }
}
