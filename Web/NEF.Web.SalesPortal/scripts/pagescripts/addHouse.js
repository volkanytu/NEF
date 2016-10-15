/// <reference path="global.js" />


var AddActivityIntrestedHouseHelper = {
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

        AddActivityIntrestedHouseHelper.OnClickEvents();
        AddActivityIntrestedHouseHelper.OnChangeEvents();
        AddActivityIntrestedHouseHelper.OnChangeKeyPressEvents();

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
    "GetEtaps": function (id) {
        var returnValue = null;

        var jData = {};
        jData.projectId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetEtaps",
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
    "GetProjectBlockTypes": function (id) {
        var returnValue = null;

        var jData = {};
        jData.projectId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProjectBlockTypes",
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
    "GetGeneralHomeTypes": function (id) {

        var jData = {};
        jData.projectId = id;
        var jSonData = JSON.stringify(jData);

        var returnValue = null;

        $.ajax({
            url: CustomServiceUrl + "/GetGeneralHomeTypes",
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
    "GetHomeTypes": function (id) {
        var returnValue = null;

        var jData = {};
        jData.generalHomeTypeId = id;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetHomeTypesByGeneralType",
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
    "GetStates": function (callbackfunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetProductStates",
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
    "GetStatus": function (state, callbackfunction) {

        var jData = {};
        jData.state = state;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/GetProductStatus",
            async: true,
            dataType: "json",
            contentType: "application/json;",
            type: "POST",
            data: jSonData,
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
    "Search": function (product, callbackFunction) {
        var minPrice = $("#txtMinPrice").val();
        var maxPrice = $("#txtMaxPrice").val();
        var jData = {};
        product.HasTerrace = product.hasOwnProperty("HasTerrace") ? product.HasTerrace : false;
        product.HasBalcony = product.hasOwnProperty("HasBalcony") ? product.HasBalcony : false;
        product.HasKitchen = product.hasOwnProperty("HasKitchen") ? product.HasKitchen : false;

        product.MinValue = minPrice == "" || minPrice == "0.00" ? null : minPrice.replace(',', '').replace(',', '');
        product.MaxValue = maxPrice == "" || maxPrice == "0.00" ? null : maxPrice.replace(',', '').replace(',', '');
        product.FloorNumber = product.FloorNumber == "" ? null : product.FloorNumber;
        product.HomeNumber = product.HomeNumber == "" ? null : product.HomeNumber;
        product.Aks = product.Aks == "" ? null : product.Aks;
        product.Direction = product.hasOwnProperty("Direction") ? product.Direction == -1 ? null : product.Direction : null;
        jData.product = product;
        jData.phonecallId = AddActivityIntrestedHouseHelper.PhoneCallId != null ? AddActivityIntrestedHouseHelper.PhoneCallId : GuidEmpty;
        jData.appointmentId = AddActivityIntrestedHouseHelper.AppointmentId != null ? AddActivityIntrestedHouseHelper.AppointmentId : GuidEmpty;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/MakeHouseSearchForActivity",
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
    "AddActivityIntrestedHouse": function (interestedHouse, callbackFunction) {
        var jData = {};
        jData.interestProduct = interestedHouse;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateActivityInterestedHouses",
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
    "GetActivityInterestedHouse": function (callbackFunction) {
        var jData = {};
        jData.activityId = AddActivityIntrestedHouseHelper.PhoneCallId != null ? AddActivityIntrestedHouseHelper.PhoneCallId : AddActivityIntrestedHouseHelper.AppointmentId;
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
    "PhoneCallId": "",
    "AppointmentId": "",
    "ObjectTypeCode": ""
}

function mainController($scope) {

    var url = $.url(document.location);
    AddActivityIntrestedHouseHelper.PhoneCallId = url.data.param.query.hasOwnProperty("phonecallid") ? url.param("phonecallid") : null;
    AddActivityIntrestedHouseHelper.AppointmentId = url.data.param.query.hasOwnProperty("appointmentid") ? url.param("appointmentid") : null;

    $scope.ShowResult = false;
    $scope.search = {};
    $scope.GuidEmpty = GlobalHelper.GuidEmpty;

    AddActivityIntrestedHouseHelper.GetProjects(function (e) {
        $scope.$apply(function () {
            if (e.Success == true) {
                $scope.projects = e.ReturnObject;
                // $scope.search.Project.Id = e.ReturnObject[0].ProjectId;
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Konut Arama");
            }
        });
    });

    AddActivityIntrestedHouseHelper.GetStates(function (e) {
        $scope.$apply(function () {
            if (e.Success == true) {
                $scope.states = e.ReturnObject;
                $scope.search.StateCode = e.ReturnObject[0];

                AddActivityIntrestedHouseHelper.GetStatus($scope.search.StateCode.Value, function (e) {
                    $scope.$apply(function () {
                        if (e.Success == true) {
                            $scope.statuses = e.ReturnObject;
                            $scope.search.StatusCode = e.ReturnObject[0];
                        }
                        else {
                            parent.IndexHelper.ToastrShow(false, e.Result, "Konut Arama");
                        }
                    });
                });
            }
            else {
                parent.IndexHelper.ToastrShow(false, e.Result, "Konut Arama");
            }
        });
    });

    AddActivityIntrestedHouseHelper.GetCurrencies(function (e) {
        $scope.$apply(function () {
            $scope.currencies = e;
            $scope.search.Currency = {};
            $scope.search.Currency.Id = $scope.currencies[0].TransactionCurrencyId;
        });
    });

    $scope.Search = function () {

        $("#rowResult th,td").removeClass("sorttable_sorted");
        $("#rowResult th,td").removeClass("sorttable_sorted_reverse");

        AddActivityIntrestedHouseHelper.Search($scope.search, function (e) {
            $scope.$apply(function () {
                var results = e;
                if (results.Success) {
                    $scope.resultNumber = ' - ' + results.ReturnObject.length + ' adet konut bulunmaktadır.';
                    $scope.searchResults = results.ReturnObject;
                    $scope.ShowResult = true;
                }
                else {
                    $scope.resultNumber = ' - Aradığınız kriterlere ait konut bulunmamaktadır.';
                    $scope.ShowResult = false;
                    parent.IndexHelper.ToastrShow(false, results.Result, "Konut Arama");
                }
            });
        });
    }

    $scope.Clear = function () {
        $scope.ShowResult = false;
        if ($scope.search.hasOwnProperty("Project")) { $scope.search.Project = null; }
        if ($scope.search.hasOwnProperty("Etap")) { $scope.search.Project = null; }
        if ($scope.search.hasOwnProperty("BlockType")) { $scope.search.Project = null; }
        if ($scope.search.hasOwnProperty("Block")) { $scope.search.Block.Id = null; }
        if ($scope.search.hasOwnProperty("GeneralHomeType")) { $scope.search.GeneralHomeType.Id = null; }
        if ($scope.search.hasOwnProperty("HomeType")) { $scope.search.HomeType.Id = null; }
        if ($scope.search.hasOwnProperty("FloorNumber")) { $scope.search.FloorNumber = ""; }
        if ($scope.search.hasOwnProperty("HomeNumber")) { $scope.search.HomeNumber = ""; }
        if ($scope.search.hasOwnProperty("Location")) { $scope.search.Location.Id = null; }
        if ($scope.search.hasOwnProperty("StatusCode")) { $scope.search.StatusCode = null; }
        if ($scope.search.hasOwnProperty("MaxValue")) { $scope.search.MaxValue = ""; }
        if ($scope.search.hasOwnProperty("MinValue")) { $scope.search.MinValue = ""; }
        if ($scope.search.hasOwnProperty("Aks")) { $scope.search.Aks = ""; }
        if ($scope.search.hasOwnProperty("LicenceNumber")) { $scope.search.LicenceNumber = ""; }
    }

    $scope.ProjectChange = function () {
        $scope.generalHomeTypes = AddActivityIntrestedHouseHelper.GetGeneralHomeTypes($scope.search.Project.Id);
        $scope.blocks = AddActivityIntrestedHouseHelper.GetBlocks($scope.search.Project.Id);
        $scope.locations = AddActivityIntrestedHouseHelper.GetLocations($scope.search.Project.Id);
        $scope.blocktypes = AddActivityIntrestedHouseHelper.GetProjectBlockTypes($scope.search.Project.Id);
        $scope.etaps = AddActivityIntrestedHouseHelper.GetEtaps($scope.search.Project.Id);
    }

    $scope.GeneralHomeTypeChange = function () {
        $scope.homeTypes = AddActivityIntrestedHouseHelper.GetHomeTypes($scope.search.GeneralHomeType.Id);
    }

    $scope.AddActivityIntrestedHouse = function (productId, statusCode) {
        if (statusCode == 1) {
            var interestProduct = {};
            interestProduct.InterestedProduct = {};
            interestProduct.InterestedProduct.ProductId = productId;
            if (AddActivityIntrestedHouseHelper.PhoneCallId != null) {
                interestProduct.PhoneCall = {};
                interestProduct.PhoneCall.Id = AddActivityIntrestedHouseHelper.PhoneCallId;
                interestProduct.PhoneCall.LogicalName = "phonecall";
            }
            else {
                interestProduct.Appointment = {};
                interestProduct.Appointment.Id = AddActivityIntrestedHouseHelper.AppointmentId;
                interestProduct.Appointment.LogicalName = "appointment";
            }

            AddActivityIntrestedHouseHelper.AddActivityIntrestedHouse(interestProduct, function (e) {
                $scope.$apply(function () {
                    var result = e;
                    if (result.Success) {
                        parent.IndexHelper.ToastrShow(true, result.Result, "Konut Arama");

                        AddActivityIntrestedHouseHelper.GetActivityInterestedHouse(function (e) {
                            $("#btnRefresh", $("#ifrmContent", parent.document)[0].contentWindow.document).click();

                            //$scope.$apply(function () {
                            // parent.$scope.activityProducts = e;
                            //$("#ifrmContent", parent.document)[0].contentWindow.$scope.activityProducts = e;
                            //});
                        });

                        $scope.Search();

                    }
                    else {
                        parent.IndexHelper.ToastrShow(false, result.Result, "Konut Arama");
                    }
                });
            });
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Yalnızca durumu Boş olan konutları ekleyebilirsiniz!", "Konut Arama");
        }
    };

    $scope.StateChange = function () {
        AddActivityIntrestedHouseHelper.GetStatus($scope.search.StateCode.Value, function (e) {
            $scope.$apply(function () {
                if (e.Success == true) {
                    $scope.statuses = e.ReturnObject;
                    $scope.search.StatusCode = e.ReturnObject[0];
                }
                else {
                    parent.IndexHelper.ToastrShow(false, e.Result, "Konut Arama");
                }
            });
        });
    }

    $scope.EditHouse = function (productId) {
        parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");
    };

    $scope.TestSort = function () {
        //$("#sortP").click();
        sorttable.innerSortFunction.apply(document.getElementById('sortP'), []);
        // parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };

    $scope.TestSortDollar = function () {
        //$("#sortP").click();
        sorttable.innerSortFunction.apply(document.getElementById('sortDollar'), []);
        // parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };

    $scope.TestSortEuro = function () {
        //$("#sortP").click();
        sorttable.innerSortFunction.apply(document.getElementById('sortEuro'), []);
        // parent.IndexHelper.ShowDialog("<iframe src='edithouse.html?productid=" + productId + "' style='width:100% !important; border:none;height:450px;padding-left:0px !important;padding-right:0px !important;overflow-y:scroll !important;overflow-x: hidden !important;' scrolling='auto'></iframe>", "", "modal90");

    };
}
