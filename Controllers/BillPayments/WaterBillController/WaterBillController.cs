﻿using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.ElectricityBillRequestModel;
using Project_Redmil_MVC.Models.RequestModel.WaterBillRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.WaterBillResponseModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Project_Redmil_MVC.Controllers.BillPayments.WaterBillController
{
    public class WaterBillController : Controller
    {
        public List<GetBBPSBillsTmpResponseModel.Data> getBBPSBillsTmpResponseModel;
        public GetWaterOperatorListResponseModel getWaterOperatorListResponseModel;
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public WaterBillController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult WaterBill()
        {

            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            ViewBag.State = new SelectList(GetOperatorState(), "StateCode", "StateName");
            return View();
        }


        #region GetOperatorState
        public List<GetWaterStateListResponseModel> GetOperatorState()
        {
            List<GetWaterStateListResponseModel> lstd = new List<GetWaterStateListResponseModel>();
            try
            {
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsStates");
                var client = new RestClient($"{Baseurl}{ApiName.BbpsStates}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    //return Json(new { Result = "Redirect", url = Url.Action("ErrorHandle", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var dataList = JsonConvert.DeserializeObject<List<GetWaterStateListResponseModel>>(JsonConvert.SerializeObject(data));
                        return dataList;
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return lstd;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = "";
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
            }
            return lstd;
        }

        #endregion

        #region FetchWaterBill

        [HttpPost]

        public JsonResult FetchBill(string Number, string Input1, string Input2, string State, string Operator, string ccf, string Amount, string Payment)
        {

            var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
            GetWaterOperatorListRequestModel requestModel = new GetWaterOperatorListRequestModel();
            try
            {
                var stateID = State.Trim();
                requestModel.State = stateID;
                requestModel.Category = "Water";

                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)
                string inputBbpsBillerByState = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, "NA", requestModel.State, requestModel.Category);
                string CheckSumBbpsBillerByState = Checksum.ConvertStringToSCH512Hash(inputBbpsBillerByState);
                #endregion

                requestModel.checksum = CheckSumBbpsBillerByState;
                var clientBbpsBillerByState = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
                var requestBbpsBillerByState = new RestRequest(Method.POST);
                requestBbpsBillerByState.AddHeader("Content-Type", "application/json");
                var jsonBbpsBillerByState = JsonConvert.SerializeObject(requestModel);
                requestBbpsBillerByState.AddJsonBody(jsonBbpsBillerByState);
                IRestResponse responseBbpsBillerByState = clientBbpsBillerByState.Execute(requestBbpsBillerByState);
                var resultBbpsBillerByState = responseBbpsBillerByState.Content;
                if (string.IsNullOrEmpty(resultBbpsBillerByState))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserializeBbpsBillerByState = JsonConvert.DeserializeObject<BaseResponseModel>(responseBbpsBillerByState.Content);
                    if (deserializeBbpsBillerByState.Statuscode == "TXN" && deserializeBbpsBillerByState != null)
                    {
                        var datadeserializeBbpsBillerByState = deserializeBbpsBillerByState.Data;
                        getWaterOperatorListResponseModel = JsonConvert.DeserializeObject<GetWaterOperatorListResponseModel>(datadeserializeBbpsBillerByState.ToString());
                        if (!string.IsNullOrEmpty(Number))
                        {
                            var dataBillerInfo = getWaterOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator);
                            var billerAdhoc = getWaterOperatorListResponseModel.billerInfo.FirstOrDefault().inputParam.FirstOrDefault().Optional;
                            var Name = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                            List<GetBBPSBillsTmpResponseModel> lstResponse = new List<GetBBPSBillsTmpResponseModel>();
                            GetBBPSBillsTmpRequestModel requestmodel = new GetBBPSBillsTmpRequestModel();
                            try
                            {
                                requestmodel.Userid = HttpContext.Session.GetString("Id").ToString();
                                requestmodel.Mobileno = Number;
                                requestmodel.BillerId = dataBillerInfo.FirstOrDefault().Bbps;
                                string inputParamKey = "";
                                int count1 = 1;
                                if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                                {
                                    for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                                    {

                                        if (count1 <= dataBillerInfo.FirstOrDefault().inputParam.Length - 1)
                                        {
                                            inputParamKey += dataBillerInfo.FirstOrDefault().inputParam[i].Name + "^";
                                        }
                                        else
                                        {
                                            inputParamKey += dataBillerInfo.FirstOrDefault().inputParam[i].Name;
                                        }
                                        count1++;
                                    }
                                }
                                else
                                {
                                    inputParamKey = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                                }
                                requestmodel.InputParam1 = inputParamKey;
                                if (!string.IsNullOrEmpty(Input2))
                                {
                                    requestmodel.InputParam2 = Input1 + "^" + Input2;
                                }
                                else
                                {
                                    requestmodel.InputParam2 = Input1;
                                }

                                requestmodel.Ip_address = "192.168.1.1";
                                requestmodel.ccf = ccf;
                                requestmodel.Token = "";
                                #region Checksum (GetBBPSBillsTmp|Unique Key|UserId)
                                string input = Checksum.MakeChecksumString("GetBBPSBillsTmp", Checksum.checksumKey, requestmodel.Userid, requestmodel.Mobileno.Trim(), requestmodel.BillerId.Trim(), requestmodel.InputParam1.Trim(),
                                    requestmodel.InputParam2.Trim(), requestmodel.Ip_address.Trim(), requestmodel.ccf);
                                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                                #endregion
                                requestmodel.checksum = CheckSum;
                                var client = new RestClient("https://proapitest4.redmilbusinessmall.com/api/GetBBPSBillsTmp");//
                                                                                                                              //var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-Type", "application/json");
                                var json = JsonConvert.SerializeObject(requestmodel);
                                request.AddJsonBody(json);
                                IRestResponse response = client.Execute(request);
                                var result = response.Content;
                                if (string.IsNullOrEmpty(result))
                                {
                                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                                }
                                else
                                {
                                    var deserialize = JsonConvert.DeserializeObject<BaseBillResponseModel>(response.Content);
                                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                                    {
                                        var data = deserialize.Data;
                                        var data22 = deserialize.AdditionalInfo;

                                        GetBBPSBillsTmpResponseModel.Data getBBPSBillsTmpResponseModel = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.Data>(data.ToString());
                                        var billNumber = Input1;
                                        GetBBPSBillsTmpResponseModel.AdditionalInfo datalist1 = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.AdditionalInfo>(data22.ToString());

                                        //Final Bill Payment
                                        if (!string.IsNullOrEmpty(Amount) && (!string.IsNullOrEmpty(ccf)) && (!string.IsNullOrEmpty(Payment)))
                                        {
                                            PayBBPSBillsTmpRequestModel requestPayModel = new PayBBPSBillsTmpRequestModel();
                                            try
                                            {
                                                requestPayModel.RequestId = getBBPSBillsTmpResponseModel.ReqestNo;
                                                requestPayModel.Mobileno = Number;
                                                requestPayModel.BillerId = getWaterOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().Bbps;
                                                requestPayModel.Biller = getBBPSBillsTmpResponseModel.billerResponseEnc;
                                                requestPayModel.AdditionalInfo = getBBPSBillsTmpResponseModel.additionalInfoEnc;
                                                requestPayModel.Input = getBBPSBillsTmpResponseModel.inputParamsEnc;
                                                requestPayModel.IpAddress = GetIp();
                                                requestPayModel.MacAddress = GetMacAddress(requestPayModel.IpAddress);
                                                requestPayModel.ccf = ccf;
                                                //requestPayModel.billerAdhoc = getElectricityOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillerAdhoc;
                                                requestPayModel.billerAdhoc = billerAdhoc;

                                                string inputParamKey1 = "";
                                                int count1N = 1;
                                                if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                                                {
                                                    for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                                                    {

                                                        if (count1N <= dataBillerInfo.FirstOrDefault().inputParam.Length - 1)
                                                        {
                                                            inputParamKey1 += dataBillerInfo.FirstOrDefault().inputParam[i].Name + "^";
                                                        }
                                                        else
                                                        {
                                                            inputParamKey1 += dataBillerInfo.FirstOrDefault().inputParam[i].Name;
                                                        }
                                                        count1N++;
                                                    }
                                                }
                                                else
                                                {
                                                    inputParamKey1 = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                                                }
                                                requestPayModel.InputParam1 = inputParamKey1;
                                                if (!string.IsNullOrEmpty(Input2))
                                                {
                                                    requestPayModel.InputParam2 = Input1 + "^" + Input2;
                                                }
                                                else
                                                {
                                                    requestPayModel.InputParam2 = Input1;
                                                }
                                                requestPayModel.type = "Pay";
                                                //requestPayModel.billValidationStatus = getElectricityOperatorListResponseModel.billerInfo.FirstOrDefault().BillValidation;
                                                requestPayModel.billValidationStatus = getWaterOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillValidation;
                                                requestPayModel.Wallet = Payment;
                                                string amount = Amount;
                                                string FinalAmount = string.Empty;
                                                if (amount.Contains('₹'))
                                                {
                                                    string[] arrAmount = amount.Split('₹');
                                                    FinalAmount = arrAmount[1].Trim();
                                                }
                                                else
                                                {
                                                    FinalAmount = amount;
                                                }

                                                var amountData = CheckingBalanceClass.GetBalance(requestmodel.Userid);
                                                double newAmount = (amountData.FirstOrDefault().MainBal);
                                                if (newAmount > double.Parse(FinalAmount))
                                                {
                                                    requestPayModel.Amount = FinalAmount;
                                                }
                                                else
                                                {
                                                    return Json(new
                                                    {
                                                        status = "Insufficient Balance",
                                                    });
                                                }
                                                requestPayModel.Mode = "App";
                                                requestPayModel.Userid = HttpContext.Session.GetString("Id").ToString();
                                                string UseridCheck = HttpContext.Session.GetString("Id").ToString();
                                                //requestPayModel.Token = "";

                                                #region Checksum (PayBBPSBillsTmp|Unique Key|UseridCheck|Mobileno|Mode|Amount|RequestID|BillerId|InputParam1|InputParam2)
                                                string inputN = Checksum.MakeChecksumString("PayBBPSBillsTmp", Checksum.checksumKey, requestPayModel.Userid,
                                                    requestPayModel.Mobileno, requestPayModel.Mode, requestPayModel.Amount, requestPayModel.RequestId, requestPayModel.BillerId,
                                                    requestPayModel.InputParam1, requestPayModel.InputParam2);

                                                string CheckSumN = Checksum.ConvertStringToSCH512Hash(inputN);
                                                #endregion
                                                requestPayModel.checksum = CheckSumN;

                                                var clientN = new RestClient("https://proapitest5.redmilbusinessmall.com/api/PayBBPSBillsTmp");
                                                // var clientN = new RestClient($"{Baseurl}{ApiName.PayBill}");
                                                var requestN = new RestRequest(Method.POST);
                                                requestN.AddHeader("Content-Type", "application/json");
                                                var jsonN = JsonConvert.SerializeObject(requestPayModel);
                                                requestN.AddJsonBody(jsonN);
                                                IRestResponse responseN = clientN.Execute(requestN);
                                                var resultN = responseN.Content;
                                                if (string.IsNullOrEmpty(resultN))
                                                {
                                                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                                                }
                                                else
                                                {
                                                    var deserializeN = JsonConvert.DeserializeObject<BaseBillResponseModel>(responseN.Content);
                                                    if (deserializeN.Statuscode == "TXN" && deserialize != null)
                                                    {
                                                        var dataFinal = deserializeN.Data;
                                                        {
                                                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN.Content);

                                                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                                                        }
                                                    }
                                                    else if (deserializeN.Statuscode == "ERR")
                                                    {

                                                        var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN.Content);
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
                                                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                                                requestModelEx.ExceptionMessage = ex;
                                                requestModelEx.Data = requestPayModel;
                                                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                                                var requestEx = new RestRequest(Method.POST);
                                                requestEx.AddHeader("Content-Type", "application/json");
                                                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                                                requestEx.AddJsonBody(jsonEx);
                                                IRestResponse responseEx = clientEx.Execute(requestEx);
                                                var resultEx = responseEx.Content;
                                                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
                                            }
                                        }
                                        else
                                        {
                                            return Json(new
                                            {
                                                data = getBBPSBillsTmpResponseModel,
                                                additionalInfo = datalist1
                                            });
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
                                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                                requestModelEx.ExceptionMessage = ex;
                                requestModelEx.Data = requestmodel;
                                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                                var requestEx = new RestRequest(Method.POST);
                                requestEx.AddHeader("Content-Type", "application/json");
                                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                                requestEx.AddJsonBody(jsonEx);
                                IRestResponse responseEx = clientEx.Execute(requestEx);
                                var resultEx = responseEx.Content;
                                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
                            }
                        }
                        return Json(getWaterOperatorListResponseModel);
                    }
                    else if (deserializeBbpsBillerByState.Statuscode == "ERR")
                    {
                        return Json(deserializeBbpsBillerByState); ;
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }

        #endregion



        #region GetWaterOperatorList
        public JsonResult GetOperatorList(string StateId, string OpId, string Number, string Image)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetWaterOperatorListRequestModel requestModel = new GetWaterOperatorListRequestModel();
            try
            {
                var stateID = StateId.Trim();
                requestModel.State = stateID;
                requestModel.Category = "Water";
                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, "NA", requestModel.State, requestModel.Category);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsBillerByState");
                var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
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
                    if (deserialize.Statuscode == "TXN" && deserialize!=null)
                    {
                        var datadeserialize = deserialize.Data;
                        var deserializeN = JsonConvert.DeserializeObject<GetElectricityOperatorListResponseModel>(datadeserialize.ToString());
                        if ((!string.IsNullOrEmpty(StateId)) && (!string.IsNullOrEmpty(OpId)) && (!string.IsNullOrEmpty(Image)))
                        {
                            var imgSample = baseUrl + deserializeN.billerInfo.Where(x => x.Id == OpId).FirstOrDefault().ImgSample;
                            return Json(imgSample);
                        }
                        else if ((!string.IsNullOrEmpty(Number)) && (!string.IsNullOrEmpty(StateId)) && (!string.IsNullOrEmpty(OpId)))
                        {
                            var data = deserializeN.billerInfo.Where(x => x.Id == OpId);
                            return Json(data);
                        }
                        else if ((!string.IsNullOrEmpty(StateId)) && (!string.IsNullOrEmpty(OpId)))
                        {
                            var arr = deserializeN.billerInfo;
                            var a = arr.Where(x => x.Id == OpId);
                            return Json(a);
                        }
                        else
                        {
                            return Json(deserializeN);
                        }

                    }
                    else if(deserialize.Statuscode=="ERR")
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
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }

        #endregion


        #region GetCCF When User Changes amount

        public JsonResult GetCCF(string ccf, string amount)
        {
            string ccF = ccf;
            string Ccf = string.Empty;
            if (ccf.Contains('₹'))
            {
                string[] cCF = ccF.Split('₹');
                Ccf = cCF[1].Trim();
            }
            else
            {
                Ccf = ccf;
            }

            GetCCFRequestModel requestModel = new GetCCFRequestModel();
            try
            {
                requestModel.ccf = Ccf;
                requestModel.amount = amount;
                var client = new RestClient($"{Baseurl}{ApiName.CCF}");
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
                    var deserialize = JsonConvert.DeserializeObject<GetCCFResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXT" && deserialize != null)
                    {
                        return Json(deserialize);
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
            catch(Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
            
        }

        #endregion


        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
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
                    if(deserialize.Statuscode=="TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));

                        lstdata = datalist.ToList();
                        return Json(lstdata);
                    }
                    else if (deserialize.Statuscode=="ERR")
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
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = getBalanceRequestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion


        #region IP Address
        public string GetIp()
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddressesAsync(hostName);
            string ipAddress = ips.Result[1].ToString();
            return ipAddress;

        }
        #endregion

        #region MAC Address

        public string GetMacAddress(string ipAddress)

        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            String sMacAddress = string.Empty;

            foreach (NetworkInterface adapter in nics)

            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();

                }

            }

            return sMacAddress;

        }

        #endregion


        #region GetOperatorListbyId
        public JsonResult GetOperatorListbyId(string StateId, string OpId)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetWaterOperatorListRequestModel requestModel = new GetWaterOperatorListRequestModel();
            try
            {
                var stateID = StateId.Trim();
                requestModel.State = stateID;
                requestModel.Category = "Water";
                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, "NA", requestModel.State, requestModel.Category);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsBillerByState");
                var client = new RestClient($"{Baseurl}{ApiName.BbpsBillerByState}");
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
                    if (deserialize.Statuscode == "TXN" && deserialize!=null)
                    {
                        var datadeserialize = deserialize.Data;
                        var deserializeN = JsonConvert.DeserializeObject<GetWaterOperatorListResponseModel>(datadeserialize.ToString());
                        var arr = deserializeN.billerInfo;
                        var a = arr.Where(x => x.Id == OpId);
                        return Json(a);
                    }
                    else if(deserialize.Statuscode=="ERR"){
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
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion
    }
}
