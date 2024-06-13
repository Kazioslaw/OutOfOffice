using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class Subdivision
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}