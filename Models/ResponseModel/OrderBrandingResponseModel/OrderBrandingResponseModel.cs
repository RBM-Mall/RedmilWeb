namespace Project_Redmil_MVC.Models.ResponseModel.OrderBrandingResponseModel
{
    public class OrderBrandingResponseModel
    {
        public int Id { get; set; }
        public string? MaterialName { get; set; }
        public string? Size { get; set; }
        public int SOrder { get; set; }
        public string? Usage { get; set; }
        public double Amount { get; set; }
        public string? MaterialType { get; set; }
        public string? ImgThumbnail { get; set; }
        public string? ImgSample { get; set; }
        public string? Qty { get; set; }
        public string? Pdfurl { get; set; }
    }
}
