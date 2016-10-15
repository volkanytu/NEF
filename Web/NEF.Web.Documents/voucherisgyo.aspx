<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="voucherisgyo.aspx.cs" Inherits="NEF.Web.Documents.voucherisgyo" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        body, html, form
        {
            margin: 0px;
            width: 100%;
            height: 100%;
        }
        span
        {
            font-family: Arial;
            font-size: 10pt;
        }
        label
        {
            font-size: 9pt;
            font-family: Arial;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divUyari" runat="server" align="center" valign="middle" style="width: 100%;
        height: 100%">
        <table style="width: 100%; height: 100%">
            <tr>
                <td align="center" valign="middle">
                    <table style="width: 500px; height: 200px; background-color: #D6E8FF; border-color: #6893CF;
                        border-style: solid; border-width: 2px;">
                        <tr>
                            <td align="center">
                                <asp:Label ID="lblUyari" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>