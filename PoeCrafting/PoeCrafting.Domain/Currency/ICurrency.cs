using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;


namespace PoeCrafting.Domain.Currency
{
    public interface ICurrency
    {
        string Name { get; }

        bool Execute(Equipment equipment);

        bool IsWarning(ItemStatus status);

        bool IsError(ItemStatus status);

        ItemStatus GetNextStatus(ItemStatus status);

        double Value { get; set; }
    }
}
