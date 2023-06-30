var mapKey = "2a7c617cd0353e40d0fee73327920948";
//var mapKey = "673c32ba6de72791d6f07a4a75fe710a";
var vElementId = "";
var map;
var zoom = 11;
var mapCenter = [113.17666, 37.89498];
var isDrag = false;
var ctrl = null;
var polygonTool = null;
var allbounds = [];
var allRegions = [];
var polygons = [];
var equipMarkers = [];
var allLabel = [];
//var mapRootDistrictId = "3135";
var mapRootDistrictId = "";
var adminRootLandId = "";
var defaultPolygonColor = "#7ab212";
var defaultPolygonFillColor = "#024db6";
var defaultPolygonMouseOverColor ="#3ddcd5"
var currentPolygonFillColor = "";
var viewPortBounds = [];
var allBounds = [];
function GPSTransfromToWGS84(lng, lat) {
    var result = coordtransform.gcj02towgs84(lng, lat);
    return {
        lng: result[0],
        lat: result[1]
    };
}

/**
 * 地图初始化
 * @param {地图容器ID} elementId
 * @param {回调函数} callback
 */
function InitMapLoader(elementId, regionsData,callback) {
    var T = window.T;
    polygons = [];
    viewPortBounds = [];
    allBounds = [];
    //var imageURL = "http://t0.tianditu.gov.cn/img_w/wmts?" +
    //"SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=img&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles" +
    //"&TILEMATRIX={z}&TILEROW={y}&TILECOL={x}&tk=" + mapKey;
    //创建自定义图层对象
    //lay = new T.TileLayer(imageURL, { minZoom: 1, maxZoom: 18 });
    //var config = { layers: [lay] };
    vElementId = elementId;
    //初始化地图对象
    //map = new T.Map(elementId, config);
    //map = new T.Map(elementId);
    if (map == null)
        map = new T.Map(elementId, { datasourcesControl: true, projection: 'EPSG:4326' });
    //map = new T.Map(elementId);
    map.clearOverLays();
    var centerPointWGS84 = GPSTransfromToWGS84(mapCenter[0], mapCenter[1]);
    var centerPoint = new T.LngLat(centerPointWGS84.lng, centerPointWGS84.lat);
    //设置显示地图的中心点和级别
    map.centerAndZoom(centerPoint, zoom);
    //initPolygonTool();
    //允许鼠标滚轮缩放地图
    //map.enableScrollWheelZoom();
    //允许拖拽
    setMapDrag();
    map.addEventListener("zoomstart", function (e) {
        var currentZoom = map.getZoom();
    });
    map.addEventListener("zoomend", function (e) {
        var currentZoom = map.getZoom();
    });
    if (ctrl == null) {
        ctrl = new T.Control.MapType(); // 初始化地图类型选择控件
        map.addControl(ctrl); //添加地图选择控件
        map.setMapType(window.TMAP_HYBRID_MAP);// 设置地图位地星混合图层
    }
    console.log("InitMapLoader");
    console.log(regionsData);
    allRegions = regionsData;
    //drawLandPlantArea(cropsImage,false);
    $(".tdt-right").css("right", "28vw");
    //console.log(map.isInertia());
    if (callback != null)
        callback(map);
}
function setMapCenter(lng,lat)
{
    var centerPointWGS84 = GPSTransfromToWGS84(lng, lat);
    var centerPoint = new T.LngLat(centerPointWGS84.lng, centerPointWGS84.lat);
    map.centerAndZoom(centerPoint, zoom);
}
function drawLandPolygon(regionid,polygon_event)
{
    allLabel = [];   
    console.log("drawLandPolygon");
    console.log("regionid=" + regionid);
    console.log("mapRootDistrictId=" + mapRootDistrictId);
    //if (regionid != mapRootDistrictId) {
        allBounds = [];
        viewPortBounds = [];
    //}
    if (allRegions != null &&
        typeof (allRegions) != "undefined" &&
        allRegions.length != null &&
        typeof (allRegions.length) != "undefined" &&
        allRegions.length > 0) {
        if (polygons == null || polygons.length <= 0) {
            for (var i = 0; i < allRegions.length; i++) {
                var bounds = [];
                var landItem = allRegions[i];
                var FillColor = '';
                if (regionid != "" && landItem.id != regionid && regionid != mapRootDistrictId)
                    continue;
                if (landItem.color != null && landItem.color != "")
                    FillColor = '#' + landItem.color;
                else
                    FillColor = defaultPolygonFillColor;
                if (landItem.gpsLocations != null && landItem.gpsLocations != "") {
                    var gpsLocations = eval(landItem.gpsLocations);
                    for (var r = 0; r < gpsLocations.length; r++) {
                        var itemLngLatAry = gpsLocations[r];
                        var Lng = itemLngLatAry[0];
                        var Lat = itemLngLatAry[1];
                        //#region wgs84转换
                        //var wgs84 = GPSTransfromToWGS84(Lng, Lat);
                        //bounds.push(new T.LngLat(wgs84.lng, wgs84.lat));
                        //allbounds.push(new T.LngLat(wgs84.lng, wgs84.lat));
                        //#endregion

                        bounds.push(new T.LngLat(Lng, Lat));
                        allBounds.push(itemLngLatAry);
                        viewPortBounds.push(new T.LngLat(Lng, Lat));
                        allbounds.push(new T.LngLat(Lng, Lat));
                    }
                }
                //创建面对象
                var polygon = new T.Polygon(
                    bounds,
                    {
                        //color: "#7ab212",
                        color: defaultPolygonColor,
                        weight: 1,
                        opacity: 0.5,
                        fillColor: FillColor,
                        fillOpacity: 0.5,
                        lineStyle: "solid",
                        extData: landItem
                    }
                );
                polygon.addEventListener(
                    "mouseover",
                    function (e) {
                        //console.log(" polygon mouseover ");
                        var eventPolygon = e.target;
                        //console.log("e.target");
                        //console.log(e.target);
                        console.log(eventPolygon);
                        eventPolygon.setFillColor(defaultPolygonMouseOverColor);
                        eventPolygon.setFillOpacity(0.9);
                        //console.log(e);
                        map.enableDrag();
                        map.enableInertia();
                        polygon_event("mouseover", e.target.options.extData);
                    }
                );
                polygon.addEventListener(
                    "mouseout",
                    function (e) {
                        //console.log(" polygon mouseout ");
                        var eventPolygon = e.target;
                        var sourceFillColor = eventPolygon.options.extData.color;
                        if (sourceFillColor!="")
                            eventPolygon.setFillColor("#"+sourceFillColor);
                        else
                            eventPolygon.setFillColor(defaultPolygonFillColor);
                        eventPolygon.setFillOpacity(0.5);
                        //console.log(e);
                        map.enableDrag();
                        map.enableInertia();
                    }
                );
                polygons.push(polygon);
                //向地图上添加面
                map.addOverLay(polygon);
                
            }
        }
    }
    console.log("viewPortBounds");
    console.log(viewPortBounds);
    if (viewPortBounds != null && viewPortBounds.length > 0) {
        var vMapCenter = null;
        var centerPoint = null;
        if (regionid != mapRootDistrictId) {
            vMapCenter = GetCenterPointFromListOfCoordinates(allBounds);
            centerPoint = new T.LngLat(vMapCenter.lng, vMapCenter.lat);
            console.log("setViewport polygon");
        } else {
            var centerPointWGS84 = GPSTransfromToWGS84(mapCenter[0], mapCenter[1]);
            centerPoint = new T.LngLat(centerPointWGS84.lng, centerPointWGS84.lat);
            console.log("setViewport mapcenter");
        }
        //设置显示地图的中心点和级别
        map.centerAndZoom(centerPoint, zoom);
        map.setViewport(viewPortBounds);
        console.log("setViewport");
    }
}
function setMapDrag() {
    map.disableDrag();
    var setDragTime = window.setTimeout(function () {
        map.enableDrag();
        if (map.isDrag()) {
            //console.log("可以拖拽地图了");
            window.clearTimeout(setDragTime);
        }
    }, 2000);
}

