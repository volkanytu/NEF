<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="NEF.Web.ProductImport.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NEF KONUT IMPORT</title>
    <link href="css/style.css" rel="stylesheet" />
    <link href="css/modern-responsive.css" rel="stylesheet" />
    <link href="css/modern.css" rel="stylesheet" />
    <link href="css/site.css" rel="stylesheet" />


    <script src="scripts/jquery-1.8.2.min.js"></script>

    <script type="text/javascript">

        var reader = new FileReader();

        $(document).ready(function () {
            //debugger;

            $("#file").hide();
            $("#btn_file").click(function () {
                $("#file").click();
            });

            $("#file").change(function () {
                //debugger;
                var file = this.files[0];
                $("#txt_file").val(file.name);

                $("#btn_import").show();
                $("#lbl_message").text("");
            });
        });

        function importResources() {

            $("#lbl_message").hide();
            $("#lbl_message").text("");

            //debugger;
            var filepath = $("#txt_file").val();
            if (filepath != null && filepath != "") {
                var file = $("#file")[0].files[0];
                SendFileToCRM(file);
            }
            else {
                $("#lbl_message").show();
                $("#lbl_message").text("Lütfen import için bir dosya seçiniz...");
            }
        }

        function SendFileToCRM(file) {
            $("#lbl_message").text("");

            reader.onloadend = function (e) {
                var xhr, provider;

                xhr = jQuery.ajaxSettings.xhr();
                if (xhr.upload) {
                    xhr.upload.addEventListener('progress', function (e) {
                        // ...
                    }, false);
                }
                provider = function () {
                    return xhr;
                };

                var data = e.target.result;
                data = data.substr(data.indexOf('base64') + 7);

                if (file.name.indexOf(".csv") != -1) {
                    $.ajax({
                        type: 'POST',
                        url: 'process.ashx',
                        xhr: provider,
                        dataType: 'json',
                        beforeSend: function () {
                            $("#btn_import").hide();
                            $("#process").show();
                        },
                        complete: function () {
                            $("#process").hide();
                        },
                        success: function (data) {
                            debugger;
                            if (data != null) {

                                if (data.Success == true) {

                                    $("#lbl_message").show();
                                    $("#lbl_message").html(data.Result);

                                    $("#btn_reimport").show();
                                }
                                else {
                                    //$("#btn_import").show();

                                    $("#lbl_message").show();
                                    $("#lbl_message").html(data.Result);
                                }
                            }
                        },
                        error: function (a, b, c) {
                            //$("#btn_import").show();

                            $("#lbl_message").show();
                            $("#lbl_message").html("İşlem sırasında hata oluştu..." + c + " <br>Lütfen tekrar deneyin.");
                        },
                        data: {
                            name: file.name,
                            size: null,
                            type: file.type,
                            data: data
                        }
                    });
                }
                else {
                    //$("#btn_import").show();

                    $("#lbl_message").show();
                    $("#lbl_message").html("Dosya formatı CSV olmalıdır... <br>Lütfen tekrar deneyin.");
                }
            };
            reader.readAsDataURL(file);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="import">
            <div class="importlogo">
                <h2>NEF Konut Import</h2>
                <p class="tertiary-info-text">Şablon örneğini indirmek için <a href="konutimportsablon.csv">Tıklayınız</a></p>
                <br />
            </div>
            <div style="margin-top: 0px;">
                <div id="Div1">
                    <input id="btn_file" type="button" value="Import için dosya seçiniz..." class="bg-color-blue fg-color-white" />
                </div>
            </div>
            <div>
                <div class="input-control text disabled" style="width: 300px;">
                    <input type="text" disabled="disabled" placeholder="Dosya yolu" id="txt_file" />
                    <span class="helper"></span>
                </div>
            </div>

            <div class="space"></div>

            <div class="space"></div>

            <div id="operation">
                <input id="btn_import" type="button" value="Konut Import" class="bg-color-yellow fg-color-black" onclick="importResources();" style="display: none;" />
                <input id='btn_reimport' type='button' value='Yeni Konut Import' class='bg-color-yellow fg-color-black' onclick='location.reload();' style="display: none;" />
                <div id="process" style="display: none;">Lütfen Bekleyiniz &nbsp;&nbsp;<img id="img_process" src="images/dots32.gif" /></div>
            </div>
            <div id="lbl_message" style="display: none;"></div>
        </div>
        <div>
            <input type="file" id="file" />
        </div>
        ​​​​​​​​​​​​​​​​​​​​​
    </form>
</body>
</html>
