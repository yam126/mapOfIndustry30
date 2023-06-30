var vElementId = "";
var map;
var zoom = 12;
var mapCenter = [113.17666, 37.89498];
var isDrag = false;
var ctrl = null;
var polygonTool = null;
var currentPolygon = null;
var defaultPolygonColor = "#7ab212";
var defaultPolygonFillColor = "#024db6";
var defaultWeight = 1;
var defaultOpacity = 0.5;
var defaultFillOpacity = 0.5;
var allPolygon = [];
var selectPolygonColorShowDialog = null;
var adminRootLandId = "";
var viewAllEvent = null;
var viewReturnEvent = null;
var viewModel = "Normal";
var menu = null;
function GPSTransfromToWGS84(lng, lat) {
    var result = coordtransform.gcj02towgs84(lng, lat);
    return {
        lng: result[0],
        lat: result[1]
    };
}

/**
 * 初始化地图
 * @param {string} elementId 地图容器编号
 */
function InitMapLoader(elementId, callback)
{
    var T = window.T;
    vElementId = elementId;
    allPolygon = [];
    if (map == null) {
        map = new T.Map(
            elementId,
            {
                datasourcesControl: true,
                projection: 'EPSG:4326'
            }
        );
    }
    map.clearOverLays();
    var centerPointWGS84 = GPSTransfromToWGS84(mapCenter[0], mapCenter[1]);
    var centerPoint = new T.LngLat(centerPointWGS84.lng, centerPointWGS84.lat);
    //设置显示地图的中心点和级别
    map.centerAndZoom(centerPoint, zoom);
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
    AddRightMenu();
    if (callback != null)
        callback(map);
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

function AddRightMenu()
{
    if (menu == null) {
        menu = new T.ContextMenu({
            width: 100
        });
    } else {
        //map.removeContextMenu(menu);
        menu = new T.ContextMenu({
            width: 100
        });
    }
    var viewAllText = "查看全部";
    if (viewModel == "All")
        viewAllText = "结束查看全部";
    var txtMenuItem = [
        {
            text: '添加',
            callback: function () {
                customAddPolygon(
                    function (landPolygon) 
                    {
                        console.log("landPolygon");
                        console.log(landPolygon);
                    }
                );
            }
        },
        {
            text: '修改',
            callback: function () {
                if (currentPolygon != null)
                    currentPolygon.enableEdit();
            }
        },
        {
            text: '结束修改',
            callback: function () {
                if (currentPolygon != null)
                    currentPolygon.disableEdit();
            }
        },
        {
            text: '删除',
            callback: function () {
                if (currentPolygon != null) {
                    if (confirm("确定要删除选中的地块儿?"))
                        map.removeOverLay(currentPolygon);
                }
            }
        },
        {
            text: '设置颜色',
            callback: function () {
                if (currentPolygon != null) {
                    if (selectPolygonColorShowDialog != null)
                        selectPolygonColorShowDialog();
                }
            }
        }
        //{
        //    text: viewAllText,
        //    callback: function () {
        //        if (viewModel = "Normal") {
        //            viewModel = "All";
        //            if (viewAllEvent != null)
        //                viewAllEvent();
        //        } else {
        //            if (viewReturnEvent != null)
        //                viewReturnEvent();
        //        }
        //    }
        //}
    ];

    for (var i = 0; i < txtMenuItem.length; i++) {
        //添加菜单项
        var menuItem = new T.MenuItem(txtMenuItem[i].text, txtMenuItem[i].callback);
        menu.addItem(menuItem);
    }

    //添加右键菜单   
    map.addContextMenu(menu);
}

/**
 * 初始化地块编辑插件
 * */
function initPolygonTool() {
    if (polygonTool == null) {
        var config = {
            showLabel: true,
            color: defaultPolygonColor,
            weight: defaultWeight,
            opacity: defaultOpacity,
            fillColor: defaultPolygonFillColor,
            fillOpacity: defaultFillOpacity
        };
        console.log("initPolygonTool");
        console.log("map");
        console.log(map);
        polygonTool = new T.PolygonTool(map, config);
    }
}
function drawLandPolygon(gpsLocations,fillColor)
{
    if (gpsLocations != null && gpsLocations.length > 0)
    {
        var bounds = [];
        for (var r = 0; r < gpsLocations.length; r++) {
            var itemLngLatAry = gpsLocations[r];
            var Lng = itemLngLatAry[0];
            var Lat = itemLngLatAry[1];
            //#region wgs84转换
            //var wgs84 = GPSTransfromToWGS84(Lng, Lat);
            //bounds.push(new T.LngLat(wgs84.lng, wgs84.lat));
            //#endregion
            bounds.push(new T.LngLat(Lng, Lat));
        }
        if (fillColor != '')
            fillColor = '#' + fillColor;
        else
            fillColor = defaultPolygonFillColor;
        var polygon = new T.Polygon(
            bounds,
            {
                color: defaultPolygonColor,
                weight: defaultWeight,
                opacity: defaultOpacity,
                fillColor: fillColor,
                fillOpacity: defaultFillOpacity,
                lineStyle: "solid"
            }
        );
        addPolygonEvent(polygon);
        if (map != null) {
            //向地图上添加面
            map.addOverLay(polygon);
            allPolygon.push(polygon);
            map.setViewport(bounds);
        }
    }
}
function clearOverLays()
{
    if (map != null)
        map.clearOverLays();
}

//注册多边形地块儿事件(多边形)
function addPolygonEvent(polygon)
{
    //点击事件
    polygon.addEventListener(
        "click",
        function (e) {
            console.log(" polygon click ");          
            var eventPolygon = e.target;
            currentPolygon = eventPolygon;
            console.log(" polygon currentPolygon ");
            console.log(currentPolygon);
            map.enableDrag();
            map.enableInertia();
        }
    );
    polygon.addEventListener(
        "mousedown",
        function (e) {
            console.log(" polygon mouseover ");
            var eventPolygon = e.target;
            if (currentPolygon != null)
                currentPolygon.disableEdit();
            currentPolygon = eventPolygon;
            console.log(" polygon currentPolygon ");
            console.log(currentPolygon);
            currentPolygon.enableEdit();
            map.enableDrag();
            map.enableInertia();
        }
    );
}

/**
 * 手动画地块儿
 * 用户双击画完地块儿时触发，参数:obj回传用户画好的多边形信息
 * @param {any} callback
 * */
function customAddPolygon(callback) {
    initPolygonTool();
    polygonTool.open();
    polygonTool.addEventListener("draw",
        function (obj) {
            console.log("draw");
            console.log(obj);
            console.log("currentPolygon");
            console.log(obj.currentPolygon);
            addPolygonEvent(obj.currentPolygon);
            allPolygon.push(obj.currentPolygon);
            if (callback != null)
                callback(obj.currentPolygon);
        }
    );
}