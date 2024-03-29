﻿using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel;
using Project_Redmil_MVC.Models.RequestModel.SelfHelp;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.SelfHelpResponseModel;

namespace Project_Redmil_MVC.Controllers.BankingServicesController.DMT2._0Controller
{
    public class DMT2Controller : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public string senderid;
        public DMT2Controller(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult StartUp()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
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
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;

                        lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                        return View(lstdata);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        var data = deserialize.Data;

                        lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                        return View(lstdata);
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
                requestModel1.Data = getBalanceRequestModel;
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

        #region GetBalance
        [HttpPost]
        public List<GetBalanceResponseModel> GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
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
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var data = deserialize.Data;
                    lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                    return lstdata;
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    var data = deserialize.Data;
                    lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                    return lstdata;
                }
                else
                {
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
            }
            return lstdata;

        }

        #endregion

        #region To Check whether user is registered or not
        [HttpPost]
        public JsonResult Registered(string mobileNum)
        {

            GetSenderStatusRequestModel requestModel = new GetSenderStatusRequestModel();
            try
            {
                requestModel.UserId = HttpContext.Session.GetString("Id").ToString();
                requestModel.SenderMobile = mobileNum;
                #region Checksum (senderstatus|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("senderstatus", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.SenderStatus}");
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
                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetSenderStatusResponseModel>>>(response.Content);
                    if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
                    }
                    else if (deserializ.Statuscode == "TXN" && deserializ != null)
                    {
                        var data = deserializ.Data;
                        if (data != null)
                        {
                            //TempData["IdNames"] = JsonConvert.SerializeObject(deserializ);
                            //ViewBag.MyMessage = JsonConvert.SerializeObject(deserializ);
                            HttpContext.Session.SetString("SenderMobile", deserializ.Data.FirstOrDefault().SenderMobile.ToString());
                            HttpContext.Session.SetString("SenderId", deserializ.Data.FirstOrDefault().SenderId.ToString());
                            HttpContext.Session.SetString("SenderName", deserializ.Data.FirstOrDefault().SenderName.ToString());
                            return Json(new { Result = "Redirect", url = Url.Action("DMTDashboard", "DMT2Dashboard"), data = deserializ });
                        }

                        else
                        {
                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
                        }
                        //return Json("");
                    }

                    else if (deserializ.Statuscode == "SNR")
                    {
                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
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
        }
        #endregion


        #region To Check List Of All Recent Senders
        public JsonResult RecentSenders()
        {
            GetRecentSenderListRequestModel requestModel = new GetRecentSenderListRequestModel();
            List<GetRecentSenderListResponseModel> lstData = new List<GetRecentSenderListResponseModel>();
            try
            {
                //requestModel.UserId = HttpContext.Session.GetString("Id").ToString();
                requestModel.UserId = HttpContext.Session.GetString("Id").ToString();
                requestModel.PageNumber = "1";
                #region Checksum (senderdetailsbyuserid|Unique Key|UserId)

                string input = Checksum.MakeChecksumString("senderdetailsbyuserid", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.SenderDetailsByuserid}");
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
                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserializ.Statuscode == "TXN" && deserializ != null)
                    {
                        var data = deserializ.Data;

                        lstData = JsonConvert.DeserializeObject<List<GetRecentSenderListResponseModel>>(data.ToString());
                        //return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                        return Json(lstData);
                    }
                    else if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
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
        }
        #endregion

        #region OTP send
        public JsonResult SendOTP(string mobNum, string name)
        {
            GetOTPRequestModel requestModel = new GetOTPRequestModel();
            try
            {
                requestModel.UserId = HttpContext.Session.GetString("Id").ToString();
                requestModel.SenderName = name;
                requestModel.SenderMobile = mobNum;
                #region Checksum (senderdetailsbyuserid|Unique Key|UserId)

                string input = Checksum.MakeChecksumString("getfinootp", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.GetFinoOtp}");
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
                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetOTPResponseModel>>>(response.Content);
                    if (deserializ.Statuscode == "TXN" && deserializ != null)
                    {
                        var data = deserializ.Data;
                        if (data != null)
                        {
                            if (deserializ.Statuscode == "TXN")
                            {
                                return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                                //TempData["IdNames"] = JsonConvert.SerializeObject(deserializ);
                                //return RedirectToAction("DMTDashboard", "DMT2Dashboard");
                            }
                            else
                            {
                                return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                            }

                        }
                    }
                    else if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
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



        #region OTP Confirmation

        public JsonResult ConfirmOTP(string mobNum, string otp, string name)
        {
            GetOtpConfirmationRequestModel requestModel = new GetOtpConfirmationRequestModel();
            try
            {
                requestModel.UserId = HttpContext.Session.GetString("Id").ToString();
                requestModel.OTP = otp;
                requestModel.SenderMobile = mobNum;
                #region Checksum (otpconfirmation|Unique Key|UserId)

                string input = Checksum.MakeChecksumString("otpconfirmation", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.OtpConfirmation}");
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
                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetOtpConfirmationResponseModel>>>(response.Content);
                    if (deserializ.Statuscode == "TXN")
                    {
                        GetAddSenderRequestModel requestmodel = new GetAddSenderRequestModel();
                        requestmodel.UserId = HttpContext.Session.GetString("Id").ToString();
                        requestmodel.SenderName = name;
                        requestmodel.SenderMobile = deserializ.Data.FirstOrDefault().SenderMobile;
                        #region Checksum (addsender|Unique Key|UserId)

                        string input1 = Checksum.MakeChecksumString("addsender", Checksum.checksumKey, requestModel.UserId);
                        string CheckSum1 = Checksum.ConvertStringToSCH512Hash(input1);

                        #endregion
                        requestmodel.Checksum = CheckSum1;
                        var client1 = new RestClient($"{Baseurl}{ApiName.AddSender}");
                        var request1 = new RestRequest(Method.POST);
                        request1.AddHeader("Content-Type", "application/json");
                        var json1 = JsonConvert.SerializeObject(requestmodel);
                        request1.AddJsonBody(json1);
                        IRestResponse response1 = client1.Execute(request1);
                        var result1 = response1.Content;
                        if (string.IsNullOrEmpty(result1))
                        {
                            return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                        }
                        else
                        {
                            var deserialize = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetAddSenderResponseModel>>>(response1.Content);
                            var data1 = deserialize.Data;
                            if (data1 != null)
                            {
                                if (data1 != null)
                                {
                                    HttpContext.Session.SetString("SenderMobileNewUser", deserialize.Data.FirstOrDefault().SenderMobile.ToString());
                                    HttpContext.Session.SetString("SenderIdNewUser", deserialize.Data.FirstOrDefault().SenderId.ToString());
                                    HttpContext.Session.SetString("SenderNameNewUser", name.ToString());
                                    return Json(new { Result = "Redirect", url = Url.Action("DMTDashboard", "DMT2Dashboard"), data = deserialize });
                                }
                            }
                        }
                    }
                    else if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
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

        #region To Check whether user is registered or not
        [HttpPost]
        public JsonResult SenderListToDashboardRegistered(string mobileNum)
        {
            GetSenderStatusRequestModel requestModel = new GetSenderStatusRequestModel();
            try
            {
                requestModel.UserId = HttpContext.Session.GetString("Id").ToString();
                requestModel.SenderMobile = mobileNum;
                #region Checksum (senderstatus|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("senderstatus", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.SenderStatus}");
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
                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetSenderStatusResponseModel>>>(response.Content);
                    if (deserializ.Statuscode == "TXN" && deserializ != null)
                    {
                        var data = deserializ.Data;
                        if (data != null)
                        {
                            HttpContext.Session.SetString("RecentSenderMobile", deserializ.Data.FirstOrDefault().SenderMobile.ToString());
                            HttpContext.Session.SetString("RecentSenderId", deserializ.Data.FirstOrDefault().SenderId.ToString());
                            HttpContext.Session.SetString("RecentSenderName", deserializ.Data.FirstOrDefault().SenderName.ToString());
                            return Json(new { Result = "Redirect", url = Url.Action("DMTDashboard", "DMT2Dashboard"), data = deserializ });
                        }

                        else
                        {
                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
                        }
                    }
                    else if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") }); 
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
        }
        #endregion
        //#region SelfHelp
        //public JsonResult SelfHelp(string ServiceId)
        //{

        //    try
        //    {
        //        SelfHelpRequestModel SelfRequest = new SelfHelpRequestModel();
        //        SelfRequest.Userid = "NA";
        //        SelfRequest.ServiceId = ServiceId;
        //        string input = Checksum.MakeChecksumString("ViewFQL", Checksum.checksumKey,
        //            SelfRequest.Userid);
        //        string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
        //        SelfRequest.Checksum = CheckSum;
        //        var client = new RestClient($"{Baseurl}{ApiName.ViewFQL}");
        //        var request = new RestRequest(Method.POST);
        //        request.AddHeader("Content-Type", "application/json");
        //        var json = JsonConvert.SerializeObject(SelfRequest);
        //        request.AddJsonBody(json);
        //        IRestResponse response = client.Execute(request);
        //        var result = response.Content;
        //        if (string.IsNullOrEmpty(result))
        //        {
        //            return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
        //        }
        //        else
        //        {
        //            var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
        //            var datadeserialize = deserialize.Data;
        //            var data = JsonConvert.DeserializeObject<List<SelfHelpResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
        //            //var datadeserialize = deserialize.Data;
        //            //var TranferData = JsonConvert.DeserializeObject<GetcashdepositeResponseModel>(JsonConvert.SerializeObject(datadeserialize));
        //            return Json(data);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
        //        requestModelEx.ExceptionMessage = ex;
        //        requestModelEx.Data = "";
        //        var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
        //        var requestEx = new RestRequest(Method.POST);
        //        requestEx.AddHeader("Content-Type", "application/json");
        //        var jsonEx = JsonConvert.SerializeObject(requestModelEx);
        //        requestEx.AddJsonBody(jsonEx);
        //        IRestResponse responseEx = clientEx.Execute(requestEx);
        //        var resultEx = responseEx.Content;
        //        return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
        //    }
        //}
        //#endregion
    }
}
