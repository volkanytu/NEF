var CommonServiceUrl = "http://35374/Common.svc";
var CommonServiceUrl = "http://fenixcrm.nef.com.tr:9091/CommonServices/Common.svc";
var GuidEmpty = '00000000-0000-0000-0000-000000000000';

var ContactTypes = {
    "Aday": 100000000,
    "Gercek": 100000001,
    "Potansiyel": 100000002,
    "": 0
};

var UserTypes = {
    "SatisMuduru": 1,
    "SatisDanismani": 2,
    "CallCenter": 3,
    "Resepsiyonist": 4
};

var DirectionCodes = {
    "Gelen": 0,
    "Giden": 1
}

var QuoteStatuses =
{
    "DevamEdiyor" : 1,
    "SözleşmeImzalandi" : 2,
    "KaporaAlindi" : 3,
    "Kazanildi" : 4,
    "Kaybedildi" : 5,
    "İptalEdildi" : 6,
    "Düzeltilmis" : 7,
    "IptalAktarildi" : 100000000,
    "MuhasebeyeAktarildi" : 100000001,
    "TeslimEdildi" : 100000002,
    "BittiSatildi" : 100000003,
    "OnayBekleniyor" : 100000004,
    "Onaylandi" : 100000005,
    "Reddedildi" : 100000006
}

var GlobalHelper = {
    "ContactTypes": {
        100000000: "Aday",
        100000001: "Gerçek",
        100000002: "Potansiyel",
        0: "Aday",
        "Aday": 100000000,
        "Gercek": 100000001,
        "Potansiyel": 100000002
    },
    "ContactTypeClass": {
        100000000: "warning",
        100000001: "success",
        100000002: "danger",
        0: "warning",
    },
    "ActivityStates": {
        0: "Açık",
        1: "Tamamlandı",
        2: "İptal Edildi",
        3: "Zamanlanmış"
    },
    "ActivityStateClass": {
        0: "warning",
        1: "success",
        2: "danger",
        3: "info"
    },
    "GuidEmpty": "00000000-0000-0000-0000-000000000000",
    "ShowTextAlert": function (containerElement, appendORprepend, alertType, message, isClosable, isResetPrevious, isFocus, closeSecond, icon) {
        //AlerType:success, danger, warning, info
        //CloseSecond 0 ise NeverClose
        Metronic.alert({
            container: containerElement,
            place: appendORprepend,
            type: alertType,
            message: message,
            close: isClosable,
            reset: isResetPrevious,
            focus: isFocus,
            closeInSeconds: closeSecond,
            icon: icon
        });

    },
    "QuoteStatusClasses": {
        1: "bg-yellow",
        2: "bg-green-jungle",
        3: "bg-yellow-gold",
        4: "bg-blue",
        5: "bg-red",
        6: "bg-red",
        7: "bg-red-intense",
        100000000: "bg-red-pink",
        100000001: "bg-purple",
        100000002: "bg-green",
        100000003: "bg-green-meadow"
    }
}