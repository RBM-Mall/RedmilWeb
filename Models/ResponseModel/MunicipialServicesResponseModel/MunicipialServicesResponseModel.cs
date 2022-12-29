namespace Project_Redmil_MVC.Models.ResponseModel.MunicipialServicesResponseModel
{
    public class MunicipialServicesResponseModel
    {
        public Billerinfo[] billerInfo { get; set; }
    }
    public class Billerinfo
    {
        public string Id { get; set; }
        public string Bbps { get; set; }
        public string Operatorname { get; set; }
        public string ServiceId { get; set; }
        public string Img { get; set; }
        public string ImgSample { get; set; }
        public string BillValidation { get; set; }
        public string FetchRequiremet { get; set; }
        public string BillerCategory { get; set; }
        public string BillerAdhoc { get; set; }
        public string BillerCoverage { get; set; }
        public string ParametersAll { get; set; }
        public string Exactness { get; set; }
        public Inputparam[] inputParam { get; set; }
    }

    public class Inputparam
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string MaxLength { get; set; }
        public string Optional { get; set; }
    }
    public class Operatornames
    {
        public string Id { get; set; }
        public string Operatorname { get; set; }
    }
}
