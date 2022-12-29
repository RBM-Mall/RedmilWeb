namespace Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel
{
    public class GetCCFResponseModel:BaseResponseModel
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string Commission { get; set; }
        public string Surcharge { get; set; }
    }
}
