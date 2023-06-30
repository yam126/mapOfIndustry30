if exists(select 1 from sysobjects where id=object_id('dbo.Create_MapLocationVerison') and xtype='P')
   drop PROCEDURE dbo.Create_MapLocationVerison
if exists(select 1 from sysobjects where id=object_id('dbo.Update_MapLocationVerison') and xtype='P')
   drop PROCEDURE dbo.Update_MapLocationVerison
if exists(select 1 from sysobjects where id=object_id('dbo.Query_MapLocationVerison') and xtype='P')
   drop PROCEDURE dbo.Query_MapLocationVerison
if exists(select 1 from sysobjects where id=object_id('dbo.Query_MapLocationVerison_Page') and xtype='P')
   drop PROCEDURE dbo.Query_MapLocationVerison_Page   
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Create_MapLocationVerison
(
                  @MapLocalVerison int, --地图边界编号
                    @Created nvarchar(50), --创建人
                  @CreatedTime datetime, --创建时间
                  @landId int --地块儿编号
)
as
begin
     insert into MapLocationVerison
     (
               [MapLocalVerison],
               [Created],
               [CreatedTime],
               [landId]
     )
     values
     (
               @MapLocalVerison,
               @Created,
               @CreatedTime,
               @landId
     )
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Update_MapLocationVerison
(
                  @MapLocalVerison int, --地图边界编号
                    @Created nvarchar(50), --创建人
                  @CreatedTime datetime, --创建时间
                  @landId int, --地块儿编号
         @SqlWhere NVARCHAR(max)
)
as
begin
     Declare @SqlStr nvarchar(max);
     Set @SqlStr='Update MapLocationVerison Set ';
              Set @SqlStr=@SqlStr+'MapLocalVerison='+rtrim(ltrim(cast(@MapLocalVerison as nvarchar(max))))+',';
              Set @SqlStr=@SqlStr+'Created='''+@Created+''',';
              Set @SqlStr=@SqlStr+'CreatedTime='''+CONVERT(varchar,@CreatedTime,120)+''',';              
              Set @SqlStr=@SqlStr+'landId='+rtrim(ltrim(cast(@landId as nvarchar(max))));
    if @SqlWhere Is Not Null And @SqlWhere<>''
       Set @SqlStr=@SqlStr+' where '+@SqlWhere;
    exec(@SqlStr); 
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Query_MapLocationVerison
(
    @SqlWhere Nvarchar(max)
)
as
begin
    Declare @SqlStr nvarchar(max);
	Set @SqlStr='select ';
                Set @SqlStr=@SqlStr+'[Id],'
                Set @SqlStr=@SqlStr+'[MapLocalVerison],'
                Set @SqlStr=@SqlStr+'[Created],'
                Set @SqlStr=@SqlStr+'[CreatedTime],'
                Set @SqlStr=@SqlStr+'[landId]'
    Set @SqlStr=@SqlStr+' from MapLocationVerison';
	if @SqlWhere is not Null Or @SqlWhere<>''
	begin
	   Set @SqlStr=@SqlStr+' where '+@SqlWhere; 
	end
	exec(@SqlStr); 
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Query_MapLocationVerison_Page
(
  @StartRow int, --开始位置
  @EndRow int, --结束位置
  @TotalNumber int out,--总数据量
  @SortField nvarchar(max),--排序字段
  @SortMethod nvarchar(10),--排序方法
  @SqlWhere nvarchar(max) --查询条件
)
as
BEGIN
	declare @SqlStr nvarchar(max),
    @totalsql nvarchar(max),
    @PageSql nvarchar(max),
    @TableName nvarchar(max);
    set @TableName='dbo.[MapLocationVerison]';
    set @SqlStr='select 
                  Row_Number() over(order by '+@SortField+' '+@SortMethod+') as Row,';
                Set @SqlStr=@SqlStr+'[Id],'
                Set @SqlStr=@SqlStr+'[MapLocalVerison],'
                Set @SqlStr=@SqlStr+'[Created],'
                Set @SqlStr=@SqlStr+'[CreatedTime],'
                Set @SqlStr=@SqlStr+'[landId]'
    Set @SqlStr=@SqlStr+' from '+@TableName;
    if @SqlWhere<>'' 
    Begin
       set @SqlStr=@SqlStr+' Where '+@SqlWhere;
    End
    set @totalsql='with Result as('+@SqlStr+')select @t=count(*) from Result';
    EXEC sp_executesql
        @totalsql, 
        N'@t AS INT OUTPUT',
        @t = @TotalNumber OUTPUT;
    set @PageSql='with Result as('+@SqlStr+')select * from Result where Row>='+cast(@StartRow as varchar)+' and Row<='+cast(@EndRow as varchar);
    print @PageSql;
    exec(@PageSql);	
END
Go