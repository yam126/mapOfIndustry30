var formatDate = {
    fixLeadingZero(value, fixLength) {
        var str = value;
        console.log("fixLeadingZero");
        console.log(str);
        if (str == '' || str == null || typeof (str) == "undefined")
            return str;
        if (str.length < fixLength) {
            var len = fixLength - str.length;
            var tempStr = "";
            for (var i = 0; i < len; i++)
                tempStr += "0";
            str = tempStr + str;
        }
        return str;
    },
    dateTimeFormat(sourceDateString, isHaveTime, dateSymbol)
    {
        var chineseWeeks = ['周一', '周二', '周三', '周四', '周五', '周六', '周日', '周天'];
        var result = "";
        var dateStr = "";
        var timeStr = "";
        var dateAry = [];
        console.log("sourceDateString=" + sourceDateString);
        if (dateSymbol == null || dateSymbol == "" || dateSymbol == undefined)
            dateSymbol = "-";
        for (var i = 0; i < chineseWeeks.length; i++)
            sourceDateString = sourceDateString.replace(chineseWeeks[i], "");
        dateStr = sourceDateString.split(' ')[0];
        console.log("dateStr=" + dateStr);
        dateStr = dateStr.substring(0, dateStr.length - 1);
        timeStr = result.split(' ')[1];
        dateAry = dateStr.split('/');
        console.log("dateAry");
        console.log(dateAry);
        dateAry[0] = this.fixLeadingZero(dateAry[0], 4);
        dateAry[1] = this.fixLeadingZero(dateAry[1], 2);
        dateAry[2] = this.fixLeadingZero(dateAry[2], 2);
        dateStr = "";
        for (var j = 0; j < dateAry.length; j++)
            dateStr += dateAry[j] + dateSymbol;
        if (dateStr != "")
            dateStr = dateStr.substring(0, dateStr.length - 1);
        if (isHaveTime)
            result = dateStr + " " + timeStr;
        else
            result = dateStr;
        return result;
    }
}