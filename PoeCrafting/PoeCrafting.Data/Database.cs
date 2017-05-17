using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Data
{
    public class Database
    {
        private string connectionString = "PoeSimCraft";

        public List<T> FetchList<T>(string command)
        {
            var db = new PetaPoco.Database(connectionString);

            return db.Query<T>(command).ToList();
        }

        public T Fetch<T>(string command)
        {
            var db = new PetaPoco.Database(connectionString);

            return db.SingleOrDefault<T>(command);
        }
    }
}
