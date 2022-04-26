using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class DBConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }//DBConnection

    public class DBConnectionResponse
    {
        public List<DBConnection> DBConnectionsList { get; set; }
        public string Discription { get; set; }
        public DBConnectionResponse()
        {
            DBConnectionsList = new List<DBConnection>();
        }//DBConnection
    }
}
