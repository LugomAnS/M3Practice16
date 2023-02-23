CREATE TABLE [dbo].[Table]
(
	[ID] INT NOT NULL IDENTITY, 
    [Surname] NCHAR(15) NOT NULL, 
    [Name] NCHAR(15) NOT NULL, 
    [Patronymic] NCHAR(15) NOT NULL, 
    [Phone] NCHAR(15) NULL, 
    [Email] NVARCHAR(50) NOT NULL 
)
