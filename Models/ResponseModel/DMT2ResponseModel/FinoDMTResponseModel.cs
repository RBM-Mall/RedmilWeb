namespace Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel
{
    public class FinoDMTResponseModel
    {
        //public List<FinoDMTModel> finoDMTModel { get; set; }
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
            public object SenderMobileNumber { get; set; }

            [JsonProperty("Transaction Amount")]
            public double TransactionAmount { get; set; }
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
    }
}
