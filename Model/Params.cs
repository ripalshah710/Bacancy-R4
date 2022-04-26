using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Common.Model
{
    public class Params
    {
        private static Params instance = null;
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public string subapi { get; set; }
        public string apiurl { get; set; }
        public string apiname { get; set; }
        private string APIName { get; set; }

        private SqlConnection sqlConnection = new SqlConnection();
        public List<Params> paramList = null;


        private List<Params> listDBParams
        {
            get
            {
                return GetDBParamsListInfo(APIName);
            }//get
        }//listDBParams

        public static Params getInstance(string apiname, SqlConnection connection)
        {

            //params information for header information for which API

            if (instance != null && (instance.APIName != apiname))
            {
                instance = new Params();
                instance.paramList = null;
                instance.APIName = apiname;
                instance.sqlConnection = connection;
                instance.paramList = instance.listDBParams;
            }
            if (instance == null)
            {
                instance = new Params();
                instance.paramList = null;
                instance.APIName = apiname;
                instance.sqlConnection = connection;
                instance.paramList = instance.listDBParams;
            }//if
            return instance;
        }//getInstance


        private List<Params> GetDBParamsListInfo(string apiname)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            List<Params> dbParamsList = new List<Params>();
            try
            {
                conn = sqlConnection;
                if (conn != null)
                {
                    conn.Open();
                    cmd = new SqlCommand("get_api_params", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (string.IsNullOrEmpty(apiname))
                    {
                        cmd.Parameters.AddWithValue("@api_name", DBNull.Value);
                    }//if
                    else
                    {
                        cmd.Parameters.AddWithValue("@api_name", apiname);
                    }//else
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Params param = new Params();
                            param.name = dr.GetString(0);
                            param.value = dr.GetString(1);
                            param.description = dr.GetString(2);
                            param.subapi = dr.GetString(3);
                            param.apiurl = dr.GetString(4);
                            param.apiname = dr.GetString(5);
                            dbParamsList.Add(param);
                        }//while
                    }//if
                    conn.Close();
                }//if
                else
                {
                    throw new Exception("No SQL connection established.");
                }//else
            }//try
            catch
            {
                throw;
            }//catch    
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }//if
            }//finally
            return dbParamsList;
        }//GetDBParamsListInfo
    }//Params
}
