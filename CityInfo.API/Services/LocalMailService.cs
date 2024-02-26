using System.Text;

namespace CityInfo.API.Services
{
    public interface ILocalMailService
    {
        void Send(string subject, string message);
    }

    public class LocalMailService : ILocalMailService
    {
        private string _mailTo = "admin@my.com";
        private string _mailFrom = "noreply@my.com";
        private ILogger<LocalMailService> _logger;

        public LocalMailService(ILogger<LocalMailService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Send(string subject, string message)
        {
            _logger.WithInfoContext(ctx =>
            {
                ctx.Header("Sending new mail.").FromUser(_mailFrom).ToUser(_mailTo)
                .Message(message);
            });
        }
    }


    public class LoggingContext : IDisposable
    {
        private ILogger<LocalMailService> _logger;
        private LogLevel _logLevel;
        private StringBuilder _builder = new StringBuilder();

        private string _header = string.Empty;  

        public LoggingContext(ILogger<LocalMailService> logger, LogLevel logLevel)
        {
            _logger = logger;
            _logLevel = logLevel;
        }

        public LoggingContext Message(string msg)
        {
            _builder.Append($"Message: {msg};");
            return this;
        }
        public LoggingContext FromUser(string user)
        {
            _builder.Append($"From: {user};");
            return this;
        }
        public LoggingContext ToUser(string user)
        {
            _builder.Append($"To: {user};");
            return this;
        }

        public LoggingContext Header(string header)
        {
            _header = header;
            return this;
        }

        private void Log()
        {
            var str = _builder.ToString();
            if (!string.IsNullOrEmpty(_header))
            {
                str = string.Concat(_header, " ", str);
            }

            _logger.Log(_logLevel, str);
        }

        public void Dispose()
        {
            Log();
        }
    }

    public static class StructureLogger
    {
        public static void WithInfoContext(this ILogger<LocalMailService> logger, Action<LoggingContext> loggingActions)
        {
            using (var ctx = new LoggingContext(logger, LogLevel.Information))
            {
                loggingActions(ctx);
            }
        }
    }
}


