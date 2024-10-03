using Assig1.Models;
using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class ExpiationDetails
    {
        [Display(Name = "Max Speed")]
        public int? MaxSpeed { get; set; }

        [Display(Name = "Min Speed")]
        public int? MinSpeed { get; set; }

        [Display(Name = "Average Speed")]
        public double? AvgSpeed { get; set; }

        public IEnumerable<dynamic>? TotalFeePerMonth { get; set; }

        public IEnumerable<dynamic>? TotalOffencesPerMonth { get; set; }
    }
}
