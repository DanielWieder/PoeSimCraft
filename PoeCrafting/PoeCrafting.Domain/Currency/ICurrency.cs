using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Crafting;


namespace PoeCrafting.Entities.Currency
{
    public interface ICurrency
    {
        string Name { get; }

        bool Execute(Equipment equipment);

        bool IsWarning(ItemStatus status);

        bool IsError(ItemStatus status);

        ItemStatus GetNextStatus(ItemStatus status);
    }
}
