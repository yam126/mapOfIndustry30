//测试数据
var testData = [];
var cropsNames = ["玉米种植产业园", "大豆种植产业园", "水稻种植产业园", "小麦种植产业园", "土豆种植产业园"];
var allGroupData = [];
//随机获取数组的一个元素
//opt.array 要获取的数组
//opt.range 获取第几个元素
function getRandomArray(opt) {
    var old_arry = opt.arry,
        range = opt.range;
    //防止超过数组的长度
    range = range > old_arry.length ? old_arry.length : range;
    var newArray = [].concat(old_arry), //拷贝原数组进行操作就不会破坏原数组
        valArray = [];
    for (var n = 0; n < range; n++) {
        var r = Math.floor(Math.random() * (newArray.length));
        valArray.push(newArray[r]);
        //在原数组删掉，然后在下轮循环中就可以避免重复获取
        newArray.splice(r, 1);
    }
    return valArray;
}

//获取指定长度随机数
//min:最小值
//max:最大值
function getRandom(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}
function getAllApiData(landId, callback) {
    apiHelper.queryGroup(landId, function (response) {
        console.log(response);
        if (response.status == 0)
            callback(response.result);
        else
            callback(null);

    });
}
function getTestDataById(dataId) {
    var result = null;
    if (testData != null && testData.length > 0) {
        for (var i = 0; i < testData.length; i++) {
            if (testData[i].id == dataId)
                result = testData[i];
        }
    }
    return result;
}
function initTestData(LandId, regionDatas) {
    getAllApiData(
        LandId,
        function (response) {
            allGroupData = response;
            console.log("allGroupData");
            console.log(allGroupData);
            if (allGroupData != null && allGroupData.length > 0) {
                for (var i = 0; i < allGroupData.length; i++) {
                    if (regionDatas[i] != null && typeof (regionDatas[i]) != "undefined") {
                        testDataItem = {
                            id: allGroupData[i].landId,
                            name: regionDatas[i].landName,
                            parent_id: regionDatas[i].parent_id,
                            CropsName: allGroupData[i].cropsList[0].cropsName + "产业园",
                            SoilType: allGroupData[i].cropsList[0].soilType,
                            PlantingArea: allGroupData[i].cropsList[0].plantingArea,
                            LastYearOutput: allGroupData[i].cropsList[0].lastYearOutput,
                            TotalPopulation: allGroupData[i].totalPopulation,
                            TotaValue: allGroupData[i].totaValue,
                            CropsList: allGroupData[i].cropsList
                        };
                        testData.push(testDataItem);
                    }
                }
            } else {
                alert('注意:没有读取到地块儿数据,请检查接口地址或网络');
                return false;
            }
        });
}