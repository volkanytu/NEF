
function formOnLoad() {
    Xrm.Page.getControl("new_referancecontact").setVisible(false);
    Xrm.Page.getControl("new_referancesales").setVisible(false);
    Xrm.Page.getControl("new_customer").setVisible(false);
    Xrm.Page.getControl("new_accountid").setVisible(false);
    GetCustomerOnSales();
    DiscountTypeChange();
    
}

function DiscountTypeChange() {
    var discountType = Xrm.Page.getAttribute("new_discounttype").getValue();

    if (discountType != null) {
        if (discountType == 100000000) { //Nef aile indirimi
            Xrm.Page.getControl("new_referancecontact").setVisible(true);
            Xrm.Page.getControl("new_referancesales").setVisible(true);
        } else {
            Xrm.Page.getControl("new_referancecontact").setVisible(false);
            Xrm.Page.getControl("new_referancesales").setVisible(false);
        }
    }
}

function GetCustomerOnSales() {
    var quoteId = Xrm.Page.getAttribute("new_sales").getValue();
    if (quoteId != null) {
        quoteId = Xrm.Page.getAttribute("new_sales").getValue()[0].id;
        quoteId = quoteId.replace("{", "").replace("}", "");

        var referenceSalesId = Xrm.Page.getAttribute("new_referancesales").getValue();
        if (referenceSalesId != null) {
            referenceSalesId = Xrm.Page.getAttribute("new_referancesales").getValue()[0].id;
            referenceSalesId = referenceSalesId.replace("{", "").replace("}", "");
        }
        
        if (quoteId == referenceSalesId) {
            Xrm.Page.getAttribute("new_referancesales").setValue(null);
        }

        var sales = new Array();
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: Xrm.Page.context.getClientUrl() + "/XRMServices/2011/OrganizationData.svc/QuoteSet(guid'" + quoteId + "')",
            data: null,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                if (data.d != null) {
                    sales = data.d;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("GetCustomerOnSales Service Error!!!");
            }
        });

        if (sales != null) {
            var customerId = sales.CustomerId;
            Xrm.Page.getAttribute("new_name").setValue(sales.Name);
            if (customerId.LogicalName == "contact") {
                Xrm.Page.getAttribute("new_customer").setValue([{ id: customerId.Id, name: customerId.Name, entityType: customerId.LogicalName }]);
                Xrm.Page.getAttribute("new_customer").setRequiredLevel("required");
                Xrm.Page.getControl("new_customer").setVisible(true);

            } else if (customerId.LogicalName == "account") {
                Xrm.Page.getAttribute("new_accountid").setValue([{ id: customerId.Id, name: customerId.Name, entityType: customerId.LogicalName }]);
                Xrm.Page.getAttribute("new_accountid").setRequiredLevel("required");
                Xrm.Page.getControl("new_accountid").setVisible(true);

            } else {
                alert("Ýlgili satýþ verilerine ulaþýlamadý.");
            }
        }
    }
}
