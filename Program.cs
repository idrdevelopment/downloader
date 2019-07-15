using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ISC.IDRDownloader.Services;
using ISC.IDRDownloader.Services.Contracts;
using ISC.IDRDownloader.Validation.Contracts;
using ISC.IDRDownloader.Validation;
using Microsoft.Extensions.Logging;

namespace ISC.IDRDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IArgumentValidator, ArgumentValidator>()
                .AddScoped<IDownloadService, DownloadService>()
                .AddLogging(log => log.SetMinimumLevel(LogLevel.Error))
                .BuildServiceProvider();

            var downloadService = serviceProvider.GetService<IDownloadService>();
            var logger = serviceProvider.GetService<ILoggerFactory>().AddLog4Net().CreateLogger<Program>();

            try
            {
                var argumentErrors = downloadService.ValidateArguments(args);

                if (argumentErrors.Length > 0)
                {
                    Console.Write(argumentErrors.ToString());
                }
                else
                {
                    var arguments = downloadService.Download(args);
                    Console.Write($"The files have been saved at this location '{arguments.SavePath}'");
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error has occured.");
                var location = Assembly.GetExecutingAssembly().Location;

                location = location.Replace("idr-downloader.dll", "logs");

                Console.Write($"An error has occured. This has been logged here '{location}'. You can email this log to help@theidregister.com for help.");
            }

            serviceProvider.Dispose();
        }
    }
}
