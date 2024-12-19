using Grpc.Net.Client;
using GrpcPricing.Protos;

namespace multiflux.Services
{
    public class Pricer
    {
        private readonly double[] _past;
        private readonly double _time;
        private readonly bool _monitoringDateReached;
        private readonly GrpcPricer.GrpcPricerClient _client;

        public Pricer(double[] past, double time, bool monitoringDateReached)
        {
            _past = past;
            _time = time;
            _monitoringDateReached = monitoringDateReached;

            var httpHandler = new HttpClientHandler();
            var channel = GrpcChannel.ForAddress("http://localhost:50051", new GrpcChannelOptions
            {
                HttpHandler = httpHandler,
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null
            });
            _client = new GrpcPricer.GrpcPricerClient(channel);
        }

        public double[] Past => _past;
        public double Time => _time;
        public bool MonitoringDateReached => _monitoringDateReached;

        public PricingInput ToPricingInput()
        {
            var request = new PricingInput
            {
                Time = _time,
                MonitoringDateReached = _monitoringDateReached
            };

            var pastLine = new PastLines();
            for (int i = 0; i < _past.Length; i++)
            {
                pastLine.Value.Add(_past[i]);
            }
            request.Past.Add(pastLine);
            return request;
        }

        public async Task<PricingOutput> GetPricingOutputAsync()
        {
            try
            {
                var request = ToPricingInput();
                return await _client.PriceAndDeltasAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel au service gRPC : {ex.Message}");
                throw;
            }
        }
    }
}