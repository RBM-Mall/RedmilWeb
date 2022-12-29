using System.ComponentModel.DataAnnotations;

namespace Project_Redmil_MVC.Models
{
    public class Login
    {
        [Required]
        [StringLength(12,MinimumLength =10)]
        public string? Mobile { get; set; }
        public string? checksum { get; set; }
        [Required]
        public string? Userid { get; set; }
        public string? Mpin { get; set; }
    }
}
