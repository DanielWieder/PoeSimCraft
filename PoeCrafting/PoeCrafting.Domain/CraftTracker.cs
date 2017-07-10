﻿using PoeCrafting.Entities;

namespace PoeCrafting.Domain
{
    public class CraftTracker
    {
        private Equipment previouslyCrafted = null;
        private int successfulUsesCount = 0;
        private int usesCount = 0;
        private int itemsSuccessfullyUsedOnCount = 0;
        private int itemsUsedOnCount = 0;

        public int SuccessfulUsesCount => successfulUsesCount;
        public int UsesCount => usesCount;
        public int ItemsSuccessfullyUsedOnCount => itemsSuccessfullyUsedOnCount;
        public int ItemsUsedOnCount => itemsUsedOnCount;

        public void TrackCraft(Equipment item, bool success)
        {
            usesCount++;
            if (success)
            {
                successfulUsesCount++;
                if (previouslyCrafted != item)
                {
                    itemsSuccessfullyUsedOnCount++;
                }
            }

            if (previouslyCrafted != item)
            {
                previouslyCrafted = item;
                itemsUsedOnCount++;
            }
        }

        public void Clear()
        {
            successfulUsesCount = 0;
            usesCount = 0;
            itemsSuccessfullyUsedOnCount = 0;
            itemsUsedOnCount = 0;
        }

    }
}