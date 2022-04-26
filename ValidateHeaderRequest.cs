using Common.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ValidateHeaderRequest
    {
        private static ValidateHeaderRequest instance = null;
        public static ValidateHeaderRequest getInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ValidateHeaderRequest();
                }//if
                return instance;
            }//get
        }//getInstance

        //singleton
        private ValidateHeaderRequest() { }//ValidateHeaderRequest //for API

        public bool ValidateHeaderRequests(HeaderParameters parameters, SqlConnection connection)
        {
            var paramsList = Params.getInstance(parameters.apiname, connection).paramList;

            string version = paramsList.Where(x => x.name == "version" && x.subapi == parameters.subapiname && x.apiname == parameters.apiname).Select(x => x.value).FirstOrDefault();
            string sourceapp = paramsList.Where(x => x.name == "sourceapp" && x.subapi == parameters.subapiname && x.apiname == parameters.apiname).Select(x => x.value).FirstOrDefault();
            string token = paramsList.Where(x => x.name == "token" && x.subapi == parameters.subapiname && x.apiname == parameters.apiname).Select(x => x.value).FirstOrDefault();
            string sessionId = paramsList.Where(x => x.name == "sessionid" && x.subapi == parameters.subapiname && x.apiname == parameters.apiname).Select(x => x.value).FirstOrDefault();
            if (parameters.version == version
               && parameters.sourceapp == sourceapp
               && parameters.sessionid == sessionId
               && parameters.token == token)
            {
                return true;
            }//if
            return false;
        }//ValidateHeaderRequests
    }//ValidateHeaderRequest
}
