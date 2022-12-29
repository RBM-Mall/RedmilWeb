using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project_Redmil_MVC.Models.RequestModel.MakeCashOutDeposite
{
    public class MakeCashoutDespoisteRequestModel
    {
        public string Token { get; set; }
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
       // public string Docs { get; set; }
        public string Remarks { get; set; }
        public string ChargedAmount { get; set; }
        public string ChargeType { get; set; }
        public string Charge { get; set; }
        public string Amount { get; set; }
        public string ModeId { get; set; }
        public string Userid { get; set; }
        public string checksum { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "Please choose file to upload.")]
        public string Docs { get; set; }
        public IFormFile Docss { get; set; }
    }
}
