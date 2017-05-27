using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class BlessedOrb : ICurrency 
    {
        private IRandom Random { get; set; }

        public string Name => "Blessed Orb";
        public double Value { get; set; }

        public BlessedOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.Corrupted || item.Implicit == null)
            {
                return false;
            }

            StatFactory.Reroll(Random, item.Implicit);

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return !status.HasImplicit || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            return status;
        }
    }
}
