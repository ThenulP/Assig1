using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assig1.ViewModels
{
    public class OffenceAndExpiationCategory
    {
        [Key]
        public string OffenceCode { get; set; } = null!;

        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [DataType(DataType.Currency)]
        [Display(Name = "Expiation Fee")]
        public int? ExpiationFee { get; set; }

        [Display(Name = "Adult Levy")]
        public int? AdultLevy { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Corporate Fee")]
        public int? CorporateFee { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total Fee")]
        public int? TotalFee { get; set; }

        [Display(Name = "Demerit Points")]
        public int? DemeritPoints { get; set; }

        [Required(ErrorMessage = "Section Id is required.")]
        [Display(Name = "Section")]
        public int? SectionId { get; set; }

        [Required(ErrorMessage = "Section Code is required")]
        [Display(Name = "Section Code")]
        public string? SectionCode { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = null!;

    }
}
