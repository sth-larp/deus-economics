using System.ComponentModel.DataAnnotations;

namespace DeusCloud.Data.Entities.Constants
{
    public class Constant
    {
        [Required]
        public float Value { get; set; }

        [Key]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}