function cleanViewPortBounds()
{
    viewPortBounds = [];
}

/**
 * 初始化地块编辑插件
 * */
function initPolygonTool() {
    if (polygonTool == null) {
        var config = { showLabel: true, color: "blue", weight: 2, opacity: 0.5, fillColor: "red", fillOpacity: 0.5 };
        //console.log("initPolygonTool");
        //console.log("map");
        //console.log(map);
        polygonTool = new T.PolygonTool(map, config);
    }
}

/**
 * 关闭手动画地块
 * */
function closeAddPolygon() {
    initPolygonTool();
    polygonTool.close();
}

/**
 * 绘制地块儿
 * @param {地块儿信息} landItem
 * @param {农作物图片} cropsImage
 */
function drawLand(landItem,isClear,cropsImage, polygon_event) {
    var bounds = [];
    var plantingArea = 0;
    var LngLat = null;   
    //console.log("landItem");
    //console.log(landItem);
    if (isClear)
        closeAddPolygon();
    allLabel = [];
    if (landItem.gpsLocations != null && landItem.gpsLocations != "") {
        var gpsLocations = eval(landItem.gpsLocations);
        for (var r = 0; r < gpsLocations.length; r++) {
            var itemLngLatAry = gpsLocations[r];
            var Lng = itemLngLatAry[0];
            var Lat = itemLngLatAry[1];
            //#region wgs84转换
            //var wgs84 = GPSTransfromToWGS84(Lng, Lat);
            //bounds.push(new T.LngLat(wgs84.lng, wgs84.lat));
            //allbounds.push(new T.LngLat(wgs84.lng, wgs84.lat));
            //#endregion
            bounds.push(new T.LngLat(Lng, Lat));
            allbounds.push(new T.LngLat(Lng, Lat));
        }
    }
    //var html = "<div class='AMapLabel'>"
    //html += "  <img class='Icon' src='" + cropsImage+"'/>";
    //html += "  <div class='landName' > " + landItem.regionName + "</div>";
    //html += "  <div class='plantArea'>{plantArea}亩</div>";
    //html += "</div>";

    //console.log("bounds");
    //console.log(bounds);
    //创建面对象
    var polygon = new T.Polygon(
        bounds,
        {
            //color: "#7ab212",
            color: "#7ab212",
            weight: 1,
            opacity: 0.5,
            fillColor: "#024db6",
            fillOpacity: 0.5,
            lineStyle: "solid",
            extData: landItem
        }
    );
    polygon.addEventListener(
        "mouseover",
        function (e) {
            //console.log(" polygon mouseover ");
            var eventPolygon = e.target;
            //console.log("e.target");
            //console.log(e.target);
            eventPolygon.setFillColor(defaultPolygonMouseOverColor);
            eventPolygon.setFillOpacity(0.9);
            //console.log(e);
            map.enableDrag();
            map.enableInertia();
            polygon_event("mouseover", e.target.options.extData);
        }
    );
    polygon.addEventListener(
        "mouseout",
        function (e) {
            //console.log(" polygon mouseout ");
            var eventPolygon = e.target;
            eventPolygon.setFillColor('#024db6');
            eventPolygon.setFillOpacity(0.5);
            //console.log(e);
            map.enableDrag();
            map.enableInertia();
        }
    );
    polygons.push(polygon);
    //向地图上添加面
    map.addOverLay(polygon);
    

    map.setViewport(bounds);
    //允许拖拽
    //map.enableDrag();
    //map.enableInertia();
    console.log(map.isDrag());
}
function drawLandPlantArea(cropsImage,isClear)
{
    if (isClear)
        clearAllCoverNoPolygon();
    if (allRegions != null &&
        typeof (allRegions) != "undefined" &&
        allRegions, length != null &&
        typeof (allRegions.length) != "undefined" &&
        allRegions.length > 0)
    {
        allRegions.forEach(function (landItem) {
            var plantingArea = 0;
            var LngLat = null;
            var html = "<div class='AMapLabel' title='" + landItem.regionName + "' ><img src='" + cropsImage + "' style='margin-left:3px;width:30px;height:30px;float:left'>{plantArea}亩</div>"
            if (landItem.statisticalInfo.totaValue != null && typeof (landItem.statisticalInfo.totaValue) != "undefined") {
                html = html.replace("{plantArea}", landItem.statisticalInfo.totaValue);
                plantingArea = landItem.statisticalInfo.totaValue;
            }
            else {
                html = html.replace("{plantArea}", "0");
            }
            landItem.center = [];
            if (landItem.gpsLocations != null && landItem.gpsLocations != "") {
                var center = GetCenterPointFromListOfCoordinates(landItem.gpsLocations);
                //console.log("center");
                //console.log(center);
                landItem.center.push(center.lng);
                landItem.center.push(center.lat);
                if (landItem.center != null && landItem.center.length > 0 && plantingArea > 0) {
                    //#region WGS84转换
                    var wgs84 = GPSTransfromToWGS84(landItem.center[0], landItem.center[1]);
                    LngLat = new T.LngLat(wgs84.lng, wgs84.lat);
                    //#endregion
                    //var lng = landItem.center[0];
                    //var lat = landItem.center[1];                   
                    //LngLat = new T.LngLat(lng, lat);
                }
            }
            //创建图片对象
            var icon = new T.Icon({
                iconUrl: cropsImage,
                iconSize: new T.Point(39, 57),
                iconAnchor: new T.Point(10, 25)
            });
            if (LngLat != null) {
                try {
                    //向地图上添加自定义标注
                    var mapIcon = new T.Marker(
                        LngLat,
                        { icon: icon }
                    );
                    var label = new T.Label({
                        //text: landItem.regionName + "<br/>" + plantingArea + "亩",
                        text: html,
                        position: LngLat,
                        offset: new T.Point(-63, 0)
                    });
                    //不画图标
                    //map.addOverLay(mapIcon);
                    allLabel.push(label);
                    map.addOverLay(label);

                }
                catch (e) {
                    //console.log("Error");
                    //console.log(e);
                }
            }
        });
    }
}

