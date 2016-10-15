/// <reference path="global.js" />
/// <reference path="index.js" />


var OpportunityHelper = {
    "OnLoad": function () {

        OpportunityHelper.OnClickEvents();
        OpportunityHelper.OnChangeEvents();
        OpportunityHelper.OnChangeKeyPressEvents();


    },
    "GetOpportunityInfo": function (callbackFunction, oppId) {

        var jData = {};

        jData.oppId = oppId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetOpportunityInfo",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $(".page-content"), iconOnly: false, boxed: true, message: "Fırsat bilgileri çekiliyor..." });
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Fırsat Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Fırsat Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Satışlarım");
            }
        });
    },
    "GetOpportunityLostStatus": function (callbackFunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetOpportunityLostStatuses",
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
                        //returnValue = data.ReturnObject;
                        callbackFunction(data.ReturnObject);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, data.Result, "Fırsat Bilgileri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Fırsat Bilgileri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Fırsat Bilgileri");
            }
        });
    },
    "CloseOpportunityAsLost": function (callbackFunction, oppId, statusCode) {

        var jData = {};

        jData.opportunityId = oppId;
        jData.statusCode = statusCode;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CloseOpportunityAsLost",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $(".page-content"), iconOnly: false, boxed: true, message: "Fırsat kapatılıyor..." });
            },
            complete: function () {
                Metronic.unblockUI($(".page-content"));
            },
            success: function (data) {
                if (data != null) {
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Fırsat Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Fırsat Bilgileri");
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
    "OppId": ""
}

function mainController($scope) {

    $scope.hasProducts = false;

    var url = $.url(document.location);
    OpportunityHelper.OppId = url.param("oppid");

    OpportunityHelper.GetOpportunityInfo(function (e) {

        $scope.$apply(function () {

            $scope.Opportunity = e.ReturnObject;

            if (e.ReturnObject.StateCode.Value == 0) {
                $scope.showLoseOpportunity = true;
            }

            if (e.ReturnObject.CustomerType == "1") {
                $scope.CustomerTypeName = "Firma";
            }
            else {
                if (e.ReturnObject.ContactTypeCode != null)
                    $scope.CustomerTypeName = GlobalHelper.ContactTypes[e.ReturnObject.ContactTypeCode] + " müşteri"
                else
                    $scope.CustomerTypeName = GlobalHelper.ContactTypes.Aday + " müşteri";
            }

            if (e.ReturnObject.OppProducts != null && e.ReturnObject.OppProducts.length > 0) {
                $scope.hasProducts = true;
                $scope.OppProducts = e.ReturnObject.OppProducts;
            }

        });

    }, OpportunityHelper.OppId);

    OpportunityHelper.GetOpportunityLostStatus(function (e) {
        $scope.$apply(function () {
            $scope.opportunityStatuses = e;
            $scope.$apply(function () {
                $scope.opportunitystatus = $scope.opportunityStatuses[0];

            });
        });
    });

    $scope.EditHouse = function (productId) {
        parent.IndexHelper.CloseDialog();
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };

    $scope.CloseOpportunityAsLost = function()
    {
        if ($scope.opportunitystatus != null) {
            OpportunityHelper.CloseOpportunityAsLost(function (e) {

                $scope.$apply(function () {

                    if (e.Success) {
                        parent.IndexHelper.CloseDialog();
                    }

                });

            }, OpportunityHelper.OppId, $scope.opportunitystatus);
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Lütfen bir durum açıklaması seçiniz!", "Fırsat Bilgileri");
        }

    }
}
