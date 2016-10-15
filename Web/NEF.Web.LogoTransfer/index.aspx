<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="NEF.Web.LogoTransfer.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/jquery.vegas.css" rel="stylesheet" />
    <link href="font-awesome/css/font-awesome.css" rel="stylesheet" />

    <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="js/bootstrap.min.js"></script>
    <script src="js/jquery.vegas.js"></script>

    <title>Logo Transfer</title>

    <script type="text/javascript">

        $(function () {


        });

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row" style="margin-top: 30px;">
                <div class="col-md-6">
                    <div class="form-group">
                        <label for="exampleInputEmail1">Satış No</label>
                        <input type="text" class="form-control input-lg" id="txtSalesNo" runat="server" placeholder="Satış No" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label for="exampleInputEmail1">Satış Durumu</label>
                        <%--<select class="form-control input-lg" id="slctType" onchange="javascript:form1.submit();" runat="server">--%>
                        <select class="form-control input-lg" id="slctType" runat="server">
                            <option value="0">---</option>
                            <option value="6">İptal Aktarıldı</option>
                            <option value="100000001">Muhasebe AKtarıldı</option>
                        </select>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <button type="button" class="btn btn-primary btn-lg pull-right" runat="server" id="btnSave" onserverclick="btnSave_ServerClick">Kaydet</button>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <h3 style="color: red;" id="lblError" runat="server">Error</h3>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
