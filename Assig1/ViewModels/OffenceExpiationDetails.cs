using Assig1.Models;
using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class OffenceExpiationDetails
    {
        [Required]
        public Offence? Offence { get; set; }  
        public IEnumerable<OffenceAndExpiation>? OffenceAndExpiations { get; set; }
    }
}
