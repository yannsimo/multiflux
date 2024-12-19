using Grpc.Net.Client;
using GrpcPricing.Protos;
using multiflux.Services;

namespace multiflux
{
    public class PricingClient
    {
        private readonly GrpcPricer.GrpcPricerClient _client;

        public PricingClient(string serverAddress = "http://ENSIPC551:50051")
        {
            var httpHandler = new HttpClientHandler();
            var channel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
            {
                HttpHandler = httpHandler,
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null
            });
            _client = new GrpcPricer.GrpcPricerClient(channel);
        }

        public async Task<PricingOutput> GetPriceAndDeltasAsync(double[] past, double time, bool monitoringDateReached)
        {
            var pricer = new Pricer(past, time, monitoringDateReached);
            return await GetPriceAndDeltasAsync(pricer);
        }

        public async Task<PricingOutput> GetPriceAndDeltasAsync(Pricer pricer)
        {
            try
            {
                var request = pricer.ToPricingInput();
                return await _client.PriceAndDeltasAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel PriceAndDeltas: {ex.Message}");
                throw;
            }
        }

        public async Task<ReqInfo> GetHeartbeatAsync()
        {
            try
            {
                return await _client.HeartbeatAsync(new Empty());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel Heartbeat: {ex.Message}");
                throw;
            }
        }
    }
}

