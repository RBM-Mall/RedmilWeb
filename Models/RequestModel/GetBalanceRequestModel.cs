namespace Project_Redmil_MVC.Models.RequestModel
{
    public class GetBalanceRequestModel
    {
        public string checksum { get; set; }
        public string Userid { get; set; }
        public string ContestId { get; set; }
        public string CategoryId { get; set; }

        public string Token { get; set; }
    }
}
