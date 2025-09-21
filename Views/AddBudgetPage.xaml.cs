using MoMoney.ViewModels;

namespace MoMoney.Views
{
    /// <summary>
    /// Represents the page for adding a new budget.
    /// Provides UI bindings to the <see cref="BudgetViewModel"/>.
    /// </summary>
    public partial class AddBudgetPage : ContentPage
    {
        private readonly BudgetViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddBudgetPage"/> class.
        /// </summary>
        /// <param name="vm">The view model used for managing budget data and logic.</param>
        public AddBudgetPage(BudgetViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        /// <summary>
        /// Called when the page appears.
        /// Ensures that categories are loaded into the view model before user interaction.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCategoriesAsync();
        }
    }
}

