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
        //var mainWindow = Services.GetRequiredService<MainWindow>();
        //mainWindow.Show();
        base.OnStartup(e);

        var mainWindow = new MainWindow();

        mainWindow.DataContext = new RecipeViewModel(new RecipeService(new AppDbContext()), new MainWindowViewModel());
        // mainWindow.Show();

    }
}
