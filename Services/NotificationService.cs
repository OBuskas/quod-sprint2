namespace ValidationAPI.Services
{
    public class NotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _fraudEndpoint;

        public NotificationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _fraudEndpoint = configuration.GetValue<string>("Notification:FraudEndpoint") ?? throw new InvalidOperationException("Fraud endpoint configuration is required");
        }

        public async Task NotifyFraudAsync(Guid uploadId)
        {
            var notification = new
            {
                uploadId = uploadId,
                timestamp = DateTime.UtcNow,
                type = "suspected_fraud"
            };

            await _httpClient.PostAsJsonAsync(_fraudEndpoint, notification);
        }
    }
}
