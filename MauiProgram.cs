using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MoMoney.Services;
using MoMoney.ViewModels;
using MoMoney.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Serilog;

namespace MoMoney
{
    /// <summary>
    /// Provides application configuration and startup logic for the MoMoney MAUI application.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the <see cref="MauiApp"/> instance,
        /// including logging, dependency injection, services, and pages.
        /// </summary>
        /// <returns>The fully configured <see cref="MauiApp"/> instance.</returns>
        public static MauiApp CreateMauiApp()
        {
            /// <summary>
            /// Configures Serilog for file-based logging with the following behavior:
            /// - Logs are written to <c>momoney-.log</c> in the app's data directory.
            /// - A new log file is created daily (<see cref="RollingInterval.Day"/>).
            /// - Up to 7 log files are retained (<c>retainedFileCountLimit = 7</c>).
            /// - Log entries are flushed to disk every 2 seconds for reliability.
            /// - Log level is set to <c>Debug</c>, capturing all messages at Debug and above.
            /// </summary>
            var logPath = Path.Combine(FileSystem.AppDataDirectory, "momoney-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // capture everything from Debug and up
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,   // new file per day
                    retainedFileCountLimit: 7,              // keep 7 days of logs
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(2))
                .CreateLogger();


            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configure logging
            builder.Logging.ClearProviders();   // remove default providers
            // ILogger is Singleton by nature, so it does not need to be registered
            builder.Logging.AddSerilog();       // plug Serilog into ILogger<T>

#if DEBUG
            builder.Logging.AddDebug(); // logging to VS Output window
#endif

            // Register services
            builder.Services.AddSingleton<IDataService>(
                s => new SQLiteDataService(Path.Combine(FileSystem.AppDataDirectory, "momoney.db")));

            // Register main viewmodel as singleton
            builder.Services.AddSingleton<MainViewModel>();

            // Register viewmodels
            builder.Services.AddTransient<BudgetViewModel>();
            builder.Services.AddTransient<BudgetItemViewModel>();
            builder.Services.AddTransient<EditBudgetViewModel>();
            builder.Services.AddTransient<EditTransactionViewModel>();
            builder.Services.AddTransient<ReportsViewModel>();
            builder.Services.AddTransient<TransactionViewModel>();

            // Register pages
            builder.Services.AddTransient<AddBudgetPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<BudgetsPage>();
            builder.Services.AddTransient<EditBudgetPage>();
            builder.Services.AddTransient<EditTransactionPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ReportsPage>();
            builder.Services.AddTransient<TransactionsPage>();

            // Build app.
            var app = builder.Build();

            // Initialize LoggerService with the app's ILoggerFactory.
            LoggerService.Init(app.Services.GetRequiredService<ILoggerFactory>());

            ServiceHelper.Current = app.Services;

            // Logging
            LoggerService.LogInfo("App started", "Startup");
            LoggerService.LogInfo($"Database path: {Path.Combine(FileSystem.AppDataDirectory, "momoney.db")}", "Startup");
            LoggerService.LogInfo("All services registered successfully", "Startup");

            return app;
        }
    }
}
