var MassifGreenHouseVP = function () {
    this.id = "";
    this.landId = "";
    this.landName = "";
    this.cropsName = "";
    this.plantingArea = "0";
    this.jobPopulation = "0";
    this.totalPopulation = "0";
    this.totalOutput = "0";
    this.totaValue = "0";
    this.soilType = "";
    this.wateringType = "";
    this.landProperty = "";
    this.leaseYear = "0";
    this.cropOutput = "0";
    this.lastYearOutput = "0";
    this.currentYearOutput = "0";
    this.cropsSalesPrice = "0";
    this.enterTime = "";
    this.creater = "";
};
var queryPageParameter = {
    where: '',
    pageIndex: "",
    pageSize: "",
    sortField: '',
    sortMethod: ''
};
var queryParam = {
    where: '',
    sortField: '',
    sortMethod: ''
};
//api帮助
var apiHelper = {
    //#region 调试配置
    urlBase: 'http://localhost:5872/api',
    //urlBase: 'https://192.168.253.1:7872/api',
    urlHost: 'http://localhost:5872/',
    //urlHost: 'https://192.168.253.1:7872',
    urlBaseFram: 'http://8.142.169.233:1001/api/edms/v1',
    currentServerUrl: 'http://8.142.169.233:5001',
    modulesApiUrl: 'http://8.142.169.233:5001/api/v1/user/modules',
    //#endregion 
    //#region 服务器配置
    //urlBase: 'http://8.142.169.233:6001/api/moi/v1/api',
    //urlHost: 'http://8.142.169.233:6001/api/moi/v1',
    //urlBaseFram: 'http://8.142.169.233:1001/api/edms/v1',
    //currentServerUrl: 'http://8.142.169.233:5001',
    //modulesApiUrl: 'http://8.142.169.233:5001/api/v1/user/modules',
    //#endregion
    response: null,
    token: "",
    level: -1,
    UnionQuery: {
        CompanyType: '',
        RegionalLevel: '',
        EnterpriseNature: ''
    },
    //getCookie(cookieName) {
    //    var cookieString = document.cookie;
    //    var start = cookieString.indexOf(cookieName + '=');
    //    // 加上等号的原因是避免在某些 Cookie 的值里有
    //    // 与 cookieName 一样的字符串。
    //    if (start == -1) // 找不到
    //        return null;
    //    start += cookieName.length + 1;
    //    var end = cookieString.indexOf(';', start);
    //    if (end == -1)
    //        return unescape(cookieString.substring(start));
    //    return unescape(cookieString.substring(start, end));
    //},
    getCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1);
            if (c.indexOf(name) != -1) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    },
    //写入cookie
    //cookieName:cookie名称
    //cookieValue:cookie值
    //expiresTime:过期时间(分钟)
    setCookie(cookieName, cookieValue, expiresTime) {
        var exp = new Date();
        exp.setTime(exp.getTime() + 60 * 1000 * expiresTime);
        document.cookie = cookieName + "=" + escape(cookieValue) + ";expires=" + exp.toGMTString() + ";path=/";
    },
    getQueryString: function (queryParam) {
        var queryStr = "";
        for (var paramName in queryParam) {
            if (queryParam[paramName] != "")
                queryStr += paramName + "=" + queryParam[paramName] + "&";
        }
        if (queryStr != "")
            queryStr = queryStr.substring(0, queryStr.length - 1);
        return queryStr;
    },
    getModules: function (callback) {
        console.log("token=" + this.token);
        console.log("modulesApiUrl=" + this.modulesApiUrl);
        try {
            $.ajax({
                type: "get",
                url: this.modulesApiUrl,
                async: false,
                jsonp: "",
                //data: queryPageParam,
                headers: {
                    "Access-Control-Allow-Origin": "*",
                    "Access-Control-Allow-Headers": "Authorization",
                    "Authorization": 'Bearer ' + this.token
                },
                success: function (response) {
                    console.log("getModules");
                    console.log(response);
                    response.data = response;
                    //console.log(response);
                    callback(response);
                }
            });
        } catch (e) {
            //console.log(e);
        }
    },
    addData: function (apiData, callback) {
        //console.log("添加数据");
        //console.log(apiData);
        var vurl = this.urlBase + "/map-of-industrys";
        $.ajax({
            type: "POST",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            data: JSON.stringify(apiData),
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("添加失败" + response.data.msg);
                else
                    alert("添加成功");
                callback(response);
            }
        });
    },
    queryPage: function (queryPageParam, callback) {
        var vurl = this.urlBase + "/map-of-industrys/page?" + this.getQueryString(queryPageParam);
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            data: queryPageParam,
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetOSCRegionByLandId: function (landId, callback) {
        //var vurl = this.urlBase + "/map-of-industrys/vw-osc-region-redis";
        var vurl = this.urlBase + "/map-of-industrys/vw-osc-region";
        $.ajax({
            type: "get",
            url: vurl,
            data: {
                landId: landId
            },
            async: true,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log("GetOSCRegionByLandId");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    ajaxCallBack: function () { },
    query: function (queryParam, callback) {
        var vurl = this.urlBase + "/map-of-industrys?" + this.getQueryString(queryParam);
        $.getJSON(vurl, function (resp) {
            //console.log(resp);
            result = resp;
            if (result.status != 0)
                alert("查询出错，原因[" + result.msg + "]");
            callback(resp);
        });

    },
    QueryOscRegion: function (landId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/osc-region";
        $.ajax({
            type: "get",
            url: vurl,
            async: true,
            jsonp: "",
            data: {
                landId: landId
            },
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    queryGroup: function (landId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/group";
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            data: {
                landId: landId
            },
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetStatisticsInfo: function (LandId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/statistics?LandId=" + LandId;
        $.getJSON(vurl, function (resp) {
            //console.log(resp);
            result = resp;
            if (result.status != 0)
                alert("查询出错，原因[" + result.msg + "]");
            callback(resp);
        });
    },
    GetOutputValueStatistics: function (YearRange, LandId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/statistics/outputvalue?YearRange=" + YearRange + "&LandId=" + LandId;
        $.getJSON(vurl, function (resp) {
            //console.log(resp);
            result = resp;
            if (result.status != 0)
                alert("查询出错，原因[" + result.msg + "]");
            callback(resp);
        });
    },
    GetPlantingAreaStatistics: function (RootLandId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/statistics/plantingarea?RootLandId=" + RootLandId;
        $.getJSON(vurl, function (resp) {
            //console.log(resp);
            result = resp;
            if (result.status != 0)
                alert("查询出错，原因[" + result.msg + "]");
            callback(resp);
        });
    },
    GetYearCountData: function (regionId, cropsName, callback) {
        //var vurl = this.urlBase + "/map-of-industrys/count-data?regionId=" + regionId + "&cropsName=" + cropsName;
        var vurl = this.urlBase + "/map-of-industrys/count-data";
        $.ajax({
            type: "get",
            url: vurl,
            data: {
                regionId: regionId,
                cropsName: cropsName
            },
            async: true,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log("GetYearCountData");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetPlantAreaStatistics: function (regionId, cropsName, callback) {
        //var vurl = this.urlBase + "/map-of-industrys/plant-area?regionId=" + regionId + "&cropsName=" + cropsName;
        var vurl = this.urlBase + "/map-of-industrys/plant-area";
        //$.getJSON(vurl, function (resp) {
        //    //console.log(resp);
        //    result = resp;
        //    if (result.status != 0)
        //        alert("查询出错，原因[" + result.msg + "]");
        //    callback(resp);
        //});
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            data: {
                regionId: regionId,
                cropsName: cropsName
            },
            success: function (response) {
                //console.log("GetPlantAreaStatistics");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetAveragePriceStatistics: function (landId, callback) {
        //var vurl = this.urlBase + "/map-of-industrys/average-price?LandId=" + landId;
        var vurl = this.urlBase + "/map-of-industrys/average-price";
        //$.getJSON(vurl, function (resp) {
        //    //console.log(resp);
        //    result = resp;
        //    if (result.status != 0)
        //        alert("查询出错，原因[" + result.msg + "]");
        //    callback(resp);
        //});
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            data: {
                LandId: landId
            },
            success: function (response) {
                //console.log("GetAveragePriceStatistics");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetPlantingAreaStatisticsGroupCrops: function (regionId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/statistics/plantingArea/GroupCrops";
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            data: {
                regionId: regionId
            },
            success: function (response) {
                //console.log("GetPlantingAreaStatisticsGroupCrops");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    QueryGrowCycles: function (cropsName, callback) {
        var vurl = this.urlBaseFram + "/grow-cycles";
        var where = '';
        switch (cropsName) {
            case "玉米":
                where = 'type=0';
                break;
            case "大豆":
                where = 'type=1';
                break;
            case "谷子":
                where = 'type=2';
                break;
        }
        //console.log(where);
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization",
                "Authorization": 'Bearer ' + this.token
            },
            data: {
                where: where
            },
            success: function (response) {
                //console.log("QueryGrowCycles");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    //获得农作物候期农作物数据
    QueryGrowCyclesCropsNew: function (cropsName, callback)
    {
        var vurl = this.urlBaseFram + "/crops";
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization",
                "Authorization": 'Bearer ' + this.token
            },
            data: {
                key: cropsName
            },
            success: function (response) {
                //console.log("QueryGrowCyclesCropsNew");
                //console.log(response);
                response.data = response;
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    delete: function (dataId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/" + dataId;
        $.ajax({
            type: "DELETE",
            url: vurl,
            async: false,
            jsonp: "",
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("删除失败" + response.data.msg);
                else
                    alert("删除成功");
                callback(response);
            }
        });
    },
    edit: function (dataId, dataParam, callback) {
        var vurl = this.urlBase + "/map-of-industrys/" + dataId;
        $.ajax({
            type: "PUT",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            data: JSON.stringify(dataParam),
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("修改失败" + response.data.msg);
                else
                    alert("修改成功");
                callback(response);
            }
        });
    },
    GetLandCropsStatisticsAsync: function (landId, callback) {
        var vurl = this.urlBase + "/map-of-industrys/land/crops/statistics";
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            data: {
                landId: landId
            },
            success: function (response) {
                //console.log("GetLandCropsStatisticsAsync");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    SearchOSCRegion: function (SearchKey, callback) {
        var vurl = this.urlBase + "/map-of-industrys/search/osc-region";
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            data: {
                SearchKey: SearchKey
            },
            success: function (response) {
                //console.log("SearchOSCRegion");
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    CompanyInfoPage(PageParm, callback) {
        var vurl = this.urlBase + "/company-info/page?" + this.getQueryString(PageParm);
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            data: PageParm,
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    //读取指定公司信息的代码
    getCompanyInfoByRecordId: function (companyInfoRecordId, callback) {
        var vurl = this.urlBase + "/company-info/ByRecordId/" + companyInfoRecordId;
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    getCompanyInfoByLandId: function (landId, callback) {
        var vurl = this.urlBase + "/company-info/ByLandId/" + landId;
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    getCompanyInfoByCompanyType: function (CompanyType, landId, callback) {
        var vurl = this.urlBase + "/company-info/ByCompanyType/" + CompanyType + "/" + landId;
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    companyInfoUnionQuery: function (
        landId,
        UnionQuery,
        callback) {
        var vurl = this.urlBase + "/company-info/UnionQuery/" + landId;
        var queryString = "";
        if (UnionQuery != null && typeof (UnionQuery) != "undefined") {
            if (UnionQuery.CompanyType != null && UnionQuery.CompanyType != "" && typeof (UnionQuery.CompanyType) != "undefined")
                queryString += "CompanyType=" + UnionQuery.CompanyType + "&";
            else
                queryString += "CompanyType=null&";
            if (UnionQuery.EnterpriseNature != null && UnionQuery.EnterpriseNature != "" && typeof (UnionQuery.EnterpriseNature) != "undefined")
                queryString += "EnterpriseNature=" + UnionQuery.EnterpriseNature + "&";
            else
                queryString += "EnterpriseNature=null&";
            if (UnionQuery.RegionalLevel != null && UnionQuery.RegionalLevel != "" && typeof (UnionQuery.RegionalLevel) != "undefined")
                queryString += "RegionalLevel=" + UnionQuery.RegionalLevel + "&";
            else
                queryString += "RegionalLevel=null&";
        }
        if (queryString != "") {
            queryString = queryString.substring(0, queryString.length - 1);
            vurl += "?" + queryString + "&sortField=CreatedTime&sortMethod=DESC";
        }
        //console.log(vurl);
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    companyInfoDelete: function (dataId, callback) {
        var vurl = this.urlBase + "/company-info/" + dataId;
        $.ajax({
            type: "DELETE",
            url: vurl,
            async: false,
            jsonp: "",
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("删除失败" + response.data.msg);
                else
                    alert("删除成功");
                callback(response);
            }
        });
    },
    addCompanyInfo: function (addData, callback) {
        var vurl = this.urlBase + "/company-info";
        $.ajax({
            type: "POST",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            data: JSON.stringify(addData),
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("添加失败" + response.data.msg);
                else
                    alert("添加成功");
                callback(response);
            }
        });
    },
    editCompanyInfo: function (recordId, editData, callback) {
        var vurl = this.urlBase + "/company-info/" + recordId;
        //console.log("editCompanyInfo");
        //console.log(vurl);
        //console.log(JSON.stringify(editData));
        $.ajax({
            type: "PUT",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            data: JSON.stringify(editData),
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("修改失败" + response.data.msg);
                else
                    alert("修改成功");
                callback(response);
            }
        });
    },
    OscRegionAutoComplate: function (landId,keyword, resultCount, sortField, sortMethod, callback) {
        var vurl = this.urlBase + "/osc-region/auto-complate"
        var queryString = "";
        if (landId != "")
            queryString += "landId=" + landId + "&";
        if (keyword != "")
            queryString += "keyword=" + keyword + "&";
        else
            queryString += "keyword=null&";
        if (resultCount != "")
            queryString += "resultCount=" + resultCount + "&";
        else
            queryString += "resultCount=10&";
        if (sortField != "")
            queryString += "sortField=" + sortField + "&";
        else
            queryString += "sortField=id&";
        if (sortMethod != "")
            queryString += "sortMethod=" + sortMethod + "&";
        else
            queryString += "sortMethod=desc&";
        if (queryString != "") {
            queryString = queryString.substring(0, queryString.length - 1);
            vurl += "?" + queryString;
        }
        //console.log("OscRegionAutoComplate");
        //console.log(vurl);
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetNewMapVerison: function (callback)
    {
        var vurl = this.urlBase + "/map-local-verison";
        //console.log("GetNewMapVerison");
        //console.log(vurl);
        $.ajax({
            type: "GET",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    importExcel: function (userName,uploadFileControlId, callback) {
        var vurl = this.urlBase + "/company-info/importExcel";
        //console.log(vurl);
        var fileUpload = $("#" + uploadFileControlId).get(0);
        var file = fileUpload.files[0];
        var data = new FormData();
        data.append(file.name, file);
        data.append("userName", userName);
        $.ajax({
            type: "POST",
            url: vurl,
            //headers: {
            //    token: window.localStorage.getItem("token")
            //},
            contentType: false,
            processData: false,
            data: data,
            success: function (response) {
                //console.log(response);
                response.data = response;
                callback(response);
                //if (response.data.status != 0)
                //    alert("导入失败" + response.data.msg);
                //else
                //    callback(response);
                //$uibModalInstance.close(e);
            },
            error: function () {
                //utils.showError("上传失败");
            },
            complete: function () {
                // utils.hideMask();
            }
        });
    },
    OscRegionPage: function (Param, callback)
    {
        var vurl = this.urlBase + "/osc-region/page";
        vurl += "?" + this.getQueryString(Param);
        console.log(vurl);
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                response.data = response;
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetAllRegionLevel: function (callback)
    {
        var vurl = this.urlBase + "/osc-region/all/region/level";
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                response.data = response;
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    importOscRegionExcel: function (userName, uploadFileControlId, callback) {
        var vurl = this.urlBase + "/osc-region/importExcel";
        //console.log(vurl);
        var fileUpload = $("#" + uploadFileControlId).get(0);
        var file = fileUpload.files[0];
        var data = new FormData();
        data.append(file.name, file);
        data.append("userName", userName);
        $.ajax({
            type: "POST",
            url: vurl,
            //headers: {
            //    token: window.localStorage.getItem("token")
            //},
            contentType: false,
            processData: false,
            data: data,
            success: function (response) {
                //console.log(response);
                response.data = response;
                callback(response);
                //if (response.data.status != 0)
                //    alert("导入失败" + response.data.msg);
                //else
                //    callback(response);
                //$uibModalInstance.close(e);
            },
            error: function () {
                //utils.showError("上传失败");
            },
            complete: function () {
                // utils.hideMask();
            }
        });
    },
    GetRegionGISByLandId: function (landId, callback) {
        var vurl = this.urlBase + "/osc-region/region/gis?landId=" + landId;
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                response.data = response;
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    saveOscRegion: function (saveData, UserName, saveMethod, callback) {
        var vurl = this.urlBase + "/osc-region";
        var method = "POST";
        var id = saveData.id;
        console.log(vurl);
        if (saveMethod == "Edit") {
            method = "PUT";
            vurl += "/" + id;
        }
        vurl += "?UserName=" + UserName
        console.log(vurl);
        console.log(saveData);
        console.log("saveMethod=" + saveMethod);
        console.log("method=" + method);
        $.ajax({
            type: method,
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            data: JSON.stringify(saveData),
            success: function (response) {
                console.log(response);
                response.data = response;
                console.log(response);
                callback(response);
            }
        });
    },
    saveRegionGis: function (saveData, callback)
    {
        var vurl = this.urlBase + "/osc-region/Save/RegionGis"
        var method = "POST";
        console.log(vurl);
        $.ajax({
            type: method,
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            data: JSON.stringify(saveData),
            success: function (response) {
                console.log(response);
                response.data = response;
                console.log(response);
                callback(response);
            }
        });
    },
    OscRegionDelete: function (landId, callback) {
        var vurl = this.urlBase + "/osc-region/" + landId;
        $.ajax({
            type: "DELETE",
            url: vurl,
            async: false,
            jsonp: "",
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("删除失败" + response.data.msg);
                else
                    alert("删除成功");
                callback(response);
            }
        });
    },
    SetRootLand: function (landId, callback)
    {
        var vurl = this.urlBase + "/osc-region/set-rootland/" + landId;
        $.ajax({
            type: "PUT",
            url: vurl,
            async: false,
            jsonp: "",
            contentType: "application/json",//设置请求参数类型为json字符串
            dataType: "json",
            success: function (response) {
                console.log(response);
                response.data = response;
                console.log(response);
                callback(response);
            }
        });
    },
    GetRootLand: function (callback)
    {
        var vurl = this.urlBase + "/osc-region/root/land";
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                //console.log(response);
                response.data = response;
                //console.log(response);
                if (response.data.status != 0)
                    alert("调用读取根地块儿接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    },
    GetAllRegionGisByRootLand: function (rootLandId, callback)
    {
        var vurl = this.urlBase + "/osc-region/all/regionGis/by/root/landId/" + rootLandId;
        $.ajax({
            type: "get",
            url: vurl,
            async: false,
            jsonp: "",
            headers: {
                "Access-Control-Allow-Origin": "*",
                "Access-Control-Allow-Headers": "Authorization"
            },
            success: function (response) {
                response.data = response;
                if (response.data.status != 0)
                    alert("调用接口出错,原因:\t\r\n" + response.data.msg);
                else
                    callback(response);
            }
        });
    }
};