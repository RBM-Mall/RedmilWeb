namespace Project_Redmil_MVC.Models.ResponseModel.SelfHelpResponseModel
{
    public class SelfHelpResponseModel:BaseResponseModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Status { get; set; }
        public DateTime Reqdate { get; set; }

    }
}
