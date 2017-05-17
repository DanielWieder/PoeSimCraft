using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSimCraftImporter
{
    public class DbCommand
    {
        const string connetionString = "Data Source=DESKTOP-HPIKISE;Initial Catalog=PoeSimCraft; Trusted_Connection=True;";
        public void Execute(string command)
        {
            using (SqlConnection cnn = new SqlConnection(connetionString))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(command, cnn))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}
