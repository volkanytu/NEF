/// <reference path="global.js" />
/// <reference path="index.js" />


var HouseHelper = {
    "OnLoad": function () {

        if (jQuery().datepicker) {
            $('.date-picker').datepicker({
                isRtl: false,
                orientation: "left",
                autoclose: true,
                language: "tr"
            });
        }

        HouseHelper.OnClickEvents();
        HouseHelper.OnChangeEvents();
        HouseHelper.OnChangeKeyPressEvents();


    },
    "GetProductInfo": function (callbackFunction, productId) {

        var jData = {};

        jData.productId = productId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProductInfo",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $(".page-content"), iconOnly: false, boxed: true, message: "Konut bilgileri çekiliyor..." });
            },
            complete: function () {
                Metronic.unblockUI($(".page-content"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);

                    if (data.Success == true)
                        callbackFunction(data);
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Konut Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Fırsat Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Bilgileri");
            }
        });
    },
    "GetProductOptionInfo": function (callbackFunction, productId) {

        var jData = {};

        jData.productId = productId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProductOptionInfo",
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

                    if (data.Success == true)
                        callbackFunction(data);
                    else {

                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Konut Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Bilgileri");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {

    },
    "UserId": "246D808B-8774-E411-80E4-005056B0203A",
    "ProductId": "",
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
            url: CustomServiceUrl + "/ProductCreate",
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
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Ouluşturma");
            }
        });

    },
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

    var url = $.url(document.location);

    HouseHelper.ProductId = url.param("productid");

    if (parent.IndexHelper.UserType == UserTypes.MusteriIliskileri) {
        $scope.showMusteriIliskileri = true;
    }
    if (parent.IndexHelper.UserType != UserTypes.IkinciElSatisDirektoru) {
        $scope.HideHouseRentInfo = true;
    }

    HouseHelper.GetProductInfo(function (e) {

        $scope.$apply(function () {

            $scope.Product = e.ReturnObject;
            $scope.Currency = {};
            $scope.Currency.Id = $scope.Product.Currency.Id;
        });

    }, HouseHelper.ProductId);

    HouseHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;

        });
    });

    HouseHelper.GetProductOptionInfo(function (e) {

        $scope.$apply(function () {

            $scope.HomeOption = e.ReturnObject;
        });

    }, HouseHelper.ProductId);

    $scope.OpenHousePage = function () {
        window.open(GlobalHelper.CrmUrl + "main.aspx?etc=1024&extraqs=&histKey=604672489&id=%7b" + HouseHelper.ProductId + "%7d&newWindow=true&pagetype=entityrecord#667163135");
    }

    $scope.Update = function () {
        var rentInfo = {};
        var product = {};
        product.Id = HouseHelper.ProductId;
        product.Name = "product";
        rentInfo.Product = product;
        rentInfo.UsedRenatAndSalesStatus = $scope.Product.UsedRentalSalesStatus;
        rentInfo.GoodsStatus = $scope.Product.GoodsStatus
        rentInfo.RentalMonths = $scope.Product.RentalMonths;
        rentInfo.RentalNot = $scope.Product.RentalNotes;
        rentInfo.PaymentOfHire = $scope.Product.PaymentOfHire;
        rentInfo.Currency = $scope.Currency;
        HouseHelper.UpdateProductRentInfo(rentInfo);
    }

    $scope.Create = function () {
        $scope.Product.ProjectId = "";
        HouseHelper.ProductCreate($scope.Product);
    }

    $scope.AddAuthorityDoc = function () {
        parent.parent.IndexHelper.CloseDialog();
        $("#ifrmContent", parent.document).attr("src", "AddAuthorityDocs.html?productid=" + HouseHelper.ProductId).css("height", "1200px");
    }
}
