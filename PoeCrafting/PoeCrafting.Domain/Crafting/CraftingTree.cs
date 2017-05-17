using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public class CraftingTree
    {
        public List<ICraftingStep> CraftingSteps;

        public void Craft(Equipment equipment)
        {
            IterateSteps(craftingStep => craftingStep.Craft(equipment), CraftingSteps);
        }

        public void UpdateStatus()
        {
            ItemStatus status = new ItemStatus();
            IterateSteps(craftingStep => status = craftingStep.UpdateStatus(status), CraftingSteps);
        }

        private void IterateSteps(Action<ICraftingStep> action, IList<ICraftingStep> craftingSteps)
        {
            foreach (var craftingStep in craftingSteps)
            {
                action(craftingStep);

                if (craftingStep.Children.Any())
                {
                    IterateSteps(action, craftingStep.Children);
                }
            }
        }
    }
}
