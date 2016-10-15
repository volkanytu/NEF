/// <reference path="global.js" />
/// <reference path="index.js" />


var PaymentHelper = {
    "OnLoad": function () {

        PaymentHelper.OnClickEvents();
        PaymentHelper.OnChangeEvents();
        PaymentHelper.OnChangeKeyPressEvents();


    },
    "GetPayments": function (callbackFunction, quoteId) {

        var jData = {};

        jData.quoteId = quoteId;

        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CommonServiceUrl + "/GetVoucher",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $(".page-content"), iconOnly: false, boxed: true, message: "Senet Bilgileri çekiliyor..." });
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Senet Bilgileri");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Senet Bilgileri");
                    parent.parent.IndexHelper.CloseDialog();
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Senetler");
            }
        });
    },
    "GetVoucherTypes": function (callbackFunction) {
        var returnValue = null;
        var jData = {};
        jData.participationId = id;
        var jSonData = JSON.stringify(jData);
        $.ajax({
            url: CommonServiceUrl + "/GetVoucherTypes",
            async: true,
            dataType: "json",
            data: null,
            contentType: "application/json;",
            type: "GET",
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
                        parent.IndexHelper.ToastrShow(false, data.Result, "Senet Tipleri");
                    }
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Senet Tipleri");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Senet Tipleri");
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
    "quoteId": ""
}

function mainController($scope) {

    $scope.hasPayments = false;

    var url = $.url(document.location);
    PaymentHelper.quoteId = url.param("quoteId");

    PaymentHelper.GetPayments(function (e) {

        $scope.$apply(function () {

            //$scope.Opportunity = e.ReturnObject;

            if (e.ReturnObject != null && e.ReturnObject.length > 0) {
                $scope.hasPayments = true;
                $scope.Payments = e.ReturnObject;
            }

        });

    },
    PaymentHelper.quoteId);
    $scope.Save = function () {

        $('.portlet-body input:checked').each(function (index) {
            if (this.value == "null") {
                return;
            }
            var data = $(this).parents('tr:eq(0)');
            var VoucherType = $(data).find("#VoucherType option:selected").val();
            var AmountType = $(data).find("#AmountType option:selected").val();
            var payment = {};
            payment.new_paymentid = this.value;
            payment.new_type = VoucherType;
            payment.new_itype = AmountType;
            var jSonData = JSON.stringify(payment);
            $.ajax({
                url: CommonServiceUrl + "/SingnedVoucher",
                async: true,
                dataType: "json",
                data: jSonData,
                contentType: "application/json;",
                type: "POST",
                beforeSend: function () {
                    Metronic.blockUI({ target: $(".page-content"), iconOnly: false, boxed: true, message: "Senetler imzalanıyor..." });
                },
                complete: function () {
                    Metronic.unblockUI($(".page-content"));

                },
                success: function (data) {
                    if (data != null) {
                        data = JSON.parse(data);

                        if (data.Success == true) {
                            if (index==$('.portlet-body input:checked').length-1) {
                                location.reload(true);
                            }
                           
                        }


                        else {
                            parent.IndexHelper.ToastrShow(false, data.Result, "Senet İmzalama");
                            parent.parent.IndexHelper.CloseDialog();
                        }
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Senet İmzalama");
                        parent.parent.IndexHelper.CloseDialog();
                    }
                },
                error: function (a, b, c) {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Senet İmzalama");
                }
            });
        });


    }
    $scope.SelectAll = function () {
        if ($('#SelectAll').is(":checked")) {
            $('input:checkbox').attr('checked', 'checked');
        }
        else {
            $('input:checkbox').removeAttr('checked');
        }

    }

}
