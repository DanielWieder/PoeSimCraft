-------------------------------------- Total Defenses/Resistances --------------------------------------

INSERT INTO Affix (Domain, GenerationType, [Group])
VALUES  ('item', 'meta', 'TotalEnergyShield'),
        ('item', 'meta', 'TotalArmour'),
        ('item', 'meta', 'TotalEvasion'),
        ('item', 'meta', 'TotalResistances'),
        ('item', 'meta', 'TotalElementalResistances')

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TotalEnergyShield'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] = 'IncreasedEnergyShield'
AND t.Value = '1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TotalArmour'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] = 'IncreasedPhysicalDamageReductionRating'
AND t.Value = '1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TotalEvasion'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] = 'IncreasedEvasionRating'
AND t.Value = '1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'Evasion'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] = 'IncreasedEvasionRating'
AND t.Value = '1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TotalResistances'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] LIKE '%Resistance%'
AND i.GenerationType = 'suffix'
AND t.Value = '1'

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
	SELECT DISTINCT 
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TotalElementalResistances'
    ),
	t.SpawnTagId, 
	1, 
	0
FROM [AffixSpawnTagMap] t
JOIN Affix i ON t.AffixId = i.AffixId
WHERE i.[Group] LIKE '%Resistance%'
AND i.GenerationType = 'suffix'
AND t.Value = '1'

-------------------------------------- Open Prefixes/Suffixes --------------------------------------

INSERT INTO Affix (Domain, GenerationType, [Group])
VALUES  ('item', 'meta', 'SingleOpenPrefix'),
        ('item', 'meta', 'DoubleOpenPrefix'),
        ('item', 'meta', 'TripleOpenPrefix'),
        ('item', 'meta', 'SingleOpenSuffix'),
        ('item', 'meta', 'DoubleOpenSuffix'),
		('item', 'meta', 'TripleOpenSuffix')


INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'SingleOpenPrefix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] = 'Default'
	), 
	1, 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'DoubleOpenPrefix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] = 'Default'
	), 
	1, 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TripleOpenPrefix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] = 'Default'
	), 
	1, 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'SingleOpenSuffix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] = 'Default'
	), 
	1, 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'DoubleOpenSuffix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] = 'Default'
	), 
	1, 
	0
)

INSERT INTO AffixSpawnTagMap (AffixId, SpawnTagId, [Value], [Weight])
VALUES(
	(
        SELECT AffixId
        FROM Affix
        WHERE [Group] = 'TripleOpenSuffix'
    ),
	(
		SELECT SpawnTagId
		FROM SpawnTag
		WHERE [Name] = 'Default'
	), 
	1, 
	0
)