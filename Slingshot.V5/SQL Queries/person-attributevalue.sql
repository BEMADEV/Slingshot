select 
[NameCounter] as [PersonId]
, [Profile] as [AttributeKey]
--, Min([Start]) as [AttributeValue]
, CONVERT(VARCHAR(10),Min([Start]), 101) as [AttributeValue]
FROM [Shelby].[NAProfiles]
WHERE [Profile] IN ('1-VISIT') and [Start] is not null
group by [NameCounter],[Profile]
UNION ALL

SELECT
	distinct 	
	[NameCounter] AS [PersonId],
	[Profile] as [AttributeKey],
	'TRUE' AS [AttributeValue]
FROM [Shelby].[NAProfiles]
WHERE [Profile] IN ('RNOWLIST')
UNION ALL

SELECT
	distinct 	
	[NameCounter] AS [PersonId],
	[Profile] as [AttributeKey],
	'TRUE' AS [AttributeValue]
FROM [Shelby].[NAProfiles]
WHERE [Profile] IN ('18VFAITH')
UNION ALL

SELECT
	distinct 	
	[NameCounter] AS [PersonId],
	[Profile] as [AttributeKey],
	'TRUE' AS [AttributeValue]
FROM [Shelby].[NAProfiles]
WHERE [Profile] IN ('18VGREET')
UNION ALL

--Membership Date
select 
[NameCounter] AS [PersonId]
, 'MembershipDate' as [AttributeKey]
,  CONVERT(VARCHAR(10),ISNULL(DateReceived, GETDATE()), 101) as [AttributeValue]
from Shelby.vw_MBNames
where DateReceived is not null and MemberStatus = 'Member'
UNION ALL

--Baptism Date
select 
[NameCounter] AS [PersonId]
, 'BaptismDate' as [AttributeKey]
,  CONVERT(VARCHAR(10),ISNULL(DateReceived, GETDATE()), 101) as [AttributeValue]
from Shelby.vw_MBNames
where HowReceived = 'Baptism' and DateReceived is not null
UNION ALL


--Baptized Here
select 
[NameCounter] AS [PersonId]
, 'BaptizedHere' as [AttributeKey]
,  '1' as [AttributeValue]
from Shelby.vw_MBNames
where HowReceived = 'Baptism' and DateReceived is not null
