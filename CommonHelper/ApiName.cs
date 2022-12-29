using System.Security.Policy;

namespace Project_Redmil_MVC.CommonHelper
{
    public class ApiName
    {

        //Electrcity bill
        public static string BbpsBillerByState = "BbpsBillerByState";
        public static string GetBBPSBillsTmp = "GetBBPSBillsTmp";
        public static string BbpsStates = "BbpsStates";
        public static string CCF = "CCF";
        public static string Getbalance = "Getbalance";
        public static string PayBill = "PayBBPSBillsTmp";


        //Prepaid Recharge
        public static string Recharge = "Recharge";
        public static string GetOperaterList = "GetOperaterList";
        public static string JRIBrowsPlan = "JRIBrowsPlan";
        public static string GetMobileMNPDetails = "GetMobileMNPDetails";


        //MyBusinessReports

        public static string TransactionCategory = "transactions/service/category";
        public static string TransactionSubCategory = "transactions/service/subcategory";
        public static string TransactionReport = "transactions/report";
        public static string CheckUTIAgentStatus = "CheckUTIAgentStatus";
        public static string SeatSellerCancelBooking = "SeatSellerCancelBooking";


        // AEPS

        public static string GetAepsKycDetailsNew = "GetAepsKycDetailsNew";
        public static string GetAepsRoutingDetails = "GetAepsRoutingDetails";
        public static string GetAgentKycId = "GetAgentKycId";



        //MyTeamDashboard
        public static string GetMyTeamMemberCountNew = "GetMyTeamMemberCountNew";
        //Login
        public static string ValidateUser = "User/ValidateUser";
        public static string Mpin = "Mpin";
        public static string SendAnotherDeviceLoginOtp = "SendAnotherDeviceLoginOtp";
        public static string ValidateOTPAnotherDeviceLogin = "ValidateOTPAnotherDeviceLogin";
        public static string SentOtpForResetMpin = "SentOtpForResetMpin";
        public static string ResetMpin = "ResetMpin";
        //UserDashboard
        public static string ViewPayOutCategory = "ViewPayOutCategory";
        public static string ViewPayOutCategoryWise = "ViewPayOutCategoryWise";
        public static string ViewRateOfIntrest = "ViewRateOfIntrest";
        public static string ApplyForFranchise = "ApplyForFranchise";
        public static string ShowMaterial = "ShowMaterial";
        public static string GetSubscriptionPlan = "GetSubscriptionPlan";
        public static string ValidateSubscriptionCouponCode = "ValidateSubscriptionCouponCode";
        public static string InsertOnlinepaymentData = "InsertOnlinepaymentData";
        public static string OfflinePaymentForSubscriptionPackageUserWiseWithCoupon = "OfflinePaymentForSubscriptionPackageUserWiseWithCoupon";
        public static string GetUserSubscriptionDetails = "GetUserSubscriptionDetails";
        //Support
        public static string GetDepartmentCall = "GetDepartmentCall";
        public static string IVRDurationCheck = "IVRDurationCheck";
        public static string GetIVRResponse2 = "GetIVRResponse2";
        public static string AddFeedback = "AddIvrfeedback";
        public static string GetRMDetailsUserwise = "GetRMDetailsUserwise";
        //Passbook
        public static string GetUserBalanceSummaryWithPaging = "GetUserBalanceSummaryWithPaging";
        public static string GetTransactionDetailsUseingBalanceId = "GetTransactionDetailsUseingBalanceId";
        public static string GetCashoutSurchargeNew = "GetCashoutSurchargeNew";
        public static string GetCashoutAcutalAmountCredit = "GetCashoutAcutalAmountCredit";
        public static string GetMultiAccountDetailsForUsers = "GetMultiAccountDetailsForUsers";
        public static string AaadharPicVerification = "AaadharPicVerification";
        public static string FaceLiveliNess = "FaceLiveliNess";
        public static string MakeCashOutNew = "MakeCashOutNew";
        public static string Getbank = "Getbank";
        public static string AccountVerificationForSignupwithCharge = "AccountVerificationForSignupwithCharge";
        public static string UploadKhataImages = "UploadKhataImages";
        public static string InsertMultiAccountDetailsForUsers = "InsertMultiAccountDetailsForUsers";
        public static string SendOTPForMultiAccount = "SendOTPForMultiAccount";
        public static string ValidateOTPForMultiAccount = "ValidateOTPForMultiAccount";
        public static string transactionsreport = "transactions/report";
        public static string GetCashdepositeMode = "GetCashdepositeMode";
        public static string MakeCashDeposite = "MakeCashDeposite";
        public static string File = "FileUploading/UploadFile";
        //public static string transactions/report = "transactions/report";


        //LIC India Bil
        public static string GetLICBill = "fetchlicbill";
        public static string PayLICBill = "PayLICBill";

        //DMT2.0
        public static string SenderStatus = "senderstatus";
        public static string SenderDetailsByuserid = "senderdetailsbyuserid";
        public static string OtpConfirmation = "otpconfirmation";
        public static string GetFinoOtp = "getfinootp";
        public static string AddSender = "addsender";
        public static string SenderDetails = "senderdetails";
        public static string AddBeneficiarydDetails = "addbeneficiarydetails";
        public static string DeleteBeneficiaryDetails = "deletebeneficiarydetails";
        public static string FinoBankCharges = "finobankcharges";
        public static string BeneficiaryAccountVerification = "beneficiaryaccountverification";
        public static string ProcessNEFTRequest = "processneftrequest";
        public static string ProcessIMPSRequest = "processimpsrequest";

    }

}
