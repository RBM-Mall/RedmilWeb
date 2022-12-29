using Newtonsoft.Json;

namespace Project_Redmil_MVC.Models.ResponseModel
{

    public class AdvanceWalletResponsePassbookDetailsModel : BaseResponseModel
    {
        internal string baseUrl;

        public int Id { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }

        [JsonProperty("Credit/Debit")]
        public string CreditDebit { get; set; }
        public string Client { get; set; }
        public DateTime Transaction_date { get; set; }
        public string Img { get; set; }
        public double Old_bal { get; set; }
        public double New_bal { get; set; }
        public int OpId { get; set; }
        public string Amount1 { get; internal set; }
        public string Detail { get; set;}
    }
    public class BRewardDetailResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public double Point { get; set; }

        [JsonProperty("Credit/Debit")]
        public string CreditDebit { get; set; }
        public string Client { get; set; }
        public DateTime Transaction_date { get; set; }
        public string Img { get; set; }
        public double Old_bal { get; set; }
        public double New_bal { get; set; }
        public int OpId { get; set; }
        public string baseUrl { get; set; }


    }
    public class RERewardResponseModel
    {
        public int Id { get; set; }
        public string Detail { get; set; }
        public string baseUrl { get; set; }
        public string Title { get; set; }
        public double Point { get; set; }

        [JsonProperty("Credit/Debit")]
        public string CreditDebit { get; set; }
        public string Client { get; set; }
        public DateTime Transaction_date { get; set; }
        public string Img { get; set; }
        public double Old_bal { get; set; }
        public double New_bal { get; set; }
        public int OpId { get; set; }


    }
}