function drawLandPlantAreaData(cropsImage, plandAreaData) {
    clearAllCoverNoPolygon();
    var countArea = plandAreaData.countArea;
    var arrayData = plandAreaData.regionAreas;
    var cropsName = '';
    //var plantingArea = "面积";
    for (var i = 0; i < arrayData.length; i++) {
        if (arrayData[i] != null)
            countArea += arrayData[i].sowArea;
    }
    if (allRegions != null &&
        typeof (allRegions) != "undefined" &&
        allRegions, length != null &&
        typeof (allRegions.length) != "undefined" &&
        allRegions.length > 0) {
        allRegions.forEach(function (landItem) {
            var plantingArea = 0;
            var LngLat = null;
            cropsName = '面积' + arrayData[i].sowArea;
            var html = "<div class='AMapLabel' title='" + cropsName + "' ><img src='" + cropsImage + "' style='margin-left:3px;width:30px;height:30px;float:left'>{plantArea}亩</div>"
            if (landItem.statisticalInfo.totaValue != null && typeof (landItem.statisticalInfo.totaValue) != "undefined") {
                html = html.replace("{plantArea}", cropsName);
                plantingArea = landItem.statisticalInfo.totaValue;
            }
            else {
                html = html.replace("{plantArea}", "0");
            }
            landItem.center = [];
            if (landItem.gpsLocations != null && landItem.gpsLocations != "") {
                var center = GetCenterPointFromListOfCoordinates(landItem.gpsLocations);
                //console.log("center");
                //console.log(center);
                landItem.center.push(center.lng);
                landItem.center.push(center.lat);
                if (landItem.center != null && landItem.center.length > 0 && plantingArea > 0) {
                    //#region WGS84转换
                    var wgs84 = GPSTransfromToWGS84(landItem.center[0], landItem.center[1]);
                    LngLat = new T.LngLat(wgs84.lng, wgs84.lat);
                    //#endregion
                    //var lng = landItem.center[0];
                    //var lat = landItem.center[1];
                    //LngLat = new T.LngLat(lng, lat);
                }
            }
            //创建图片对象
            var icon = new T.Icon({
                iconUrl: cropsImage,
                iconSize: new T.Point(39, 57),
                iconAnchor: new T.Point(10, 25)
            });
            if (LngLat != null) {
                try {
                    //向地图上添加自定义标注
                    var mapIcon = new T.Marker(
                        LngLat,
                        { icon: icon }
                    );
                    var label = new T.Label({
                        //text: landItem.regionName + "<br/>" + plantingArea + "亩",
                        text: html,
                        position: LngLat,
                        offset: new T.Point(-63, 0)
                    });
                    //不画图标
                    //map.addOverLay(mapIcon);
                    allLabel.push(label);
                    map.addOverLay(label);

                }
                catch (e) {
                    //console.log("Error");
                    //console.log(e);
                }
            }
        });
    }
}

