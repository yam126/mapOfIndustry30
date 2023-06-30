var echartsAry = [];
//产值统计图
//elementId:dom元素编号
//yearLabelData:年代统计选项
//histogramData:柱状图数据
//lineChartData:折线图数据
function chartsIndustryStatistics(
    elementId,
    yearLabelData,
    histogramData,
    lineChartData) {
    // 基于准备好的dom，初始化echarts实例
    var myChart = echarts.init(document.getElementById(elementId));

    // 指定图表的配置项和数据
    var option = {
        legend: {
            data: ['人均产值'],
            orient: 'vertical',
            right: 10,
            top: 'center',
            // 图例文字样式
            textStyle: {
                fontSize: 16,
                fontWeight: 'bolder',
                color: '#fff'
            }
        },
        title: {

            //图标标题
            text: '产值(亿)',

            //文字样式
            textStyle: {
                fontSize: 18,
                fontWeight: 'bolder',
                color: '#fff'
            }
        },
        tooltip: {},
        grid: {
            x: 1,
            y: 2,
            left: '0%',
            right: '0%',
            bottom: '3%',
            top: '4%',
            containLabel: true,
        },
        color: ['#f5b644'], // 修改折线图的图例颜色要写在这里(其他写在lenged中)还要写在lengend中
        legend: {
            data: ['名称1', '名称2'],
            textStyle: {
                fontSize: 8, // 设置文字大小
                color: ['#5abff', '#50aeff', '#f5b644'],
            },
            itemWidth: 7, // 设置标志的小图标
            itemHeight: 7,
            top: -5,
            align: 'left', // 图例图标的方向，这里设置为左
            itemGap: -12, // 每项图例的距离
            right: -10, // 图例的位置
        },
        xAxis: {
            type: 'category',
            //data: ['2016', '2017', '2018', '2019', '2020', '2021']
            data: yearLabelData
        },
        axisLabel: {
            //x和y轴字体颜色
            color: '#fff'
        },
        yAxis: {
            type: 'value'
        },
        series: [
            //柱状图设置
            {
                name: '(亿)',
                type: 'bar',
                //data: [95, 100, 86, 60, 50, 30],
                data: histogramData,
                barWidth: 12,
                itemStyle: {
                    normal: {
                        //柱状图柱子颜色
                        color: function (params) {
                            var colorList = ['#00D81C', '#00D81C', '#00D81C', '#00D81C', '#00D81C', '#00D81C'];
                            return colorList[params.dataIndex];
                        },
                        // 柱形图圆角，初始化效果
                        barBorderRadius: [10, 10, 5, 5],
                        //统计柱状图线性颜色
                        color: new echarts.graphic.LinearGradient(1, 1, 0, 0, [
                            {
                                offset: 0,
                                color: "#00D81C"
                            },
                            {
                                offset: 1,
                                color: "#00D81C"
                            }
                        ])
                    }
                }
            },
            //折线图设置
            {
                //data: [150, 230, 224, 218, 135, 147, 260],
                data: lineChartData,
                type: 'line',
                symbol: 'circle',//拐点样式
                symbolSize: 18,//拐点大小
                itemStyle: {
                    normal: {
                        lineStyle: {
                            width: 3,//折线宽度
                            color: "#E8834D"//折线颜色
                        },
                        color: '#E8834D',//拐点颜色
                        borderColor: '#E8834D',//拐点边框颜色
                        borderWidth: 1//拐点边框大小
                    },
                    emphasis: {
                        color: '#E8834D'//hover拐点颜色定义
                    }
                }
            }
        ]
    };

    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    echartsAry.push(myChart);
}

//种植面积统计
function plantingAreaStatistics(elementId,copsName,staticsData)
{
    // 基于准备好的dom，初始化echarts实例
    var myChart = echarts.init(document.getElementById(elementId));

    // 指定图表的配置项和数据
    var option = {
        legend: {
            data: ['人均产值'],
            orient: 'vertical',
            right: 10,
            top: 'center',
            // 图例文字样式
            textStyle: {
                fontSize: 16,
                fontWeight: 'bolder',
                color: '#fff'
            }
        },
        title: {

            //图标标题
            text: '亩',

            //文字样式
            textStyle: {
                fontSize: 18,
                fontWeight: 'bolder',
                color: '#fff'
            }
        },
        tooltip: {},
        grid: {
            left: '3%',
            right: 0,
            bottom: '10%',
            top: '20%',
            containLabel: true,
        },
        color: ['#f5b644'], // 修改折线图的图例颜色要写在这里(其他写在lenged中)还要写在lengend中
        legend: {
            data: ['名称1', '名称2'],
            textStyle: {
                fontSize: 8, // 设置文字大小
                color: ['#5abff', '#50aeff', '#f5b644'],
            },
            itemWidth: 7, // 设置标志的小图标
            itemHeight: 7,
            top: -5,
            align: 'left', // 图例图标的方向，这里设置为左
            itemGap: -12, // 每项图例的距离
            right: -10, // 图例的位置
        },
        xAxis: {
            type: 'category',
            //data: ['景尚乡', '郭占强', '刘环静', '马龄', '刘静秋', '王鹏']
            data: regionNames
        },
        axisLabel: {
            //x和y轴字体颜色
            color: '#fff'
        },
        yAxis: {
            type: 'value'
        },
        series: [
            //柱状图设置
            {
                name: '亩',
                type: 'bar',
                //data: [95, 100, 86, 60, 50, 30],
                data: staticsData,
                barWidth: 12,
                itemStyle: {
                    normal: {
                        //柱状图柱子颜色
                        color: function (params) {
                            var colorList = ['#00D81C', '#00D81C', '#00D81C', '#00D81C', '#00D81C', '#00D81C'];
                            return colorList[params.dataIndex];
                        },
                        // 柱形图圆角，初始化效果
                        barBorderRadius: [10, 10, 5, 5],
                        //统计柱状图线性颜色
                        color: new echarts.graphic.LinearGradient(1, 1, 0, 0, [
                            {
                                offset: 0,
                                color: "#DC7147"
                            },
                            {
                                offset: 1,
                                color: "#F4D6CA"
                            }
                        ])
                    }
                }
            }
        ]
    };

    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    echartsAry.push(myChart);
}

