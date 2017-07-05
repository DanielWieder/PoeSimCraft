using System;


namespace PoeCrafting.Data
{
    public class FetchTypeByItemName : IFetchTypeByItemName
    {
        public string Name { get; set; }

        public string Execute()
        {
            Database db = new Database();

            string command = $@"SELECT
                                    it.Name
                                FROM Item i
                                JOIN ItemSubType ist ON ist.ItemSubtypeId = i.ItemSubtypeId
                                JOIN ItemType it ON ist.ItemTypeId = it.ItemTypeId
                                WHERE i.Name = '{Name.Replace("'", "''")}'";

            return db.Fetch<string>(command);
        }
    }

    public interface IFetchTypeByItemName : IQueryObject<string>
    {
        string Name { get; set; }
    }
}
