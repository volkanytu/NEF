/// <reference path="global.js" />
/// <reference path="index.js" />


var HouseHelper = {
    "OnLoad": function () {

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
    "ProductId": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    HouseHelper.ProductId = url.param("productid");

    if (parent.IndexHelper.UserType == UserTypes.MusteriIliskileri) {
        $scope.showMusteriIliskileri = true;
    }

    HouseHelper.GetProductInfo(function (e) {

        $scope.$apply(function () {

            $scope.Product = e.ReturnObject;
        });

    }, HouseHelper.ProductId);

    HouseHelper.GetProductOptionInfo(function (e) {

        $scope.$apply(function () {

            $scope.HomeOption = e.ReturnObject;
        });

    }, HouseHelper.ProductId);

    $scope.OpenHousePage = function () {
        window.open(GlobalHelper.CrmUrl + "main.aspx?etc=1024&extraqs=&histKey=604672489&id=%7b" + HouseHelper.ProductId + "%7d&newWindow=true&pagetype=entityrecord#667163135");
    }
}
