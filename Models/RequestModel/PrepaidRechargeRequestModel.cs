using System.ComponentModel.DataAnnotations;

namespace Project_Redmil_MVC.Models.RequestModel
{
    public class PrepaidRechargeRequestModel
    {
        public string Userid { get; set; }
        public string checksum { get; set; }
        public string Token { get; set; }
        public string ServiceId { get; set; }
        public string OpId { get; set; }
        public string Mobileno { get; set; }
        public string Mode { get; set; }
        public string Amount { get; set; }
        public string Wallet { get; set; }
        public object Exception { get; set; }
    }

}
