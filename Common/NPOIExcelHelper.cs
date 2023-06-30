using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// NPOI操作Excel
    /// </summary>
    public class NPOIExcelHelper
    {
        /// <summary>
        /// 将excel导入到datatable
        /// </summary>
        /// <param name="filePath">excel路径</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回datatable</returns>
        public static DataSet ExcelToDataTable(string filePath, bool isColumnName, out string message)
        {
            #region 声明
            message = string.Empty;
            DataSet dataSet = new DataSet();
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            #endregion

            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);
                    if (workbook != null)
                    {
                        for (int s = 0; s < workbook.NumberOfSheets; s++)
                        {
                            #region 读取单个sheet
                            sheet = workbook.GetSheetAt(s);//读取第一个sheet，当然也可以循环读取每个sheet
                            dataTable = new DataTable(sheet.SheetName);
                            if (sheet != null)
                            {
                                int rowCount = sheet.LastRowNum;//总行数
                                if (rowCount > 0)
                                {
                                    IRow firstRow = sheet.GetRow(0);//第一行
                                    int cellCount = firstRow.LastCellNum;//列数

                                    #region 构建datatable的列
                                    if (isColumnName)
                                    {
                                        startRow = 1;//如果第一行是列名，则从第二行开始读取
                                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                        {
                                            cell = firstRow.GetCell(i);
                                            if (cell != null)
                                            {
                                                if (cell.StringCellValue != null)
                                                {
                                                    column = new DataColumn(cell.StringCellValue);
                                                    dataTable.Columns.Add(column);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                        {
                                            column = new DataColumn("column" + (i + 1));
                                            dataTable.Columns.Add(column);
                                        }
                                    }
                                    #endregion

                                    #region 填充行
                                    for (int i = startRow; i <= rowCount; ++i)
                                    {
                                        row = sheet.GetRow(i);
                                        if (row == null) continue;
                                        dataRow = dataTable.NewRow();
                                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                                        {
                                            cell = row.GetCell(j);
                                            if (cell == null)
                                            {
                                                dataRow[j] = "";
                                            }
                                            else
                                            {
                                                //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                                switch (cell.CellType)
                                                {
                                                    case CellType.Blank:
                                                        dataRow[j] = "";
                                                        break;
                                                    case CellType.Numeric:
                                                        short format = cell.CellStyle.DataFormat;
                                                        //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                        if (format == 14 || format == 31 || format == 57 || format == 58 || format == 176)
                                                            dataRow[j] = cell.DateCellValue;
                                                        else
                                                            dataRow[j] = cell.NumericCellValue;
                                                        break;
                                                    case CellType.String:
                                                        dataRow[j] = cell.StringCellValue;
                                                        break;
                                                }
                                            }
                                        }
                                        dataTable.Rows.Add(dataRow);
                                    }
                                    #endregion
                                }
                                dataSet.Tables.Add(dataTable);
                            }
                            #endregion
                        }
                    }
                }
                return dataSet;
            }
            catch (Exception exp)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                message = exp.Message;
                return null;
            }
        }

        /// <summary>
        /// 数据表到Excel
        /// </summary>
        /// <param name="workbook">Excel表</param>
        /// <param name="dt">数据表</param>
        /// <param name="SheetName">Sheet名</param>
        private static void DataTableToExcel(ref IWorkbook workbook, DataTable dt, string SheetName)
        {
            #region 声明
            int rowCount = dt.Rows.Count;//行数  
            int columnCount = dt.Columns.Count;//列数  
            IRow row = null;//Excel行
            ISheet sheet = null;//Excel Sheet
            ICell cell = null;//Excel列
            #endregion

            //创建Sheet表，参数Sheet名称
            sheet = workbook.CreateSheet(SheetName);

            //表头样式设置
            ICellStyle cellHeaderStyle = workbook.CreateCellStyle();
            cellHeaderStyle.FillPattern = FillPattern.SolidForeground;
            cellHeaderStyle.FillForegroundColor = HSSFColor.Yellow.Index2;
            SetExcelBorderStyle(ref cellHeaderStyle);

            //数据行样式设置
            ICellStyle cellStyle = workbook.CreateCellStyle();
            SetExcelBorderStyle(ref cellStyle);

            //日期型样式设置
            IDataFormat dateformat = workbook.CreateDataFormat();
            ICellStyle dateStyle = workbook.CreateCellStyle();
            dateStyle.DataFormat = dateformat.GetFormat("yyyy-mm-dd");

            //文本型样式设置
            IDataFormat textformat = workbook.CreateDataFormat();
            ICellStyle textStyle = workbook.CreateCellStyle();
            textStyle.DataFormat = textformat.GetFormat("@");

            #region 设置列头
            row = sheet.CreateRow(0);//excel第一行设为列头  
            for (int c = 0; c < columnCount; c++)
            {
                cell = row.CreateCell(c);
                //设置背景颜色
                cell.CellStyle = cellHeaderStyle;
                cell.SetCellValue(dt.Columns[c].ColumnName);
            }
            #endregion

            #region 设置每行每列的单元格,
            for (int i = 0; i < rowCount; i++)
            {
                IRow irow = sheet.CreateRow(i + 1);
                for (int j = 0; j < columnCount; j++)
                {
                    ICell Cellrow = irow.CreateCell(j);//excel第二行开始写入数据           
                    ConvertToNPOIExcelType(dt.Columns[j].DataType, dt.Rows[i][j].ToString(), ref Cellrow, ref cellStyle, ref dateStyle, ref textStyle);
                }
            }
            #endregion

            if (dt.TableName.IndexOf("上海医药销售日报") == -1)
                SetAutoSizeColumn(ref dt, ref sheet);
        }

        /// <summary>
        /// 数据表导出Excel
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="FilePath">Excel文件路径</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool DataTableToExcel(DataTable dt, string FilePath, out string message)
        {
            #region 声明变量
            //是否生成成功
            bool result = false;
            //NPOI生成excel
            IWorkbook workbook = null;

            XSSFWorkbook xworkbook = null;
            //文件写入
            FileStream fs = null;
            //错误消息
            message = string.Empty;

            #region 分页设置
            int DataCount = 0;
            int PageSize = 65533;
            int PageCount = 0;
            #endregion
            #endregion
            try
            {
                if (dt == null || dt.Rows.Count <= 0)
                    return false;
                string[] FileExtension = FilePath.Split('.');
                if (FileExtension[FileExtension.Length - 1] == "xls")
                    workbook = new HSSFWorkbook();
                else if (FileExtension[FileExtension.Length - 1] == "xlsx")
                {
                    //if (!File.Exists(FilePath))
                    //{
                    //    using (fs = new FileStream(FilePath, FileMode.Create))
                    //    {
                    //        //workbook.Write(fs);//向打开的这个xls文件中写入数据 
                    //        fs.Close();
                    //        fs.Dispose();
                    //        result = true;
                    //    }
                    //}
                    workbook = new XSSFWorkbook();

                }
                DataCount = dt.Rows.Count;//行数  
                if (DataCount < PageSize)
                {
                    DataTableToExcel(ref workbook, dt, "Sheet0");
                    PageCount = 1;
                    result = true;
                }
                else
                {
                    if (DataCount % PageSize == 0)
                        PageCount = DataCount / PageSize;
                    else
                        PageCount = (DataCount / PageSize) + 1;
                    for (int i = 1; i <= PageCount; i++)
                    {
                        DataTable iPageData = GetPagedTable(dt, i, PageSize);
                        if (iPageData == null || iPageData.Rows.Count <= 0)
                            continue;
                        DataTableToExcel(ref workbook, iPageData, "Sheet" + Convert.ToString(i));
                    }
                }
                using (fs = new FileStream(FilePath, FileMode.Create))
                {
                    workbook.Write(fs);//向打开的这个xls文件中写入数据 
                    fs.Close();
                    fs.Dispose();
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                message = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// 数据表导出Excel
        /// </summary>
        /// <param name="workbook">Excel工作表</param>
        /// <param name="dt">数据表</param>
        /// <param name="FilePath">Excel文件路径</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static bool DataTableToExcel(ref IWorkbook workbook, DataTable dt, out string message)
        {
            #region 声明变量
            //是否生成成功
            bool result = false;
            //错误消息
            message = string.Empty;

            #region 分页设置
            int DataCount = 0;
            int PageSize = 65533;
            int PageCount = 0;
            #endregion
            #endregion
            try
            {
                if (dt == null || dt.Rows.Count <= 0)
                    return false;
                if (workbook == null)
                    workbook = new HSSFWorkbook();
                DataCount = dt.Rows.Count;//行数  
                if (DataCount < PageSize)
                {
                    DataTableToExcel(ref workbook, dt, dt.TableName);
                    PageCount = 1;
                    result = true;
                }
                else
                {
                    if (DataCount % PageSize == 0)
                        PageCount = DataCount / PageSize;
                    else
                        PageCount = (DataCount / PageSize) + 1;
                    for (int i = 1; i <= PageCount; i++)
                    {
                        DataTable iPageData = GetPagedTable(dt, i, PageSize);
                        if (iPageData == null || iPageData.Rows.Count <= 0)
                            continue;
                        DataTableToExcel(ref workbook, iPageData, dt.TableName + Convert.ToString(i));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 数据集导出Excel
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="FilePath">文件路径</param>
        /// <param name="message">错误消息</param>
        /// <returns>是否成功</returns>
        public static bool DateSetToExcel(DataSet ds, string FilePath, out string message)
        {
            #region 声明部分

            //错误消息
            message = string.Empty;

            //文件错误消息
            string fileMessage = string.Empty;

            //Excel工作表
            IWorkbook workbook = new HSSFWorkbook(); ;

            //返回值
            bool result = true;

            #endregion

            #region 循环DataSet

            foreach (DataTable dt in ds.Tables)
            {
                string TableMessage = string.Empty;
                result = DataTableToExcel(ref workbook, dt, out TableMessage);
                message += TableMessage;
            }

            #endregion

            //写入Excel到文件
            WorkbookWriterToFile(workbook, FilePath, FileMode.Create, out fileMessage);
            message += fileMessage;

            return result;
        }

        /// <summary>
        /// 写入工作表到Excel文件
        /// </summary>
        /// <param name="workbook">工作表</param>
        /// <param name="message">错误消息</param>
        /// <param name="FilePath">路径</param>
        /// <param name="Method">写入方式，创建或追加</param>
        private static void WorkbookWriterToFile(IWorkbook workbook, string FilePath, FileMode Method, out string message)
        {
            message = string.Empty;
            FileStream fs = null;
            try
            {
                using (fs = new FileStream(FilePath, Method))
                {
                    workbook.Write(fs);//向打开的这个xls文件中写入数据 
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception exp)
            {
                message = exp.Message;
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 设置Excel边框样式
        /// </summary>
        /// <param name="cellStyle">Excel样式</param>
        private static void SetExcelBorderStyle(ref ICellStyle cellStyle)
        {
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;
        }

        /// <summary>
        /// 设置列宽根据内容自适应
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="paymentSheet">Sheet表</param>
        private static void SetAutoSizeColumn(ref DataTable dt, ref ISheet paymentSheet)
        {
            //列宽自适应，只对英文和数字有效
            for (int i = 0; i <= dt.Columns.Count; i++)
            {
                paymentSheet.AutoSizeColumn(i);
            }
            //获取当前列的宽度，然后对比本列的长度，取最大值
            for (int columnNum = 0; columnNum <= dt.Columns.Count; columnNum++)
            {
                int columnWidth = paymentSheet.GetColumnWidth(columnNum) / 256;
                for (int rowNum = 1; rowNum <= paymentSheet.LastRowNum; rowNum++)
                {
                    IRow currentRow;
                    //当前行未被使用过
                    if (paymentSheet.GetRow(rowNum) == null)
                    {
                        currentRow = paymentSheet.CreateRow(rowNum);
                    }
                    else
                    {
                        currentRow = paymentSheet.GetRow(rowNum);
                    }

                    if (currentRow.GetCell(columnNum) != null)
                    {
                        ICell currentCell = currentRow.GetCell(columnNum);
                        int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                paymentSheet.SetColumnWidth(columnNum, columnWidth * 256);
            }
        }

        /// <summary>
        /// 获得数据表分页数据
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">每页记录数</param>
        /// <returns>分页数据</returns>
        private static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)//PageIndex表示第几页，PageSize表示每页的记录数
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = new DataTable();
            foreach (DataColumn dc in dt.Columns)
            {
                newdt.Columns.Add(new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));
            }

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="datavalue"></param>
        /// <param name="cell"></param>
        public static void ConvertToNPOIExcelType(Type DataType, string datavalue, ref ICell cell, ref ICellStyle cellStyle, ref ICellStyle dateformat, ref ICellStyle textformat)
        {
            switch (DataType.FullName)
            {
                case "System.Decimal":
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell.CellStyle = cellStyle;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(double.Parse(datavalue));
                    break;
                case "System.Boolean":
                    cell.SetCellType(CellType.Boolean);
                    cell.SetCellValue(datavalue);
                    break;
                case "System.String":
                    DateTime dt1 = Utils.StrToDateTime(datavalue, "MM/dd/yyyy");
                    if (dt1 == (DateTime)SqlDateTime.MinValue)
                    {
                        if (!string.IsNullOrEmpty(datavalue) && datavalue.Length > 3)
                        {
                            if (datavalue.Substring(0, 3) == "@!N")
                            {
                                datavalue = datavalue.Replace("@!N", "");
                                cell.SetCellValue(datavalue);
                                cell.CellStyle = textformat;
                            }
                            else
                            {
                                cell.SetCellType(CellType.String);
                                cell.SetCellValue(datavalue);
                            }
                        }
                        else
                        {
                            cell.SetCellType(CellType.String);
                            cell.SetCellValue(datavalue);
                        }
                    }
                    else
                    {
                        cell.SetCellValue(dt1);
                        cell.CellStyle = dateformat;
                    }
                    break;
                case "System.DateTime":
                    DateTime dt = Utils.StrToDateTime(datavalue, "MM/dd/yyyy");
                    string dtvalue = string.Empty;
                    cell.SetCellValue(dt);
                    cell.CellStyle = dateformat;
                    break;
                case "System.Int16":
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell.CellStyle = cellStyle;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(double.Parse(datavalue));
                    break;
                case "System.Int32":
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                    cell.CellStyle = cellStyle;
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(int.Parse(datavalue));
                    break;
                case "System.Int64":
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(datavalue);
                    break;
                case "System.Guid":
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(datavalue);
                    break;
                    //case SqlDbType.Real:
                    //    return typeof(Single);
                    //case SqlDbType.SmallDateTime:
                    //    return typeof(DateTime);
                    //case SqlDbType.SmallInt:
                    //    return typeof(Int16);
                    //case SqlDbType.SmallMoney:
                    //    return typeof(Decimal);
                    //case SqlDbType.Text:
                    //    return typeof(String);
                    //case SqlDbType.Timestamp:
                    //    return typeof(Object);
                    //case SqlDbType.TinyInt:
                    //    return typeof(Byte);
                    //case SqlDbType.Udt://自定义的数据类型
                    //    return typeof(Object);
                    //case SqlDbType.VarBinary:
                    //    return typeof(Object);
                    //case SqlDbType.Variant:
                    //    return typeof(Object);
                    //case SqlDbType.Xml:
                    //    return typeof(Object);
                    //default:
                    //    return null;
            }
        }
    }
}
