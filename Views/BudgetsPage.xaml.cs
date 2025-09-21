using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page that displays and manages budgets.
    /// Provides UI bindings to the <see cref="BudgetViewModel"/>.
    /// </summary>
    public partial class BudgetsPage : ContentPage
    {
        private readonly BudgetViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetsPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for managing budgets and related logic.</param>
        public BudgetsPage(BudgetViewModel vm)
        {
            InitializeComponent();
            BindingContext = _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears. Refreshes budgets to ensure the latest data is loaded.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Always refresh budgets on page load
            await _viewModel.LoadBudgetsAsync();
        }
    }
}
