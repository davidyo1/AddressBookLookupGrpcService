namespace AddressBookLookup.Grpc.Server.Services
{
    public class RandomLogBackgroundService(ILogger<RandomLogBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation($"Background service running, it's {DateTime.Now}");
                await Task.Delay(new Random().Next(10000), cancellationToken);
            }
        }
    }
}