//平均价格统计图
//elementId:元素编号,
//cropsName:农作物名称,
//yearsData:年份数据
//cropsYearData:农作物年份数据(数组)
/*示例{
 *   cropsName:'玉米',
 *   yearData:[100,200,300]
 * }*/
//注意:yearsData和cropsYearData的yearData必须一一对应
function averagePrice(element, cropsName, yearsData, cropsYearData) {
    var myChart = echarts.init(
        element,
        {
            renderer: 'canvas',
            useDirtyRect: false
        }
    );
    var seriesAry = new Array();
    console.log("cropsName");
    console.log(cropsName);
    console.log("yearsData");
    console.log(yearsData);
    console.log("cropsYearData");
    console.log(cropsYearData);
    if (cropsYearData == null || (cropsYearData.length == null || cropsYearData.length <= 0))
        return false;
    //#region 循环农作物年份数据
    for (var i = 0; i < cropsYearData.length; i++)
    {
        var seriesDataItem = {
            name: cropsYearData[i].cropsName,
            type: 'line',
            stack: 'Total',
            smooth: true,
            data: cropsYearData[i].yearsData
        };
        seriesAry.push(seriesDataItem);
    }
    //#endregion
    var option;
    option = {
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: cropsName,
            textStyle: {
                fontSize: "12",
                color:'#fff'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        axisLabel: {
            //x和y轴字体颜色
            color: '#fff',
            axisLabel: {
                interval:10,
                textStyle: {
                    fontSize: "12",
                }
            }
        },
        toolbox: {
            show: false,
            feature: {
                saveAsImage: {}
            }
        },
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: yearsData
        },
        yAxis: {
            type: 'value'
        },
        series: seriesAry
    };
    console.log("chartOption");
    console.log(option);
    if (option && typeof option === 'object') {
        myChart.setOption(option);
        echartsAry.push(myChart);
    }
}

function averagePriceCharts(element, cropsName, yearsData, cropsYearData) {
    var myChart = echarts.init(
        element,
        {
            renderer: 'canvas',
            useDirtyRect: false
        }
    );
    var seriesAry = new Array();
    console.log("cropsName");
    console.log(cropsName);
    console.log("yearsData");
    console.log(yearsData);
    console.log("cropsYearData");
    console.log(cropsYearData);
    if (cropsYearData == null || (cropsYearData.length == null || cropsYearData.length <= 0))
        return false;
    //#region 循环农作物年份数据
    for (var i = 0; i < cropsYearData.length; i++) {
        var seriesDataItem = {
            name: cropsYearData[i].cropsName,
            type: 'line',
            stack: 'Total',
            //areaStyle: {},
            smooth: true,
            //emphasis: {
            //    focus: 'series'
            //},
            data: cropsYearData[i].yearsData
        };
        seriesAry.push(seriesDataItem);
    }
    //#endregion
    var option;
    option = {
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross',
                label: {
                    backgroundColor: '#6a7985'
                }
            }
        },
        legend: {
            data: cropsName,
            textStyle: {
                fontSize: "12",
                color: '#fff'
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        axisLabel: {
            //x和y轴字体颜色
            color: '#fff',
            axisLabel: {
                interval: 10,
                textStyle: {
                    fontSize: "12",
                }
            }
        },
        toolbox: {
            show: false,
            feature: {
                saveAsImage: {}
            }
        },
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: yearsData
        },
        yAxis: {
            type: 'value'
        },
        series: seriesAry
    };
    console.log("chartOption");
    console.log(option);
    if (option && typeof option === 'object') {
        myChart.setOption(option);
        echartsAry.push(myChart);
    }
}

//统计图尺寸自适应
window.addEventListener("resize", function () {
    for (var i = 0; i < echartsAry.length; i++)
        echartsAry[i].resize();
});
