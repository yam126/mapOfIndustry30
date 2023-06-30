if exists(select 1 from sysobjects where id=object_id('dbo.Create_osc_region') and xtype='P')
   drop PROCEDURE dbo.Create_osc_region
if exists(select 1 from sysobjects where id=object_id('dbo.Update_osc_region') and xtype='P')
   drop PROCEDURE dbo.Update_osc_region
if exists(select 1 from sysobjects where id=object_id('dbo.Query_osc_region') and xtype='P')
   drop PROCEDURE dbo.Query_osc_region
if exists(select 1 from sysobjects where id=object_id('dbo.Query_osc_region_Page') and xtype='P')
   drop PROCEDURE dbo.Query_osc_region_Page   
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Create_osc_region
(
                  @id int, --编号数据
                    @name varchar(150), --地块儿名称
                  @parent_id int, --上级地块儿编号
                    @citycode varchar(50), --城市编号
                    @adcode varchar(50), --区域编码
                    @center varchar(50), --中心坐标
                    @level varchar(50), --级别
                    @created varchar(50), --创建人
                  @CreatedTime datetime, --创建时间
                    @Modifier varchar(50), --修改人
                  @ModifiedTime datetime --修改时间
)
as
begin
     insert into osc_region
     (
               [id],
               [name],
               [parent_id],
               [citycode],
               [adcode],
               [center],
               [level],
               [created],
               [CreatedTime],
               [Modifier],
               [ModifiedTime]
     )
     values
     (
               @id,
               @name,
               @parent_id,
               @citycode,
               @adcode,
               @center,
               @level,
               @created,
               @CreatedTime,
               @Modifier,
               @ModifiedTime
     )
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Update_osc_region
(
                  @id int, --编号数据
                    @name varchar(150), --地块儿名称
                  @parent_id int, --上级地块儿编号
                    @citycode varchar(50), --城市编号
                    @adcode varchar(50), --区域编码
                    @center varchar(50), --中心坐标
                    @level varchar(50), --级别
                    @created varchar(50), --创建人
                  @CreatedTime datetime, --创建时间
                    @Modifier varchar(50), --修改人
                  @ModifiedTime datetime, --修改时间
         @SqlWhere NVARCHAR(max)
)
as
begin
     Declare @SqlStr nvarchar(max);
     Set @SqlStr='Update osc_region Set ';
              Set @SqlStr=@SqlStr+'id='+rtrim(ltrim(cast(@id as nvarchar(max))))+',';
              Set @SqlStr=@SqlStr+'name='''+@name+''',';
              Set @SqlStr=@SqlStr+'parent_id='+rtrim(ltrim(cast(@parent_id as nvarchar(max))))+',';
              Set @SqlStr=@SqlStr+'citycode='''+@citycode+''',';
              Set @SqlStr=@SqlStr+'adcode='''+@adcode+''',';
              Set @SqlStr=@SqlStr+'center='''+@center+''',';
              Set @SqlStr=@SqlStr+'level='''+@level+''',';
              Set @SqlStr=@SqlStr+'created='''+@created+''',';
              Set @SqlStr=@SqlStr+'CreatedTime='''+CONVERT(varchar,@CreatedTime,120)+''',';              
              Set @SqlStr=@SqlStr+'Modifier='''+@Modifier+''',';
              Set @SqlStr=@SqlStr+'ModifiedTime='''+CONVERT(varchar,@ModifiedTime,120)+'';              
    if @SqlWhere Is Not Null And @SqlWhere<>''
       Set @SqlStr=@SqlStr+' where '+@SqlWhere;
    exec(@SqlStr); 
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
create PROCEDURE Query_osc_region
(
    @SqlWhere Nvarchar(max)
)
as
begin
    Declare @SqlStr nvarchar(max);
	Set @SqlStr='select ';
                Set @SqlStr=@SqlStr+'[id],'
                Set @SqlStr=@SqlStr+'[name],'
                Set @SqlStr=@SqlStr+'[parent_id],'
                Set @SqlStr=@SqlStr+'[citycode],'
                Set @SqlStr=@SqlStr+'[adcode],'
                Set @SqlStr=@SqlStr+'[center],'
                Set @SqlStr=@SqlStr+'[level],'
                Set @SqlStr=@SqlStr+'[created],'
                Set @SqlStr=@SqlStr+'[CreatedTime],'
                Set @SqlStr=@SqlStr+'[Modifier],'
                Set @SqlStr=@SqlStr+'[ModifiedTime]'
    Set @SqlStr=@SqlStr+' from osc_region';
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
create PROCEDURE Query_osc_region_Page
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
    set @TableName='dbo.[osc_region]';
    set @SqlStr='select 
                  Row_Number() over(order by '+@SortField+' '+@SortMethod+') as Row,';
                Set @SqlStr=@SqlStr+'[id],'
                Set @SqlStr=@SqlStr+'[name],'
                Set @SqlStr=@SqlStr+'[parent_id],'
                Set @SqlStr=@SqlStr+'[citycode],'
                Set @SqlStr=@SqlStr+'[adcode],'
                Set @SqlStr=@SqlStr+'[center],'
                Set @SqlStr=@SqlStr+'[level],'
                Set @SqlStr=@SqlStr+'[created],'
                Set @SqlStr=@SqlStr+'[CreatedTime],'
                Set @SqlStr=@SqlStr+'[Modifier],'
                Set @SqlStr=@SqlStr+'[ModifiedTime]'
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