using Project_Redmil_MVC.Helper;
using System.Xml.Linq;

namespace Project_Redmil_MVC.Models
{
    public class RequestModel1
    {
        public string Userid { get; set; }
        public string checksum { get; set; }
        public string ServiceId{ get; set; }
        public string MobileNo { get; set; }
        public string Circle { get; set; }
        public string OpName { get; set; }
    }

    public class AepsKycDetailsNewRequestModel
    {
        public string UserId { get; set; }
        public string checksum { get; set; }
    }

    public class AepsRoutingDetailsRequestModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string TransactionType { get; set; }
    }

    public class AaadharPicVerificationRequestModal
    {
        public string Userid { get; set; }
        public string checksum { get; set; }
    }

    // Getting fingerprint Data as a response after scanning finger biometric, will be send as a request to hit api
    public class fingurprintDataRequestModel
    {
        public string AnsiTemplate { get; set; }
        public string BitmapData { get; set; }
        public int Bpp { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public int GrayScale { get; set; }
        public double InArea { get; set; }
        public double InHeight { get; set; }
        public double InWidth { get; set; }
        public string IsoImage { get; set; }
        public string IsoTemplate { get; set; }
        public int Nfiq { get; set; }
        public int Quality { get; set; }
        public string RawData { get; set; }
        public int Resolution { get; set; }
        public int WSQCompressRatio { get; set; }
        public string WSQInfo { get; set; }
        public string WsqImage { get; set; }
    }

    public class UserVerificationWithPaytmRequestModel
    {
        public string userId { get; set; }
        public int RetailerTxnId { get; set; }
        public int AadharNumber { get; set; }
        public int IIN { get; set; }
        public int Amount { get; set; }
        public string BiometricData { get; set; }
        public int RetailerId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int Param1 { get; set; }
        public int Param2 { get; set; }
        public int AcquirerInstitutationId { get; set; }
        public string TxnTypeCode { get; set; }
        public int AppId { get; set; }
        public int AppVersion { get; set; }
        public int CustomerConsent { get; set; }
        public string IsVID { get; set; }
        public int Param3 { get; set; }
        public int Param4 { get; set; }
        public int Param5 { get; set; }
        public int Param6 { get; set; }
        public int Param7 { get; set; }
        public int Param8 { get; set; }
        public int Param9 { get; set; }
        public int Param10 { get; set; }
    }
    public class UserVerificationWithPaytmRequestModel1
    {
        public string? EncAadhaar { get; set; }
        public string? TxnType { get; set; }
        public string? Device { get; set; }
        public string? fingureData { get; set; }
        public string? Amount { get; set; }
        public string? Mode { get; set; }
        public string? Token { get; set; }
        public string? EncData { get; set; }
        public string? CustomerMobileno { get; set; }
        public string? BCLocation { get; set; }
        public string? PidData { get; set; }
        public string? Userid { get; set; }
        public string? BCName { get; set; }
        public string? checksum { get; set; }
        public string? CustomerName { get; set; }
        public string? AgentID { get; set; }
        public string? RetailerTxnId { get; set; }
        public string? CustomerAadhaarNo { get; set; }
        public string? PinCode { get; set; }
    }

    public class MakeAepsTransactionRequestModel
    {
        public string EncData { get; set; }
        public string PinCode { get; set; }
        public string Userid { get; set; }
        public string CustomerAadhaarNo { get; set; }
        public string CustomerMobileno { get; set; }
        public string CustomerName { get; set; }
        public string checksum { get; set; }
        public string Amount { get; set; }
        public string TxnType { get; set; }
        public string Mode { get; set; }
        public string BCName { get; set; }
        public string AgentID { get; set; }
        public string BCLocation { get; set; }
        public string Device { get; set; }
        public string Token { get; set; }
        public string RetailerTxnId { get; set; }
    }
}