/// <summary>
/// 根据输入的地点坐标计算中心点
/// </summary>
/// <param name="geoCoordinateList"></param>
/// <returns></returns>
function GetCenterPointFromListOfCoordinates(geoCoordinateList) {
    var array = eval(geoCoordinateList);
    var total = array.length;
    //console.log("GetCenterPointFromListOfCoordinates");
    //console.log("geoCoordinateList");
    //console.log(array);
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
        //console.log(e);
    }
    X = X / total;
    Y = Y / total;
    Z = Z / total;
    var Lon = Math.atan2(Y, X);
    var Hyp = Math.sqrt(X * X + Y * Y);
    var Lat = Math.atan2(Z, Hyp);
    return { lat: Lat * 180 / Math.PI, lng: Lon * 180 / Math.PI };
}

//清除地图上的所有覆盖物(地块信息除外)
function clearAllCoverNoPolygon()
{
    if (allLabel != null && allLabel.length > 0)
        mapRemoveOverLay(allLabel);
    if (equipMarkers != null && equipMarkers.length > 0) 
        mapRemoveOverLay(equipMarkers);
    if (polygons != null)
        mapRemoveOverLay(polygons);
    map.clearOverLays();
}

function mapRemoveOverLay(array) {
    for (var i = 0; i < array.length; i++)
        map.removeOverLay(array[i]);
}

