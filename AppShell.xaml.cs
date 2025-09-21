using MoMoney.Services;
using MoMoney.Views;

namespace MoMoney
{
    /// <summary>
    /// Defines the navigation structure and routes for the application.
    /// Handles shell navigation events and logging.
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppShell"/> class.
        /// Configures routes and attaches navigation event handlers.
        /// </summary>
        public AppShell()
        {
            InitializeComponent();

            Navigating += OnShellNavigating;

            // Register non-tab routes here
            Routing.RegisterRoute(nameof(AddBudgetPage), typeof(AddBudgetPage));
            Routing.RegisterRoute(nameof(AddTransactionPage), typeof(AddTransactionPage));
            Routing.RegisterRoute(nameof(BudgetsPage), typeof(BudgetsPage));
            Routing.RegisterRoute(nameof(EditBudgetPage), typeof(EditBudgetPage));
            Routing.RegisterRoute(nameof(EditTransactionPage), typeof(EditTransactionPage));
            Routing.RegisterRoute(nameof(ReportsPage), typeof(ReportsPage));
            Routing.RegisterRoute(nameof(TransactionsPage), typeof(TransactionsPage));
        }

        /// <summary>
        /// Handles navigation events within the shell.
        /// Logs navigation details including target, source, and current locations.
        /// </summary>
        /// <param name="sender">The sender of the event, typically the shell.</param>
        /// <param name="e">Event arguments containing navigation details.</param>
        private void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var target = e.Target?.Location?.ToString() ?? "(unknown target)";
            var current = e.Current?.Location?.ToString() ?? "(none)";
            var source = e.Source.ToString();

            LoggerService.LogInfoWithCategory("ShellNavigation",
                $"[Navigating] To: {target}, From: {current}, Source: {source}");
        }
    }
}
