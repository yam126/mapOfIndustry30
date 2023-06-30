window._AMapSecurityConfig = {
    securityJsCode: 'acca991ccaf8f2bb97df91a7bf547a24', //高德Api密钥
};
var map;
var amap;
var maker_event;
var markPlant = [];
var equipMarkers = [];
var rootDistrictName = "晋中市";
var districtNameDatas = new Array();//下级行政区列表
var polygons = null;//高德地块儿点位
var district = null;
var markers = [];
var markerIcon = ["http://192.168.3.82:5000/images/icon/corn_small.png", "http://192.168.3.82:5000/images/icon/soybean_small.png", "http://192.168.3.82:5000/images/icon/wheat_small.png"];
var crops = ["玉米", "大豆", "小麦"];
var GlobalBoundaries = [];
function CreateAMap(htmlElementId, districtName, polygon_event) {
    AMapLoader.load({
        "key": "af03b4b1406996840b39d11d81850677",              // 申请好的Web端开发者Key，首次调用 load 时必填
        "version": "2.0",   // 指定要加载的 JSAPI 的版本，缺省时默认为 1.4.15
        "plugins": [
            "AMap.Autocomplete",
            "AMap.PlaceSearch",
            "AMap.Scale",
            "AMap.OverView",
            "AMap.ToolBar",
            "AMap.MapType",
            "AMap.PolyEditor",
            "AMap.CircleEditor",
            "AMap.DistrictSearch",
            "AMap.Geocoder"
        ],           // 需要使用的的插件列表，如比例尺'AMap.Scale'等
        "AMapUI": {             // 是否加载 AMapUI，缺省不加载
            "version": '1.1',   // AMapUI 版本
            "plugins": ['overlay/SimpleMarker'],       // 需要加载的 AMapUI ui插件
        },
        "Loca": {                // 是否加载 Loca， 缺省不加载
            "version": '2.0'  // Loca 版本
        },
    }).then((AMap) => {
        map = new AMap.Map(htmlElementId);
        var opts = {
            level: "biz_area",
            extensions: "all",
            showbiz: true,
            subdistrict: 4
        };
        district = new AMap.DistrictSearch(opts);
        map.setMapStyle("amap://styles/blue");
        getChildDistrictRegion(polygon_event);
        //maker_event = maker_event;
    }).catch((e) => {
        console.error(e);  //加载错误提示
    }); 0
}
function CreateAMapByApiData(htmlElementId, regionsData, cropsImage, polygon_event, marker_event, callback) {
    var result = null;
    AMapLoader.load({
        "key": "af03b4b1406996840b39d11d81850677",              // 申请好的Web端开发者Key，首次调用 load 时必填
        "version": "2.0",   // 指定要加载的 JSAPI 的版本，缺省时默认为 1.4.15
        "plugins": [
            "AMap.Autocomplete",
            "AMap.PlaceSearch",
            "AMap.Scale",
            "AMap.OverView",
            "AMap.ToolBar",
            "AMap.MapType",
            "AMap.PolyEditor",
            "AMap.CircleEditor",
            "AMap.DistrictSearch",
            "AMap.Geocoder"
        ],           // 需要使用的的插件列表，如比例尺'AMap.Scale'等
        "AMapUI": {             // 是否加载 AMapUI，缺省不加载
            "version": '1.1',   // AMapUI 版本
            "plugins": ['overlay/SimpleMarker'],       // 需要加载的 AMapUI ui插件
        },
        "Loca": {                // 是否加载 Loca， 缺省不加载
            "version": '2.0'  // Loca 版本
        },
    }).then((AMap) => {
        map = new AMap.Map(htmlElementId);
        var opts = {
            level: "biz_area",
            extensions: "all",
            showbiz: true,
            center: [113.17666, 37.89498],
            subdistrict: 4
        };
        district = new AMap.DistrictSearch(opts);
        map.setMapStyle("amap://styles/blue");
        getChildDistrictRegionByApiData(cropsImage, polygon_event);
        maker_event = marker_event;
        callback(AMap);
        result = AMap;
        return result;
    }).catch((e) => {
        console.error(e);  //加载错误提示
    });
    return result;
}
function initAllChildDistrict() {
    AMapLoader.load({
        "key": "af03b4b1406996840b39d11d81850677",              // 申请好的Web端开发者Key，首次调用 load 时必填
        "version": "2.0",   // 指定要加载的 JSAPI 的版本，缺省时默认为 1.4.15
        "plugins": [
            "AMap.Autocomplete",
            "AMap.PlaceSearch",
            "AMap.Scale",
            "AMap.OverView",
            "AMap.ToolBar",
            "AMap.MapType",
            "AMap.PolyEditor",
            "AMap.CircleEditor",
            "AMap.DistrictSearch",
            "AMap.Geocoder"
        ],           // 需要使用的的插件列表，如比例尺'AMap.Scale'等
        "AMapUI": {             // 是否加载 AMapUI，缺省不加载
            "version": '1.1',   // AMapUI 版本
            "plugins": ['overlay/SimpleMarker'],       // 需要加载的 AMapUI ui插件
        },
        "Loca": {                // 是否加载 Loca， 缺省不加载
            "version": '2.0'  // Loca 版本
        },
    }).then((AMap) => {
        map = new AMap.Map(htmlElementId);
        var opts = {
            level: "biz_area",
            extensions: "all",
            showbiz: true,
            subdistrict: 4
        };
        district = new AMap.DistrictSearch(opts);
        map.setMapStyle("amap://styles/blue");
        amap = AMap;
        getChildDistrictAll(rootDistrictName, AMap);
    }).catch((e) => {
        console.error(e);  //加载错误提示
    });
}
function getGlobalRegions(DistrictId) {
    var result = null;
    for (var i = 0; i < globalAllRegions.length; i++) {
        if (globalAllRegions[i].id == DistrictId)
            result = globalAllRegions[i];
    }
    return result;
}
var tempRegionItem = null;
function drawPlantArea(cropsImage, plandAreaData, AMap) {
    var countArea = plandAreaData.countArea;
    var arrayData = plandAreaData.regionAreas;
    var cropsName = '';
    if (markPlant != null) {
        map.remove(markPlant);
        markPlant = [];
    }
    if (equipMarkers != null) {
        map.remove(equipMarkers);
        equipMarkers = [];
    }
    //markPlant = [];
    //var plantingArea = "面积";
    for (var i = 0; i < arrayData.length; i++) {
        if (arrayData[i] != null)
            countArea += arrayData[i].sowArea;
    }
    console.log("drawPlantArea");
    console.log("plandAreaData");
    console.log(plandAreaData);
    console.log("arrayData");
    console.log(arrayData);
    for (var i = 0; i < arrayData.length; i++) {
        var regionData = getDistrictName(arrayData[i].id);
        var regionDataItem = getGlobalRegions(arrayData[i].id);
        console.log("regionDataItem");
        console.log(regionDataItem);
        if (regionData != null && regionDataItem.gpsLocations!=null) {
            var center = GetCenterPointFromListOfCoordinates(regionDataItem.gpsLocations);
            console.log("center");
            console.log(center);
            regionData.center = [];
            regionData.center.push(center.lng);
            regionData.center.push(center.lat);
            if (regionData.center != null && regionData.center != undefined) {
                var circleRadius = Math.round((parseFloat(arrayData[i].sowArea) / parseFloat(countArea)) * 10000) / 10.0
                circleRadius = circleRadius.toFixed(2);
                circleRadius = circleRadius * 100;
                //circleRadius += Math.floor((Math.random() * 100) + 1);
                //circleRadius = parseFloat(circleRadius) + Math.floor((Math.random() * 100) + 1)+50;
                console.log("circleRadius=" + circleRadius);
                /*var circle = new AMap.CircleMarker({
                    map: map,
                    center: new AMap.LngLat(regionData.center[0], regionData.center[1]),
                    fillOpacity: 1,
                    fillColor: '#00D936',
                    strokeOpacity: 0,
                    radius: circleRadius,
                    zIndex: 20
                });*/
                //switch (cropsImage) {
                //    case '/images/newhome/crops-01.png':
                //        cropsName = '玉米' + plantingArea + arrayData[i].sowArea;
                //        currentPlantingArea = arrayData[i].sowArea;
                //        break;
                //    case '/images/newhome/crops-02.png':
                //        cropsName = '谷子' + plantingArea + arrayData[i].sowArea;
                //        currentPlantingArea = arrayData[i].sowArea;
                //        break;
                //    case '/images/newhome/crops-03.png':
                //        cropsName = '大豆' + plantingArea + arrayData[i].sowArea;
                //        currentPlantingArea = arrayData[i].sowArea;
                //        break;
                //}
                //在地图上显示面积图标和面积数字
                cropsName = '面积' + arrayData[i].sowArea;
                var left = arrayData.length == 1 ? -95 : - 65;
                var top = -3;
                var marker = new AMap.Marker({
                    map: map,
                    position: new AMap.LngLat(regionData.center[0], regionData.center[1]),
                    content: "<div class='AMapLabel' title='" + cropsName + "' ><img src='" + cropsImage + "' style='margin-left:3px;width:30px;height:30px;float:left'>" + cropsName + "</div>",
                    zIndex: 200 + i,
                    offset: new AMap.Pixel(left, top)
                });
                marker.setPosition(regionData.center);
                //在地图上显示面积图标和面积数字
                markPlant.push(marker);
            }
        }
    }
}

