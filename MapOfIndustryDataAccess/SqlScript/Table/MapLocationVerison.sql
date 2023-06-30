CREATE TABLE [dbo].[MapLocationVerison]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [MapLocationVerison] INT NULL, 
    [Created] NVARCHAR(50) NULL, 
    [CreatedTime] DATETIME NULL,
    [landId] INT NULL
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'自增编号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MapLocationVerison',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'地图边界编号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MapLocationVerison',
    @level2type = N'COLUMN',
    @level2name = N'MapLocationVerison'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MapLocationVerison',
    @level2type = N'COLUMN',
    @level2name = N'Created'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MapLocationVerison',
    @level2type = N'COLUMN',
    @level2name = N'CreatedTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'地块儿编号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MapLocationVerison',
    @level2type = N'COLUMN',
    @level2name = N'landId'