using System.ComponentModel.DataAnnotations;

namespace DeusCloud.Data.Entities.Constants
{
    public class Constant
    {
        [Required]
        public float PercentValue { get; set; }

        [Key]
        public ConstantType Type { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}