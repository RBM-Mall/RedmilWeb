using System.ComponentModel.DataAnnotations;

namespace Project_Redmil_MVC.Models
{
    public class BusinessReportModel : BaseResponseModel2
    {
        public string baseUrl;
        public int Id { get; set; }
        public string Name { get; set; }
        public AePSCashWithdrawalModel aePSCashWithdrawalModel { get; set; }
        public AePSMiniStatementModel aePSMiniStatementModel1 { get; set; }
        public AePSBalanceEnquiryModel aePSBalanceEnquiryModel { get; set; }
        public MicroATMCashWithdrawalModel microATMCashWithdrawalModel { get; set; }
        public MicroATMBalanceEnquiryModel microATMBalanceEnquiryModel { get; set; }
        public MicroATMCashWithdrawalICICIModel microATMCashWithdrawalICICIModel { get; set; }
        public MicroATMBalanceEnquiryICICIModel microATMBalanceEnquiryICICIModel { get; set; }
        public POSPaymentsModel pOSPaymentsModel { get; set; }
        public FinoDMTModel finoDMTModel { get; set; }
        public GoldInvestmentBuyModel goldInvestmentBuyModel { get; set; }
        public GoldInvestmentSellModel goldInvestmentSellModel { get; set; }
        public LICPremiumPaymentsModel licPremiumPaymentsModel { get; set; }
        public YBLDomesticMoneyTransferModel yBLDomesticMoneyTransferModel { get; set; }
        public CreditScoreModel creditScoreModel { get; set; }
        public GoldInvestmentBuyModel1 goldInvestmentBuyModel1 { get; set; }
        public GoldInvestmentSellModel1 goldInvestmentSellModel1 { get; set; }
        public AmazonPayGiftCardModel amazonPayGiftCardModel { get; set; }
        public MobilePrepaidModel mobilePrepaidModel { get; set; }
        public MobilePostpaidModel mobilePostpaidModel { get; set; }
        public BillPaymentsModel billPaymentsModel { get; set; }
        public DTHRechargeModel dthRechargeModel { get; set; }
        public MobilePrepaidPostpaidDTHRechargeModel mobilePrepaidPostpaidDthRechargeModel { get; set; }
        public MobilePrepaidPostpaid mobilePrepaidPostpaid { get; set; }

        //public MicroATMBalanceEnquiryModel microATMBalanceEnquiryModel { get; set; }
    }

    public class SubCategoryReportsModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string baseUrl { get; set; }

