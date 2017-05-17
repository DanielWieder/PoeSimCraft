using System;
using System.Collections.Generic;


namespace PoeCrafting.Data
{
    public class FetchSubtypes : IFetchSubtypes
    {
        public List<string> Execute()
        {
            Database db = new Database();

            string command = $@"SELECT
                                    Name
                                FROM ItemSubType";
            return db.FetchList<string>(command);
        }
    }

    public interface IFetchSubtypes : IQueryObject<List<string>>
    {}
}
