using Assig1.Models;
using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class ExpiationDetails
    {
        public int? MaxSpeed { get; set; }

        public int? MinSpeed { get; set; }

        public double? AvgSpeed { get; set; }

        public IEnumerable<dynamic> TotalFeePerMonth { get; set; }

        public IEnumerable<dynamic> TotalOffencesPerMonth { get; set; }
    }
}
