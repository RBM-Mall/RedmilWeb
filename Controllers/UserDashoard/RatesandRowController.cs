using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.RatesandRoiRequestmodel;
using Project_Redmil_MVC.Models.ResponseModel.RatesandRoi;

namespace Project_Redmil_MVC.Controllers.UserDashoard
{
    public class RatesandRowController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        //public string baseURL = "api.redmilbusinessmall.com";
        public RatesandRowController(IConfiguration config)
        {
            _config = config;
            Baseurl = "https://proapitest4.redmilbusinessmall.com/api/";
            HelperMethod.GetBaseURl(_config);

        }
        public IActionResult Index()
        {
            return View();
        }


        #region RatesandRow
        public IActionResult RatesandRow(string Id,string Cat)
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            var baseUrl = "https://api.redmilbusinessmall.com";
            RatesandRoiRequestModel obj = new RatesandRoiRequestModel();
            var datainsert = new List<RatesandRoiResponseModel>();
            List<RatesandRoiResponseModel> lstresponse = new List<RatesandRoiResponseModel>();
            var Data = new List<RatesandRowController>();
            try
            {
                obj.UserId = HttpContext.Session.GetString("Id").ToString();
                obj.Token = "";

                #region Checksum (addsender|Unique Key|UserId|)
                string input = Checksum.MakeChecksumString("ViewPayOutCategory", obj.UserId, obj.Token);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                obj.checksum = CheckSum;
                var client = new RestClient("https://proapitest4.redmilbusinessmall.com/api/ViewRateOfIntrest");
                //var client = new RestClient($"{Baseurl}{ApiName.ViewRateOfIntrest}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else
                {
                    var Payoutdata = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if(Payoutdata.Statuscode=="TXN")
                    {
                        var data = Payoutdata.Data;
                        var datalist = JsonConvert.DeserializeObject<List<RatesandRoiResponseModel>>(JsonConvert.SerializeObject(data));
                        lstresponse = datalist.ToList();
                        if (!string.IsNullOrEmpty(Cat))
                        {
                            var a = baseUrl + lstresponse.Where(x => x.Title == Cat).FirstOrDefault().ImgLink;
                            //baseUrl + item.ImgLink
                            return Json(a);

                        }
                        if (lstresponse != null)
                        {
                            foreach (var item in lstresponse)
                            {
                                datainsert.Add(new RatesandRoiResponseModel
                                {
                                    Id = item.Id,
                                    Title = item.Title,
                                    ImgLink = baseUrl + item.ImgLink
                                });
                            }
                            return View(datainsert);

                        }
                        return View(lstresponse);
                    }
                    else if(Payoutdata.Statuscode == "ERR")
                    {
                        return View(lstresponse);
                    }
                    else
                    {
                        return RedirectToAction("ErrorForExceptionLog", "Error");
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
                return RedirectToAction("ErrorForExceptionLog", "Error");
            }
        }
        #endregion
    }
}
