using PoeCrafting.Entities;


namespace PoeCrafting.Currency
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
