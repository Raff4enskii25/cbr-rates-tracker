using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
   
namespace CbrRatesTracker.CbrIntegration
{
    public class CbrClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CbrClientService> _logger;
        private readonly string _xmlDailyUrl;

        public CbrClientService(HttpClient httpClient, IConfiguration configuration, ILogger<CbrClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _xmlDailyUrl = configuration.GetValue<string>("Cbr:XmlDailyUrl");
        }

        public async Task<List<ValuteDTO>> GetLatestRatesAsync()
        {
            try
            {
                using var response = await _httpClient.GetAsync(_xmlDailyUrl);
                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var xml = System.Text.Encoding.GetEncoding("windows-1251").GetString(bytes);
                var serializer = new XmlSerializer(typeof(ValCurs));
                using var reader = new StringReader(xml);
                var valCurs = (ValCurs)serializer.Deserialize(reader);

                var valuteDTOs = new List<ValuteDTO>();
                valuteDTOs.AddRange(valCurs.Valutes.Select(valute => new ValuteDTO(
                    valute.CharCode,
                    int.Parse(valute.NumCode),
                    int.Parse(valute.Nominal),
                    valute.Name,
                    decimal.Parse(valute.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                    DateOnly.Parse(valCurs.Date, CultureInfo.GetCultureInfo("ru-RU"))
                )));
                
                return valuteDTOs;
            }

            catch(HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching latest rates from CBR.");
                throw;
            }
            
            catch(InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error occurred while deserializing the XML response.");
                throw;
            }

            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");
                throw;
            }
        }
    }
}
