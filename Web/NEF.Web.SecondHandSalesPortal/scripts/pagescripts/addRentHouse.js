/// <reference path="global.js" />
/// <reference path="index.js" />


var RentHouseHelper = {
    "OnLoad": function () {
        RentHouseHelper.OnClickEvents();
        RentHouseHelper.OnChangeEvents();
        RentHouseHelper.OnChangeKeyPressEvents();
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
    "GetLocations": function (id) {
        var returnValue = null;

        var jData = {};
        jData.projectId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProjectLocations",
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
    "GetBlocks": function (id) {
        var returnValue = null;

        var jData = {};
        jData.projectId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProjectBlocks",
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
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "UserId": "246D808B-8774-E411-80E4-005056B0203A",
    "UpdateProductRentInfo": function (product) {
        var returnValue = null;
        var jData = {};
        jData.rentInfo = product;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/UpdateProductRentInfo",
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
                    parent.IndexHelper.ToastrShow(true, "Konut başarılı ile güncellendi! ", "Konut Bilgileri");
                } else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "HATA");
                }

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Bilgileri");
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
                Metronic.blockUI({ target: $("#plSalesAmount"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($(".page-content"));
            },
            success: function (data) {

                if (data.Success) {
                    parent.IndexHelper.ToastrShow(true, "Konut başarılı ile eklendi! ", "Konut Bilgileri");
                    parent.IndexHelper.CloseDialog();
                } else {
                    parent.IndexHelper.ToastrShow(false, data.Result, "HATA");
                    Metronic.unblockUI($(".page-content"));
                }

            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Ouluşturma");
            }
        });

    },
    "CountryId": "F40F7BF4-33B3-E211-8EE7-842B2B64B22F",
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
    }
}

function mainController($scope) {

    $scope.GuidEmpty = GlobalHelper.GuidEmpty;
    var url = $.url(document.location);
    RentHouseHelper.ProjectId = url.param("projectid");

    if (parent.IndexHelper.UserType == UserTypes.MusteriIliskileri) {
        $scope.showMusteriIliskileri = true;
    } else {
        $scope.showMusteriIliskileri = false;
    }

    

    $scope.Product = {};
    $scope.Product.ProjectId = RentHouseHelper.ProjectId;
    $scope.Product.City = null;
    $scope.Product.County = null;
    $scope.cities = null;
    $scope.towns = null;
    $scope.generalHomeTypes = RentHouseHelper.GetGeneralHomeTypes();

    RentHouseHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;
            $scope.Product.Currency = {};
            $scope.Product.Currency.Id = $scope.currencies[0].TransactionCurrencyId;
        });
    });

    $scope.OpenHousePage = function () {
        window.open(GlobalHelper.CrmUrl + "main.aspx?etc=1024&extraqs=&histKey=604672489&id=%7b" + RentHouseHelper.ProductId + "%7d&newWindow=true&pagetype=entityrecord#667163135");
    }

    $scope.Create = function () {
        var project = {};
        project.Id = RentHouseHelper.ProjectId;
        project.Name = "new_project";
        $scope.Product.Project = project;
        RentHouseHelper.ProductCreate($scope.Product);
    }

    RentHouseHelper.GetCities(RentHouseHelper.CountryId, function (e) {
        $scope.$apply(function () {
            $scope.cities = e;
        });

    });

    $scope.changeCities = function () {

        RentHouseHelper.GetTowns($scope.Product.City, function (e) {
            $scope.$apply(function () {
                $scope.towns = e;
            });
        });
    }

    $scope.GeneralHomeTypeChange = function () {
        $scope.homeTypes = RentHouseHelper.GetHomeTypes($scope.Product.GeneralHomeType.Id);
    }

}
