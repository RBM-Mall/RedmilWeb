using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel.ApplyCoupan;
using Project_Redmil_MVC.Models.RequestModel.UpgradeSubscription;
using Project_Redmil_MVC.Models.RequestOnline.RequestOnlinePay;
using Project_Redmil_MVC.Models.ResponseModel.UpgradeSubscriptionResponseModel;
using Project_Redmil_MVC.Models.RequestModel.PaywithWalletRequestModel;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.GetUserSubscriptionDetails;
using Project_Redmil_MVC.Models.ResponseModel.GetUserSubscriptionDetailsResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;

namespace Project_Redmil_MVC.Controllers.UserDashoard
{
    public class upgradesubscriptionsController : Controller
    {
        public List<UpgradeSubscriptionDatumResponseModel> lstResponse;
        public List<UpgradeSubscriptionAddOnResponseModel> lstDataAddon;
        concatemodels con = new concatemodels();

        private readonly string Baseurl;
        private readonly IConfiguration _config;

        public double GST { get; private set; }

        public upgradesubscriptionsController(IConfiguration config)
        {
            _config = config;
            Baseurl = "https://api.redmilbusinessmall.com/api/";
            HelperMethod.GetBaseURl(_config);
        }
        public IActionResult Index()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            return View();
        }


        #region getSubscription
        public IActionResult getSubscription(string foropenpdf, string showplan, string showprice)
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            ViewBag.Subscribed = "Subscribed";
            //GetUserSubscriptionDetails();
            var Mallname = HttpContext.Session.GetString("Mallname");
            ViewBag.Mallname = Mallname;
            var baseUrl = "https://api.redmilbusinessmall.com";
            var datainsert = new List<UpgradeSubscriptionDatumResponseModel>();
            var datainsert1 = new List<UpgradeSubscriptionAddOnResponseModel>();
            //List<UpgradeSubscriptionDatumResponseModel> lstresponse = new List<UpgradeSubscriptionDatumResponseModel>();
            var Data = new List<upgradesubscriptionsController>();
            GetSubscriptionPlanReuestModel requestModel = new GetSubscriptionPlanReuestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();

