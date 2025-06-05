using System.Threading.Tasks;

public class CleanupBackgroundService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<CleanupBackgroundService> logger;
    private Timer? timer;
    private bool cleanedToday;

    public CleanupBackgroundService(IServiceProvider serviceProvider, ILogger<CleanupBackgroundService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Cleanup background service starting...");
        timer = new Timer(CheckIfTimeToCleanup, null, TimeSpan.Zero, TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Cleanup background service stopping...");

        timer?.Dispose();
        timer = null;

        return Task.CompletedTask;
    }

    private void CheckIfTimeToCleanup(object? state)
    {
        _ = RunIfTimeToCleanup();
    }

    private async Task RunIfTimeToCleanup()
    {
        try
        {
            var swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var currentSwedishTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, swedishTimeZone);

            if (currentSwedishTime.Hour == 2 && !cleanedToday)
            {
                logger.LogInformation("Time to run cleanup! Current Swedish time: {Time}.", currentSwedishTime);

                using var scope = serviceProvider.CreateScope();
                var cleanupService = scope.ServiceProvider.GetRequiredService<ICleanupService>();
                await cleanupService.CleanupSoftDeletedItemsAsync();

                cleanedToday = true;
            }
            else if (currentSwedishTime.Hour != 2)
            {
                cleanedToday = false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during clceanup process.");
        }

    }
}
