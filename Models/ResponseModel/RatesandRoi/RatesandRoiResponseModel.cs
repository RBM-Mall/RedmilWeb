namespace Project_Redmil_MVC.Models.ResponseModel.RatesandRoi
{
    public class RatesandRoiResponseModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImgLink { get; set; }
        public bool? Status { get; set; }
        public DateTime Reqdate { get; set; }
        public object? Editdate { get; set; }
    }
}
