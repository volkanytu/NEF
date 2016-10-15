/// <reference path="global.js" />
/// <reference path="index.js" />


var MainHelper = {
    "OnLoad": function () {
        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        QuickSidebar.init() // init quick sidebar
        Index.init();
        Index.initDashboardDaterange();
        Index.initJQVMAP(); // init index page's custom scripts
        Index.initCalendar(); // init index page's custom scripts
        //Index.initCharts(); // init index page's custom scripts
        Index.initChat();
        Index.initMiniCharts();
        Tasks.initDashboardWidget();

        $(".page-title", parent.document).html("PANO <small>pano & istatistik</small>");
        $("#btnBreadPage", parent.document).html("Pano");

        $("#liSearch", parent.document).removeClass("active");
        $("#liHome", parent.document).addClass("active");

        MainHelper.OnClickEvents();
        MainHelper.OnChangeEvents();
        MainHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");       

    },
    "GetUserMonthlySalesAmountData": function () {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserMonthlySalesAmountData",
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
    "GetUserSalesQuantityByProject": function () {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserSalesQuantityByProject",
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

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserSalesAmountByProject",
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
    "GetUserOpenActivities": function (callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserOpenActivities",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plOpenActivities"), iconOnly: false, boxed: true, message: "Aktiviteler çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plOpenActivities"));
                
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Açık Aktivitelerim");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Açık AKtivitelerim");
            }
        });
    },
    "GetUserOpenOpportunities": function (callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserOpenOpportunities",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyOpportunities"), iconOnly: false, boxed: true, message: "Fırsatlar çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyOpportunities"));
                
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Açık Fırsatlarım");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Açık Fırsatlarım");
            }
        });
    },
    "GetUserContacts": function (callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserContacts",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodyContacts"), iconOnly: false, boxed: true, message: "Müşteriler çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodyContacts"));
                
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşterilerim");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşterilerim");
            }
        });
    },
    "GetUserSales": function (callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserSales",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#plBodySales"), iconOnly: false, boxed: true, message: "Satışlar çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($("#plBodySales"));
                
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Satışlarım");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satışlarım");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "UserId": "",// "246D808B-8774-E411-80E4-005056B0203A",
    "ActivityType": {
        "Randevu": "randevunuz",
        "Telefon": "telefon görüşmeniz"
    },
    "ActivityIcon": {
        "Randevu": "fa-calendar",
        "Telefon": "fa-phone"
    }
}

var initSalesAmountCharts = function (chartData) {
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
            }],

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

                    showChartTooltip(item.pageX, item.pageY, item.datapoint[0], item.datapoint[1] + ' TL');
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


function mainController($scope) {

    MainHelper.UserId = parent.IndexHelper.UserId;


    MainHelper.GetUserMonthlySalesAmountData();
    MainHelper.GetUserMonthlySalesQuantityData();

    $scope.ActivityType = MainHelper.ActivityType;
    $scope.ActivityIcon = MainHelper.ActivityIcon;
    $scope.QuoteStatusClasses = GlobalHelper.QuoteStatusClasses;
    $scope.ContactTypes = GlobalHelper.ContactTypes;
    $scope.ContactTypeClass = GlobalHelper.ContactTypeClass;

    $scope.hasActivity = false;

    MainHelper.GetUserSalesAmountByProject(function (e) {

        $scope.$apply(function () {

            $scope.SalesAmountByProject = e;
        });

    });

    MainHelper.GetUserSalesQuantityByProject(function (e) {

        $scope.$apply(function () {

            $scope.SalesQuantityByProject = e;
        });

    });

    MainHelper.GetUserOpenActivities(function (e) {

        $scope.$apply(function () {

            $scope.OpenAcitivites = e.ReturnObject;

            if (e.ReturnObject.length == 0) {
                $scope.hasActivity = true;
            }

        });

    });

    MainHelper.GetUserOpenOpportunities(function (e) {

        $scope.$apply(function () {

            $scope.OpenOpportunities = e.ReturnObject;

            if (e.ReturnObject.length == 0) {
                $scope.hasOpportunity = true;
            }

        });

    });

    MainHelper.GetUserSales(function (e) {

        $scope.$apply(function () {

            $scope.UserSales = e.ReturnObject;

            if (e.ReturnObject.length == 0) {
                $scope.hasSales = true;
            }

        });

    });

    MainHelper.GetUserContacts(function (e) {

        $scope.$apply(function () {

            $scope.UserContacts = e.ReturnObject;

            if (e.ReturnObject.length == 0) {
                $scope.hasContacts = true;
            }

        });

    });

    $scope.OpenActivityPage = function (contactId, activityId, activityType) {

        var activityString = "";

        if (activityType == "Randevu") {
            activityString = "&appointmentid=" + activityId;
        }
        else if (activityType == "Telefon") {
            activityString = "&phonecallid=" + activityId;
        }

        $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + contactId + activityString);//.css("height", "1300px");

    };

    $scope.OpenOpportunityPage = function (opportunityId) {

        parent.IndexHelper.ShowDialog("<iframe src='editopportunity.html?oppid=" + opportunityId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='fa fa-bomb'></i> Fırsat", "modal90");

    };

    $scope.OpenSalesPage = function (salesId) {
        parent.IndexHelper.ShowDialog("<iframe src='editsale.html?quoteid=" + salesId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-flag'></i> Satış Kaydı", "modal90");
    };
}
