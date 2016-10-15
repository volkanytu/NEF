// JavaScript source code
function formOnSave() {
    var quoteIdName = Xrm.Page.getAttribute("new_quoteid").getValue();
    if (quoteIdName != null) {
        quoteIdName = Xrm.Page.getAttribute("new_quoteid").getValue()[0].name;
    }
    var now = new Date();
    now = now.format("dd.MM.yyyy hh:mm");
    Xrm.Page.getAttribute("new_name").setValue("Ýptal - " + quoteIdName + " - " + now);
    Xrm.Page.getAttribute("new_name").setSubmitMode("always");
    var cancelReason = Xrm.Page.getAttribute("new_canceledreasonid").getValue();
    if (cancelReason != null) {
        cancelReason = cancelReason[0].name;
    }
    var customer = {};
    if (Xrm.Page.getAttribute("new_trasnferredcontactid").getValue()!=null) {
        customer.Id = Xrm.Page.getAttribute("new_trasnferredcontactid").getValue()[0].id;
        customer.Name = Xrm.Page.getAttribute("new_trasnferredcontactid").getValue()[0].name;
        customer.LogicalName = Xrm.Page.getAttribute("new_trasnferredcontactid").getValue()[0].entityType;
    }
    else if (Xrm.Page.getAttribute("new_transferredaccountid").getValue() != null) {
        customer.Id = Xrm.Page.getAttribute("new_transferredaccountid").getValue()[0].id;
        customer.Name = Xrm.Page.getAttribute("new_transferredaccountid").getValue()[0].name;
        customer.LogicalName = Xrm.Page.getAttribute("new_transferredaccountid").getValue()[0].entityType;
    }
    
    
    var url = $.url(window.top.opener.location.href);
    if (url.data.param.query.hasOwnProperty("etc")) {
        var etc = url.param("etc");
        if (etc == "1084") {
            window.top.opener.CreateReason(cancelReason);
        }
    }
}