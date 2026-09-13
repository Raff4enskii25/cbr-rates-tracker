using System.Xml.Serialization;

namespace CbrRatesTracker.CbrIntegration
{
    [XmlRoot("ValCurs")]
    public class ValCurs
    {
        [XmlAttribute("Date")]
        public string Date { get; set; }

        [XmlElement("Valute")]
        public List<Valute> Valutes { get; set; }
    }
}
