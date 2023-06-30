/**
 * 旧版地图边界数据加载方法
 * @param {any} regionId
 * @param {any} callback
 */
function loadOSCRegion(regionId, callback)
{
    $.getJSON(
        "/osc_region_new.json",
        function (jsonData) {
            console.log("osc_region_new-jsonData.json");
            console.log(jsonData);
            console.log("osc_region_new.json");
            console.log(regionId);
            var response = {
                data:{
                    status: 0,
                    msg: "",
                    result: []
                }
            };
            for (var i = 0; i < jsonData.result.length; i++)
            {
                var item = jsonData.result[i];

                if (item.id == regionId || item.parent_id == regionId)
                    response.data.result.push(item);
            }
            callback(response);
        });
}

/**
 * 地图数据加载方法(本地缓存)
 * @param {省份编号} regionId
 * @param {本地缓存方法} localStorage
 * @param {api帮助类} apiHelper
 * @param {回调函数} callback
 */
function loadOSCRegionLocalStorage(regionId, mapRootDistrictId,localStorage, apiHelper, callback)
{
    var localVerison = null;   
    //localStorage.remove("MapData");
    //localStorage.remove("MapVerison");
    localVerison = localStorage.get("MapVerison");   
    if (localVerison == null || typeof (localVerison) == "undefined") {
        loadApiMapData(regionId, mapRootDistrictId,apiHelper,localStorage,localVerison,false,callback);
    }
    else {
        loadApiMapData(regionId, mapRootDistrictId,apiHelper,localStorage,localVerison,true,callback);
    }
}

/**
 * 读取判断方法
 * @param {省份编号} regionId
 * @param {api帮助类} apiHelper
 * @param {本地缓存} localStorage
 * @param {本地版本} localVerison
 * @param {是否比较版本} isCompareVerison
 * @param {回调函数} callback
 */
function loadApiMapData(regionId, mapRootDistrictId,apiHelper, localStorage, localVerison, isCompareVerison, callback) {
    var localMapData = null;
    localMapData = localStorage.get("MapData");
    console.log("loadApiMapData");
    console.log("localMapData");
    console.log("regionId=" + regionId);
    console.log(localMapData);
    apiHelper.GetNewMapVerison(
        function (response) {
            console.log("loadApiMapData response");
            console.log(response);
            if (response.data.status == "0") {
                var apiVerison = response.data.result;
                localStorage.remove("MapVerison");
                localStorage.set("MapVerison", apiVerison);
                if (isCompareVerison) {
                    if (localVerison == null || (parseInt(apiVerison.mapLocalVerison) > parseInt(localVerison.mapLocalVerison))) {
                        //从数据库读取数据
                        loadApiDataGetOSCRegionByLandId(regionId, localStorage, callback);
                    }
                    else {
                        var responseMapData = [];
                        if (localMapData != null && localMapData.length > 0) {
                            for (var i = 0; i < localMapData.length; i++) {
                                if (localMapData[i].parent_id == regionId)
                                    responseMapData.push(localMapData[i]);
                            }
                            for (var i = 0; i < localMapData.length; i++) {
                                if (localMapData[i].id == regionId)
                                    responseMapData.push(localMapData[i]);
                            }
                            callback(responseMapData);
                        }
                        else
                        {
                            //从数据库读取数据
                            loadApiDataGetOSCRegionByLandId(regionId, localStorage, callback);
                        }
                        

                    }
                } else {
                    //从数据库读取数据
                    loadApiDataGetOSCRegionByLandId(regionId, localStorage, callback);
                }
            }
        }
    );
}
function loadApiDataGetOSCRegionByLandId(regionId,localStorage,callback)
{
    //从数据库读取数据
    apiHelper.GetOSCRegionByLandId(
        regionId,
        function (response) {
            console.log("loadApiDataGetOSCRegionByLandId");
            console.log(response);
            if (response.data.status == "0") {
                var localMapData = response.data.result;
                localStorage.remove("MapData");
                localStorage.set("MapData", localMapData);
                callback(localMapData);
            }
        }
    );
}