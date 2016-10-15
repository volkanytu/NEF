/// <reference path="global.js" />


var AuthorityDocSearchHelper = {
    "OnLoad": function () {


        if (jQuery().datepicker) {
            $('.date-picker').datepicker({
                isRtl: false,
                orientation: "left",
                autoclose: true,
                language: "tr"
            });
        }

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

        $(".page-title", parent.document).html("Yetki Dokümanı Arama");
        $("#btnBreadPage", parent.document).html("Yetki Dokümanı Arama");

        $("#liHouseSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liSearch", parent.document).removeClass("active");
        $("#liAllChart", parent.document).removeClass("active");

        $('#txtMinPrice').mask('000,000,000,000', { reverse: true });
        $("#txtMaxPrice").mask('000,000,000,000', { reverse: true });


        AuthorityDocSearchHelper.OnClickEvents();
        AuthorityDocSearchHelper.OnChangeEvents();
        AuthorityDocSearchHelper.OnChangeKeyPressEvents();

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");

    },
    "GetProjects": function (callbackfunction) {
        var returnValue = null;
        var jData = {};
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProjects",
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
                    callbackfunction(data);
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

    "GetStatus": function (callbackfunction) {

        $.ajax({
            url: CustomServiceUrl + "/GetRentalProductStatus",
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
                    callbackfunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Konut Arama");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Arama");
            }
        });
    },
    "Search": function (project, callbackFunction) {

        var jData = {};

        jData.startDate = "/Date(" + $('#txtContractStartDate').data("datepicker").getDate().getTime() + ")/";
        jData.endDate = "/Date(" + $('#txtContractEndDate').data("datepicker").getDate().getTime() + ")/";
        jData.projectId = project.hasOwnProperty("Project") ? project.Project.Id : null;

        if (jData.startDate == "/Date(NaN)/") {
            jData.startDate = null;
        }
        if (jData.endDate == "/Date(NaN)/") {
            jData.endDate = null;
        }

        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/MakeAuthorityDocSearch",
            async: true,
            dataType: "json",
            data: jSonData,
            contentType: "application/json;",
            type: "POST",
            beforeSend: function () {
                Metronic.blockUI({ target: $("#pcMain"), iconOnly: true });
            },
            complete: function () {
                Metronic.unblockUI($("#pcMain"));
            },
            success: function (data) {
                if (data != null) {
                    data = JSON.parse(data);
                    callbackFunction(data);
                }
                else {
                    parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!", "Konut Arama");
                }
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı! " + c, "Konut Arama");
            }
        });
    },
    "OnClickEvents": function () {

    },
    "OnChangeKeyPressEvents": function () {

    },
    "OnChangeEvents": function () {
    },
    "ContactId": ""
}

function mainController($scope) {
    $scope.ShowResult = false;
    $scope.search = {};
    $scope.GuidEmpty = GlobalHelper.GuidEmpty;

    AuthorityDocSearchHelper.GetProjects(function (e) {
        $scope.$apply(function () {
            if (e.Success == true) {
                $scope.projects = e.ReturnObject;
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Konut Arama");
            }
        });
    });

    $scope.Search = function () {
        $("#rowResult th,td").removeClass("sorttable_sorted");
        $("#rowResult th,td").removeClass("sorttable_sorted_reverse");
        AuthorityDocSearchHelper.Search($scope.search, function (e) {
            $scope.$apply(function () {
                var results = e;
                if (results.Success) {
                    $scope.resultNumber = ' - ' + results.ReturnObject.length + ' adet konut bulunmaktadır.';

                    $scope.searchResults = results.ReturnObject;
                    $scope.ShowResult = true;

                    setTimeout(function () {
                        parent.IndexHelper.AutoResize("ifrmContent");
                    }, 200);
                }
                else {
                    $scope.resultNumber = ' - Aradığınız kriterlere ait konut bulunmamaktadır.';
                    $scope.ShowResult = false;
                    parent.IndexHelper.ToastrShow(false, results.Result, "Konut Arama");
                }
            });
        });
    }

    function closeIFrame() {
        $('#rentframe').remove();
    }


    $scope.Clear = function () {
        $scope.ShowResult = false;
        if ($scope.search.hasOwnProperty("Project")) { $scope.search.Project = null; }
        $('#txtContractStartDate').val(null);
        $('#txtContractEndDate').val(null);
    }



    $scope.EditHouse = function (productId) {
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");
    };

    $scope.DownloadFile = function (documentId) {
        var winOpener = window.open("upload.ashx?operation=10&id=" + documentId);
    };

}
