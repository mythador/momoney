using MoMoney.ViewModels;

namespace MoMoney
{
    /// <summary>
    /// Represents the main landing page of the application.
    /// Provides bindings to the <see cref="MainViewModel"/>.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        /// <param name="vm">The view model associated with this page.</param>
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
