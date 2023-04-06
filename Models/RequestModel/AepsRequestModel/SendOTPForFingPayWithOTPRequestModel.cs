namespace Project_Redmil_MVC.Models.RequestModel.AepsRequestModel
{
    public class SendOTPForFingPayWithOTPRequestModel
    {
            public string RequestId { get; set; }
            public string merchantLoginId { get; set; }
            public string primaryKeyId { get; set; }
            public string encodeFPTxnId { get; set; }
            public string MobileNo { get; set; }
            public string UserId { get; set; }
        
    }
}
