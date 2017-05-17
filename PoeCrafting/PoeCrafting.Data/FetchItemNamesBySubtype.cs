using System;
using System.Collections.Generic;


namespace PoeCrafting.Data
{
    public class FetchItemNamesBySubtype : IFetchItemNamesBySubtype
    {
        public string Subtype { get; set; }

        public List<string> Execute()
        {
            Database db = new Database();

            string command = $@"SELECT
                                    i.Name
                                FROM Item i
                                JOIN ItemSubType ist ON ist.ItemSubtypeId = i.ItemSubtypeId
                                JOIN ItemType it ON ist.ItemTypeId = it.ItemTypeId
                                WHERE ist.Name = '{Subtype}'";

            return db.FetchList<string>(command);
        }
    }

    public interface IFetchItemNamesBySubtype : IQueryObject<List<string>>
    {
        string Subtype { get; set; }
    }
}
