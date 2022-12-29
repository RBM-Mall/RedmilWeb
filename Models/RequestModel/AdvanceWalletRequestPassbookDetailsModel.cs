namespace Project_Redmil_MVC.Models.RequestModel
{
    public class AdvanceWalletRequestPassbookDetailsModel
    {
 
        public string Userid { get; set; }
        public string WalletType { get; set; }
        public string FilterBy { get; set; }
        public string PageNumber { get; set; }
        public string checksum { get; set; }
        public string Token { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
