using Newtonsoft.Json;
using Project_Redmil_MVC.Models.ResponseModel.GetBank;

namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class CashWalletRecentResponseDetailsPassbookModel:BaseResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }

        [JsonProperty("Credit/Debit")]
        public string CreditDebit { get; set; }
        public string Client { get; set; }
        public string? GstAmount { get; set; }
        public string? TdsAmount { get; set; }
        public DateTime Transaction_date { get; set; }
        public string Img { get; set; }
        public string baseUrl { get; set; }
        public string Type { get; set; }
        public double Old_bal { get; set; }
        public string New_bal { get; set; }
        public int OpId { get; set; }
        public string Amount1 { get; internal set; }
        public string? Detail { get; set; }
        public List<AdvanceWalletResponsePassbookDetailsModel> AdvanceWalletResponsePassbookDetailsModel { get; set; }
        public List<BRewardDetailResponseModel> BRewardDetailResponseModel { get; set; }
        public List<RERewardResponseModel> RERewardResponseModel { get; set; }
        public List<GetBalanceResponseModel> getBalanceResponseModels { get; set; }
        public List<GetBankResponseModel> getBankResponseModels { get; set; }
     

    }

}
