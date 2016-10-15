/// <reference path="global.js" />
/// <reference path="index.js" />


var ContactSearchHelper = {
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
        //Index.initIntro();
        //Tasks.initDashboardWidget();

        $(".page-title", parent.document).html("Müşteri Arama");
        $("#btnBreadPage", parent.document).html("Müşteri Arama");

        $("#liSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liHouseSearch", parent.document).removeClass("active");
        $("#liAllChart", parent.document).removeClass("active");

        ContactSearchHelper.OnClickEvents();
        ContactSearchHelper.OnChangeEvents();
        ContactSearchHelper.OnChangeKeyPressEvents();


        //var url = $.url(document.location);
        //if (url.param("phonenumber") != undefined) {
        //    parent.IndexHelper.PhoneNumber = url.param("phonenumber");
        //}

        $(window).resize(function () {
            parent.IndexHelper.AutoResize("ifrmContent");
        });

        parent.IndexHelper.AutoResize("ifrmContent");


    },
    "MakeSearch": function (callbackFunction) {
        var jData = {};
        jData.searchedWord = $("#txtKey").val();

        //Business Unit Alternatif ise owning user'a göre arama yapılır 
        if (parent.IndexHelper.BusinessUnitId.toUpperCase() == RetailerBusinessUnitId.toUpperCase() && jData.searchedWord != "") {
            jData.ownerId = parent.IndexHelper.UserId.toUpperCase();
            var jSonData = JSON.stringify(jData);
            $.ajax({
                url: CustomServiceUrl + "/MakeContactSearchWithOwner",
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
        else if (jData.searchedWord != "") {
            var jSonData = JSON.stringify(jData);
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

    },
    "CreateActivityForCallCenter": function (customerId, type, callbackfunction) {
        var activity = {};
        activity.Direction = parent.IndexHelper.Direction == "false" ? DirectionCodes.Gelen : DirectionCodes.Giden;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = type == EntityTypes.Kisi ? "contact" : "account";
        activity.ActivityParty.Id = customerId;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = parent.IndexHelper.UserId;

        var jData = {};
        jData.activity = activity;
        jData.systemUserId = parent.IndexHelper.UserId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/CreateActivity",
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
                callbackfunction(data);
            },
            error: function (a, b, c) {
                parent.IndexHelper.ToastrShow(false, "Beklenmedik bir hata ile karşılaşıldı!" + c, "Aktivite Oluşturma");
            }
        });
    },
}

function mainController($scope) {
    $scope.isVisible = false;
    $scope.isWarning = false;
    $scope.showActivityCreate = true;

    if (parent.IndexHelper.IsCallCenterSearch) {
        $scope.showActivityCreate = false;
    }


    $scope.makeSearch = function ($keyEvent) {
        if ($keyEvent == null || $keyEvent.which == 13) {

            ContactSearchHelper.MakeSearch(function (e) {
                $scope.$apply(function () {
                    $scope.isNewContact = true;
                    if (e.Success) {
                        $scope.searchResults = e.ReturnObject;
                        $scope.isVisible = true;
                        $scope.isWarning = false;

                        setTimeout(function () {
                            parent.IndexHelper.AutoResize("ifrmContent");
                        }, 200);

                    }
                    else {
                        setTimeout(function () {
                            parent.IndexHelper.AutoResize("ifrmContent");
                        }, 200);

                        $scope.isVisible = false;
                        $scope.isWarning = true;
                        parent.IndexHelper.ToastrShow(false, e.Result, "Müşteri Arama");
                    }

                });
            });
        }
    };

    $scope.contactTypes = GlobalHelper.ContactTypes;
    $scope.contactTypeClass = GlobalHelper.ContactTypeClass;


    if (parent.IndexHelper.IsCallCenterSearch) {
        setTimeout(function () {
            $("#txtKey").val(parent.IndexHelper.SearchText);
            $scope.makeSearch();
        }, 200);
    }

    $scope.openDetail = function (id, type) {

        if (parent.IndexHelper.UserType != UserTypes.CallCenter) {
            if (type == EntityTypes.Kisi) {
                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id).css("height", "1300px");
            }
            else {
                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + id).css("height", "1300px");
            }
        }
        else if (parent.IndexHelper.UserType == UserTypes.CallCenter) 
        {
            if (parent.IndexHelper.ActivityId != null && parent.IndexHelper.ActivityId != "") 
            {
                if (type == EntityTypes.Kisi) {
                    $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "1300px");
                }
                else {
                    $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + id + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "1300px");
                }
            }
            else {
                ContactSearchHelper.CustomerHasCallCenterCall(id, function (e) {
                    $scope.$apply(function () {
                        if (e.Success) {
                            if (type == EntityTypes.Kisi) {
                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id + "&phonecallid=" + e.CrmId).css("height", "1300px");
                            }
                            else {
                                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + id + "&phonecallid=" + e.CrmId).css("height", "1300px");
                            }
                        }
                        else {
                            if (type == EntityTypes.Kisi) {
                                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id).css("height", "1300px");
                            }
                            else {
                                $("#ifrmContent", parent.document).attr("src", "editaccount.html?accountid=" + id).css("height", "1300px");
                            }
                        }
                    });
                });
            }
        }
    };

    $scope.createActivity = function (id, name, type) {
        if (type == EntityTypes.Kisi) {
            parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + id + "&name=" + encodeURI(name) + "' style='width:100%;height:450px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
        }
        else {
            parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?accountid=" + id + "&name=" + encodeURI(name) + "' style='width:100%;height:450px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
        }

    }

    $scope.newContact = function () {
        parent.IndexHelper.ShowDialog("<iframe src='createcontact.html' style='width:100%;height:750px;border:none;' scrolling='no'></iframe>", "<i class='icon-user'></i> Yeni Kişi ");
    }

    $scope.newAccount = function () {
        parent.IndexHelper.ShowDialog("<iframe src='createaccount.html' style='width:100%;height:500px;border:none;' scrolling='no'></iframe>", "<i class='icon-user'></i> Yeni Firma ");
    }
}
