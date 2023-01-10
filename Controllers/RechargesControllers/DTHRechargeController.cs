using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.DTHRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.DTHResponseModel;
using System.Text.RegularExpressions;

namespace Project_Redmil_MVC.Controllers.RechargesControllers
{
    public class DTHRechargeController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;

        public List<GetDTHAllPlansResponseModel> getDTHAllPlans;
        public DTHRechargeController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult DTHRecharge()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");
            }
            ViewBag.Operator = new SelectList(DTHOperatorList(), "Operatorname", "Operatorname");
            return View();
        }

        #region final payment
        [HttpPost]
        public JsonResult DTHRecharge(string opName, string cutomerid, string amount, string payment)
        {
            DTHRechargeRequestModel dTHRechargeRequestModel = new DTHRechargeRequestModel();
            try
            {
                dTHRechargeRequestModel.Userid = "2084";
                dTHRechargeRequestModel.Mobileno = cutomerid;
                dTHRechargeRequestModel.Amount = ToDigitsOnly(amount);
                dTHRechargeRequestModel.Wallet = payment;
                var responseOperators = DTHOperatorList();
                dTHRechargeRequestModel.OpId = responseOperators.Where(x => x.Operatorname == opName).FirstOrDefault().Id.ToString();
                dTHRechargeRequestModel.ServiceId = responseOperators.Where(x => x.Operatorname == opName).FirstOrDefault().ServiceId.ToString();
                dTHRechargeRequestModel.Mode = "App";
                #region Checksum (Recharge|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Recharge", Checksum.checksumKey, dTHRechargeRequestModel.Userid,
                    dTHRechargeRequestModel.ServiceId.Trim(), dTHRechargeRequestModel.OpId.Trim(), dTHRechargeRequestModel.Mobileno.Trim(),
                    dTHRechargeRequestModel.Mode, dTHRechargeRequestModel.Amount.Trim(), dTHRechargeRequestModel.Wallet);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                dTHRechargeRequestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/Recharge"); 
                var client = new RestClient($"{Baseurl}{ApiName.Recharge}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(dTHRechargeRequestModel);
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
                    if (deserialize.Statuscode == "TXN")
                    {
                        var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<PrepaidRechargeResponseModel>>>(response.Content);

                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<PrepaidRechargeResponseModel>>>(response.Content);

                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
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
                requestModel1.Data = dTHRechargeRequestModel;
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


        #region DTHOperatorList
        public List<GetDTHOperatorListResponseModel> DTHOperatorList()
        {
            GetDTHOperatorListRequestModel requestModel = new GetDTHOperatorListRequestModel();
            List<GetDTHOperatorListResponseModel> lstresponse = new List<GetDTHOperatorListResponseModel>();
            try
            {
                requestModel.Userid = "2084";
                requestModel.ServiceId = "23";


                #region Checksum (GetOperaterList|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("GetOperaterList", Checksum.checksumKey, requestModel.Userid, requestModel.ServiceId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion

                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/GetOperaterList");
                var client = new RestClient($"{Baseurl}{ApiName.GetOperaterList}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);

                if (deserialize.Statuscode == "TXN")
                {
                    var data = deserialize.Data;
                    var dataList = JsonConvert.DeserializeObject<List<GetDTHOperatorListResponseModel>>(JsonConvert.SerializeObject(data));
                    if (dataList != null)
                    {
                        foreach (var item in dataList)
                        {
                            lstresponse.Add(new GetDTHOperatorListResponseModel
                            {
                                Id = item.Id,
                                Operatorname = item.Operatorname,
                                Opcode = item.Opcode,
                                Img = Baseurl + item.Img,
                                ServiceId = item.ServiceId,
                            });
                        }
                    }
                    return lstresponse;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return lstresponse;
                }
                else
                {

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
            }
            return lstresponse;

        }
        #endregion



        #region GetAllPlansDTH For Every Operator
        [HttpPost]
        public List<GetDTHAllPlansResponseModel> GetAllDTHPlans(string opName)
        {
            GetDTHAllPlansRequestModel requestModel = new GetDTHAllPlansRequestModel();
            try
            {
                requestModel.Userid = "2084";
                requestModel.Circle = "";
                requestModel.OpName = opName;
                #region Checksum (DthPlan|Unique Key|UserId|ServiceId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deserialize != null && deserialize.Statuscode == "TXN")
                {
                    var data = deserialize.Data;
                    var datalist = JsonConvert.DeserializeObject<List<GetDTHAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                    getDTHAllPlans = datalist.ToList();
                    //GetAirtelHDPack("Airtel HD Pack");
                    return getDTHAllPlans;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return getDTHAllPlans;
                }
                else
                {

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
            }
            return getDTHAllPlans;


        }
        #endregion



        #region GetFirstDTHPlan For All Operator 
        [HttpPost]
        public JsonResult GetFirstDTHPlan(string opName, string planName)
        {
            GetDTHAllPlansRequestModel requestModel = new GetDTHAllPlansRequestModel();
            try
            {
                requestModel.Userid = "2084";
                requestModel.Circle = "";
                requestModel.OpName = opName;
                #region Checksum (DthPlan|Unique Key|UserId|ServiceId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
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
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetDTHAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        getDTHAllPlans = datalist.ToList();
                        if (opName == "Airtel DTH")
                        {
                            var a = getDTHAllPlans.Where(x => x.PlanName == "Airtel SD Pack");
                            if (!string.IsNullOrEmpty(planName))
                            {
                                var b = getDTHAllPlans.Where(x => x.PlanName == planName);
                                return Json(b);
                            }
                            return Json(a);
                        }
                        else if (opName == "DishTV")
                        {
                            var a = getDTHAllPlans.Where(x => x.PlanName == "Kannada South Combo Pack");
                            if (!string.IsNullOrEmpty(planName))
                            {
                                var b = getDTHAllPlans.Where(x => x.PlanName == planName);
                                return Json(b);
                            }
                            return Json(a);
                        }
                        else if (opName == "Videocon D2H")
                        {
                            var a = getDTHAllPlans.Where(x => x.PlanName == "Ala Carte Top Up");
                            if (!string.IsNullOrEmpty(planName))
                            {
                                var b = getDTHAllPlans.Where(x => x.PlanName == planName);
                                return Json(b);
                            }
                            return Json(a);
                        }
                        else if (opName == "Tata Sky DTH")
                        {
                            var a = getDTHAllPlans.Where(x => x.PlanName == "OTT Combo Packs");
                            if (!string.IsNullOrEmpty(planName))
                            {
                                var b = getDTHAllPlans.Where(x => x.PlanName == planName);
                                return Json(b);
                            }
                            return Json(a);
                        }
                        else if (opName == "Sun Direct DTH")
                        {
                            var a = getDTHAllPlans.Where(x => x.PlanName == "Malayalam Curated Pack");
                            if (!string.IsNullOrEmpty(planName))
                            {
                                var b = getDTHAllPlans.Where(x => x.PlanName == planName);
                                return Json(b);
                            }
                            return Json(a);
                        }
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
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
                requestModel1.Data = requestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
            return Json("");
        }
        #endregion


        #region GetAllPlansWithPlanName
        [HttpPost]
        public JsonResult GetAllPlansWithPlanName(string planName)
        {
            var a = getDTHAllPlans.Where(x => x.PlanName == planName).ToList();
            return Json(a);
        }
        #endregion


        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            try
            {
                getBalanceRequestModel.Userid = "2084";
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
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));
                        List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
                        lstdata = datalist.ToList();
                        return Json(lstdata);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
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

        private string ToDigitsOnly(string input)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(input, "");
        }

    }
}
