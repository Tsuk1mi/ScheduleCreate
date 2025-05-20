using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScheduleCreate.Data;
using ScheduleCreate.Services;

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

        private void ConfigureServices(ServiceCollection services)
        {
            // Регистрация контекста базы данных
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ScheduleCreate;Trusted_Connection=True;"));

            // Регистрация сервисов
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ITeacherService, TeacherService>();

            // Регистрация окон
            services.AddTransient<Views.MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = serviceProvider.GetService<Views.MainWindow>();
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
        }
    }
} 