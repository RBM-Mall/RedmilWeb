namespace Project_Redmil_MVC.Models.ResponseModel.HowToWork
{
    public class HowToWork
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgLink { get; set; }
        public DateTime Reqdate { get; set; }
        public object Editdate { get; set; }
        public int CategoryId { get; set; }
        public string Thumbnail { get; set; }
        public string Folder { get; set; }
    }
}
