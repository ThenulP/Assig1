using Assig1.Models;
using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class OffenceAndExpiation
    {
        [Display(Name = "Incident Date")]
        public DateOnly IncidentStartDate { get; set; }

        [Display(Name = "Vehicle Speed")]
        public int? VehicleSpeed { get; set; }

        [Display(Name = "Speed Limit")]
        public int? LocationSpeedLimit { get; set; }

        [Display(Name = "Driver State")]
        public string? DriverState { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total Fee")]
        public int? TotalFeeAmt { get; set; }

        [Display(Name = "Offence")]
        public virtual Offence? Offence { get; set; }

        [Display(Name = "Section")]
        public virtual Section? Section { get; set; }

    }
}
