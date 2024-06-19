using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class ProjectType
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}