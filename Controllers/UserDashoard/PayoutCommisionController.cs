using System.Dynamic;
using Project_Redmil_MVC.Models.RequestModel;
using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.Models.PayoutCommision;
using Project_Redmil_MVC.Models.ResponseModel.PayoutCommisonResponseModel;
using Project_Redmil_MVC.Models.RequestModel.PayoutCommisionRequestModel;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;

namespace Project_Redmil_MVC.Controllers.UserDashoard
{
    public class PayoutCommisionController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public PayoutCommisionController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        [HttpGet]
        public IActionResult Payoutcommision(string Userid)
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            ViewBag.Payout = new SelectList(PayoutCommision(), "Id", "CategoryName");
            return View();


        }
        #region PayoutCommision
        public List<PayOutCommisionResponseModel> PayoutCommision()
        {

            PayoutCommisionRequestModel obj = new PayoutCommisionRequestModel();
            
            List<PayOutCommisionResponseModel> lstresponse = new List<PayOutCommisionResponseModel>();
            try {
                var Data = new List<PayoutCommisionController>();
                obj.Userid = HttpContext.Session.GetString("Id").ToString();
                #region Checksum (addsender|Unique Key|UserId|)
                string input = Checksum.MakeChecksumString("ViewPayOutCategory", Checksum.checksumKey, obj.Userid, "");
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                obj.checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.ViewPayOutCategory}");
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/ViewPayOutCategory");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var Payoutdata = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                var data = Payoutdata.Data;
                var datalist = JsonConvert.DeserializeObject<List<PayOutCommisionResponseModel>>(JsonConvert.SerializeObject(data));
                lstresponse = datalist.ToList();
                return lstresponse;
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
            }
            return lstresponse;
        }
        #endregion

        #region SubCategoryPayOutCommision
        [HttpPost]
        public JsonResult SubCategoryPayOutCommision(string Category, string SubCategory)
         {
            var baseUrl = "https://api.redmilbusinessmall.com";
            var subCatData1 = new List<PcSubcateResponse>();
            PcSubcategoryRquestModel obj = new PcSubcategoryRquestModel();
            try
            {
                //PcSubcateResponse lst = new PcSubcateResponse();
                obj.Userid = HttpContext.Session.GetString("Id").ToString();
                obj.CategoryId = Category.ToString();
                #region Checksum (addsender|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("ViewPayOutCategoryWise", Checksum.checksumKey, obj.Userid, obj.CategoryId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                //var client  =  new RestClient("https://api.redmilbusinessmall.com/api/ViewPayOutCategoryWise");
                var client = new RestClient($"{Baseurl}{ApiName.ViewPayOutCategoryWise}");

                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                obj.checksum = CheckSum;
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var subpayoutdata = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (subpayoutdata.Statuscode == "TXN")
                    {
                        var data = subpayoutdata.Data;
                        if ((!string.IsNullOrEmpty(SubCategory) && int.Parse(SubCategory) > 0))
                        {
                            var datalist = JsonConvert.DeserializeObject<List<PcSubcateResponse>>(JsonConvert.SerializeObject(data));
                            var lstResponse = datalist.Where(x => x.Id == int.Parse(SubCategory));
                            if (lstResponse != null)
                            {

                                foreach (var item in lstResponse)
                                {
                                    subCatData1.Add(new PcSubcateResponse
                                    {
                                        ImgLing = baseUrl + item.ImgLing
                                    });
                                }
                            }

                            return Json(subCatData1);
                        }
                        else
                        {
                            var datalist = JsonConvert.DeserializeObject<List<PcSubcateResponse>>(JsonConvert.SerializeObject(data));
                            List<PcSubcateResponse> lstResponse = datalist.Where(x => x.CategoryId == int.Parse(Category)).ToList();
                            return Json(lstResponse);
                        }
                    }
                    else if (subpayoutdata.Statuscode == "ERR")
                    {
                        return Json(subpayoutdata);    
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                    
                }
                

            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
    }
}



















