/// <reference path="global.js" />
/// <reference path="index.js" />


var ContactSearchHelper = {
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

        $(".page-title", parent.document).html("Müşteri Arama");
        $("#btnBreadPage", parent.document).html("Müşteri Arama");

        $("#liSearch", parent.document).addClass("active");
        $("#liHome", parent.document).removeClass("active");
        $("#liHouseSearch", parent.document).removeClass("active");

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
    "ContactHasCallCenterCall": function (contactId,callbackFunction) {
        var jData = {};
        jData.contactId = contactId;
        var jSonData = JSON.stringify(jData);

        $.ajax({
            url: CustomServiceUrl + "/ContactHasCallCenterCall",
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
    "CreateActivityForCallCenter": function (contactId, callbackfunction) {
        var activity = {};
        activity.Direction = parent.IndexHelper.Direction == "false" ? DirectionCodes.Gelen : DirectionCodes.Giden;
        activity.ObjectTypeCode = 4210;
        activity.ActivityParty = {};
        activity.ActivityParty.LogicalName = "contact";
        activity.ActivityParty.Id = contactId;
        activity.Owner = {};
        activity.Owner.LogicalName = "systemuser";
        activity.Owner.Id = parent.IndexHelper.UserId;

        var jData = {};
        jData.activity = activity;
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
    $scope.makeSearch = function ($keyEvent) {
        if ($keyEvent == null || $keyEvent.which == 13) {
            
            ContactSearchHelper.MakeSearch(function (e) {
                $scope.$apply(function () {
                    $scope.isNewContact = true;
                    if (e.Success) {
                        $scope.searchResults = e.ReturnObject;
                        $scope.isVisible = true;

                        setTimeout(function () {
                            parent.IndexHelper.AutoResize("ifrmContent");
                        }, 200);

                    }
                    else {
                        $scope.isVisible = false;
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

    $scope.editContact = function (id) {
        if (parent.IndexHelper.UserType != UserTypes.Resepsiyonist && parent.IndexHelper.UserType != UserTypes.CallCenter) //Resepsiyonistler arama sayfası dışında herhangi bir sayfaya gidemez.
        {
            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id).css("height", "1300px");
        }
        else if (parent.IndexHelper.UserType == UserTypes.CallCenter) //CallCenter araması ise önceden açık bir call center tel. gör. var mı kontrol edilir.
        {
            if (parent.IndexHelper.ActivityId != null && parent.IndexHelper.ActivityId != "") //Eğer bu arama var olan bir tel. gör. ile geliyor direk yönlendirilir.
            {
                $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id + "&phonecallid=" + parent.IndexHelper.ActivityId).css("height", "1300px");
            }
            else {
                ContactSearchHelper.ContactHasCallCenterCall(id, function (e) {
                    $scope.$apply(function () {
                        if (e.Success) {
                            $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id + "&phonecallid=" + e.CrmId).css("height", "1300px");
                        }
                        else {
                            ContactSearchHelper.CreateActivityForCallCenter(id, function (createResult) {
                                if (createResult.Success) {
                                    $("#ifrmContent", parent.document).attr("src", "editcontact.html?contactid=" + id + "&phonecallid=" + createResult.CrmId).css("height", "3500px");
                                }
                                else {
                                    parent.IndexHelper.ToastrShow(false, createResult.Result, "Aktivite Oluşturma");
                                }
                            });
                        }
                    });
                });
            }
        }
    };

    $scope.createActivity = function (id, name) {
        parent.IndexHelper.ShowDialog("<iframe src='createactivity.html?contactid=" + id + "&name=" + encodeURI(name) + "' style='width:100%;height:350px;border:none;' scrolling='no'></iframe>", "<i class='icon-calendar'></i> Aktivite");
    }

    $scope.newContact = function () {
        parent.IndexHelper.ShowDialog("<iframe src='createcontact.html' style='width:100%;height:500px;border:none;' scrolling='no'></iframe>", "<i class='icon-user'></i> Yeni Kişi ");
    }
}
