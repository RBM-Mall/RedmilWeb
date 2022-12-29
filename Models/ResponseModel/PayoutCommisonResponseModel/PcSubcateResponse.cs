namespace Project_Redmil_MVC.Models.ResponseModel.PayoutCommisonResponseModel
{
    public class PcSubcateResponse:BaseResponseModel
    {

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImgLing { get; set; }
        public DateTime Reqdate { get; set; }
        public int CategoryId { get; set; }

    }
}
