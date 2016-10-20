/// <reference path="global.js" />
/// <reference path="index.js" />


var ContactSearchForSaleHelper = {
    "OnLoad": function () {
        Metronic.init(); // init metronic core componets
        Layout.init(); // init layout
        //QuickSidebar.init() // init quick sidebar
        //Index.init();
        //Index.initDashboardDaterange();
        //Index.initJQVMAP(); // init index page's custom scripts
        //Index.initCalendar(); // init index page's custom scripts
        //Index.initCharts(); // init index page's custom scripts
        //Index.initChat();
        //Index.initMiniCharts();
        ////Index.initIntro();
        //Tasks.initDashboardWidget();

        $(".page-title", parent.document).html("Müşteri Arama");
        $("#btnBreadPage", parent.document).html("Müşteri Arama");

        $("#liSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liHouseSearch", parent.document).removeClass("active");
        $("#liAllChart", parent.document).removeClass("active");

        ContactSearchForSaleHelper.OnClickEvents();
        ContactSearchForSaleHelper.OnChangeEvents();
        ContactSearchForSaleHelper.OnChangeKeyPressEvents();

    },
    "MakeSearch": function (callbackFunction) {
        var jData = {};
        jData.searchedWord = $("#txtKey").val();
        var jSonData = JSON.stringify(jData);

        if (jData.searchedWord != "") {
            $.ajax({
                url: CustomServiceUrl + "/MakeContactSearch",
                async: true,
                dataType: "json",
                contentType: "application/json;",
                type: "POST",
                data: jSonData,
                beforeSend: function () {
                    Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
                },
                complete: function () {
                    Metronic.unblockUI($("#pcBody"));
                },
                success: function (data) {
                    if (data != null) {
                        data = JSON.parse(data);
                        callbackFunction(data);
                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Arama");
                    }
                },
                error: function (a, b, c) {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Arama");
                }
            });
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Lütfen aranacak bilgiyi yazınız!", "Müşteri Arama");
        }

    },
    "CustomerHasCallCenterCall": function (contactId, callbackFunction) {
        var jData = {};
        jData.customerId = contactId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CustomerHasCallCenterCall",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pcBody"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#pcBody"));
            },
            success: function (data) {
                if (data != null) {
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Müşteri Arama");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Müşteri Arama");
            }
        });

    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {
        //$("#txtKey").keydown(function (e) {
        //    if (e.which == 13) {
        //        $scope.makeSearch();
        //    }
        //});
    },
    "OnChangeEvents": function () {

    }
}

function mainController($scope) {
    $scope.isVisible = false;
    $scope.makeSearch = function ($keyEvent) {
        if ($keyEvent == null || $keyEvent.which == 13) {

            ContactSearchForSaleHelper.MakeSearch(function (e) {
                $scope.$apply(function () {
                    $scope.isNewContact = true;
                    if (e.Success) {
                        $scope.searchResults = e.ReturnObject;
                        $scope.isVisible = true;

                    }
                    else {
                        $scope.isVisible = false;
                    }

                });
            });
        }
    };

    $scope.contactTypes = GlobalHelper.ContactTypes;
    $scope.contactTypeClass = GlobalHelper.ContactTypeClass;

    $scope.selectContact = function (id, name, type) {

        $("#txtContact", parent.document).val(name).attr("crmid", id);
        dialog.modal('hide');
        //parent.IndexHelper.CloseDialog();
    }
}
