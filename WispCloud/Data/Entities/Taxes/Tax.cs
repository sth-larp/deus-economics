using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeusCloud.Data.Entities.Taxes
{
    public class Tax
    {
        [Required]
        public float PercentValue { get; set; }

        [Key]
        public TaxType Type { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}