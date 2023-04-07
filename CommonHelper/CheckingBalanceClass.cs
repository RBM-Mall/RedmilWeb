using Project_Redmil_MVC.Models.RequestModel.AepsRequestModel;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel;
using Project_Redmil_MVC.Models.ResponseModel;
using System.Security.Policy;

namespace Project_Redmil_MVC.CommonHelper
{
    public class CheckingBalanceClass
    {

        #region GetBalance
        public static List<GetBalanceResponseModel> GetBalance(string UId)
        {

            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            try
            {
                getBalanceRequestModel.Userid = UId;
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                getBalanceRequestModel.checksum = CheckSum;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/Getbalance");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return null;
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
                        return lstdata;
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));
                        List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
                        lstdata = datalist.ToList();
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
                return null;
            }
        }
        #endregion
    }
}
