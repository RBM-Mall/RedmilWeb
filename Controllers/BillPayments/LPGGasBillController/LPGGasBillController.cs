using Microsoft.AspNetCore.Http;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.ElectricityBillRequestModel;
using Project_Redmil_MVC.Models.RequestModel.LPGGasBillRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.LPGGasBillResponseModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Project_Redmil_MVC.Controllers.BillPayments.LPGGasBillController
{
    public class LPGGasBillController : Controller
    {
        public List<GetBBPSBillsTmpResponseModel.Data> getBBPSBillsTmpResponseModel;
        public LPGGasBillOperatorListResponseModel getLPGOperatorListResponseModel;
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public LPGGasBillController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        public IActionResult LPGGasBill()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }


            ViewBag.State = new SelectList(GetOperatorState(), "StateCode", "StateName");
            return View();
        }


        #region FetchBill and PayBill

        [HttpPost]

        public JsonResult FetchBill(string input1, string input2, string input3, string input4, string customerMobile, string State, string Operator, string ccf, string Amount, string Payment)
        {
            var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
            GetLPGGasOperatorListRequestModel requestModel = new GetLPGGasOperatorListRequestModel();
            try
            {
                var stateID = State.Trim();
                requestModel.State = stateID;
                requestModel.Category = "LPG Gas";

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
                        getLPGOperatorListResponseModel = JsonConvert.DeserializeObject<LPGGasBillOperatorListResponseModel>(datadeserializeBbpsBillerByState.ToString());
                        if (!string.IsNullOrEmpty(customerMobile))
                        {
                            var dataBillerInfo = getLPGOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator);
                            var billerAdhoc = getLPGOperatorListResponseModel.billerInfo.FirstOrDefault().inputParam.FirstOrDefault().Optional;

                            var Name = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;

                            List<GetBBPSBillsTmpResponseModel> lstResponse = new List<GetBBPSBillsTmpResponseModel>();
                            GetLPGGasBBPSBillsTmpRequestModel requestmodel = new GetLPGGasBBPSBillsTmpRequestModel();
                            try
                            {
                                requestmodel.Userid = "2084";
                                requestmodel.Mobileno = customerMobile;
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
                                if (!string.IsNullOrEmpty(input1))
                                {
                                    string inputpar = "";
                                    inputpar = input1 + "^" + input2;
                                    if (!string.IsNullOrEmpty(input3))
                                    {
                                        inputpar += "^" + input3 + "^" + input4;
                                    }
                                    requestmodel.InputParam2 = inputpar;
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

                                    if (deserialize.Statuscode == "TXN")
                                    {
                                        var data = deserialize.Data;
                                        var data22 = deserialize.AdditionalInfo;

                                        GetBBPSBillsTmpResponseModel.Data getBBPSBillsTmpResponseModel = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.Data>(data.ToString());
                                        GetBBPSBillsTmpResponseModel.AdditionalInfo datalist1 = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.AdditionalInfo>(data22.ToString());
                                        //Final Bill Payment
                                        if (!string.IsNullOrEmpty(Amount) && (!string.IsNullOrEmpty(ccf)) && (!string.IsNullOrEmpty(Payment)))
                                        {
                                            PayBBPSBillsTmpRequestModel requestPayModel = new PayBBPSBillsTmpRequestModel();
                                            try
                                            {
                                                requestPayModel.RequestId = getBBPSBillsTmpResponseModel.ReqestNo;
                                                requestPayModel.Mobileno = customerMobile;
                                                requestPayModel.BillerId = getLPGOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().Bbps;
                                                requestPayModel.Biller = getBBPSBillsTmpResponseModel.billerResponseEnc;
                                                requestPayModel.AdditionalInfo = getBBPSBillsTmpResponseModel.additionalInfoEnc;
                                                requestPayModel.Input = getBBPSBillsTmpResponseModel.inputParamsEnc;
                                                requestPayModel.IpAddress = GetIp();
                                                requestPayModel.MacAddress = GetMacAddress(requestPayModel.IpAddress);
                                                requestPayModel.ccf = ccf;
                                                requestPayModel.billerAdhoc = billerAdhoc;
                                                string inputParamKeyN = "";
                                                int count1N = 1;
                                                if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                                                {
                                                    for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                                                    {

                                                        if (count1 <= dataBillerInfo.FirstOrDefault().inputParam.Length - 1)
                                                        {
                                                            inputParamKeyN += dataBillerInfo.FirstOrDefault().inputParam[i].Name + "^";
                                                        }
                                                        else
                                                        {
                                                            inputParamKeyN += dataBillerInfo.FirstOrDefault().inputParam[i].Name;
                                                        }
                                                        count1N++;
                                                    }
                                                }
                                                else
                                                {
                                                    inputParamKeyN = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                                                }
                                                requestPayModel.InputParam1 = inputParamKey;
                                                if (!string.IsNullOrEmpty(input1))
                                                {
                                                    string inputpar = "";
                                                    inputpar = input1 + "^" + input2;
                                                    if (!string.IsNullOrEmpty(input3))
                                                    {
                                                        inputpar += "^" + input3 + "^" + input4;
                                                    }
                                                    requestPayModel.InputParam2 = inputpar;
                                                }

                                                requestPayModel.type = "Pay";
                                                requestPayModel.billValidationStatus = getLPGOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillValidation;
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
                                                requestPayModel.Amount = FinalAmount;
                                                requestPayModel.Mode = "App";
                                                requestPayModel.Userid = "2084";
                                                string UseridCheck = "2084";
                                                //requestPayModel.Token = "";

                                                #region Checksum (PayBBPSBillsTmp|Unique Key|UseridCheck|Mobileno|Mode|Amount|RequestID|BillerId|InputParam1|InputParam2)
                                                string inputN1 = Checksum.MakeChecksumString("PayBBPSBillsTmp", Checksum.checksumKey, requestPayModel.Userid,
                                                    requestPayModel.Mobileno, requestPayModel.Mode, requestPayModel.Amount, requestPayModel.RequestId, requestPayModel.BillerId,
                                                    requestPayModel.InputParam1, requestPayModel.InputParam2);
                                                string CheckSumN1 = Checksum.ConvertStringToSCH512Hash(inputN1);
                                                #endregion

                                                requestPayModel.checksum = CheckSumN1;

                                                var clientN1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/PayBBPSBillsTmp");

                                                var requestN1 = new RestRequest(Method.POST);
                                                requestN1.AddHeader("Content-Type", "application/json");
                                                var jsonN1 = JsonConvert.SerializeObject(requestPayModel);
                                                requestN1.AddJsonBody(jsonN1);
                                                IRestResponse responseN1 = clientN1.Execute(requestN1);
                                                var resultN1 = responseN1.Content;
                                                if (string.IsNullOrEmpty(resultN1))
                                                {
                                                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                                                }
                                                else
                                                {
                                                    var deserializeN1 = JsonConvert.DeserializeObject<BaseBillResponseModel>(responseN1.Content);
                                                    if (deserializeN1.Statuscode == "TXN" && deserializeN1 != null)
                                                    {
                                                        var dataFinal = deserializeN1.Data;
                                                        {
                                                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN1.Content);

                                                            return Json(new BaseResponseModel() { Statuscode = deserializeN1.Statuscode, Message = deserializeN1.Message, Data = deserializeN1.Data });
                                                        }
                                                    }
                                                    else if (deserializeN1.Statuscode == "ERR")
                                                    {
                                                        {
                                                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN1.Content);
                                                            return Json(new BaseResponseModel() { Statuscode = deserializeN1.Statuscode, Message = deserializeN1.Message, Data = deserializeN1.Data });
                                                        }
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
                        return Json(getLPGOperatorListResponseModel);
                    }
                    else if (deserializeBbpsBillerByState.Statuscode == "ERR")
                    {
                        return Json(deserializeBbpsBillerByState);
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

        #region GetOperatorState
        public List<LPGGasBillStateResponseModel> GetOperatorState()
        {
            List<LPGGasBillStateResponseModel> lstdata = new List<LPGGasBillStateResponseModel>();
            try
            {
                var clientBbpsStates = new RestClient($"{Baseurl}{ApiName.BbpsStates}");
                var requestBbpsStates = new RestRequest(Method.POST);
                requestBbpsStates.AddHeader("Content-Type", "application/json");
                IRestResponse responseBbpsStates = clientBbpsStates.Execute(requestBbpsStates);
                var resultBbpsStates = responseBbpsStates.Content;
                if (string.IsNullOrEmpty(resultBbpsStates))
                {
                    //return Json(new { Result = "Redirect", url = Url.Action("ErrorHandle", "Error") });
                }
                else
                {
                    var deserializeBbpsStates = JsonConvert.DeserializeObject<BaseResponseModel>(responseBbpsStates.Content);
                    if (deserializeBbpsStates.Statuscode == "TXN" && deserializeBbpsStates != null)
                    {
                        var dataBbpsStates = deserializeBbpsStates.Data;
                        var dataListBbpsStates = JsonConvert.DeserializeObject<List<LPGGasBillStateResponseModel>>(JsonConvert.SerializeObject(dataBbpsStates));
                        return dataListBbpsStates;
                    }
                    else if (deserializeBbpsStates.Statuscode == "ERR")
                    {
                        return lstdata;
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
            return lstdata;

        }

        #endregion

        #region GetLPGGasBillOperatorList
        public JsonResult GetOperatorList(string StateId)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetLPGGasOperatorListRequestModel requestModel = new GetLPGGasOperatorListRequestModel();
            try
            {
                var stateID = StateId.Trim();
                requestModel.State = stateID;
                requestModel.Category = "LPG Gas";
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
                    
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var deserializeN = JsonConvert.DeserializeObject<GetElectricityOperatorListResponseModel>(deserialize.Data.ToString());
                        return Json(deserializeN);
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

        #region Split Value
        public string Split(string value)
        {
            string param = value;
            string rvalue = string.Empty;
            if (param.Contains('₹'))
            {
                string[] Param = param.Split('₹');
                rvalue = Param[1].Trim();
            }
            else
            {
                rvalue = value;
            }
            return rvalue;

        }
        #endregion

        #region GetCCF When User Changes amount

        public JsonResult GetCCF(string ccf, string amount)
        {
            GetCCFRequestModel requestModel = new GetCCFRequestModel();
            try
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
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
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


        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
            try
            {
                getBalanceRequestModel.Userid = "2084";
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
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));

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

        #region GetLPGGasBillOperatorListById
        public JsonResult GetOperatorListById(string StateId, string Operator)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetLPGGasOperatorListRequestModel requestModel = new GetLPGGasOperatorListRequestModel();
            try
            {
                var stateID = StateId.Trim();
                requestModel.State = stateID;
                requestModel.Category = "LPG Gas";

                #region Checksum (BbpsBillerByState|Unique Key|UserId|ServiceId)
                string input = Checksum.MakeChecksumString("BbpsBillerByState", Checksum.checksumKey, "NA", requestModel.State, requestModel.Category);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                requestModel.checksum = CheckSum;
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
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var datadeserialize = deserialize.Data;
                        var deserializeN = JsonConvert.DeserializeObject<LPGGasBillOperatorListResponseModel>(datadeserialize.ToString());
                        var arr = deserializeN.billerInfo;
                        var a = arr.Where(x => x.Id == Operator);
                        return Json(a);
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
