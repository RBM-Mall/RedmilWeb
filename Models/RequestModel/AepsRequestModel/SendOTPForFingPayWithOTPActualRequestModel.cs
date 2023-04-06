namespace Project_Redmil_MVC.Models.RequestModel.AepsRequestModel
{
    public class SendOTPForFingPayWithOTPActualRequestModel
    {
        public string  UserId { get; set; }
        public string encodeFPTxnId { get; set; }
        public string primaryKeyId { get; set; }
        public string Otp { get; set; }
        public string merchantLoginId { get; set; }
        public string RequestId { get; set; }
    }
}
