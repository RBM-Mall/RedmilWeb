using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.ElectricityBillRequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Project_Redmil_MVC.Controllers.RechargesControllers
{
    public class ElectricityBillController : Controller
    {
        public List<GetBBPSBillsTmpResponseModel.Data> getBBPSBillsTmpResponseModel;
        public GetElectricityOperatorListResponseModel getElectricityOperatorListResponseModel;
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public ElectricityBillController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        public IActionResult ElectricityBill()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            ViewBag.State = new SelectList(GetOperatorState(), "StateCode", "StateName");
            return View();
        }


        #region FetchBill and PayAlso

        [HttpPost]

        public JsonResult FetchBill(string Number, string Input1, string Input2, string State, string Operator, string ccf, string Amount, string Payment)
        {
            var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
            GetElectricityOperatorListRequestModel requestModel = new GetElectricityOperatorListRequestModel();
            var stateID = State.Trim();
            try
            {
                requestModel.State = stateID;
                requestModel.Category = "Electricity";

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
                    if (deserializeBbpsBillerByState != null && deserializeBbpsBillerByState.Statuscode == "TXN")
                    {
                        var datadeserializeBbpsBillerByState = deserializeBbpsBillerByState.Data;
                        getElectricityOperatorListResponseModel = JsonConvert.DeserializeObject<GetElectricityOperatorListResponseModel>(datadeserializeBbpsBillerByState.ToString());
                        if (!string.IsNullOrEmpty(Number))
                        {
                            var dataBillerInfo = getElectricityOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator);
                            var billerAdhoc = getElectricityOperatorListResponseModel.billerInfo.FirstOrDefault().inputParam.FirstOrDefault().Optional;

                            #region Fetching Input Param Value Statically

                            //Static Set Input Param 1 value and Input Param 2 value

                            var Name = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                            #endregion

                            List<GetBBPSBillsTmpResponseModel> lstResponse = new List<GetBBPSBillsTmpResponseModel>();
                            GetBBPSBillsTmpRequestModel requestmodel = new GetBBPSBillsTmpRequestModel();
                            requestmodel.Userid = "2084";
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
                                if (deserialize.Statuscode == "TXN")
                                {
                                    var data = deserialize.Data;
                                    var data22 = deserialize.AdditionalInfo;

                                    GetBBPSBillsTmpResponseModel.Data getBBPSBillsTmpResponseModel = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.Data>(data.ToString());
                                    //var billNumber = Input1;
                                    GetBBPSBillsTmpResponseModel.AdditionalInfo datalist1 = JsonConvert.DeserializeObject<GetBBPSBillsTmpResponseModel.AdditionalInfo>(data22.ToString());
                                    //Final Bill Payment
                                    if (!string.IsNullOrEmpty(Amount) && (!string.IsNullOrEmpty(ccf)) && (!string.IsNullOrEmpty(Payment)))
                                    {
                                        PayBBPSBillsTmpRequestModel requestPayModel = new PayBBPSBillsTmpRequestModel();
                                        requestPayModel.RequestId = getBBPSBillsTmpResponseModel.ReqestNo;
                                        requestPayModel.Mobileno = Number;
                                        requestPayModel.BillerId = getElectricityOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().Bbps;
                                        requestPayModel.Biller = getBBPSBillsTmpResponseModel.billerResponseEnc;
                                        requestPayModel.AdditionalInfo = getBBPSBillsTmpResponseModel.additionalInfoEnc;
                                        requestPayModel.Input = getBBPSBillsTmpResponseModel.inputParamsEnc;
                                        requestPayModel.IpAddress = GetIp();
                                        requestPayModel.MacAddress = GetMacAddress(requestPayModel.IpAddress);
                                        requestPayModel.ccf = ccf;
                                        //requestPayModel.billerAdhoc = getElectricityOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillerAdhoc;
                                        requestPayModel.billerAdhoc = billerAdhoc;
                                        string inputParamKey1 = "";
                                        int countN1 = 1;
                                        if (dataBillerInfo.FirstOrDefault().inputParam.Length > 1)
                                        {
                                            for (int i = 0; i < dataBillerInfo.FirstOrDefault().inputParam.Length; i++)
                                            {

                                                if (countN1 <= dataBillerInfo.FirstOrDefault().inputParam.Length - 1)
                                                {
                                                    inputParamKey1 += dataBillerInfo.FirstOrDefault().inputParam[i].Name + "^";
                                                }
                                                else
                                                {
                                                    inputParamKey1 += dataBillerInfo.FirstOrDefault().inputParam[i].Name;
                                                }
                                                countN1++;
                                            }
                                        }
                                        else
                                        {
                                            inputParamKey = dataBillerInfo.FirstOrDefault().inputParam.FirstOrDefault().Name;
                                        }
                                        requestPayModel.InputParam1 = inputParamKey;
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
                                        requestPayModel.billValidationStatus = getElectricityOperatorListResponseModel.billerInfo.Where(x => x.Id == Operator).FirstOrDefault().BillValidation;
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
                                            if (deserializeN.Statuscode == "TXN" && deserializeN != null)
                                            {
                                                var dataFinal = deserializeN.Data;
                                                {
                                                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetElectricityFinalResponseModel>>>(responseN.Content);

                                                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                                                }
                                            }
                                            else if (deserializeN.Statuscode == "ERR")
                                            {
                                                return Json(deserializeN);
                                            }
                                            else
                                            {
                                                return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                                            }
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
                        return Json(getElectricityOperatorListResponseModel);
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



        #region GetOperatorState
        public List<GetElectricityStateListResponseModel> GetOperatorState()
        {
            List<GetElectricityStateListResponseModel> lstRes = new List<GetElectricityStateListResponseModel>();
            try
            {
                var clientBbpsStates = new RestClient($"{Baseurl}{ApiName.BbpsStates}");
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/BbpsStates");
                var requestBbpsStates = new RestRequest(Method.POST);
                requestBbpsStates.AddHeader("Content-Type", "application/json");
                IRestResponse responseBbpsStates = clientBbpsStates.Execute(requestBbpsStates);
                var resultBbpsStates = responseBbpsStates.Content;
                var deserializeBbpsStates = JsonConvert.DeserializeObject<BaseResponseModel>(responseBbpsStates.Content);
                if(deserializeBbpsStates.Statuscode=="TXN" && deserializeBbpsStates != null)
                {
                    var dataBbpsStates = deserializeBbpsStates.Data;
                    var dataListBbpsStates = JsonConvert.DeserializeObject<List<GetElectricityStateListResponseModel>>(JsonConvert.SerializeObject(dataBbpsStates));
                    return dataListBbpsStates;
                }
                else if (deserializeBbpsStates.Statuscode == "ERR")
                {
                    
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = "";
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
            }
            return lstRes;
        }

        #endregion


        #region GetElectricityOperatorList
        public JsonResult GetOperatorList(string StateId, string OpId, string Number, string Image)
        {
            var baseUrl = "https://api.redmilbusinessmall.com";
            GetElectricityOperatorListRequestModel requestModel = new GetElectricityOperatorListRequestModel();
            try
            {
                var stateID = StateId.Trim();
                requestModel.State = stateID;
                requestModel.Category = "Electricity";
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
                        //return Json(deserializeN);
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
    }
}