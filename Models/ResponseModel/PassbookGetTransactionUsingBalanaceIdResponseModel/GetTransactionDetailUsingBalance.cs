namespace Project_Redmil_MVC.Models.ResponseModel.PassbookGetTransactionUsingBalanaceIdResponseModel
{
    public class GetTransactionDetailUsingBalance
    {
        public string Logo { get; set; }
        public string Title { get; set; }

        [JsonProperty("Transaction ID")]
        public int TransactionID { get; set; }

        [JsonProperty("Benificiary Name")]
        public string BenificiaryName { get; set; }

        [JsonProperty("Benificiary Account No.")]
        public string BenificiaryAccountNo { get; set; }

        [JsonProperty("IFSC Code")]
        public string IFSCCode { get; set; }

        [JsonProperty("Sender Name")]
        public string SenderName { get; set; }

        [JsonProperty("Sender Mobile No")]
        public string SenderMobileNo { get; set; }

        [JsonProperty("Payment Mode")]
        public string PaymentMode { get; set; }

        [JsonProperty("Transaction Amount")]
        public string TransactionAmount { get; set; }
        public string Surcharge { get; set; }

        [JsonProperty("Debited Amount")]
        public string DebitedAmount { get; set; }
        public string Status { get; set; }

        [JsonProperty("Bank reference number (RRN)")]
        public string BankreferencenumberRRN { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