//地图上显示企业或者合作社
function drawMakers(AMap, statu, markers) {
    //var markers = [{ name: '企业1', lng: '113.177708', lat: '37.891136' }, { name: '企业2', lng: '113.176870', lat: '37.888630' }, { name: '企业3', lng: '113.068910', lat: '37.909970' }, { name: '企业4', lng: '112.911620', lat: '37.945300' }]
    if (markPlant != null) {
        map.remove(markPlant);
        markPlant = [];
    }
    if (equipMarkers != null) {
        map.remove(equipMarkers);
        equipMarkers = [];
    }
    console.log(markers)
    if (markers != null && markers != "") {
        markers.forEach(function (marker) {
            var localIcon = new AMap.Icon({
                size: new AMap.Size(45, 55),
                image: statu == 3 ? '../images/newhome/qiye.png' : '../images/newhome/hezuoshe.png',
                imageSize: new AMap.Size(45, 55),
            });
            console.log(localIcon)
            var equip = new AMap.Marker({
                map: map,
                icon: localIcon,
                size: new AMap.Size(45, 55),
                imageSize: new AMap.Size(45, 55),
                position: [marker.lng, marker.lat],
                offset: new AMap.Pixel(-13, -30),
                extData: {
                    companyName: marker.companyName,
                    currentYearSales: marker.currentYearSales,
                    enterpriseType: marker.enterpriseType,
                    regionalLevel: marker.regionalLevel,
                    townShip: marker.townShip,
                    contacts: marker.contacts,
                    contactPhone: marker.contactPhone,
                    enterpriseIntroduction: marker.enterpriseIntroduction,
                    focusImages: marker.focusImages,
                    videoUrl: marker.videoUrl
                }
            });
            console.log(equip)
            equipMarkers.push(equip)
            map.add(equip)
            equip.on('click', markerPop);
        })
        map.setFitView()
    }

}

