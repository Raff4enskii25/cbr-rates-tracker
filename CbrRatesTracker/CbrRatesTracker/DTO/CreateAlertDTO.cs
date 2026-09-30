using CbrRatesTracker.Enums;
using CbrRatesTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace CbrRatesTracker.DTO
{
    public class CreateAlertDTO
    {
        [Required]
        public string CharCode { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal ThresholdValue { get; set; }

        [Required]
        public Direction Direction { get; set; }

        public string? Email { get; set; }
    };
}
