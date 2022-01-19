IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Pessoas] (
    [Id] int NOT NULL IDENTITY,
    [Nome] varchar(50) NULL,
    CONSTRAINT [PK_Pessoas] PRIMARY KEY ([Id])
);
GO

SELECT 1
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220118193211_PrimeiraMigracao', N'5.0.3');
GO

COMMIT;
GO

