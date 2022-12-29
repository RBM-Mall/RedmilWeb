using Project_Redmil_MVC.Models.RequestModel.ForimageRequestModel;
using Project_Redmil_MVC.Models.RequestModel.InsertMultiAccountDetailForUserRequestModel;
using Project_Redmil_MVC.Models.ResponseModel.GetBank;

namespace Project_Redmil_MVC.Models.ResponseModel.PassbookGetCashOutSurchargeNewResponseModel
{
    public class PassbookGetCashsurchargeRespnseModel: GetBankResponseModel
    {
        public Imps imps { get; set; }
        public Neft neft { get; set; }
        public Rtgs rtgs { get; set; }
        public InserMutltiAccount InserMutltiAccount;
    }

    public class Imps
    {
        public string Amount { get; set; }
        public string Timing { get; set; }
        public string Days { get; set; }
        public List<Slab> Slab { get; set; }
    }

    public class Neft
    {
        public string Amount { get; set; }
        public string Timing { get; set; }
        public string Days { get; set; }
        public List<Slab> Slab { get; set; }
    }


    public class Rtgs
    {
        public string Amount { get; set; }
        public string Timing { get; set; }
        public string Days { get; set; }
        public List<Slab> Slab { get; set; }
    }

    public class Slab
    {
        public string Slabname { get; set; }
        public string SlabAmount { get; set; }
        public string ServiceId { get; set; }
        public string Opid { get; set; }
    }
    //public class GetBankResponseModel
    //{
    //    public int Id { get; set; }
    //    public string BankName { get; set; }
    //}
}
