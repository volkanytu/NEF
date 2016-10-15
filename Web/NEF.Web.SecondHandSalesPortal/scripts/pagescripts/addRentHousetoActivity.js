/// <reference path="global.js" />AddActivityIntrestedRentHouseHelper


var AddActivityIntrestedRentHouseHelper = {
    "OnLoad": function () {
        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        QuickSidebar.init() // init quick sidebar
        Index.init();
        Index.initDashboardDaterange();
        Index.initJQVMAP(); // init index page's custom scripts
        Index.initCalendar(); // init index page's custom scripts
        Index.initCharts(); // init index page's custom scripts
        Index.initChat();
        Index.initMiniCharts();
        //Index.initIntro();
        Tasks.initDashboardWidget();

        $(".page-title", parent.document).html("Konut Arama");
        $("#btnBreadPage", parent.document).html("Konut Arama");

        $("#liHouseSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liSearch", parent.document).removeClass("active");

        $('#txtMinPrice').mask('000,000,000,000', { reverse: true });
        $("#txtMaxPrice").mask('000,000,000,000', { reverse: true });

        AddActivityIntrestedRentHouseHelper.OnClickEvents();
        AddActivityIntrestedRentHouseHelper.OnChangeEvents();
        AddActivityIntrestedRentHouseHelper.OnChangeKeyPressEvents();

    },
    "GetActivityInterestedHouse": function (callbackFunction) {
        var jData = {};
        jData.activityId = AddActivityIntrestedRentHouseHelper.PhoneCallId != null ? AddActivityIntrestedRentHouseHelper.PhoneCallId : AddActivityIntrestedRentHouseHelper.AppointmentId;
        var jSonData = JSON.stringify(jData)

        $.ajax({
            url: CustomServiceUrl + "/GetActivityInterestedHouses",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            data: jSonData,
            type: "POST",
            beforeSend: function () {
                //Metronic.blockUI({ target: $("#portletActivity"), iconOnly: false, boxed: true, message: "Aktivite ilgili konutları getiriliyor..." });
            },
            complete: function () {
                //Metronic.unblockUI($("#portletActivity"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        callbackFunction(null);
                        parent.IndexHelper.ToastrShow(false, data.Result, "Aktivite Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Aktivite Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Aktivite Bilgileri");
            }
        });
    },
    "GetCities": function (countryId, callbackFunction) {
        var jData = {};
        jData.countryId = countryId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetCities",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetTowns": function (cityId, callbackFunction) {
        var jData = {};
        jData.cityId = cityId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetTowns",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Müşteri Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Bilgileri");
            }
        });
    },
    "GetGeneralHomeTypes": function () {
        var returnValue = null;
        $.ajax({
            url: CustomServiceUrl + "/GetGeneralHomeTypesForRent",
            async: false,
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
                    if (data.Success) {
                        returnValue = data.ReturnObject;
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Konut Ekleme");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Konut Ekleme");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Ekleme");
            }
        });

        return returnValue;
    },
    "GetHomeTypes": function (id) {
        var returnValue = null;

        var jData = {};
        jData.generalHomeTypeId = id;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetHomeTypesByGeneralTypeForRent",
            async: false,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    if (data.Success) {
                        returnValue = data.ReturnObject;
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Konut Arama");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Konut Arama");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Arama");
            }
        });

        return returnValue;
    },
    "ProductCreate": function (product) {
        var jData = {};
        jData.product = product;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CustomServiceUrl + "/CreateProductForRent",
            async: false,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {

            },
            complete: function () {

            },
            success: function (data) {

                if (data.Success) {
                    parent.IndexHelper.ToastrShow(true, "Konut başarılı ile eklendi! ", "Konut Bilgileri");

                } else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "HATA");
                }

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Oluşturma");
            }
        });

    },
    "ProjectId": "",
    "GetCurrencies": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetCurrencies",
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
                    if (data.Success) {
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Kapora Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Kapora Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Kapora Bilgileri");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {
    },
    "CountryId": "F40F7BF4-33B3-E211-8EE7-842B2B64B22F",
    "PhoneCallId": "",
    "AppointmentId": "",
    "ObjectTypeCode": ""

}

function mainController($scope) {

    var url = $.url(document.location);
    $scope.Product = {};
    $scope.Product.City = null;
    $scope.Product.County = null;
    $scope.cities = null;
    $scope.towns = null;
    $scope.generalHomeTypes = AddActivityIntrestedRentHouseHelper.GetGeneralHomeTypes();

    var project = {};

    if (url.data.param.query.hasOwnProperty("projectid")) {
        project.Id = url.param("projectid");
        project.Name = "new_project";
        $scope.Product.Project = project;
    } else {
        return;
    }
    if (url.data.param.query.hasOwnProperty("ActivityId") && url.data.param.query.hasOwnProperty("ActivityType")) {
        if (url.param("ActivityType") == 1) {
            $scope.Product.PhoneCall = {};
            $scope.Product.PhoneCall.Id = url.param("ActivityId");
            $scope.Product.PhoneCall.LogicalName = "phonecall";
            AddActivityIntrestedRentHouseHelper.PhoneCallId = url.param("ActivityId");
            AddActivityIntrestedRentHouseHelper.AppointmentId = null;
        } else if (url.param("ActivityType") == 2) {
            $scope.Product.Appointment = {};
            $scope.Product.Appointment.Id = url.param("ActivityId");
            $scope.Product.Appointment.LogicalName = "appointment";
            AddActivityIntrestedRentHouseHelper.AppointmentId = url.param("ActivityId");
            AddActivityIntrestedRentHouseHelper.PhoneCallId = null;
        }
    }
    else {
        return;
    }

    $scope.OpenHousePage = function () {
        window.open(GlobalHelper.CrmUrl + "main.aspx?etc=1024&extraqs=&histKey=604672489&id=%7b" + AddActivityIntrestedRentHouseHelper.ProductId + "%7d&newWindow=true&pagetype=entityrecord#667163135");
    }

    $scope.Create = function () {
        AddActivityIntrestedRentHouseHelper.ProductCreate($scope.Product);
        AddActivityIntrestedRentHouseHelper.GetActivityInterestedHouse(function (e) {
            $("#btnRefresh", $("#ifrmContent", parent.document)[0].contentWindow.document).click();
        });
    }

    AddActivityIntrestedRentHouseHelper.GetCities(AddActivityIntrestedRentHouseHelper.CountryId, function (e) {
        $scope.$apply(function () {
            $scope.cities = e;
        });
    });

    $scope.changeCities = function () {
        AddActivityIntrestedRentHouseHelper.GetTowns($scope.Product.City, function (e) {
            $scope.$apply(function () {
                $scope.towns = e;
            });
        });
    }

    $scope.GeneralHomeTypeChange = function () {
        $scope.homeTypes = AddActivityIntrestedRentHouseHelper.GetHomeTypes($scope.Product.GeneralHomeType.Id);
    }


    AddActivityIntrestedRentHouseHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;
            $scope.Product.Currency = {};
            $scope.Product.Currency.Id = $scope.currencies[0].TransactionCurrencyId;
        });
    });
}
