using Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel;

namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class AepsKycDetailsNewResponseModel : Bank
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankLogo { get; set; }
        public AaadharPicVerificationResponseModal aaadharPics { get; set; }
        public AepsGetAllBankListResponseModel bankList { get; set; }
    }
    public class AepsRoutingDetailsResponseModel
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public object Bank { get; set; }
        public object Kyc { get; set; }
    }

    public class AaadharPicVerificationResponseModal
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
    public class AepsKycDetailsStatusPendingResponseModel
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankLogo { get; set; }

    }
    public class BaseMakeAepsTransactionResponseModel
    {
        public bool VoiceStatus { get; set; }
        public int LanguageId { get; set; }
        public object VoiceMessage { get; set; }
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public object MiniStatementData { get; set; }
    }

    public class MakeAepsTransactionResponseModel
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public int ServiceId { get; set; }
        public int OpId { get; set; }
        public long Mobileno { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public bool CommType { get; set; }
        public double CommissionAmount { get; set; }
        public double Surcharge { get; set; }
        public bool SurType { get; set; }
        public double SurChargeAmount { get; set; }
        public string TransactionType { get; set; }
        public string Mode { get; set; }
        public int ApiId { get; set; }
        public object TransactionId { get; set; }
        public string RetailerTxnID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerAadhaarNo { get; set; }
        public string BCName { get; set; }
        public string AgentID { get; set; }
        public string BCLocation { get; set; }
        public string TerminalID { get; set; }
        public string STAN { get; set; }
        public string RRN { get; set; }
        public string UIDAIAuthCode { get; set; }
        public double ACBalance { get; set; }
        public string Status { get; set; }
        public object Editdate { get; set; }
        public DateTime Reqdate { get; set; }
        public string Device { get; set; }
        public string MiniSTMTdata { get; set; }
        public double Gst { get; set; }
        public double Tds { get; set; }
        public string Response { get; set; }
        public int IIN { get; set; }
        public int DirectId { get; set; }
        public double DirectCommission { get; set; }
        public bool DirectCommissionType { get; set; }
        public double DirectCommissionAmount { get; set; }
        public int InDirectId { get; set; }
        public double InDirectCommission { get; set; }
        public bool InDirectCommissionType { get; set; }
        public double InDirectCommissionAmount { get; set; }
        public int SuperInDirectId { get; set; }
        public double SuperInDirectCommission { get; set; }
        public bool SuperInDirectCommissionType { get; set; }
        public double SuperInDirectCommissionAmount { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string uid { get; set; }
        public string DeviceID { get; set; }
        public string PinCode { get; set; }
        public string Distance { get; set; }
    }

    public class MiniSTMTdata
    {
        public string date { get; set; }
        public string txnType { get; set; }
        public string amount { get; set; }
        public string narration { get; set; }
    }

}