        public string Status { get; set; }

    }

    // RedMil Wallet

    public class VirtualPaymentReportsModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Virtual Account Number")]
        public string VirtualAccountNumber { get; set; }

        [JsonProperty("Remitter Account Number")]
        public string RemitterAccountNumber { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }
        public double Amount { get; set; }
        public double Charges { get; set; }

        [JsonProperty("Net Amount")]
        public double NetAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Redmil Reference Id")]
        public int RedmilReferenceId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class CashOutModel
    {
        public string baseUrl;

        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}",ApplyFormatInEditMode =true)]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Transfer Type")]
        public string TransferType { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }
        public double Charges { get; set; }

        [JsonProperty("Actual Bank Credit")]
        public double ActualBankCredit { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Current Status")]
        public string CurrentStatus { get; set; }

        [JsonProperty("Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [JsonProperty("Account No")]
        public string AccountNo { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("Redmil Reference Number")]
        public int RedmilReferenceNumber { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class UPIWalletRechargeModel
    {

        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Payment Amount")]
        public double PaymentAmount { get; set; }
        public double Charges { get; set; }

        [JsonProperty("Actual Credit")]
        public double ActualWalletCredit { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("Redmil Reference Number")]
        public object RedmilReferenceNumber { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }

    // Banking Services

    public class AePSCashWithdrawalModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Customer Aadhaar No.")]
        public string CustomerAadhaarNo { get; set; }
        public string Status { get; set; }

        [JsonProperty("Transaction Amount")]
        public double? TransactionAmount { get; set; }

        [JsonProperty("Amount")]
        public double Amount { get; set; }

        [JsonProperty("Commission Amount")]
        public float CommissionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public float? BalanceAmount { get; set; }

        public double BalAmount { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }
        public int? IIN { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
        //public string ServiceType { get; set; }
    }
    public class AePSMiniStatementModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Customer Aadhaar No.")]
        public string CustomerAadhaarNo { get; set; }
        public string Status { get; set; }

        [JsonProperty("Transaction Amount")]
        public float? TransactionAmount { get; set; }

        [JsonProperty("Amount")]
        public double Amount { get; set; }

        [JsonProperty("Commission Amount")]
        public float CommissionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public float? BalanceAmount { get; set; }

        public double BalAmount { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }
        public int? IIN { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class AePSBalanceEnquiryModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Customer Aadhaar No.")]
        public string CustomerAadhaarNo { get; set; }
        public string Status { get; set; }

        [JsonProperty("Transaction Amount")]
        public double? TransactionAmount { get; set; }

        [JsonProperty("Amount")]
        public double Amount { get; set; }

        [JsonProperty("Commission Amount")]
        public float CommissionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public float? BalanceAmount { get; set; }

        public double BalAmount { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }
        public int? IIN { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class MicroATMCashWithdrawalModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Card Holder Name")]
        public string CardHolderName { get; set; }

        [JsonProperty("Card Holder Mobile Number")]
        public string CardHolderMobileNumber { get; set; }

        [JsonProperty("Card Type")]
        public string CardType { get; set; }

        [JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public double BalanceAmount { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class MicroATMBalanceEnquiryModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Card Holder Name")]
        public string CardHolderName { get; set; }

        [JsonProperty("Card Holder Mobile Number")]
        public string CardHolderMobileNumber { get; set; }

        [JsonProperty("Card Type")]
        public string CardType { get; set; }

        [JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public double BalanceAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
    }
    public class YBLDomesticMoneyTransferModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Sender Name")]
        public string SenderName { get; set; }

        [JsonProperty("Sender Mobile Number")]
        public long SenderMobileNumber { get; set; }

        [JsonProperty("Receiver Name")]
        public string ReceiverName { get; set; }

        [JsonProperty("Receiver Mobile Number")]
        public string ReceiverMobileNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }
        public double Surcharge { get; set; }
        public string Status { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }

        [JsonProperty("Receiver Account No")]
        public string? ReceiverAccountNo { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }

        [JsonProperty("Transaction Type")]
        public string TransactionType { get; set; }

        [JsonProperty("Redmil Reference Id")]
        public int RedmilReferenceId { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("Wallet Type")]
        public string WalletType { get; set; }
        public string Reason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class CreditScoreModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile No")]
        public string CustomerMobileNo { get; set; }
        public string Status { get; set; }

        [JsonProperty("Amount Paid")]
        public double? AmountPaid { get; set; }

        [JsonProperty("Commission Amount")]
        public double? CommissionAmount { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }

        [JsonProperty("Wallet Type")]
        public string WalletType { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
        public string CIBILScorePDFLink { get; set; }
    }
    public class UPICashWithdrawalModel
    {

        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Payer Name")]
        public string PayerName { get; set; }

        [JsonProperty("Payer Virtual ID")]
        public string PayerVirtualID { get; set; }

        [JsonProperty("Cash Withdrawal Amount")]
        public double CashWithdrawalAmount { get; set; }
        public double Commission { get; set; }
        public string Status { get; set; }

        [JsonProperty("Merchant Transaction ID")]
        public string MerchantTransactionID { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public object RedmilTransactionId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class MicroATMCashWithdrawalICICIModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Card Holder Name")]
        public string CardHolderName { get; set; }

        [JsonProperty("Card Holder Mobile Number")]
        public string CardHolderMobileNumber { get; set; }

        [JsonProperty("Card Type")]
        public string CardType { get; set; }

        [JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public double BalanceAmount { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class MicroATMBalanceEnquiryICICIModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Card Holder Name")]
        public string CardHolderName { get; set; }

        [JsonProperty("Card Holder Mobile Number")]
        public string CardHolderMobileNumber { get; set; }

        [JsonProperty("Card Type")]
        public string CardType { get; set; }

        [JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }

        [JsonProperty("Balance Amount")]
        public double BalanceAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
    }
    public class FinoDMTModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Sender Name")]
        public string SenderName { get; set; }

        [JsonProperty("Sender Mobile Number")]
        public string SenderMobileNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public string TransactionAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Request Id")]
        public string RequestId { get; set; }

        [JsonProperty("Client Unique Id")]
        public string ClientUniqueId { get; set; }

        [JsonProperty("Receiver Name")]
        public string ReceiverName { get; set; }

        [JsonProperty("Receiver Account No")]
        public string ReceiverAccountNo { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }

        [JsonProperty("Transaction Type")]
        public string TransactionType { get; set; }

        [JsonProperty("Bank Charges")]
        public double BankCharges { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }

        [JsonProperty("Redmil Reference Id")]
        public int RedmilReferenceId { get; set; }

        [JsonProperty("Wallet Type")]
        public string WalletType { get; set; }
        public string TransactionID { get; set; }
        public string Reason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }

        [JsonProperty("Receiver Mobile Number")]
        public string ReceiverMobileNumber { get; set; }
        public string Surcharge { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }
    }

    // Payment Services
    public class POSPaymentsModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Card Holder Name")]
        public string CardHolderName { get; set; }

        [JsonProperty("Card Holder Mobile Number")]
        public string CardHolderMobileNumber { get; set; }

        [JsonProperty("Card Type")]
        public string CardType { get; set; }

        [JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }

        [JsonProperty("Failure Reason")]
        public string FailureReason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class SMSPaymentsModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }
        public string RequestData { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer  Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile")]
        public string CustomerMobile { get; set; }
        public string Mode { get; set; }

        [JsonProperty("Payment Amount")]
        public double PaymentAmount { get; set; }
        public double Surcharge { get; set; }
        public double CreditAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Transaction ID")]
        public object TransactionID { get; set; }

        [JsonProperty("Order ID")]
        public string OrderID { get; set; }

        [JsonProperty("Redmil Transaction ID")]
        public object RedmilTransactionID { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }

    public class UPIDynamicQR
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }
        public string Status { get; set; }

        [JsonProperty("Payer Name")]
        public string PayerName { get; set; }

        [JsonProperty("Payer Virtual ID")]
        public string PayerVirtualID { get; set; }

        [JsonProperty("Payment Amount")]
        public double PaymentAmount { get; set; }
        public double Charges { get; set; }

        [JsonProperty("Actual Wallet Credit")]
        public double ActualWalletCredit { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("Merchant Transaction ID")]
        public string MerchantTransactionID { get; set; }

        [JsonProperty("Redmil Reference ID")]
        public int RedmilReferenceID { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }

    public class UPIStaticQR
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Payer Name")]
        public string PayerName { get; set; }

        [JsonProperty("Payer Virtual ID")]
        public string PayerVirtualID { get; set; }

        [JsonProperty("Payment Amount")]
        public double PaymentAmount { get; set; }
        public double Charges { get; set; }

        [JsonProperty("Actual Wallet Credit")]
        public double ActualWalletCredit { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank RRN Number")]
        public string BankRRNNumber { get; set; }

        [JsonProperty("Merchant Transaction Id")]
        public string MerchantTransactionId { get; set; }

        [JsonProperty("Redmil Reference Id")]
        public int RedmilReferenceId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }

    #region Investment Services
    public class GoldInvestmentBuyModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Purchase Quantity")]
        public string PurchaseQuantity { get; set; }

        [JsonProperty("Gold rate Per Gm.")]
        public string GoldRatePerGm { get; set; }

        [JsonProperty("Purchase Amount")]
        public string PurchaseAmount { get; set; }

        [JsonProperty("Old Gold balance")]
        public string OldGoldBalance { get; set; }

        [JsonProperty("New Gold balance")]
        public string NewGoldBalance { get; set; }
        public string Status { get; set; }
        public double Commission { get; set; }

        [JsonProperty("PAN No")]
        public string PANNo { get; set; }

        [JsonProperty("Account No")]
        public string AccountNo { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("Customer ID")]
        public int CustomerID { get; set; }

        [JsonProperty("Transaction Id")]
        public string TransactionId { get; set; }

        [JsonProperty("Invoice ID")]
        public string InvoiceID { get; set; }

        [JsonProperty("Wallet Type")]
        public string WalletType { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class GoldInvestmentSellModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Name")]
        public string CustomerName { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Gold rate Per Gm.")]
        public string GoldRatePerGm { get; set; }

        [JsonProperty("Sell Quantity")]
        public string SellQuantity { get; set; }

        [JsonProperty("Sell Amount")]
        public string SellAmount { get; set; }

        [JsonProperty("Old Gold balance")]
        public string OldGoldBalance { get; set; }

        [JsonProperty("New Gold balance")]
        public string NewGoldBalance { get; set; }

        [JsonProperty("Bank Charges")]
        public string BankCharges { get; set; }

        [JsonProperty("Net Amount credited")]
        public string NetAmountCredited { get; set; }
        public string Status { get; set; }

        [JsonProperty("PAN No")]
        public string PANNo { get; set; }

        [JsonProperty("Account No")]
        public string AccountNo { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }

        [JsonProperty("Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("Transaction Id")]
        public string TransactionId { get; set; }

        [JsonProperty("Invoice ID")]
        public string InvoiceID { get; set; }

        [JsonProperty("Customer ID")]
        public int? CustomerID { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }

    }

    #endregion

    #region Recharge and Bill Payments
    public class BillPaymentsModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Biller Category")]
        public string BillerCategory { get; set; }

        [JsonProperty("Biller Name")]
        public string BillerName { get; set; }
        public string CCF { get; set; }

        [JsonProperty("Customer Input Field")]
        public string CustomerInputField { get; set; }

        [JsonProperty("Customer Input Field Value")]
        public string CustomerInputFieldValue { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Bill Amount")]
        public string BillAmount { get; set; }

        [JsonProperty("Commission Amount")]
        public string CommissionAmount { get; set; }

        [JsonProperty("Biller Reference Id")]
        public string BillerReferenceId { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }
        public string Status { get; set; }

        [JsonProperty("Wallet Type")]
        public string WalletType { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class DTHRechargeModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer ID")]
        public string CustomerID { get; set; }

        [JsonProperty("Operator Name")]
        public string OperatorName { get; set; }
        public double Amount { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Operator Transaction Id")]
        public string OperatorTransactionId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }

        [JsonProperty("Payment Mode")]
        public string PaymentMode { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class MobilePrepaidModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("Operator Name")]
        public string OperatorName { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Commission Amount")]
        public string CommissionAmount { get; set; }

        [JsonProperty("Payment Mode")]
        public string PaymentMode { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }

        [JsonProperty("Operator Transaction Id")]
        public string OperatorTransactionId { get; set; }
        public string CustomerID { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class MobilePostpaidModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Customer Mobile Number")]
        public object CustomerMobileNumber { get; set; }

        [JsonProperty("Operator Name")]
        public string OperatorName { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }

        [JsonProperty("Payment Mode")]
        public string PaymentMode { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public object RedmilTransactionId { get; set; }

        [JsonProperty("Operator Transaction Id")]
        public string OperatorTransactionId { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class LICPremiumPaymentsModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }
        public string CaNumber { get; set; }
        public string Status { get; set; }

        [JsonProperty("Response Message")]
        public string ResponseMessage { get; set; }

        [JsonProperty("Bill Pay Amount")]
        public double BillPayAmount { get; set; }

        [JsonProperty("Commission Amount")]
        public double CommissionAmount { get; set; }

        [JsonProperty("Bill Reference Id")]
        public string BillReferenceId { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public string RedmilTransactionId { get; set; }

        [JsonProperty("Wallet Type")]
        public string WalletType { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }

    #endregion

    #region View Receipt Models

    // View Receipt Start    

    public class AePSCashWithdrawalModel1
    {
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public float? Amount { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public double CommissionAmount { get; set; }
        public string AadhaarNumber { get; set; }
        public float BalAmount { get; set; }
        public string BankName { get; set; }
        public int IIN { get; set; }
        public string RRN { get; set; }
        public string NpciTransId { get; set; }
        public string Reason { get; set; }
        public int RedmilTransactionId { get; set; }
        public string ServiceType { get; set; }
        public double TransactionAmount { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class MicroATMCashWithdrawalModel1
    {
        public string ServiceType { get; set; }

        //[JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        //[JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        //[JsonProperty("Card Holder Name")]
        public string CardHolderName { get; set; }

        //[JsonProperty("Card Holder Mobile Number")]
        public string CardHolderContactNumber { get; set; }

        //[JsonProperty("Card Type")]
        public string CardType { get; set; }

        //[JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        //[JsonProperty("Transaction Amount")]
        public double? TransactionAmount { get; set; }

        //[JsonProperty("Balance Amount")]
        public double? BalanceAmount { get; set; }
        public string BankName { get; set; }

        public float? Balamount { get; set; }
        public string Status { get; set; }

        //[JsonProperty("Bank RRN Number")]
        public string RRN { get; set; }

        //[JsonProperty("NPCI Transaction Id")]
        public string NPCITransactionID { get; set; }

        //[JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionID { get; set; }

        //[JsonProperty("Failure Reason")]
        public string Reason { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class LICPremiumPaymentsModel1
    {
        public string ServiceType { get; set; }
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string caNumber { get; set; }
        public string BillerAmount { get; set; }
        public string Status { get; set; }
        public string BillerTransactionID { get; set; }
        public string RedmilTransactionID { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class BillPaymentsModel1
    {
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string CustomerMobile { get; set; }
        public string BillCategory { get; set; }
        public string BillerName { get; set; }
        public string BillerAmount { get; set; }
        public string BillerTransactionID { get; set; }
        public string RedmilTransactionID { get; set; }
        public string CustomerConvenienceFees { get; set; }
        public string CustomerInputField { get; set; }
        public string CustomerInputFieldValue { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class MobilePrepaidPostpaid
    {
        public string RequestDate { get; set; }

        public string RequestTime { get; set; }

        public string CustomerID { get; set; }

        public string OperatorName { get; set; }
        public string Amount { get; set; }

        public double CommissionAmount { get; set; }
        public string Status { get; set; }

        public string OperatorTransactionId { get; set; }

        public string RedmilTransactionId { get; set; }

        public string PaymentMode { get; set; }
        public string ServiceType { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }

        //public string RequestDate { get; set; }
        //public string RequestTime { get; set; }
        //public string Amount { get; set; }
        public string CustomerMobile { get; set; }
        //public string CustomerID { get; set; }
        //public string OperatorName { get; set; }
        //public string TransactionId { get; set; }
        //public string RedmilTransactionId { get; set; }
        //public string Status { get; set; }
        //public string ServiceType { get; set; }
        //public string DEMobile { get; set; }
        //public string DEName { get; set; }
    }
    public class CreditScoreModel1
    {
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNo { get; set; }
        public string TransactionAmount { get; set; }
        public string RedmilTransactionID { get; set; }
        public string CIBILScorePDFLink { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class FinoDMTModel1
    {
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string TransId { get; set; }
        public string reqId { get; set; }
        public string TransType { get; set; }
        public string RedmilTransId { get; set; }
        public string Unique { get; set; }
        public string sendName { get; set; }
        public string sendMobile { get; set; }
        public string recName { get; set; }
        public string recAcNum { get; set; }
        public string recBankName { get; set; }
        public string recIfsc { get; set; }
        public string Surcharge { get; set; }
        public string amount { get; set; }
        public string Status { get; set; }
        public string BankCharges { get; set; }
        public string ServiceType { get; set; }
        public string bankCharges { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    public class YBLDomesticMoneyTransferModel1
    {
        public string? RequestDate { get; set; }
        public string? RequestTime { get; set; }
        public string? SenderName { get; set; }
        public string? SenderMobileNumber { get; set; }
        public string? RedmilTransId { get; set; }
        public string? recMobile { get; set; }
        public string? recBankName { get; set; }
        public string? recAcNum { get; set; }
        public string? TransType { get; set; }
        public string? recName { get; set; }
        public string? Reason { get; set; }
        public string? rrn { get; set; }
        public string? recIfsc { get; set; }
        public string? Surcharge { get; set; }
        public string? Amount { get; set; }
        public string? Status { get; set; }
        public string? ServiceType { get; set; }
        public string DEMobile { get; set; }
        public string DEName { get; set; }
    }
    public class GoldInvestmentBuySellModel
    {
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string GoldRatePerGram { get; set; }
        public string TransactionAmount { get; set; }
        public string BankCharge { get; set; }
        public string NetAmount { get; set; }
        public string CustomerID { get; set; }
        public string OldGoldBalance { get; set; }
        public string NewGoldBalance { get; set; }
        public string cusInvoice { get; set; }
        public string Quantity { get; set; }
        public string InvoiceId { get; set; }
        public string RedmilTransactionID { get; set; }
        public string pan { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSC { get; set; }
        public string ServiceType { get; set; }
    }

    #region BusBookingsModel

    public class BusBookingModel
    {
        [JsonProperty("Sr.No")]
        public int? SrNo { get; set; }

        [JsonProperty("Booking Date")]
        public string? BookingDate { get; set; }

        [JsonProperty("Booking Time")]
        public string? BookingTime { get; set; }

        [JsonProperty("Passenger Name")]
        public string? PassengerName { get; set; }

        [JsonProperty("Passenger Contact")]
        public string? PassengerContact { get; set; }

        [JsonProperty("Booking Status")]
        public string? BookingStatus { get; set; }

        [JsonProperty("Base Fare")]
        public double? BaseFare { get; set; }

        [JsonProperty("Bus Operator GST")]
        public double? BusOperatorGST { get; set; }

        [JsonProperty("Booking Amount")]
        public double? BookingAmount { get; set; }

        [JsonProperty("Convenience Fees")]
        public double? ConvenienceFees { get; set; }

        [JsonProperty("Final Amount")]
        public double? FinalAmount { get; set; }

        [JsonProperty("Payment Mode")]
        public string? PaymentMode { get; set; }

        [JsonProperty("Commission Amount")]
        public double? CommissionAmount { get; set; }
        public double? Gst { get; set; }
        public double? Tds { get; set; }

        [JsonProperty("Cancellation Charges")]
        public string? CancellationCharges { get; set; }

        [JsonProperty("Refund Amount")]
        public string? RefundAmount { get; set; }

        [JsonProperty("Source/Pickup City")]
        public string? SourcePickupCity { get; set; }

        [JsonProperty("Destination City")]
        public string? DestinationCity { get; set; }

        [JsonProperty("Date Of Journey")]
        public string? DateOfJourney { get; set; }

        [JsonProperty("Pick Up Time")]
        public string? PickUpTime { get; set; }

        [JsonProperty("Drop Time")]
        public string? DropTime { get; set; }

        [JsonProperty("Travel Agency Name")]
        public string? TravelAgencyName { get; set; }

        [JsonProperty("Bus Name")]
        public string? BusName { get; set; }

        [JsonProperty("Pickup Address")]
        public string? PickupAddress { get; set; }

        [JsonProperty("Pickup Contact Number")]
        public string? PickupContactNumber { get; set; }

        [JsonProperty("Destination Address")]
        public string? DestinationAddress { get; set; }

        [JsonProperty("Destination Contact Number")]
        public string? DestinationContactNumber { get; set; }

        [JsonProperty("Seat Number")]
        public string? SeatNumber { get; set; }

        [JsonProperty("Booking Id")]
        public string? BookingId { get; set; }
        public string? DEName { get; set; }
        public string? DEMobile { get; set; }
    }


    #endregion

    #region model For View Page
    public class GoldInvestmentBuyModel1
    {
        public string? RequestDate { get; set; }
        public string? RequestTime { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerMobileNumber { get; set; }
        public string? GoldRatePerGram { get; set; }
        public string? TransactionAmount { get; set; }
        public string? BankCharge { get; set; }
        public string? NetAmount { get; set; }
        public string? CustomerID { get; set; }
        public string? OldGoldBalance { get; set; }
        public string? NewGoldBalance { get; set; }
        public string? cusInvoice { get; set; }
        public string? Quantity { get; set; }
        public string? InvoiceId { get; set; }
        public string? RedmilTransactionID { get; set; }
        public string? pan { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? IFSC { get; set; }
        public string? ServiceType = "BuyGold";
        public string? DEName { get; set; }
        public string? DEMobile { get; set; }


    }

    public class GoldInvestmentSellModel1
    {
        public string? RequestDate { get; set; }
        public string? RequestTime { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerMobileNumber { get; set; }
        public string? GoldRatePerGram { get; set; }

        public string? TransactionAmount { get; set; }
        public string? BankCharge { get; set; }
        public string? NetAmount { get; set; }
        public int? CustomerID { get; set; }

        public string? OldGoldBalance { get; set; }
        public string? NewGoldBalance { get; set; }
        public string? cusInvoice { get; set; }
        public string? InvoiceId { get; set; }
        public string? Quantity { get; set; }
        public string? RedmilTransactionID { get; set; }
        public string? pan { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }

        public string? IFSC { get; set; }
        public string? DEName { get; set; }
        public string? DEMobile { get; set; }
        public string? ServiceType = "SellGold";
        //public string? InvoiceId { get; set; }
        //public string? TransactionID { get; set; }
        //public string? SellQuantity { get; set; }

        //public string? CustomerInvoice { get; set; }
        //public string? CustomerPanNumber { get; set; }
        //public string? BankName { get; set; }

        //public string? IFSCCode { get; set; }

        //public string? NetAmountCredited { get; set; }

    }
    #endregion

    #region Model for Amazon Pay Gift Card
    public class AmazonPayGiftCardModel
    {
        [JsonProperty("Sr.No")]
        public int SrNo { get; set; }

        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Receiver Name")]
        public string ReceiverName { get; set; }

        [JsonProperty("Receiver Mobile Number")]
        public string ReceiverMobileNumber { get; set; }

        [JsonProperty("Receiver Email ID")]
        public string ReceiverEmailID { get; set; }
        public string Status { get; set; }

        [JsonProperty("Transaction Amount")]
        public double TransactionAmount { get; set; }

        [JsonProperty("Sender Name")]
        public string SenderName { get; set; }

        [JsonProperty("Sender Mobile")]
        public string SenderMobile { get; set; }
        public string Quantity { get; set; }

        [JsonProperty("Total Amount")]
        public double TotalAmount { get; set; }

        [JsonProperty("Card Number")]
        public string CardNumber { get; set; }

        [JsonProperty("Card Pin")]
        public string CardPin { get; set; }

        [JsonProperty("Amazon Reference Number")]
        public string AmazonReferenceNumber { get; set; }

        [JsonProperty("Gift Message")]
        public string GiftMessage { get; set; }

        [JsonProperty("Response Code")]
        public string ResponseCode { get; set; }

        [JsonProperty("Card Design category Name")]
        public string CardDesigncategoryName { get; set; }

        [JsonProperty("Card Design name")]
        public string CardDesignname { get; set; }

        [JsonProperty("Order Id")]
        public string OrderId { get; set; }
        public string TDS { get; set; }
        public string GST { get; set; }

        [JsonProperty("Commission Amount")]
        public string CommissionAmount { get; set; }

        [JsonProperty("Redmil Transaction Id")]
        public int RedmilTransactionId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
        public string ServiceType = "AmazonGiftCard";
    }

    #endregion


    #region Mobile Prepaid, Postpaid, DTH Request Model
    public class MobilePrepaidPostpaidDTHRechargeModel
    {
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string Amount { get; set; }
        public string CustomerMobile { get; set; }
        public string OperatorName { get; set; }
        public string TransactionId { get; set; }
        public string RedmilTransactionId { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }
    }
    #endregion


    #region ViewRecieptAmazonGiftCard
    public class ViewRecieptAmazonGiftCard
    {
        public string ServiceType { get; set; }
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string customerEmail { get; set; }
        public string Status { get; set; }
        public double tAmount { get; set; }
        public double Amount { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string qty { get; set; }
        public string card { get; set; }
        public string cardpin { get; set; }
        public string refno { get; set; }
        public string GiftMessage { get; set; }
        public string ResponseCode { get; set; }
        public string catname { get; set; }
        public string carddesign { get; set; }
        public string orderid { get; set; }
        public int RedmilTransId { get; set; }
        public string DEName { get; set; }
        public string DEMobile { get; set; }


    }


    #endregion
    // View Receipt End
    #endregion
}