//地图上显示企业或者合作社
function drawMakers(regionid, statu, markers, polygon_event, maker_event) {
    //var markers = [{ name: '企业1', lng: '113.177708', lat: '37.891136' }, { name: '企业2', lng: '113.176870', lat: '37.888630' }, { name: '企业3', lng: '113.068910', lat: '37.909970' }, { name: '企业4', lng: '112.911620', lat: '37.945300' }]
    console.log("drawMakers");
    console.log(markers)
    clearAllCoverNoPolygon();
    map.clearOverLays();
    polygons = [];
    drawLandPolygon(regionid,polygon_event);
    if (markers != null && markers != "") {
        markers.forEach(function (marker) {
            var localIcon = new T.Icon({
                //size: new AMap.Size(45, 55),
                iconAnchor: new T.Point(10, 25),
                iconUrl: statu == 3 ? '../images/newhome/qiye.png' : '../images/newhome/hezuoshe.png',
                iconSize: new T.Point(45, 55),
            });
            //console.log(localIcon)
            /*var equip = new AMap.Marker({
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
            });*/
            if (marker.lng != "" && marker.lat != "") {

                //#region wgs84转换
                var wgs84 = GPSTransfromToWGS84(marker.lng, marker.lat);
                var LngLat = new T.LngLat(wgs84.lng, wgs84.lat);
                //#endregion

                var equip = new T.Marker(
                    LngLat,
                    {
                        icon: localIcon,
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
                    }
                );
                equip.addEventListener(
                    "click",
                    function (e) {
                        var currentData = e.target.options.extData;
                        maker_event(currentData);
                        map.enableDrag();
                        map.enableInertia();
                    }
                );
                console.log(equip)
                equipMarkers.push(equip);
                map.addOverLay(equip);
            }
            //equip.on('click', markerPop);
        });
        //map.setViewport(equipMarkers);
    }
    $(".tdt-right").css("right", "51vw");
}