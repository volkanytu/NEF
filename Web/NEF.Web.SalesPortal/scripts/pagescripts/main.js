/// <reference path="global.js" />
/// <reference path="index.js" />


var MainHelper = {
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


        $(".page-title", parent.document).html("PANO <small>pano & istatistik</small>");
        $("#btnBreadPage", parent.document).html("Pano");


        $("#liAllChart", parent.document).removeClass("active");
        $("#liSearch", parent.document).removeClass("active");
        $("#liHome", parent.document).addClass("active");
       

        if (parent.IndexHelper.BusinessUnitId.toUpperCase() === RetailerBusinessUnitId.toUpperCase()) {
            $("#liAllChart", parent.document).hide(100);
            $("#liCallCenter", parent.document).hide(100);
        }

        MainHelper.OnClickEvents();
        MainHelper.OnChangeEvents();
        MainHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");

    },
    "GetQuoteStatuses": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetQuoteStatuses",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Satış Durumu");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satış Durumu");
            }
        });
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
    "GetUserMonthlySalesQuantityData": function () {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserMonthlySalesQuantityData",
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
    "GetUserSalesQuantityByProject": function (callbackFunction) {

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
    "GetUserOpenActivities": function (stateCode, callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;
        jData.stateCode = stateCode;
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
    "GetUserOpenOpportunities": function (stateCode, callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;
        jData.stateCode = stateCode;
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
    "GetUserCustomers": function (stateCode, callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;
        jData.stateCode = stateCode;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserCustomers",
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
    "GetUserSales": function (stateCode, callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;
        jData.stateCode = stateCode;
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
    "GetUserHeaderInfo": function (callbackFunction) {

        var jData = {};

        jData.userId = MainHelper.UserId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetUserHeaderInfo",
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
    "UserBusinesUnit": "",
    "Team": "",
    "UserId": "", //"246D808B-8774-E411-80E4-005056B0203A",
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


function mainController($scope) {

    MainHelper.UserId = parent.IndexHelper.UserId;
    MainHelper.UserBusinesUnit = parent.IndexHelper.BusinessUnitId;
    MainHelper.Team = parent.IndexHelper.TeamId;


    MainHelper.GetUserMonthlySalesAmountData();
    MainHelper.GetUserMonthlySalesQuantityData();

    $scope.ActivityType = MainHelper.ActivityType;
    $scope.ActivityIcon = MainHelper.ActivityIcon;
    $scope.QuoteStatusClasses = GlobalHelper.QuoteStatusClasses;
    $scope.ContactTypes = GlobalHelper.ContactTypes;
    $scope.ContactTypeClass = GlobalHelper.ContactTypeClass;

    $scope.hasActivity = false;

    MainHelper.GetUserHeaderInfo(function (e) {

        $scope.$apply(function () {

            $scope.UserHeaderInfo = e;
        });

    });

    MainHelper.GetQuoteStatuses(function (e) {

        $scope.$apply(function () {
            if (e.Success == true) {
                $scope.quoteStatus = e.ReturnObject;
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Satış Durumu");
            }
        });

    });

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

    GetActivities(0);
    GetOpportunities(0);
    GetCustomers(-1);
    GetSales(-1);

    $scope.OpenActivityPage = function (contactId, activityId, activityType, type, stateCode) {

        if (stateCode == 0 || stateCode == 3) {

            var activityString = "";

            if (activityType == "Randevu") {
                activityString = "&appointmentid=" + activityId;
            }
            else if (activityType == "Telefon") {
                activityString = "&phonecallid=" + activityId;
            }

            if (type == EntityTypes.Kisi) {
                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + contactId + activityString);//.css("height", "1300px");
            }
            else {
                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + contactId + activityString);//.css("height", "1300px");
            }
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='editactivity.html?activityid=" + activityId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='fa fa-bomb'></i> Aktivite", "modal90");
        }


    };

    $scope.OpenDetailPage = function (objectId, type) {
        if (type == EntityTypes.Firma) {
            $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + objectId);//.css("height", "1300px");
        }
        else {
            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + objectId);//.css("height", "1300px");
        }
    };

    $scope.OpenOpportunityPage = function (opportunityId) {

        parent.IndexHelper.ShowDialog("<iframe src='editopportunity.html?oppid=" + opportunityId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='fa fa-bomb'></i> Fırsat", "modal90");

    };

    $scope.OpenSalesPage = function (salesId) {

        parent.IndexHelper.CheckSaleCustomerIdentityAndAddress(salesId, function (e) {

            if (e.Success == true) {
                parent.IndexHelper.ShowDialog("<iframe src='editsale.html?quoteid=" + salesId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "<i class='icon-flag'></i> Satış Kaydı", "modal90");
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Bilgileri");
            }
        });
    };

    $scope.EditHouse = function (productId) {
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };

    $scope.ListPriceSort = function () {
        sorttable.innerSortFunction.apply(document.getElementById('sortListPrice'), []);
    };

    $scope.SalesPriceSort = function () {
        sorttable.innerSortFunction.apply(document.getElementById('sortSalesPrice'), []);
    };

    $scope.PerSquareMeterSort = function () {
        sorttable.innerSortFunction.apply(document.getElementById('sortPersquarePrice'), []);
    };

    $scope.ActivityStateFilterChange = function (stateCode) {
        GetActivities(stateCode);
    }

    $scope.OpportunityStateFilterChange = function (stateCode) {
        GetOpportunities(stateCode);
    }


    $scope.CustomerStateFilterChange = function (stateCode) {
        GetCustomers(stateCode);
    }

    $scope.SalesStateFilterChange = function (stateCode) {
        GetSales(stateCode);
    }

    function GetActivities(stateCode) {
        MainHelper.GetUserOpenActivities(stateCode, function (e) {

            $scope.$apply(function () {

                $scope.PageOpenAcitivites = e.ReturnObject;

                if (e.ReturnObject.length == 0) {
                    $scope.hasActivity = true;
                    $scope.activityNumber = ' - Aktivite bulunmamaktadır.';
                }
                else {
                    $scope.activityNumber = ' - ' + e.ReturnObject.length + ' adet aktivite bulunmaktadır.';
                    $scope.OpenAcitivites = $scope.PageOpenAcitivites.slice(0, 9);

                    $('#page-selectionActivities').bootpag({
                        total: Math.ceil($scope.PageOpenAcitivites.length / 9),
                        page: 1,
                        maxVisible: 8
                    }).on("page", function (event, num) {
                        $scope.$apply(function () {
                            $scope.OpenAcitivites = $scope.PageOpenAcitivites.slice((num - 1) * 9, num * 9);
                        });
                    });
                }

            });

        });
    }

    function GetOpportunities(stateCode) {
        MainHelper.GetUserOpenOpportunities(stateCode, function (e) {

            $scope.$apply(function () {

                $scope.PageOpenOpportunities = e.ReturnObject;


                if (e.ReturnObject.length == 0) {
                    $scope.hasOpportunity = true;
                    $scope.opportunityNumber = ' - Fırsat bulunmamaktadır.';
                }
                else {
                    $scope.opportunityNumber = ' - ' + e.ReturnObject.length + ' adet fırsat bulunmaktadır.';
                    $scope.OpenOpportunities = $scope.PageOpenOpportunities.slice(0, 7);

                    $('#page-selectionOpportunities').bootpag({
                        total: Math.ceil($scope.PageOpenOpportunities.length / 7),
                        page: 1,
                        maxVisible: 10
                    }).on("page", function (event, num) {
                        $scope.$apply(function () {
                            $scope.OpenOpportunities = $scope.PageOpenOpportunities.slice((num - 1) * 7, num * 7);
                        });
                    });
                }

            });

        });
    }

    function GetCustomers(stateCode) {
        MainHelper.GetUserCustomers(stateCode, function (e) {

            $scope.$apply(function () {

                $scope.PageUserContacts = e.ReturnObject;

                if (e.ReturnObject.length == 0) {
                    $scope.hasContacts = true;
                    $scope.contactNumber = ' - Müşteri bulunmamaktadır.';
                }
                else {
                    $scope.contactNumber = ' - ' + e.ReturnObject.length + ' adet müşteri bulunmaktadır.';
                    $scope.UserContacts = $scope.PageUserContacts.slice(0, 7);

                    $('#page-selectionContacts').bootpag({
                        total: Math.ceil($scope.PageUserContacts.length / 7),
                        page: 1,
                        maxVisible: 10
                    }).on("page", function (event, num) {
                        $scope.$apply(function () {
                            $scope.UserContacts = $scope.PageUserContacts.slice((num - 1) * 7, num * 7);
                        });
                    });
                }

            });

        });
    }

    function GetSales(stateCode) {
        MainHelper.GetUserSales(stateCode, function (e) {

            $scope.$apply(function () {

                $scope.PageUserSales = e.ReturnObject;



                if (e.ReturnObject.length == 0) {
                    $scope.hasSales = true;
                    $scope.salesNumber = ' - Satış bulunmamaktadır.';
                }
                else {
                    $scope.salesNumber = ' - ' + e.ReturnObject.length + ' adet satış bulunmaktadır.';
                    $scope.hasSales = false;
                    $scope.UserSales = $scope.PageUserSales.slice(0, 7);

                    $('#page-selection').bootpag({
                        total: Math.ceil($scope.PageUserSales.length / 7),
                        page: 1,
                        maxVisible: 10
                    }).on("page", function (event, num) {
                        $scope.$apply(function () {
                            $scope.UserSales = $scope.PageUserSales.slice((num - 1) * 7, num * 7);
                        });
                    });
                }

            });

        });
    }
}
