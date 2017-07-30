using System;
using System.Collections.Generic;
using PoeCrafting.Entities;


namespace PoeCrafting.Data
{
    public class FetchAffixesByItemName : IFetchAffixesByItemName
    {
        public string Name { get; set; }

        public List<Affix> Execute()
        {
            Database db = new Database();

            string command = $@"SELECT 
	                                a.ModName as ModName, 
	                                a.[Name] as Name, 
	                                a.GenerationType as Type, 
	                                a.[Group],
	                                astm.[Weight],
                                    astm.[Priority],
	                                a.ILvl, 
	                                a.StatName1,
	                                a.StatMin1,
	                                a.StatMax1,
	                                a.StatName2,
	                                a.StatMin2,
	                                a.StatMax2,
	                                a.StatName3,
	                                a.StatMin3,
	                                a.StatMax3,
                                    t.Name as SpawnTag
                                FROM Affix a
                                JOIN AffixSpawnTagMap astm ON astm.AffixId = a.AffixId
                                JOIN ItemSpawnTagMap istm ON istm.SpawnTagId = astm.SpawnTagId
                                Join SpawnTag t ON astm.SpawnTagId = t.SpawnTagId
                                join Item i ON istm.ItemId = i.ItemId 
                                WHERE i.Name = '{Name.Replace("'", "''")}'";

            return db.FetchList<Affix>(command);
        }
    }

    public interface IFetchAffixesByItemName : IQueryObject<List<Affix>>
    {
        string Name { get; set; }
    }
}
