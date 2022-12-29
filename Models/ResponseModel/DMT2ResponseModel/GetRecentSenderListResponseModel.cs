namespace Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel
{
    public class GetRecentSenderListResponseModel
    {
        public int SenderId { get; set; }
        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string MonthlyLimit { get; set; }
        public string UsedLimit { get; set; }
    }
}
