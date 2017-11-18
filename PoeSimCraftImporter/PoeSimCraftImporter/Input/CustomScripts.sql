-------------------------------------- Total Defenses/Resistances --------------------------------------

INSERT INTO Affix (Domain, GenerationType, [Group], [Name], [ModName], ILvl)
VALUES  ('Item','Meta','Total Energy Shield','Total Energy Shield','Total Energy Shield',1),
        ('Item','Meta','Total Armour','Total Armour','Total Armour',1),
        ('Item','Meta','Total Evasion','Total Evasion','Total Evasion',1),
        ('Item','Meta','Total Resistances','Total Resistances','Total Resistances',1),
        ('Item','Meta','Total Elemental Resistances','Total Elemental Resistances','Total Elemental Resistances',1)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='Total Energy Shield'
    ),
	t.SpawnTagId,
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='Increased Energy Shield'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='TotalArmour'
    ),
	t.SpawnTagId, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='Increased Physical Damage Reduction Rating'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='Total Evasion'
    ),
	t.SpawnTagId, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='Increased Evasion Rating'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='Evasion'
    ),
	t.SpawnTagId, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] ='Increased Evasion Rating'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='Total Resistances'
    ),
	t.SpawnTagId, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] LIKE'%Resistance%'
AND i.GenerationType ='Suffix'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'Total Elemental Resistances'
    ),
	t.SpawnTagId, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] LIKE'%Resistance%'
AND i.GenerationType ='Suffix'

-------------------------------------- Global Spawn Tag --------------------------------------

INSERT INTO SpawnTag (Name)
VALUES ('Global')

INSERT ItemSpawnTagMap
(
    ItemId, SpawnTagId
)
SELECT i.ItemId, (SELECT SpawnTagId
	FROM SpawnTag
	WHERE [Name] ='Global')
FROM Item i

-------------------------------------- Open Prefixes/Suffixes --------------------------------------

INSERT INTO Affix (Domain, GenerationType, [Name], ModName, [Group], StatMin1, StatMax1)
VALUES  ('Item','Meta', '', 'Open Prefix', 'Open Prefix', 1, 3),
        ('Item','Meta', '', 'Open Prefix', 'Open Suffix', 1, 3)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='Open Prefix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] ='Global'
	), 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] ='OpenSuffix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] ='Global'
	), 
	0
)


