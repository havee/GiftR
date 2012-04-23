<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MySites.aspx.cs" Inherits="GiftR.WebUI.MySites" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="grid">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="pnlForm" class="boxed-group-inner">
            <ContentTemplate>
                <asp:Repeater runat="server" ID="rptSites">
                    <HeaderTemplate>
                        <table class="repo">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="title">
                                <%# Eval("title").ToString().Trim() %>
                            </td>
                            <td rowspan="2">
                                <asp:Button runat="server" ID="btnDelete" class="minibutton" Text="Eliminar" OnCommand="btnDelete_Command"
                                    CommandArgument='<%# Eval("id") %>' />
                            </td>
                        </tr>
                        <tr class="desc">
                            <td>
                                <a href="<%# GetSiteUrl(Eval("verification_code").ToString()).Trim() %>">
                                    <%# GetSiteUrl(Eval("verification_code").ToString()).Trim() %></a>
                            </td>
                        </tr>
                        <tr class="sep">
                            <td class="border" colspan="2">
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
            <Triggers>
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
