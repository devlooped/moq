<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="ProductsMvcSample.Views.Home.Welcome" Title="Untitled Page" %>
<%@ Import Namespace="ProductsMvcSample.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

<ul>
	<li><%= Html.ActionLink<ProductsController>(c => c.Category(1), "View Category 1") %></li>
	<li><%= Html.ActionLink<ProductsController>(c => c.Category(2), "View Category 2") %></li>
</ul>	
</asp:Content>
