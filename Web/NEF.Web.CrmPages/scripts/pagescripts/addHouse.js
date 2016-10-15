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

        $('#txtMinPrice').mask('000,000,000,000.00', { reverse: true });
        $("#txtMaxPrice").mask('000,000,000,000.00', { reverse: true });

        AddActivityIntrestedHouseHelper.OnClickEvents();
        AddActivityIntrestedHouseHelper.OnChangeEvents();
        AddActivityIntrestedHouseHelper.OnChangeKeyPressEvents();

    },
    "GetProjects": function (callbackfunction) {
        var returnValue = null;

        $.ajax({
            url: CustomServiceUrl + "/GetProjects",
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
    "GetGeneralHomeTypes": function () {
        var returnValue = null;

        $.ajax({
            url: CustomServiceUrl + "/GetGeneralHomeTypes",
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
    "GetStatus": function (callbackfunction) {
        $.ajax({
            url: CustomServiceUrl + "/GetProductStatus",
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
    "Search": function (product, callbackFunction) {
        if (product != null && product.hasOwnProperty("Project") && product.Project.Id != GlobalHelper.GuidEmpty) {
            var minPrice = $("#txtMinPrice").val();
            var maxPrice = $("#txtMaxPrice").val();
            var jData = {};
            product.MinValue = minPrice == "" || minPrice == "0.00" ? null : minPrice.replace(',', '').replace(',', '');
            product.MaxValue = maxPrice == "" || maxPrice == "0.00" ? null : maxPrice.replace(',', '').replace(',', '');
            product.FloorNumber = product.FloorNumber == "" ? null : product.FloorNumber;
            product.HomeNumber = product.HomeNumber == "" ? null : product.HomeNumber;
            product.Aks = product.Aks == "" ? null : product.Aks;
            jData.product = product;
            jData.phonecallId = AddActivityIntrestedHouseHelper.PhoneCallId != null ? AddActivityIntrestedHouseHelper.PhoneCallId : GuidEmpty;
            jData.appointmentId = AddActivityIntrestedHouseHelper.AppointmentId != null ? AddActivityIntrestedHouseHelper.AppointmentId : GuidEmpty;
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
        }
        else {
            parent.IndexHelper.ToastrShow(false, "Proje seçmeden konut arayamazsınız! ", "Konut Arama");
        }
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

    $scope.generalHomeTypes = AddActivityIntrestedHouseHelper.GetGeneralHomeTypes();

    AddActivityIntrestedHouseHelper.GetStatus(function (e) {
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

    $scope.Search = function () {

        //parent.frames[0].ContactDetailHelper.RefreshActivityIntrestedHouse();

        AddActivityIntrestedHouseHelper.Search($scope.search, function (e) {
            $scope.$apply(function () {
                var results = e;
                if (results.Success) {
                    $scope.searchResults = results.ReturnObject;
                    $scope.ShowResult = true;
                }
                else {
                    $scope.ShowResult = false;
                    parent.IndexHelper.ToastrShow(false, results.Result, "Konut Arama");
                }
            });
        });
    }

    $scope.Clear = function () {

        $scope.ShowResult = false;
    }

    $scope.ProjectChange = function () {
        $scope.blocks = AddActivityIntrestedHouseHelper.GetBlocks($scope.search.Project.Id);
        $scope.locations = AddActivityIntrestedHouseHelper.GetLocations($scope.search.Project.Id);
    }

    $scope.GeneralHomeTypeChange = function () {
        $scope.homeTypes = AddActivityIntrestedHouseHelper.GetHomeTypes($scope.search.GeneralHomeType.Id);
    }

    $scope.AddActivityIntrestedHouse = function (productId) {
        if ($scope.search.StatusCode.Value == 1) {
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
}
