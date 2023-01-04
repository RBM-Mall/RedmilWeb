namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class GetBalanceResponseModel:BaseResponseModel
    {
       
            public string MainBal { get; set; }
            public double AdBal { get; set; }
            public double Reward { get; set; }
            public double REReward { get; set; }
            public double BReward { get; set; }
            public double WalletAmount { get; set; }
            public double? TotalIncentives { get; set; }
        
    }
}
