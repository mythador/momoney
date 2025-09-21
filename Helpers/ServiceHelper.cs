namespace MoMoney
{
    /// <summary>
    /// Provides helper methods for retrieving registered services
    /// from the application's service provider.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// Gets a service of the specified type from the current service provider.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <see cref="Current"/> service provider has not been initialized.
        /// </exception>
        public static T GetService<T>() where T : class
        {
            if (Current is null)
                throw new InvalidOperationException("Service provider is not initialized.");

            return Current.GetService<T>()!;
        }

        /// <summary>
        /// Gets or sets the current service provider instance.
        /// </summary>
        public static IServiceProvider? Current { get; set; }
    }
}
