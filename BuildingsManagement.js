// JavaScript source code
//SÝTE YÖNETÝMÝ JAVA SCRIPTS

function formOnLoad() {
    GetProductDetail();
    ChangeResidentType();
    Xrm.Page.getAttribute("new_project").setSubmitMode("always");
    Xrm.Page.getAttribute("new_block").setSubmitMode("always");
    Xrm.Page.getAttribute("new_generaltypeofhome").setSubmitMode("always");
    Xrm.Page.getAttribute("new_kat").setSubmitMode("always");
    Xrm.Page.getAttribute("new_ruhsatno").setSubmitMode("always");
    Xrm.Page.getAttribute("statuscode").setSubmitMode("always");
    Xrm.Page.getAttribute("new_homenumber").setSubmitMode("always");
    Xrm.Page.getAttribute("new_m2").setSubmitMode("always");
    Xrm.Page.getAttribute("new_name").setSubmitMode("always");
}
function formOnSave(context) {
    var outDate = Xrm.Page.getAttribute("new_outdate").getValue();
    var entryDate = Xrm.Page.getAttribute("new_entrydate").getValue();

    if (outDate != null && entryDate == null) {
        alert("Giriþ tarihini giriniz!!!");
        Xrm.Page.getControl("new_entrydate").setFocus(true);
        context.getEventArgs().preventDefault();
        return false;
    }
    if (outDate != null && entryDate != null && outDate < entryDate) {
        alert("Çýkýþ tarihi Giriþ tarihininden önce olamaz!!!");
        Xrm.Page.getControl("new_outdate").setFocus(true);
        context.getEventArgs().preventDefault();
        return false;
    }
    CheckRecord(context);
}
function ChangeOutDate() {
    var outDate = Xrm.Page.getAttribute("new_outdate").getValue();
    if (outDate != null)
        Xrm.Page.getAttribute("statuscode").setValue(100000000);
    else
        Xrm.Page.getAttribute("statuscode").setValue(1);


}


function ChangeResidentType() {
    var residentType = Xrm.Page.getAttribute("new_residenttype").getValue();
    if (residentType != null && residentType == 1) {
        Xrm.Page.getAttribute("new_entrydate").setRequiredLevel("required");
        Xrm.Page.getControl("new_mobilephone").setVisible(true);
        Xrm.Page.getControl("new_emailaddress").setVisible(true);
    }
    else {
        Xrm.Page.getAttribute("new_entrydate").setRequiredLevel("none");
        Xrm.Page.getAttribute("new_mobilephone").setValue(null);
        Xrm.Page.getAttribute("new_emailaddress").setValue(null);
        Xrm.Page.getControl("new_mobilephone").setVisible(false);
        Xrm.Page.getControl("new_emailaddress").setVisible(false);

    }
}


function GetProductDetail() {
    var productId = Xrm.Page.getAttribute("new_productid").getValue();
    if (productId != null) {
        var Id = productId[0].id;
        Id = Id.replace("{", "").replace("}", "");
        var project = new Array();
        var blok = new Array();
        var generalTypeOfHome = new Array();
        var floorNumber = null;
        var licenceNumber = null;
        var homeNumber = null;
        var m2 = null;
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/ProductSet?$filter=ProductId eq (guid'" + Id + "')&$select=new_projectid,new_blockid,new_generaltypeofhomeid,new_floornumber,new_licencenumber,new_homenumber,new_netm2",
            data: null,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                if (data.d.results.length > 0) {
                    if (data.d.results[0].new_projectid != null) {
                        project[0] = data.d.results[0].new_projectid;
                    }
                    if (data.d.results[0].new_blockid != null) {
                        blok[0] = data.d.results[0].new_blockid;
                    }
                    if (data.d.results[0].new_generaltypeofhomeid != null) {
                        generalTypeOfHome[0] = data.d.results[0].new_generaltypeofhomeid;
                    }
                    if (data.d.results[0].new_floornumber != null) {
                        floorNumber = data.d.results[0].new_floornumber;
                    }
                    if (data.d.results[0].new_licencenumber != null) {
                        licenceNumber = data.d.results[0].new_licencenumber;
                    }
                    if (data.d.results[0].new_homenumber != null) {
                        homeNumber = data.d.results[0].new_homenumber;
                    }
                    if (data.d.results[0].new_netm2 != null) {
                        m2 = data.d.results[0].new_netm2;
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("GetProductById Error!!!");

            }
        });
        if (project != null) {
            Xrm.Page.getAttribute("new_project").setValue([{ id: project[0].Id, name: project[0].Name, entityType: project[0].LogicalName }]);
        }
        else {
            Xrm.Page.getAttribute("new_project").setValue(null);
        }
        if (blok != null) {
            Xrm.Page.getAttribute("new_block").setValue([{ id: blok[0].Id, name: blok[0].Name, entityType: blok[0].LogicalName }]);

        }
        else {
            Xrm.Page.getAttribute("new_block").setValue(null);
        }
        if (generalTypeOfHome != null) {
            Xrm.Page.getAttribute("new_generaltypeofhome").setValue([{ id: generalTypeOfHome[0].Id, name: generalTypeOfHome[0].Name, entityType: generalTypeOfHome[0].LogicalName }]);
        }
        else {
            Xrm.Page.getAttribute("new_generaltypeofhome").setValue(null);
        }
        if (floorNumber != null) {
            Xrm.Page.getAttribute("new_kat").setValue(floorNumber);
        }
        else {
            Xrm.Page.getAttribute("new_kat").setValue(null);
        }
        if (licenceNumber != null) {
            Xrm.Page.getAttribute("new_ruhsatno").setValue(licenceNumber);
        }
        else {
            Xrm.Page.getAttribute("new_ruhsatno").setValue(null);
        }
        if (homeNumber != null) {
            Xrm.Page.getAttribute("new_homenumber").setValue(homeNumber);
        }
        else {
            Xrm.Page.getAttribute("new_homenumber").setValue(null);
        }
        if (m2 != null) {
            Xrm.Page.getAttribute("new_m2").setValue(parseFloat(m2).toFixed(2));
        }
        else {
            Xrm.Page.getAttribute("new_m2").setValue(null);
        }


    }
}
function CheckRecord(context) {
    var result = false;
    var productId = Xrm.Page.getAttribute("new_productid").getValue();
    var currentId = Xrm.Page.data.entity.getId();
    if (currentId=="") {
        currentId = "00000000-0000-0000-0000-000000000000";
    }
    else {
        currentId = currentId.replace("{", "").replace("}", "");
    }
    
    if (productId != null) {
        var Id = productId[0].id;
        Id = Id.replace("{", "").replace("}", "");
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/new_buildingsmanagementSet?$filter=new_productid/Id eq guid'" + Id + "' and new_buildingsmanagementId ne guid'" + currentId + "'  and new_outdate eq null &$select=new_buildingsmanagementId",
            data: null,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                if (data.d.results.length > 0) {
                    alert("Bu konuta Baðlý Çýkýþ Yapýlmamýþ Kayýt vardýr!!!");
                    context.getEventArgs().preventDefault();
                    return false;

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("CheckRecord Error!!!");

            }
        });
    }
    return result;
}