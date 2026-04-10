using Microsoft.Extensions.Hosting;

namespace AiCallCenterBackend.Services
{
    public class EscalationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EscalationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var escalationService = scope.ServiceProvider.GetRequiredService<EscalationService>();
                    await escalationService.AutoEscalateComplaints();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // runs every 30 sec
            }
        }
    }
}