function markerPop(e) {
    console.log(e)
    var name = e.target._opts.extData.name;
    maker_event(e.target._opts.extData)
}

function getDistrictName(landId) {
    var result = null;
    for (var i = 0; i < districtNameDatas.length; i++) {
        if (districtNameDatas[i].id == landId) {
            result = districtNameDatas[i];
            return result;
        }
    }
    return result;
}
//地图划分行政区
function mapDivideDistrictApiData(cropsImage, polygon_event) {
    if (polygons != null)
        map.remove(polygons);
    GlobalBoundaries = [];
    for (var i = 0; i < districtNameDatas.length; i++) {
        tempRegionItem = districtNameDatas[i];
        console.log(i);
        console.log(tempRegionItem.regionName);
        polygons = [];
        console.log("mapDivideDistrictApiData");
        console.log(tempRegionItem);
        //for (var j = 0; j < districtNameDatas.length; j++) {
        //    if (result.districtList[0].name == districtNameDatas[j].name)
        //        tempRegionItem = districtNameDatas[j];
        //}
        console.log(tempRegionItem);
        if (tempRegionItem.gpsLocations == "")
            continue;
        //#region 生成GPS坐标数组
        //var boundsAry = tempRegionItem.gpsLocations.split(',');
        //var bounds = [];
        //var boundsItem = [];       
        //for (var g = 0; g < boundsAry.length; g++) {
        //    if (boundsItem.length<2) {
        //        boundsItem.push(boundsAry[g]);
        //    } else {
        //        bounds.push(boundsItem);
        //        boundsItem = [];
        //        boundsItem.push(boundsAry[g]);
        //    }
        //}
        //#endregion
        var cropsName = '';
        var cropsList = tempRegionItem.statisticalInfo.cropsList;
        var plantingArea = "种植面积";
        var currentPlantingArea = 0;
        switch (cropsImage) {
            case '/images/newhome/crops-01.png':
                cropsName = '玉米' + plantingArea + cropsList[0].plantingArea;
                currentPlantingArea = cropsList[0].plantingArea;
                break;
            case '/images/newhome/crops-02.png':
                cropsName = '谷子' + plantingArea + cropsList[1].plantingArea;
                currentPlantingArea = cropsList[1].plantingArea;
                break;
            case '/images/newhome/crops-03.png':
                cropsName = '大豆' + plantingArea + cropsList[2].plantingArea;
                currentPlantingArea = cropsList[2].plantingArea;
                break;
        }
        var totalplantingArea = cropsList[0].plantingArea + cropsList[1].plantingArea + cropsList[2].plantingArea;
        //#region 标点标记
        if (tempRegionItem.center != null) {
            //var circleRadius = Math.round(totalplantingArea / currentPlantingArea, 2) * 100;
            //var circle = new AMap.CircleMarker({
            //    map: map,
            //    center: new AMap.LngLat(tempRegionItem.center[0], tempRegionItem.center[1]),
            //    fillOpacity: 0.4,
            //    fillColor: '#DDEB92',
            //    strokeOpacity: 0,
            //    radius: circleRadius,
            //    zIndex: 20
            //});
            /*var marker = new AMap.Marker({
                map: map,
                position: new AMap.LngLat(tempRegionItem.center[0], tempRegionItem.center[1]),
                content: "<div class='AMapLabel'><img src='" + cropsImage + "' style='margin-left:3px;width:30px;height:30px;float:left'>" + cropsName+"</div>",
                zIndex: 200,
                offset: new AMap.Pixel(-(circleRadius/2)+15, 0)
            });*/
        }
        //#endregion
        //#region 如果是数组形式
        var bounds = JSON.parse(tempRegionItem.gpsLocations);
        //#endregion
        if (bounds) {
            //#region 生成行政区划polygon
            //tempRegionItem.gpsLocations = "";
            var polygon = new AMap.Polygon({
                map: map,
                strokWeight: 1,
                path: bounds,
                fillOpacity: 0.4,
                fillColor: "#BFFFFF",
                strokeColor: "#CC66CC",
                cursor: "pointer",
                bubble: true,
                extData: tempRegionItem
            });
            AMap.Event.addListener(polygon, 'mousedown', function (e) {
                e.target._opts.fillColor = "#FF4D4D";
                e.target.setOptions(e.target._opts);
                console.log("e.target._opts.extData");
                console.log(e.target._opts.extData);
                polygon_event("mousedown", e.target._opts.extData);
            });
            //polygon.on('mousedown', function (e) {
            //    e.target._opts.fillColor = "#FF4D4D";
            //    e.target.setOptions(e.target._opts);
            //    console.log("e.target._opts.extData");
            //    console.log(e.target._opts.extData);
            //    polygon_event("mousedown", e.target._opts.extData);
            //});
            AMap.Event.addListener(polygon, 'click', function (e) {
                e.target._opts.fillColor = "#FF4D4D";
                e.target.setOptions(e.target._opts);
                console.log("e.target._opts.extData");
                console.log(e.target._opts.extData);
                polygon_event("click", e.target._opts.extData);
            });
            //polygon.on('click', function (e) {
            //    e.target._opts.fillColor = "#FF4D4D";
            //    e.target.setOptions(e.target._opts);
            //    console.log("e.target._opts.extData");
            //    console.log(e.target._opts.extData);
            //    polygon_event("click", e.target._opts.extData);
            //});
            AMap.Event.addListener(polygon, 'mouseover', function (e) {
                e.target._opts.fillColor = "#FF4D4D";
                e.target.setOptions(e.target._opts);
                console.log("e.target._opts.extData");
                console.log(e.target._opts.extData);
                polygon_event("mouseover", e.target._opts.extData);
            });
            //polygon.on("mouseover", function (e) {
            //    e.target._opts.fillColor = "#FF4D4D";
            //    e.target.setOptions(e.target._opts);
            //    console.log("e.target._opts.extData");
            //    console.log(e.target._opts.extData);
            //    polygon_event("mouseover", e.target._opts.extData);
            //});
            AMap.Event.addListener(polygon, 'mouseout', function (e) {
                e.target._opts.fillColor = "#BFFFFF";
                e.target.setOptions(e.target._opts);
                polygon_event("mouseout", e.target._opts.extData);
            });
            polygon.on("mouseout", function (e) {
                //e.target._opts.fillColor = "#CCF3FF";
                //e.target.setOptions(e.target._opts);
                //polygon_event("mouseout", e.target._opts.extData);
            });
            GlobalBoundaries.push(bounds);
            //#endregion 
            polygons.push(polygon);
            map.setFitView();
        }
    }

}
function getChildDistrictAll(districtName, AMap) {
    districtNameDatas = [];
    district.search(districtName, function (status, result) {
        if (status == 'complete') {
            if (districtNameDatas == null || districtNameDatas.length <= 0) {
                for (var i = 0; i < result.districtList[0].districtList[0].districtList.length; i++) {
                    var item = result.districtList[0].districtList[0].districtList[i];
                    districtNameDatas.push(item.name);
                }
            }
        }
    });
}

