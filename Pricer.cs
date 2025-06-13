using Grpc.Net.Client;
using GrpcPricing.Protos;

namespace multiflux.Services
{
    public class Pricer
    {
        private readonly List<List<double>> _spots;
        private readonly double _time;
        private readonly bool _monitoringDateReached;
        private readonly GrpcPricer.GrpcPricerClient _client;

        public Pricer(List<List<double>> spots, double time, bool monitoringDateReached)
        {
            _spots = spots;
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


        public double Time => _time;
        public bool MonitoringDateReached => _monitoringDateReached;
        public PricingInput ToPricingInput()
        {
            var request = new PricingInput
            {
                Time = _time,
                MonitoringDateReached = _monitoringDateReached
            };



            foreach (var spotLine in _spots)
            {
                var pastLine = new PastLines();
                pastLine.Value.AddRange(spotLine);
                request.Past.Add(pastLine);


            }




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
