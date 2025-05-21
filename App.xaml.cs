using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScheduleCreate.Data;
using ScheduleCreate.Services;
using ScheduleCreate.Views;
using ScheduleCreate.ViewModels;
using System.Threading.Tasks;

namespace ScheduleCreate
{
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Регистрация DbContext с использованием SQLite
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=ScheduleCreate.db")
            );

            // Регистрация сервиса инициализации БД
            services.AddTransient<DbInitializerService>();

            // Регистрация сервисов
            services.AddTransient<ITeacherService, TeacherService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IAuditoriumService, AuditoriumService>();
            services.AddTransient<IScheduleService, ScheduleService>();
            services.AddTransient<IScheduleExportService, ScheduleExportService>();
            services.AddTransient<IStatisticsService, StatisticsService>();

            // Регистрация ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализируем базу данных
            using (var scope = serviceProvider.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializerService>();
                await dbInitializer.InitializeAsync();
            }

            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