//随机标记地图
function mapRandomSign(typeInfo) {
    map.clearMap();


    //var labelMarker = new AMap.LabelMarker({
    //    name: "京味斋烤鸭店",
    //    position: [115.898359, 39.909869],
    //    icon: {
    //        type: "image",
    //        image: "https://a.amap.com/jsapi_demos/static/images/poi-marker.png",
    //        clipOrigin: [547, 92],
    //        clipSize: [50, 68],
    //        size: [25, 34],
    //        anchor: "bottom-center",
    //        angel: 0,
    //        retina: true,
    //    },
    //    text: {
    //        content: "京味斋烤鸭店",
    //        direction: "top",
    //        offset: [0, 0],
    //        style: {
    //            fontSize: 13,
    //            fontWeight: "normal",
    //            fillColor: "#fff",
    //            padding: "2, 5",
    //            backgroundColor: "#22884f",
    //        },
    //    },
    //});

    //layer.add(labelMarker);
    for (var i = 0; i < GlobalBoundaries.length; i++) {
        var cropNameIndex = getRandom(0, crops.length - 1);
        var cropName = crops[cropNameIndex];
        var iconPath = "";
        var tipsText = "";
        switch (cropName) {
            case "玉米":
                iconPath = markerIcon[0];
                break;
            case "大豆":
                iconPath = markerIcon[1];
                break;
            case "小麦":
                iconPath = markerIcon[2];
                break;
        }
        console.log(iconPath);
        switch (typeInfo) {
            case "种植面积":
                tipsText = cropName + typeInfo + "亩";
                break;
            default:
                tipsText = typeInfo;
                break;
        }
        var layer = new AMap.LabelsLayer({
            zooms: [3, 20],
            zIndex: 1000 + i,
            // 开启标注避让，默认为开启，v1.4.15 新增属性
            collision: true,
            // 开启标注淡入动画，默认为开启，v1.4.15 新增属性
            animation: true,
        });
        map.add(layer);
        var labelMarker = new AMap.LabelMarker({
            name: tipsText,
            position: [GlobalBoundaries[i][0].lng, GlobalBoundaries[i][0].lat],
            icon: {
                type: "image",
                image: iconPath,
                clipOrigin: [547, 92],
                clipSize: [50, 68],
                size: [25, 34],
                anchor: "bottom-center",
                angel: 0,
                retina: true,
            },
            text: {
                content: tipsText + getRandom(1, 100),
                direction: "top",
                offset: [0, 0],
                style: {
                    fontSize: 13,
                    fontWeight: "normal",
                    fillColor: "#fff",
                    padding: "2, 5",
                    backgroundColor: "#22884f",
                },
            },
        });
        markers.push(labelMarker);
    }
    layer.add(markers);
    map.setFitView();
}

