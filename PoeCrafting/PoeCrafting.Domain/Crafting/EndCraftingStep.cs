﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    class EndCraftingStep : ICraftingStep
    {
        public CraftingStepStatus Status => _initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;
        public string Name => "End";

        public List<ICraftingStep> Children => null;
        public List<string> Options => null;
        public CraftingCondition Condition => null;

        private bool _initialized = false;

        public void ClearStatus()
        {
            _initialized = false;
        }

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            _initialized = true;
            status.Completed = true;
            return status;
        }

        public Equipment Craft(Equipment equipment)
        {
            equipment.Completed = true;
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
