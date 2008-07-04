<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ProductsList.aspx.cs" Inherits="ProductsMvcSample.Views.Products.ProductsList" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

<h1><%= ViewData.CategoryName %></h1>

<ul>
<% foreach (var product in ViewData.Products)
   { %>
	<li><%= product.Name %></li>	   
<% } %>
</ul>

</asp:Content>
