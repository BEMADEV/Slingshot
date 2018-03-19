USE [ShelbyDB]
GO

/****** Object:  UserDefinedFunction [dbo].[KeepSafeCharacters_RemoveAccents]    Script Date: 5/9/2017 9:15:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[KeepSafeCharacters_RemoveAccents] (@Temp VARCHAR(1000))
RETURNS VARCHAR(1000)
AS
BEGIN
	-- GRAVE
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'u')
	-- ACUTE
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'Y')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'u')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'y')
	-- CIRCUMFLEX
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'u')
	-- TILDE
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'N')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'n')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'o')
	-- UMLAUT
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'Y')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'u')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, '�', 'y')

	RETURN Rtrim(Ltrim(@Temp))
END


GO


