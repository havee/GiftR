﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SitesCreate.aspx.cs" Inherits="GiftR.WebUI.SitesCreate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crear Sitio</title>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="boxed-group">
        <div class="boxed-group-inner">
            <h3>
                Creacion de Sitio</h3>
            <dl class="form">
                <dt>
                    <asp:Label runat="server" ID="lblTitle">Titulo</asp:Label></dt>
                <dd>
                    <asp:TextBox CssClass="textbox" runat="server" ID="txtTitle"></asp:TextBox>
                </dd>
                <dt>
                    <asp:Label runat="server" ID="lblFlickrUsername">Usuario Flickr</asp:Label></dt>
                <dd>
                    <asp:TextBox runat="server" ID="txtFlickrUsername"></asp:TextBox>
                </dd>
                <dt>
                    <asp:Label runat="server" ID="lblEmail">Email</asp:Label></dt>
                <dd>
                    <asp:TextBox runat="server" ID="txtEmail"></asp:TextBox>
                </dd>
                <dt>
                    <asp:Label runat="server" ID="lblType">Tipo</asp:Label></dt>
                <dd>
                    <asp:TextBox runat="server" ID="txtType"></asp:TextBox>
                </dd>
                <dt>
                </dt>
                <dd>
                    <asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Text="Crear" />
                </dd>
            </dl>
        </div>
    </div>
    </form>
</body>
</html>
