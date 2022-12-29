namespace Project_Redmil_MVC.Models.ResponseModel.PassBookGetMultiAccountDetailResponseModel
{
    public class GetMultiAccountDetailResponse
    {

        public string AccountNo { get; set; }
        public string Ifsc { get; set; }
        public string BeniName { get; set; }
        public string BankName { get; set; }
        public int BankId { get; set; }
        public List<GetMultiAccountDetailResponse> RERewardResponseModel { get; set; }
    }

}
