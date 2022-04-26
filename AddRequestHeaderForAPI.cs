using Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Common
{
    public class AddRequestHeaderForAPI
    {
        private static AddRequestHeaderForAPI instance = null;

        public static AddRequestHeaderForAPI getInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AddRequestHeaderForAPI();
                }//if
                return instance;
            }//get
        }//getInstance

        //singleton
        private AddRequestHeaderForAPI() { }//AddRequestHeaderForAPI // for consumeapplication

        public string AddRequestHeaderToGetToken(List<Params> paramsList, string apiName
            , string subapiName, string prefix, string customerId, string serverName, string isEntity)
        {
            HttpClient request = new HttpClient();
            try
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    var plainTextBytes = Encoding.UTF8.GetBytes(GetRequestDataFromConfig("username") + ":" + GetRequestDataFromConfig("password"));
                    string encodedpassword = Convert.ToBase64String(plainTextBytes);
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    // stripe token

                    request.DefaultRequestHeaders.Add(prefix + "-version", GetValue(paramsList, apiName, subapiName, "version"));
                    request.DefaultRequestHeaders.Add(prefix + "-sourceapp", GetValue(paramsList, apiName, subapiName, "sourceapp"));
                    request.DefaultRequestHeaders.Add(prefix + "-token", GetValue(paramsList, apiName, subapiName, "token"));
                    request.DefaultRequestHeaders.Add(prefix + "-sessionid", GetValue(paramsList, apiName, subapiName, "sessionid"));

                    //DB Token
                    request.DefaultRequestHeaders.Add("dbtkn-version", GetRequestDataFromConfig("dbtkn-version"));
                    request.DefaultRequestHeaders.Add("dbtkn-sourceapp", GetRequestDataFromConfig("dbtkn-sourceapp"));
                    request.DefaultRequestHeaders.Add("dbtkn-token", GetRequestDataFromConfig("dbtkn-token"));
                    request.DefaultRequestHeaders.Add("dbtkn-sessionid", GetRequestDataFromConfig("dbtkn-sessionid"));

                    //DB Connection string
                    request.DefaultRequestHeaders.Add("dbconn-version", GetRequestDataFromConfig("dbconn-version"));
                    request.DefaultRequestHeaders.Add("dbconn-sourceapp", GetRequestDataFromConfig("dbconn-sourceapp"));
                    request.DefaultRequestHeaders.Add("dbconn-token", GetRequestDataFromConfig("dbconn-token"));
                    request.DefaultRequestHeaders.Add("dbconn-sessionid", GetRequestDataFromConfig("dbconn-sessionid"));

                    request.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedpassword);
                    request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = new HttpResponseMessage();

                    Uri uri = new Uri(GetURL(paramsList, apiName, subapiName));

                    var payload = "{\"CustomerId\": \"" + customerId + "\",\"ServerName\": \"" + serverName + "\",\"isEntity\": \"" + isEntity + "\"}";
                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                    // HTTP GET  
                    response = request.PostAsync(uri, content).ConfigureAwait(false).GetAwaiter().GetResult();

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        dynamic data = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        dynamic result = JsonConvert.DeserializeObject(data);
                        return result.access_token.ToObject<string>();
                    }//if
                    else
                    {
                        return "unauthorized user";
                    }//else
                }//if
                else
                {
                    return "unauthorized user";
                }//else
            }//try
            catch
            {
                throw;
            }//catch
        }//AddRequestHeaderToGetToken

        public HttpClient AddRequestHeaderToTransaction(List<Params> paramsList, string apiName, string subapiName, string prefix, string token)
        {
            HttpClient request = new HttpClient();
            if (!string.IsNullOrEmpty(prefix))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //     stripe process/refund transaction as an example but common for allother API 
                request.DefaultRequestHeaders.Add(prefix + "-version", GetValue(paramsList, apiName, subapiName, "version"));
                request.DefaultRequestHeaders.Add(prefix + "-sourceapp", GetValue(paramsList, apiName, subapiName, "sourceapp"));
                request.DefaultRequestHeaders.Add(prefix + "-token", GetValue(paramsList, apiName, subapiName, "token"));
                request.DefaultRequestHeaders.Add(prefix + "-sessionid", GetValue(paramsList, apiName, subapiName, "sessionid"));


                //DB Token
                request.DefaultRequestHeaders.Add("dbtkn-version", GetRequestDataFromConfig("dbtkn-version"));
                request.DefaultRequestHeaders.Add("dbtkn-sourceapp", GetRequestDataFromConfig("dbtkn-sourceapp"));
                request.DefaultRequestHeaders.Add("dbtkn-token", GetRequestDataFromConfig("dbtkn-token"));
                request.DefaultRequestHeaders.Add("dbtkn-sessionid", GetRequestDataFromConfig("dbtkn-sessionid"));

                //DB Connection string
                request.DefaultRequestHeaders.Add("dbconn-version", GetRequestDataFromConfig("dbconn-version"));
                request.DefaultRequestHeaders.Add("dbconn-sourceapp", GetRequestDataFromConfig("dbconn-sourceapp"));
                request.DefaultRequestHeaders.Add("dbconn-token", GetRequestDataFromConfig("dbconn-token"));
                request.DefaultRequestHeaders.Add("dbconn-sessionid", GetRequestDataFromConfig("dbconn-sessionid"));

                request.DefaultRequestHeaders.Add("Authorization", "Bearer " + token); // stripe token as an example here
                // Setting content type.  
                //request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }//if
            return request;
        }//AddRequestHeaderToTransaction

        public static bool ValidateUser(List<Params> paramsList, string apiName, string subapiName)
        {
            try
            {
                HttpClient request = new HttpClient();
                var plainTextBytes = Encoding.UTF8.GetBytes(GetRequestDataFromConfig("username") + ":" + GetRequestDataFromConfig("password")); // from consuming application //Test API console application
                string encodedpassword = Convert.ToBase64String(plainTextBytes);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                request.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedpassword);
                // Setting content type.  
                request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Uri uri = new Uri(GetURL(paramsList, apiName, subapiName)); // for stripe or  other api goin gto get header information from database
                HttpResponseMessage response = new HttpResponseMessage();
                response = request.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    string deserializedString = JsonConvert.DeserializeObject<string>(data);
                    if (deserializedString.ToLower() == "success")
                    {
                        return true;
                    }//if
                }//if
                return false;
            }//try
            catch
            {
                throw;
            }//catch
        }//ValidateUser

        private static string GetRequestDataFromConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }//GetRequestDataFromConfig

        public static string GetURL(List<Params> paramsList, string apiName, string subapiName)
        {
            return paramsList.Where(x => x.subapi == subapiName && x.apiname == apiName).Select(x => x.apiurl).FirstOrDefault();
        }//GetURL

        public static string GetValue(List<Params> paramsList, string apiName, string subapiName, string keyname)
        {
            return paramsList.Where(x => x.name == keyname && x.subapi == subapiName && x.apiname == apiName).Select(x => x.value).FirstOrDefault();
        }//GetValue
    }//AddRequestHeaderForAPI
}
