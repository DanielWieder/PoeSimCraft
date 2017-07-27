-------------------------------------- Total Defenses/Resistances --------------------------------------

INSERT INTO Affix (Domain, GenerationType, [Group], [Name], [ModName], Level)
VALUES  ('item','meta','TotalEnergyShield','TotalEnergyShield','TotalEnergyShield',1),
        ('item','meta','TotalArmour','TotalArmour','TotalArmour',1),
        ('item','meta','TotalEvasion','TotalEvasion','TotalEvasion',1),
        ('item','meta','TotalResistances','TotalResistances','TotalResistances',1),
        ('item','meta','TotalElementalResistances','TotalElementalResistances','TotalElementalResistances',1)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='TotalEnergyShield'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='IncreasedEnergyShield'
AND t.Value ='1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='TotalArmour'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='IncreasedPhysicalDamageReductionRating'
AND t.Value ='1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='TotalEvasion'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='IncreasedEvasionRating'
AND t.Value ='1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='Evasion'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='IncreasedEvasionRating'
AND t.Value ='1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='TotalResistances'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] LIKE'%Resistance%'
AND i.GenerationType ='suffix'
AND t.Value ='1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='TotalElementalResistances'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] LIKE'%Resistance%'
AND i.GenerationType ='suffix'
AND t.Value ='1'

-------------------------------------- Global Spawn Tag --------------------------------------

INSERT INTO SpawnTag (Name)
VALUES ('global')

INSERT ItemSpawnTagMap
(
    ItemId, SpawnTagId
)
SELECT i.ItemId, (SELECT SpawnTagId
	FROM SpawnTag
	WHERE [Name] ='global')
FROM Item i

-------------------------------------- Open Prefixes/Suffixes --------------------------------------

INSERT INTO Affix (Domain, GenerationType, [Group], StatMin1, StatMax1)
VALUES  ('item','meta','OpenPrefix', 1, 3),
        ('item','meta','OpenSuffix', 1, 3)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='OpenPrefix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] ='global'
	), 
	1, 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='OpenSuffix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] ='global'
	), 
	1, 
	0
)

