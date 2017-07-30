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
        const string connetionString = "Persist Security Info=False;User ID=wiederd;Password=Hghghgg1;Initial Catalog=PoeSimCraft;Server=poesimcraftitem.czqfmqxgizhc.us-west-2.rds.amazonaws.com,1433";
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
