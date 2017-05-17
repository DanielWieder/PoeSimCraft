/* Type Spawn Tags */

  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, st.SpawnTagId
  From Item i
  JOIN ItemSubType s ON i.ItemSubTypeId = s.ItemSubTypeId
  JOIN ItemType t ON t.ItemTypeId = s.ItemTypeId
  CROSS JOIN SpawnTag st
  where REPLACE(st.Name, '_', ' ') = LOWER(t.Name)
  
  /* Subtype Spawn Tags */
  
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, st.SpawnTagId
  From Item i
  JOIN ItemSubType s ON i.ItemSubTypeId = s.ItemSubTypeId
  JOIN ItemType t ON t.ItemTypeId = s.ItemTypeId
  CROSS JOIN SpawnTag st
  where REPLACE(st.Name, '_', ' ') = LOWER(s.Name)  
  
  /* Unset Ring Spawn Tags */
  
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, st.SpawnTagId
  From Item i
  JOIN ItemSubType s ON i.ItemSubTypeId = s.ItemSubTypeId
  JOIN ItemType t ON t.ItemTypeId = s.ItemTypeId
  CROSS JOIN SpawnTag st
  where REPLACE(st.Name, '_', ' ') = LOWER(i.Name)    
  
    /* One Handed Weapon */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, 8073
  From Item i
  JOIN ItemSubType s ON i.ItemSubTypeId = s.ItemSubTypeId
  JOIN ItemType t ON t.ItemTypeId = s.ItemTypeId
  WHERE s.ItemSubtypeID IN (35, 36, 37, 38, 39, 40, 42, 46)
  

  /* Two Handed Weapon */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, 8061
  From Item i
  JOIN ItemSubType s ON i.ItemSubTypeId = s.ItemSubTypeId
  JOIN ItemType t ON t.ItemTypeId = s.ItemTypeId
  WHERE s.ItemSubtypeID IN (41, 43, 44, 45, 34)
  
      /* Str Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'str_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Str > 0

    /* Dex Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'dex_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Dex > 0

      /* Int Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'int_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Int > 0

  /* Int/Dex Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'dex_int_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Int > 0 AND a.Dex > 0

  /* Int/Str Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'str_int_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Int > 0 AND a.Str > 0

  /* Dex/Str Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'str_dex_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Dex > 0 AND a.Str > 0

  /* Dex/Str/Int Armour */
  INSERT INTO ItemSpawnTagMap (ItemId, SpawnTagId)
  Select i.ItemId, (SELECT SpawnTagId FROM SpawnTag WHERE Name = 'str_dex_int_armour')
  From Item i
  JOIN Armour a on i.ItemId = a.ItemId
  WHERE a.Dex > 0 AND a.Str > 0 AND a.Int > 0