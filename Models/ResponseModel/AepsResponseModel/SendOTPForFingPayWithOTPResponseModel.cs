namespace Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel
{
    public class SendOTPForFingPayWithOTPResponseModel
    {
        public string RequestId { get; set; }
        public string merchantLoginId { get; set; }
        public string primaryKeyId { get; set; }
        public string encodeFPTxnId { get; set; }
        
    }
}
