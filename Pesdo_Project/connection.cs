using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Pesdo_Project
{
    internal class connection
    {
        public static string ConnectionString = @"Data Source=jingy\sqlexpress;Initial Catalog=Pesdo;Integrated Security=True;Encrypt=False";


        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
