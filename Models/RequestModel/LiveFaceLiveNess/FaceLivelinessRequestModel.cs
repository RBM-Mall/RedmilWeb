using Microsoft.Exchange.WebServices.Data;

namespace Project_Redmil_MVC.Models.RequestModel.LiveFaceLiveNess
{
    public class FaceLivelinessRequestModel
    {
        public string Userid { get; set; }
        public string FileName { get; set; }
        public string checksum { get; set; }
        public string Mobile { get; set; }
        public string Token { get; set; }   
        
    }
}
