<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="ProductsMvcSample.Views.Home.Welcome" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

<ul>
	<li><%= Html.ActionLink("View Category 1", new { controller = "Products" , action = "Category" , id = 1 })%></li>
	<li><%= Html.ActionLink("View Category 2", new { controller = "Products" , action = "Category" , id = 2 })%></li>
</ul>	
</asp:Content>
