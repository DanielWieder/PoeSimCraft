using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Data;


namespace PoeCrafting.Domain
{
    public class EquipmentFetch
    {
        private IFetchItemNamesBySubtype fetchItemNamesBySubtype;
        private IFetchSubtypes fetchSubtypes;

        public EquipmentFetch(IFetchItemNamesBySubtype fetchItemNamesBySubtype, IFetchSubtypes fetchSubtypes)
        {
            this.fetchItemNamesBySubtype = fetchItemNamesBySubtype;
            this.fetchSubtypes = fetchSubtypes;
        }

        public List<string> FetchBasesBySubtype(string subtype)
        {
            fetchItemNamesBySubtype.Subtype = subtype;
            return fetchItemNamesBySubtype.Execute();
        }

        public List<string> FetchSubtypes()
        {
            return fetchSubtypes.Execute();
        }

        public List<string> FetchItemNamesBySubtype(string subtype)
        {
            fetchItemNamesBySubtype.Subtype = subtype;
            return fetchItemNamesBySubtype.Execute();
        }
    }
}
