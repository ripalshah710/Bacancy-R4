using Common.Model;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Common
{
    public class ConnectionString
    {
        private static ConnectionString instance = null;

        public SqlConnection sqlConnection = null;
        private string CustomerId { get; set; }
        private string WebServer { get; set; }
        private string IsEntity { get; set; }

        private SqlConnection connecitonString
        {
            get
            {
                try
                {
                    return GetConnectionString();
                }//try
                catch
                {
                    throw;
                }//catch
            }//get
        }//listDBParams

        public static ConnectionString getInstance(string customerId, string serverName, string isEntity)
        {
            try
            {
                if (instance != null && (instance.CustomerId != customerId || instance.WebServer != serverName))
                {
                    instance = new ConnectionString();
                    instance.CustomerId = customerId;
                    instance.WebServer = serverName;
                    instance.IsEntity = isEntity;
                    instance.sqlConnection = instance.connecitonString;
                }
                if (instance == null)
                {
                    instance = new ConnectionString();
                    instance.CustomerId = customerId;
                    instance.WebServer = serverName;
                    instance.IsEntity = isEntity;
                    instance.sqlConnection = instance.connecitonString;
                }//if
                return instance;
            }//try
            catch
            {
                throw;
            }//catch
        }//getInstance

        //singleton
        private ConnectionString() { }//AddRequestHeaderForAPI ;Singleton class
        private static bool IsValidateUser() //1st validate user before obtain token
        {
            try
            {
                using (HttpClient client = ConnectionString.ValidateUser())
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["db_connection_base_url"]);

                    Uri uri = new Uri(client.BaseAddress + ConfigurationManager.AppSettings["db_connection_resource_user_url"]);

                    response = client.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
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
                }//using
            }//try
            catch
            {
                throw;
            }//catch
        }//ValidateUser
        private string GetToken()
        {
            string token = string.Empty;
            try
            {
                if (IsValidateUser())
                {
                    HttpClient request = new HttpClient();
                    var plainTextBytes = Encoding.UTF8.GetBytes(GetRequestDataFromConfig("username") + ":" + GetRequestDataFromConfig("password"));
                    string encodedpassword = Convert.ToBase64String(plainTextBytes);
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    request.DefaultRequestHeaders.Add("dbtkn-version", GetRequestDataFromConfig("dbtkn-version"));
                    request.DefaultRequestHeaders.Add("dbtkn-sourceapp", GetRequestDataFromConfig("dbtkn-sourceapp"));
                    request.DefaultRequestHeaders.Add("dbtkn-token", GetRequestDataFromConfig("dbtkn-token"));
                    request.DefaultRequestHeaders.Add("dbtkn-sessionid", GetRequestDataFromConfig("dbtkn-sessionid"));
                    request.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedpassword);
                    request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = new HttpResponseMessage();
                    request.BaseAddress = new Uri(GetRequestDataFromConfig("db_connection_base_url"));

                    Uri uri = new Uri(request.BaseAddress + GetRequestDataFromConfig("db_connection_resource_token_url"));

                    var payload = "{\"CustomerId\": \"" + CustomerId + "\",\"ServerName\": \"" + WebServer + "\",\"isEntity\": \"" + IsEntity + "\"}";
                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                    // HTTP GET  
                    response = request.PostAsync(uri, content).ConfigureAwait(false).GetAwaiter().GetResult();

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        dynamic data = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        dynamic result = JsonConvert.DeserializeObject(data);
                        token = result.access_token.ToObject<string>();
                    }//if
                    else
                    {
                        token = "unauthorized user";
                    }//else
                }//if
                return token;
            }//try
            catch
            {
                throw;
            }//catch
        }//GetHttpClientForToken

        private SqlConnection GetConnectionString()
        {
            string token = instance.GetToken(); //obtain token
            try
            {
                if (!string.IsNullOrEmpty(token) && token.Length > 200)
                {
                    HttpClient request = new HttpClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    request.DefaultRequestHeaders.Add("dbconn-version", GetRequestDataFromConfig("dbconn-version"));
                    request.DefaultRequestHeaders.Add("dbconn-sourceapp", GetRequestDataFromConfig("dbconn-sourceapp"));
                    request.DefaultRequestHeaders.Add("dbconn-token", GetRequestDataFromConfig("dbconn-token"));
                    request.DefaultRequestHeaders.Add("dbconn-sessionid", GetRequestDataFromConfig("dbconn-sessionid"));
                    request.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    request.BaseAddress = new Uri(GetRequestDataFromConfig("db_connection_base_url"));

                    Uri uri = new Uri(request.BaseAddress + GetRequestDataFromConfig("db_connection_resource_conn_url"));

                    var payload = "{\"CustomerId\": \"" + CustomerId + "\",\"ServerName\": \"" + WebServer + "\",\"isEntity\": \"" + IsEntity + "\"}";
                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                    // HTTP GET  
                    response = request.PostAsync(uri, content).ConfigureAwait(false).GetAwaiter().GetResult();

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        dynamic result = JsonConvert.DeserializeObject(data);
                        DBConnectionResponse dbConnectionResponse = result.ToObject<DBConnectionResponse>();
                        SqlConnection connection = new SqlConnection(dbConnectionResponse.DBConnectionsList[0].ConnectionString);
                        return connection;
                    }//if
                }//if
                return null;
            }//try
            catch
            {
                throw;
            }//catch
        }//GetRequestObject

        private static HttpClient ValidateUser()
        {
            try
            {
                HttpClient request = new HttpClient();
                var plainTextBytes = Encoding.UTF8.GetBytes(GetRequestDataFromConfig("username") + ":" + GetRequestDataFromConfig("password"));
                string encodedpassword = Convert.ToBase64String(plainTextBytes);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                request.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedpassword);
                // Setting content type.  
                request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return request;
            }//try
            catch
            {
                throw;
            }//catch
        }//VlidateUser

        private static string GetRequestDataFromConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }//GetRequestDataFromConfig
    }
}