function getChildDistrictRegionById(regionId) {
    var result = null;
    for (var i = 0; i < globalAllRegions.length; i++) {
        if (globalAllRegions[i].id == regionId)
            result = globalAllRegions[i];
    }
    return result;
}

function getChildDistrictRegionByApiData(cropsImage, polygon_event) {
    districtNameDatas = [];
    for (var i = 0; i < globalAllRegions.length; i++) {
        if (globalAllRegions[i].parent_id == rootDistrictId)
            districtNameDatas.push(globalAllRegions[i]);
    }
    if (districtNameDatas == null || districtNameDatas.length <= 0)
        districtNameDatas.push(getChildDistrictRegionById(rootDistrictId));
    console.log("getChildDistrictRegionByApiData");
    console.log("districtNameDatas");
    console.log(districtNameDatas);
    mapDivideDistrictApiData(cropsImage, polygon_event);
}

function getChildDistrictRegion(polygon_event) {
    districtNameDatas = [];
    for (var i = 0; i < globalAllRegions.length; i++) {
        if (globalAllRegions[i].parent_id == rootDistrictId)
            districtNameDatas.push(globalAllRegions[i]);
    }
    if (districtNameDatas == null || districtNameDatas.length <= 0)
        districtNameDatas.push(getChildDistrictRegionById(rootDistrictId));
    console.log("getChildDistrictRegion");
    console.log("districtNameDatas");
    console.log(districtNameDatas);
    mapDivideDistrict(polygon_event);
}
//获得下级行政区
function getChildDistrict(districtName, AMap, polygon_event) {
    districtNameDatas = [];
    district.search(districtName, function (status, result) {
        console.log("districtName=" + districtName);
        if (status == 'complete') {
            if (districtNameDatas == null || districtNameDatas.length <= 0) {
                if (result.districtList[0].level != "district") {
                    for (var i = 0; i < result.districtList[0].districtList[0].districtList.length; i++) {
                        var item = result.districtList[0].districtList[0].districtList[i];
                        districtNameDatas.push(item.name);
                    }
                }
                else {
                    var item = result.districtList[0];
                    districtNameDatas.push(item.name);
                }
                mapDivideDistrict(polygon_event);
            }
        }
    });
}

