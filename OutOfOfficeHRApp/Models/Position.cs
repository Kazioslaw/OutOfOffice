using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class Position
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}