                requestModel.Token = "";
                #region Checksum (addsender|Unique Key|UserId)
                string input = Checksum.MakeChecksumString(ApiName.GetSubscriptionPlan, Checksum.checksumKey, requestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.GetSubscriptionPlan}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                requestModel.checksum = CheckSum;
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else
                {
                    var dataAddOn = JsonConvert.DeserializeObject<GetSubscriptionPlanBaseResponseModel>(result);
                    if (dataAddOn.Statuscode == "TXN")
                    {
                        var data = dataAddOn.Data;
                        var data1 = dataAddOn.AddOn;
                        var datalist = JsonConvert.DeserializeObject<List<UpgradeSubscriptionDatumResponseModel>>(JsonConvert.SerializeObject(data));
                        lstDataAddon = JsonConvert.DeserializeObject<List<UpgradeSubscriptionAddOnResponseModel>>(JsonConvert.SerializeObject(data1));
                        concatemodels con1 = new concatemodels()
                        {
                            addOn = datainsert1,

                        };
                        if (!string.IsNullOrEmpty(showplan) && !string.IsNullOrEmpty(showprice))
                        {
                            var planName = Split(showplan);
                            lstDataAddon = lstDataAddon.Where(x => x.PlanName == planName).ToList();
                            if (lstDataAddon != null)
                            {
                                foreach (var item in lstDataAddon)
                                {
                                    datainsert1.Add(new UpgradeSubscriptionAddOnResponseModel
                                    {
                                        Id = item.Id,
                                        PlanName = item.PlanName,
                                        Price = item.Price,
                                        PlanType = item.PlanType,
                                        Icon = baseUrl + item.Icon,
                                        Desc = item.Desc,
                                        ItemName = item.ItemName,
                                        PlanId = item.PlanId
                                    });
                                }
                                return Json(con1); ;
                                //return View();
                            }
                        }
                        lstResponse = datalist.ToList();
                        string Subscribed = "";
                        var a111 = string.Empty;
                        List<GetUserSubscriptionDetailsResponseModel> lsassa = new List<GetUserSubscriptionDetailsResponseModel>();
                        lsassa = GetUserSubscriptionDetails();
                        if (lsassa.Count > 0)
                        {
                            var a1 = lstResponse.Where(x => x.PlanName.Equals(lsassa.FirstOrDefault().PlanName));
                            a111 = a1.FirstOrDefault().PlanName;

                            //con.DataumModel.AddRange(lstResponse);
                            if (!string.IsNullOrEmpty(foropenpdf))
                            {
                                var a = baseUrl + lstResponse.Where(x => x.PlanName == foropenpdf).FirstOrDefault().SampleImg;
                                //baseUrl + item.ImgLink
                                return Json(a);
                            }
                            //double GS,TotaL;
                            if (lstResponse != null)
                            {
                                foreach (var item in lstResponse)
                                {
                                    datainsert.Add(new UpgradeSubscriptionDatumResponseModel
                                    {

                                        Id = item.Id,
                                        PlanName = item.PlanName,
                                        Price = item.Price,
                                        GST = Math.Round((item.Price * 18) / 100),
                                        Total = (item.Price + Math.Round((item.Price * 18) / 100)),
                                        Status = item.Status,
                                        Img = baseUrl + item.Img,
                                        PlanType = item.PlanType,
                                        SampleImg = baseUrl + item.SampleImg,
                                        Subscribed = Subscribed,

                                    });
                                }



                                datainsert.Where(x => x.PlanName == a111).FirstOrDefault().Subscribed = "Subscribed";
                                //con.DataumModel.AddRange(lstResponse);
                                concatemodels con2 = new concatemodels()
                                {
                                    DataumModel = datainsert
                                };
                                return View(con2);

                            }
                        }
                        else
                        {
                            foreach (var item in lstResponse)
                            {
                                datainsert.Add(new UpgradeSubscriptionDatumResponseModel
                                {

                                    Id = item.Id,
                                    PlanName = item.PlanName,
                                    Price = item.Price,
                                    GST = Math.Round((item.Price * 18) / 100),
                                    Total = (item.Price + Math.Round((item.Price * 18) / 100)),
                                    Status = item.Status,
                                    Img = baseUrl + item.Img,
                                    PlanType = item.PlanType,
                                    SampleImg = baseUrl + item.SampleImg,
                                    Subscribed = Subscribed,

                                });
                            }
                            concatemodels con2 = new concatemodels()
                            {
                                DataumModel = datainsert
                            };
                            return View(con2);

                        }
                        return View(con1);
                    }
                    else if (dataAddOn.Statuscode == "ERR")
                    {
                        return View(dataAddOn);
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
                requestModel1.Data = requestModel;
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



        #region ApplyCoupan
        [HttpPost]
        public JsonResult ApplyCoupan(string PlanId, string CouponCode)
        {
            RequestApplyCoupanCode obj = new RequestApplyCoupanCode();
            try
            {
                obj.PlanId = PlanId;
                obj.CouponCode = CouponCode;
                obj.Userid = "2084";
                #region Checksum (addsender|Unique Key|UserId)
                string input = Checksum.MakeChecksumString(ApiName.ValidateSubscriptionCouponCode, Checksum.checksumKey, obj.Userid, obj.PlanId, obj.CouponCode);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient($"{Baseurl}{ApiName.ValidateSubscriptionCouponCode}");
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
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        var ds = JsonConvert.DeserializeObject<List<ApplyCoupanCodeResponseModel>>(deserialize.Data.ToString());
                        return Json(ds);

                    }
                    //if (deserialize.Statuscode == "ERR")
                    //{

                    //    var ds = JsonConvert.DeserializeObject<ApplyCoupanCodeResponseModel>(deserialize.ToString());
                    //    return Json(ds);
                    //}
                    return Json(deserialize);
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



        #region Split
        [HttpPost]
        public string Split(string value)
        {
            string param = value;
            string rvalue = string.Empty;
            if (param.Contains(','))
            {
                string[] Param = param.Split(',');
                rvalue = Param[0].Trim();
            }
            else
            {
                rvalue = value;
            }
            return rvalue;

        }
        #endregion


        public Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary GetTempData()
        {
            return TempData;
        }


        #region OnlinePayWallet
        [HttpPost]
        public JsonResult OnlinePayWallet(string Amount, string PlanId, string checksum, string PlanType, string OrderId, string RequestData, string Itemname)
        {

            RequestOnlinePayModel obj = new RequestOnlinePayModel();
            try
            {
                obj.RequestData = "";
                obj.Amount = Amount;
                obj.Userid = HttpContext.Session.GetString("Id").ToString();
                obj.Token = "";
                obj.Mode = "Online";
                obj.PlanType = PlanType;
                obj.AddOnItems = Itemname;
                var name = HttpContext.Session.GetString("Name");  
                var mobile = HttpContext.Session.GetString("Mobile").ToString();
                var email = HttpContext.Session.GetString("Email");
                var Mallname = HttpContext.Session.GetString("Mallname");
                //ViewBag.name = name;
                string OrderId1 = string.Format("{0}{1}", obj.Userid, DateTime.Now.ToString("ssmmHHddMMyyyy"));
                OrderId1 = "S" + PlanId + "P" + OrderId1;
                obj.OrderId = OrderId1;
                string transaction_id = string.Format("{0}{1}", obj.Userid, DateTime.Now.ToString("yyyyMMddHHmmss"));
                transaction_id = transaction_id;
                var webViewURL = "https://redmilbusinessmall.com/ccavRequestHandler.aspx?tid=" + transaction_id + "&order_id=" + OrderId1 + "&amount=" + obj.Amount + "&billing_name=" + name +
                       "&billing_tel=" + mobile + "&billing_email=" + email;
                #region Checksum (addsender|Unique Key|UserId ) 
                string input = Checksum.MakeChecksumString(ApiName.InsertOnlinepaymentData, Checksum.checksumKey, obj.Userid, obj.Amount, obj.Mode, obj.OrderId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/InsertOnlinepaymentData");
                //var client = new RestClient($"{Baseurl}{ApiName.ValidateSubscriptionCouponCode}");
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
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    return Json(new { deserialize, webViewURL, name });

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




        #region PayWithWallet
        [HttpPost]
        public JsonResult PayWithWallet(string Amount, string PlanId, string checksum, string Itemname, string CouponCode, string DiscountedAmount, string Wallet)
        {
            PayWithWalletRequest obj = new PayWithWalletRequest();
            try
            {
                //AddOnItems = "";
                obj.Amount = Amount;
                obj.UserId = HttpContext.Session.GetString("Id").ToString();
                obj.AddOnItems = Itemname;
                obj.PlanId = PlanId;
                obj.Wallet = Wallet;
                if (!string.IsNullOrEmpty(CouponCode))
                {
                    obj.CouponCode = CouponCode;
                }
                else
                {
                    obj.CouponCode = "";
                }
                obj.DiscountedAmount = DiscountedAmount;
                #region Checksum (addsender|Unique Key|UserId ) 
                string input = Checksum.MakeChecksumString(ApiName.OfflinePaymentForSubscriptionPackageUserWiseWithCoupon, Checksum.checksumKey, obj.UserId, obj.PlanId, obj.Amount, obj.CouponCode, obj.DiscountedAmount);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                obj.checksum = CheckSum;
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/OfflinePaymentForSubscriptionPackageUserWiseWithCoupon");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if(string.IsNullOrEmpty(result)) {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else {
                    var Payoutdata = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    return Json(Payoutdata);
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


        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            try
            {
                getBalanceRequestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                getBalanceRequestModel.checksum = CheckSum;
                //API URL Has been changed by Siddhartha Sir
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/Getbalance");
                var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    var data = deserialize.Data;
                    var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));
                    List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
                    lstdata = datalist.ToList();
                    return Json(lstdata);
                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = getBalanceRequestModel;
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

        #region GetUserSubscriptionDetails
        [HttpPost]
        public List<GetUserSubscriptionDetailsResponseModel> GetUserSubscriptionDetails()
        {

            GetUserSubscriptionDetailsRequestModel GetUserSubscriptionDetailsobj = new GetUserSubscriptionDetailsRequestModel();
            List<GetUserSubscriptionDetailsResponseModel> lstdata = new List<GetUserSubscriptionDetailsResponseModel>();
            try
            {
                GetUserSubscriptionDetailsobj.UserId = HttpContext.Session.GetString("Id").ToString();
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("GetUserSubscriptionDetails", Checksum.checksumKey, GetUserSubscriptionDetailsobj.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                GetUserSubscriptionDetailsobj.checksum = CheckSum;
                //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetUserSubscriptionDetails");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(GetUserSubscriptionDetailsobj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deserialize.Statuscode == "TXN")
                {
                    var data = deserialize.Data;
                    var datalist = JsonConvert.DeserializeObject<List<GetUserSubscriptionDetailsResponseModel>>(JsonConvert.SerializeObject(data));

                    lstdata = datalist.ToList();

                    return lstdata;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return lstdata;
                }
                return lstdata;
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = GetUserSubscriptionDetailsobj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
            }
            return lstdata;
        }
        #endregion
    }
}