/// <summary>
/// 根据输入的地点坐标计算中心点
/// </summary>
/// <param name="geoCoordinateList"></param>
/// <returns></returns>
function GetCenterPointFromListOfCoordinates(geoCoordinateList) {
    var array = eval(geoCoordinateList);
    var total = array.length;
    console.log("GetCenterPointFromListOfCoordinates");
    console.log("geoCoordinateList");
    console.log(array);
    var X = 0, Y = 0, Z = 0;
    try {
        for (var i = 0; i < total; i++) {
            var g = array[i];
            var lat, lon, x, y, z;
            //console.log(g);
            //lat = g.lat * Math.PI / 180;
            //lon = g.lng * Math.PI / 180;
            lat = g[1] * Math.PI / 180;
            lon = g[0] * Math.PI / 180;
            x = Math.cos(lat) * Math.cos(lon);
            y = Math.cos(lat) * Math.sin(lon);
            z = Math.sin(lat);
            X += x;
            Y += y;
            Z += z;
        }
    } catch (e) {
        console.log(e);
    }
    X = X / total;
    Y = Y / total;
    Z = Z / total;
    var Lon = Math.atan2(Y, X);
    var Hyp = Math.sqrt(X * X + Y * Y);
    var Lat = Math.atan2(Z, Hyp);
    return { lat: Lat * 180 / Math.PI, lng: Lon * 180 / Math.PI };
}