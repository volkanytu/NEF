/// <reference path="global.js" />
/// <reference path="index.js" />


var AllChartHelper = {
    "OnLoad": function () {
        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        QuickSidebar.init() // init quick sidebar
        Index.init();
        //Index.initDashboardDaterange();
        //Index.initJQVMAP(); // init index page's custom scripts
        //Index.initCalendar(); // init index page's custom scripts
        //Index.initCharts(); // init index page's custom scripts
        //Index.initChat();
        //Index.initMiniCharts();
        //Tasks.initDashboardWidget();


        $(".page-title", parent.document).html("SATIŞ EKİP PANOSU <small>pano & istatistik</small>");
        $("#btnBreadPage", parent.document).html("Satış Ekip Panosu");

        $("#liHome", parent.document).removeClass("active");
        $("#liHouseSearch", parent.document).removeClass("active");
        $("#liSearch", parent.document).removeClass("active");
        $("#liAllChart", parent.document).addClass("active");

        AllChartHelper.OnClickEvents();
        AllChartHelper.OnChangeEvents();
        AllChartHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");

    },
    "GetUserMonthlySalesAmountData": function () {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetMonthlySalesAmountData",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    initSalesAmountCharts(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aylık Satış Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aylık Satış Datası");
            }
        });
    },
    "GetUserMonthlySalesQuantityData": function () {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetMonthlySalesQuantityData",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    initSalesQuantityCharts(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aylık Satış Adeti Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aylık Satış Adeti Datası");
            }
        });
    },
    "GetUserSalesAmountByProject": function (callbackFunction) {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetSalesAmountByProject",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Proje Satış Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Proje Satış Datası");
            }
        });
    },
    "GetUserSalesQuantityByProject": function (callbackFunction) {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetSalesQuantityByProject",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aylık Satış Adeti Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aylık Satış Adeti Datası");
            }
        });
    },

    "GetUserGeneralMonthlySalesAmountData": function () {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetMonthlySalesGeneralAmountData",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    initSalesGeneralAmountCharts(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Genel Aylık Satış Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Genel Aylık Satış Datası");
            }
        });
    },
    "GetUserGeneralMonthlySalesQuantityData": function () {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetMonthlySalesGeneralQuantityData",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    initSalesGeneralQuantityCharts(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Genel Aylık Satış Adeti Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Genel Aylık Satış Adeti Datası");
            }
        });
    },
    "GetUserGeneralSalesAmountByProject": function (callbackFunction) {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetSalesGeneralAmountByProject",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Proje Satış Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Genel Proje Satış Datası");
            }
        });
    },
    "GetUserGeneralSalesQuantityByProject": function (callbackFunction) {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetGeneralSalesQuantityByProject",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                //Metronic.unblockUI($("#plSalesAmount"));

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data.ReturnObject);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Genel Aylık Satış Adeti Datası");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Genel Aylık Satış Adeti Datası");
            }
        });
    },

    "GetUserHeaderInfo": function (callbackFunction) {

        var jData = {};

        jData.userId = AllChartHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetAllHeaderInfo",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#rowUserHeader"), iconOnly: false, boxed: true, message: "Kullancı bilgileri çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#rowUserHeader"));
            },
            success: function (data) {
                if (data != null) {
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Kullanıcı Özet Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kullanıcı Özet Bilgileri");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "UserId": "", //"246D808B-8774-E411-80E4-005056B0203A",
}

var initSalesAmountCharts = function (chartData) {
    if (!jQuery.plot) {
        return;
    }

    var data2 = [];

    //for (var i = 0; i < chartData.length; i++) {

    //    data2.push([chartData[i][0], (parseInt(chartData[i][1]) - 1000000)]);
    //}
    function showChartTooltip(x, y, xValue, yValue) {
        $('<div id="tooltip" class="chart-tooltip">' + yValue + '<\/div>').css({
            position: 'absolute',
            display: 'none',
            top: y - 40,
            left: x - 40,
            border: '0px solid #ccc',
            padding: '2px 6px',
            'background-color': '#fff'
        }).appendTo("body").fadeIn(200);
    }

    if ($('#site_statistics').size() != 0) {


        $('#site_statistics_loading').hide();
        $('#site_statistics_content').show();

        var plot_statistics = $.plot($("#site_statistics"),
            [{
                data: chartData,
                lines: {
                    fill: 0.6,
                    lineWidth: 0
                },
                color: ['#f89f9f']
            }, {
                data: chartData,
                points: {
                    show: true,
                    fill: true,
                    radius: 5,
                    fillColor: "#f89f9f",
                    lineWidth: 3
                },
                color: '#fff',
                shadowSize: 0
            }
            //,{
            //    data: data2,
            //    lines: { show: true,fillColor:"blue" },
            //    points: { show: true }
            //}
            ],

            {
                xaxis: {
                    tickLength: 0,
                    tickDecimals: 0,
                    mode: "categories",
                    min: 0,
                    font: {
                        lineHeight: 14,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                yaxis: {
                    ticks: 5,
                    tickDecimals: 0,
                    tickColor: "#eee",
                    font: {
                        lineHeight: 14,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                grid: {
                    hoverable: true,
                    clickable: true,
                    tickColor: "#eee",
                    borderColor: "#eee",
                    borderWidth: 1
                }
            });

        parent.IndexHelper.AutoResize("ifrmContent");

        var previousPoint = null;
        $("#site_statistics").bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));
            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var x = item.datapoint[0].toFixed(2),
                        y = item.datapoint[1].toFixed(2);

                    showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1].toFixed(2).replace(/./g, function(c, i, a) {
                        return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
                    }) + ' TL');
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

}

var initSalesQuantityCharts = function (chartData) {
    if (!jQuery.plot) {
        return;
    }

    function showChartTooltip(x, y, xValue, yValue) {
        $('<div id="tooltip" class="chart-tooltip">' + yValue + '<\/div>').css({
            position: 'absolute',
            display: 'none',
            top: y - 40,
            left: x - 40,
            border: '0px solid #ccc',
            padding: '2px 6px',
            'background-color': '#fff'
        }).appendTo("body").fadeIn(200);
    }

    if ($('#site_activities').size() != 0) {

        $('#site_activities_loading').hide();
        $('#site_activities_content').show();

        var plot_statistics = $.plot($("#site_activities"),
            [{
                data: chartData,
                lines: {
                    fill: 0.6,
                    lineWidth: 0
                },
                color: ['#BAD9F5']
            }, {
                data: chartData,
                points: {
                    show: true,
                    fill: true,
                    radius: 3,
                    fillColor: "#9ACAE6",
                    lineWidth: 2
                },
                color: '#9ACAE6',
                shadowSize: 1
            }],

            {
                xaxis: {
                    tickLength: 0,
                    tickDecimals: 0,
                    mode: "categories",
                    min: 0,
                    font: {
                        lineHeight: 18,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                yaxis: {
                    ticks: 5,
                    tickDecimals: 0,
                    tickColor: "#eee",
                    font: {
                        lineHeight: 14,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                grid: {
                    hoverable: true,
                    clickable: true,
                    tickColor: "#eee",
                    borderColor: "#eee",
                    borderWidth: 1
                }
            });

        parent.IndexHelper.AutoResize("ifrmContent");

        var previousPoint = null;
        $("#site_activities").bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));
            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var x = item.datapoint[0].toFixed(2),
                        y = item.datapoint[1].toFixed(2);

                    showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1] + ' Adet');
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

}


var initSalesGeneralAmountCharts = function (chartData) {
    if (!jQuery.plot) {
        return;
    }

    var data2 = [];

    //for (var i = 0; i < chartData.length; i++) {

    //    data2.push([chartData[i][0], (parseInt(chartData[i][1]) - 1000000)]);
    //}
    function showChartTooltip(x, y, xValue, yValue) {
        $('<div id="tooltip" class="chart-tooltip">' + yValue + '<\/div>').css({
            position: 'absolute',
            display: 'none',
            top: y - 40,
            left: x - 40,
            border: '0px solid #ccc',
            padding: '2px 6px',
            'background-color': '#fff'
        }).appendTo("body").fadeIn(200);
    }

    if ($('#general_site_statistics').size() != 0) {


        $('#general_site_statistics_loading').hide();
        $('#general_site_statistics_content').show();

        var plot_statistics = $.plot($("#general_site_statistics"),
            [{
                data: chartData,
                lines: {
                    fill: 0.6,
                    lineWidth: 0
                },
                color: ['#f89f9f']
            }, {
                data: chartData,
                points: {
                    show: true,
                    fill: true,
                    radius: 5,
                    fillColor: "#f89f9f",
                    lineWidth: 3
                },
                color: '#fff',
                shadowSize: 0
            }
            //,{
            //    data: data2,
            //    lines: { show: true,fillColor:"blue" },
            //    points: { show: true }
            //}
            ],

            {
                xaxis: {
                    tickLength: 0,
                    tickDecimals: 0,
                    mode: "categories",
                    min: 0,
                    font: {
                        lineHeight: 14,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                yaxis: {
                    ticks: 5,
                    tickDecimals: 0,
                    tickColor: "#eee",
                    font: {
                        lineHeight: 14,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                grid: {
                    hoverable: true,
                    clickable: true,
                    tickColor: "#eee",
                    borderColor: "#eee",
                    borderWidth: 1
                }
            });

        parent.IndexHelper.AutoResize("ifrmContent");

        var previousPoint = null;
        $("#general_site_statistics").bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));
            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var x = item.datapoint[0].toFixed(2),
                        y = item.datapoint[1].toFixed(2);

                    showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1].toFixed(2).replace(/./g, function (c, i, a) {
                        return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
                    }) + ' TL');
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

}

var initSalesGeneralQuantityCharts = function (chartData) {
    if (!jQuery.plot) {
        return;
    }

    function showChartTooltip(x, y, xValue, yValue) {
        $('<div id="tooltip" class="chart-tooltip">' + yValue + '<\/div>').css({
            position: 'absolute',
            display: 'none',
            top: y - 40,
            left: x - 40,
            border: '0px solid #ccc',
            padding: '2px 6px',
            'background-color': '#fff'
        }).appendTo("body").fadeIn(200);
    }

    if ($('#general_site_activities').size() != 0) {

        $('#general_site_activities_loading').hide();
        $('#general_site_activities_content').show();

        var plot_statistics = $.plot($("#general_site_activities"),
            [{
                data: chartData,
                lines: {
                    fill: 0.6,
                    lineWidth: 0
                },
                color: ['#BAD9F5']
            }, {
                data: chartData,
                points: {
                    show: true,
                    fill: true,
                    radius: 3,
                    fillColor: "#9ACAE6",
                    lineWidth: 2
                },
                color: '#9ACAE6',
                shadowSize: 1
            }],

            {
                xaxis: {
                    tickLength: 0,
                    tickDecimals: 0,
                    mode: "categories",
                    min: 0,
                    font: {
                        lineHeight: 18,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                yaxis: {
                    ticks: 5,
                    tickDecimals: 0,
                    tickColor: "#eee",
                    font: {
                        lineHeight: 14,
                        style: "normal",
                        variant: "small-caps",
                        color: "#6F7B8A"
                    }
                },
                grid: {
                    hoverable: true,
                    clickable: true,
                    tickColor: "#eee",
                    borderColor: "#eee",
                    borderWidth: 1
                }
            });

        parent.IndexHelper.AutoResize("ifrmContent");

        var previousPoint = null;
        $("#general_site_activities").bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));
            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var x = item.datapoint[0].toFixed(2),
                        y = item.datapoint[1].toFixed(2);

                    showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1] + ' Adet');
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

}


function mainController($scope) {

    AllChartHelper.UserId = parent.IndexHelper.UserId;

    AllChartHelper.GetUserMonthlySalesAmountData();
    AllChartHelper.GetUserMonthlySalesQuantityData();

    AllChartHelper.GetUserGeneralMonthlySalesAmountData();
    AllChartHelper.GetUserGeneralMonthlySalesQuantityData();

    AllChartHelper.GetUserHeaderInfo(function (e) {

        $scope.$apply(function () {

            $scope.UserHeaderInfo = e;
        });

    });

    AllChartHelper.GetUserSalesAmountByProject(function (e) {

        $scope.$apply(function () {

            $scope.SalesAmountByProject = e;
        });

    });

    AllChartHelper.GetUserSalesQuantityByProject(function (e) {

        $scope.$apply(function () {

            $scope.SalesQuantityByProject = e;
        });

    });


    AllChartHelper.GetUserGeneralSalesAmountByProject(function (e) {

        $scope.$apply(function () {

            $scope.SalesGeneralAmountByProject = e;
        });

    });

    AllChartHelper.GetUserGeneralSalesQuantityByProject(function (e) {

        $scope.$apply(function () {

            $scope.SalesGeneralQuantityByProject = e;
        });

    });
}
