
SELECT
	Individual_ID AS [PersonId]
	,ROW_NUMBER() OVER (ORDER BY [NoteTextCreated]) AS [Id]
	,[Note_Type_Name] AS [NoteType]
	,'' AS [Caption]
	,'FALSE' AS [IsAlert]
	,'FALSE' AS [IsPrivateNote]
	,[Note_Text] AS [Text]
	,[NoteTextCreated] AS [DateTime]
	,[NoteCreatedByUserID] AS [CreatedByPersonId]
FROM [Notes]
WHERE Individual_ID IS NOT NULL
ORDER BY [NoteTextCreated]