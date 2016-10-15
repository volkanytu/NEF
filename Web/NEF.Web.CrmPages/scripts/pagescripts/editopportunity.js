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
                    else
                    {
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

            if (e.ReturnObject.OppProducts != null && e.ReturnObject.OppProducts.length > 0) {
                $scope.hasProducts = true;
                $scope.OppProducts = e.ReturnObject.OppProducts;
            }

        });

    }, OpportunityHelper.OppId);
}
