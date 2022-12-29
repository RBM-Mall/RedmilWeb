using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project_Redmil_MVC.Models.RequestModel.InsertMultiAccountDetailForUserRequestModel
{
    public class InserMutltiAccount
    {
        public string Userid { get; set; }
        public string Id { get; set; }
       
        public string BankId { get; set; }
        [Required]
        public string AccountNo { get; set; }
        [Required]
        public string IFSC { get; set; }
        [Required]
        public string BeniName { get; set; }
        public string PanStatus { get; set; }
        [Required]
        public string RelationName { get; set; }
        [Required]
        public string RelationPanImagePath { get; set; }
        public string UserNameInBank { get; set; }
        public string checksum { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "Please choose file to upload.")]
        public IFormFile file { get; set; }
      
    }
}
