namespace MoMoney
{
    /// <summary>
    /// Represents the main application entry point.
    /// Configures and initializes the application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Sets the <see cref="MainPage"/> to <see cref="AppShell"/>.
        /// </summary>
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
