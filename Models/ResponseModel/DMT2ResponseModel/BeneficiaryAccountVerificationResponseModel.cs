namespace Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel
{
    public class BeneficiaryAccountVerificationResponseModel:BaseBillResponseModel
    {
        public List<Data> data { get; set; }
        public List<AdData> additionalInfo { get; set; }


        public class AdData
        {
            public string ActCode { get; set; }
            public string TxnID { get; set; }
            public double AmountRequested { get; set; }
            public double ChargesDeducted { get; set; }
            public double TotalAmount { get; set; }
            public string BeneName { get; set; }
            public object Rfu1 { get; set; }
            public object Rfu2 { get; set; }
            public object Rfu3 { get; set; }
            public string TransactionDatetime { get; set; }
            public string TxnDescription { get; set; }
        }

        public class Data
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int ServiceId { get; set; }
            public int OpId { get; set; }
            public long MobileNo { get; set; }
            public double Amount { get; set; }
            public double Comm { get; set; }
            public bool CommType { get; set; }
            public double CommAmount { get; set; }
            public double Surcharge { get; set; }
            public bool SurType { get; set; }
            public double SurAmount { get; set; }
            public double Gst { get; set; }
            public double Tds { get; set; }
            public double Rewards { get; set; }
            public double Cost { get; set; }
            public double OpBalane { get; set; }
            public double ClBalane { get; set; }
            public string PMode { get; set; }
            public object Mode { get; set; }
            public int ApiId { get; set; }
            public DateTime Editdate { get; set; }
            public DateTime Reqdate { get; set; }
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
            public string Status { get; set; }
            public string TransactionType { get; set; }
            public string FinoRequestId { get; set; }
            public string ResponseCode { get; set; }
            public string MessageString { get; set; }
            public string DisplayMessage { get; set; }
            public string ClientID { get; set; }
            public string TransactionID { get; set; }
            public string SenderName { get; set; }
            public string SenderMobile { get; set; }
            public string BeneficiaryName { get; set; }
            public string AccountNumber { get; set; }
            public string IFSCCode { get; set; }
            public string BankName { get; set; }
        }


    }
}
