// JavaScript source code
var SystemAdminRoleId;
var MFTRoleId;
var NEFSatisDestekRoleId;
var NEFSatisDanismaniRoleId;
var NEFIsGYOSatisDanismaniRoleId;
var NEFIsGYOSatisDestekRoleId;
var NEFSatisDirektoruRoleId;
var NEFSatisMuduruRoleId;
var TopkapiProjectId;
var NEFSalesCollaboraterateId;

var orgName = Xrm.Page.context.getOrgUniqueName();
if (orgName == "PiLOT") {
    SystemAdminRoleId = "604A461F-A6A7-E411-80C5-005056A62B8A";
    MFTRoleId = "5AC7CBF2-A99C-E411-80C2-005056A62B8A";
    NEFSatisDestekRoleId = "364AF22D-932C-4C72-B0B0-ED265229CC17";
    NEFSatisDanismaniRoleId = "885AA8DB-F78F-E411-80C1-005056A62B8A";
    NEFIsGYOSatisDanismaniRoleId = "2C9BC599-DD2C-4AF0-957D-891B062C0870";
    NEFIsGYOSatisDestekRoleId = "61229A73-5400-4F90-B499-1D74745ADD94";
    NEFSatisDirektoruRoleId = "0868ECB7-AE9C-E411-80C2-005056A62B8A";
    NEFSatisMuduruRoleId = "D53EC428-798B-E411-80E8-005056B0203A";
    TopkapiProjectId = "DDBD958D-46BB-E411-80C9-005056A60603";
    NEFSalesCollaboraterateId = "0381190A-C6B1-E411-80C7-005056A60603";
}
else {
    SystemAdminRoleId = "7C5ACBFA-5997-E411-80C0-005056A60603";
    MFTRoleId = "F5C24121-78FC-49B5-947B-F46D5991323A";
    NEFSatisDestekRoleId = "364AF22D-932C-4C72-B0B0-ED265229CC17";
    NEFSatisDanismaniRoleId = "885AA8DB-F78F-E411-80C1-005056A62B8A";
    NEFIsGYOSatisDanismaniRoleId = "2C9BC599-DD2C-4AF0-957D-891B062C0870";
    NEFIsGYOSatisDestekRoleId = "CC92E1EE-4F0F-4FBB-A0F6-5F6823BF6A26";
    NEFSatisDirektoruRoleId = "0868ECB7-AE9C-E411-80C2-005056A62B8A";
    NEFSatisMuduruRoleId = "D53EC428-798B-E411-80E8-005056B0203A";
    TopkapiProjectId = "DDBD958D-46BB-E411-80C9-005056A60603";
    NEFSalesCollaboraterateId = "0381190A-C6B1-E411-80C7-005056A60603";
}

function checkEmail(emailaddr) {
    email = emailaddr;
    var regexObj = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (regexObj.test(email)) {
        return email;
    }
    else {
        alert("Lütfen geçerli bir e-posta adresi giriniz");
        return email;
    }
}

function phoneFormat(phonenumber) {
    var regexObj = /^(?:\+?1[-. ]?)?(?:\(?([0-9]{3})\)?[-. ]?)?([0-9]{3})[-. ]?([0-9]{4})$/;
    if (regexObj.test(phonenumber)) {

        var parts = phonenumber.match(regexObj);
        var phone = "";
        if (parts[1]) { phone += "+90 (" + parts[1] + ") "; }
        phone += parts[2] + "-" + parts[3];
        return phone;
    }
    else {
        return phonenumber;
    }
}