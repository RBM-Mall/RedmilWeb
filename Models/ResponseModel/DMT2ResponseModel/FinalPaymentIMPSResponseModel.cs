namespace Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel
{
    public class FinalPaymentIMPSResponseModel
    {
        [JsonProperty("Request Date")]
        public string RequestDate { get; set; }

        [JsonProperty("Request Time")]
        public string RequestTime { get; set; }

        [JsonProperty("Request ID")]
        public string RequestID { get; set; }

        [JsonProperty("Client Unique ID")]
        public string ClientUniqueID { get; set; }

        [JsonProperty("Sender Mobile Number")]
        public string SenderMobileNumber { get; set; }

        [JsonProperty("Sender Name")]
        public string SenderName { get; set; }

        [JsonProperty("Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [JsonProperty("Beneficiary IFSC Code")]
        public string BeneficiaryIFSCCode { get; set; }

        [JsonProperty("Account Number")]
        public string AccountNumber { get; set; }

        [JsonProperty("Transfer Mode")]
        public string TransferMode { get; set; }

        [JsonProperty("Transfer Amount")]
        public string TransferAmount { get; set; }

        [JsonProperty("Bank Charges")]
        public string BankCharges { get; set; }

        [JsonProperty("Net Debit")]
        public string NetDebit { get; set; }
        public string Status { get; set; }
        public string DisplayMessage { get; set; }
    }
}
