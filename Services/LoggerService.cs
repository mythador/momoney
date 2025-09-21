using Microsoft.Extensions.Logging;

namespace MoMoney.Services
{
    /// <summary>
    /// Provides logging functionality with support for file-based fallback.
    /// Includes helpers for page logging, generic logging, and category-based logging.
    /// </summary>
    public static class LoggerService
    {
        private static ILoggerFactory _loggerFactory;
        private static readonly string _logFilePath =
            Path.Combine(FileSystem.AppDataDirectory, "momoney-manual.log");

        /// <summary>
        /// Initializes the logger service with a specified logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to initialize with.</param>
        public static void Init(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates a logger for the specified category.
        /// </summary>
        /// <param name="category">The logging category. Defaults to "MoMoney".</param>
        /// <returns>An <see cref="ILogger"/> instance for the category.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the service is not initialized with <see cref="Init(ILoggerFactory)"/>.
        /// </exception>
        private static ILogger CreateLogger(string category = "MoMoney")
        {
            if (_loggerFactory == null)
                throw new InvalidOperationException("LoggerService not initialized. Call LoggerService.Init in MauiProgram.cs");

            return _loggerFactory.CreateLogger(category);
        }

        // =====================================
        // ===== Page Logging Helpers =====
        // =====================================

        /// <summary>
        /// Logs an informational message for a specific page.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="message">The message to log.</param>
        public static void LogPageInfo(string pageName, string message) =>
            SafeLog(pageName, LogLevel.Information, message);

        /// <summary>
        /// Logs a warning message for a specific page.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="message">The message to log.</param>
        public static void LogPageWarning(string pageName, string message) =>
            SafeLog(pageName, LogLevel.Warning, message);

        /// <summary>
        /// Logs an error message for a specific page, with an optional exception.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The exception to log (optional).</param>
        public static void LogPageError(string pageName, string message, Exception ex = null) =>
            SafeLog(pageName, LogLevel.Error, message, ex);

        /// <summary>
        /// Logs a message safely with a fallback to file writing if the logger fails.
        /// </summary>
        /// <param name="category">The logging category.</param>
        /// <param name="level">The severity level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The exception to log (optional).</param>
        private static void SafeLog(string category, LogLevel level, string message, Exception ex = null)
        {
            var msg = WithTimestamp(message);

            try
            {
                var logger = CreateLogger(category);

                switch (level)
                {
                    case LogLevel.Information:
                        logger.LogInformation(msg);
                        break;
                    case LogLevel.Warning:
                        logger.LogWarning(msg);
                        break;
                    case LogLevel.Error:
                        logger.LogError(ex, msg);
                        break;
                    case LogLevel.Debug:
                        logger.LogDebug(msg);
                        break;
                }
            }
            catch
            {
                WriteToFile(level.ToString().ToUpper(), category, $"{msg}{(ex != null ? $" | EX: {ex}" : "")}");
            }
        }

        /// <summary>
        /// Prefixes a message with a timestamp.
        /// </summary>
        /// <param name="message">The message to prefix.</param>
        /// <returns>The message with a timestamp prefix.</returns>
        private static string WithTimestamp(string message) =>
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

        /// <summary>
        /// Writes a log message to a file as a fallback when logging fails.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="category">The category name.</param>
        /// <param name="message">The log message.</param>
        private static void WriteToFile(string level, string category, string message)
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {category} - {message}";
            try
            {
                File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
            catch
            {
                // Fail silently to avoid crashing app if file is locked/unavailable
            }
        }

        // =====================================
        // ===== Basic Logging (manual category or default) =====
        // =====================================

        /// <summary>
        /// Logs an informational message with an optional category.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="category">The logging category. Defaults to "MoMoney".</param>
        public static void LogInfo(string message, string category = "MoMoney") =>
            SafeLog(category, LogLevel.Information, message);

        /// <summary>
        /// Logs a warning message with an optional category.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="category">The logging category. Defaults to "MoMoney".</param>
        public static void LogWarning(string message, string category = "MoMoney") =>
            SafeLog(category, LogLevel.Warning, message);

        /// <summary>
        /// Logs an error message with an optional category.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="category">The logging category. Defaults to "MoMoney".</param>
        public static void LogError(Exception ex, string message, string category = "MoMoney") =>
            SafeLog(category, LogLevel.Error, message, ex);

        /// <summary>
        /// Logs a debug message with an optional category.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="category">The logging category. Defaults to "MoMoney".</param>
        public static void LogDebug(string message, string category = "MoMoney") =>
            SafeLog(category, LogLevel.Debug, message);

        // =====================================
        // ===== Generic Overloads (auto-category based on T) =====
        // =====================================

        /// <summary>
        /// Logs an informational message, using the type name as the category.
        /// </summary>
        /// <typeparam name="T">The type used for the category.</typeparam>
        /// <param name="message">The message to log.</param>
        public static void LogInfo<T>(string message) =>
            LogInfo(message, typeof(T).Name);

        /// <summary>
        /// Logs a warning message, using the type name as the category.
        /// </summary>
        /// <typeparam name="T">The type used for the category.</typeparam>
        /// <param name="message">The message to log.</param>
        public static void LogWarning<T>(string message) =>
            LogWarning(message, typeof(T).Name);

        /// <summary>
        /// Logs an error message, using the type name as the category.
        /// </summary>
        /// <typeparam name="T">The type used for the category.</typeparam>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        public static void LogError<T>(Exception ex, string message) =>
            LogError(ex, message, typeof(T).Name);

        /// <summary>
        /// Logs a debug message, using the type name as the category.
        /// </summary>
        /// <typeparam name="T">The type used for the category.</typeparam>
        /// <param name="message">The message to log.</param>
        public static void LogDebug<T>(string message) =>
            LogDebug(message, typeof(T).Name);

        // =====================================
        // ===== Explicit Category Logging =====
        // =====================================

        /// <summary>
        /// Logs an informational message with an explicit category, bypassing generic overloads.
        /// </summary>
        /// <param name="category">The logging category.</param>
        /// <param name="message">The message to log.</param>
        public static void LogInfoWithCategory(string category, string message)
        {
            var msg = WithTimestamp(message);
            CreateLogger(category).LogInformation(msg);
            WriteToFile("INFO", category, msg);
        }
    }
}
