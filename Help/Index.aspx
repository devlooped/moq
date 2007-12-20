<%@ Page Language="C#" EnableViewState="False" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>

<head>
<title>Table of Content</title>
<link rel="stylesheet" href="TOC.css">
<script type="text/javascript" src="TOC.js"></script>
</head>

<body onload="javascript: Initialize();" onresize="javascript: ResizeTree();">
<form id="IndexForm" runat="server">

<div id="TOCDiv" class="TOCDiv">

<div id="divSearchOpts" class="SearchOpts" style="height: 100px; display: none;">
<img class="TOCLink" onclick="javascript: ShowHideSearch(false);"
    src="CloseSearch.png" height="17" width="17" alt="Hide search" style="float: right;"/>
Keyword(s) for which to search:
<input id="txtSearchText" type="text" style="width: 100%;"
  onkeypress="javascript: return OnSearchTextKeyPress(event);" /><br />
<input id="chkSortByTitle" type="checkbox" /><label for="chkSortByTitle">&nbsp;Sort results by title</label><br />
<input type="button" value="Search" onclick="javascript: return PerformSearch();" />
</div>

<div id="divNavOpts" class="NavOpts" style="height: 20px;">
    <img class="TOCLink" onclick="javascript: SyncTOC();" src="SyncTOC.gif"
        height="16" width="16" alt="Sync to TOC"/>
    <img class="TOCLink" onclick="javascript: ExpandOrCollapseAll(true);"
        src="ExpandAll.bmp" height="16" width="16" alt="Expand all "/>
    <img class="TOCLink" onclick="javascript: ExpandOrCollapseAll(false);"
        src="CollapseAll.bmp" height="16" width="16" alt="Collapse all" />
    <img class="TOCLink" onclick="javascript: ShowHideSearch(true);"
        src="Search.gif" height="16" width="16" alt="Search" />
</div>

<div class="Tree" id="divSearchResults" style="display: none;"
    onselectstart="javascript: return false;">
</div>

<div class="Tree" id="divTree" onselectstart="javascript: return false;">
<asp:Literal ID="lcTOC" runat="server" />
</div>

</div>

<div id="TOCSizer" class="TOCSizer" onmousedown="OnMouseDown(event)" onselectstart="javascript: return false;"></div>

<iframe id="TopicContent" name="TopicContent" class="TopicContent" src="html/dd9bb368-2453-98f9-307a-d54845fd190b.htm">
This page uses an IFRAME but your browser does not support it.
</iframe>

</form>

</body>

</html>

<script runat="server">
//=============================================================================
// System  : Sandcastle Help File Builder
// File    : Index.aspx
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 09/14/2007
// Note    : Copyright 2007, Eric Woodruff, All rights reserved
// Compiler: Microsoft C#
//
// This file contains the code used to display the index page for a website
// produced by the help file builder.  The root nodes are loaded for the table
// of content.  Child nodes are loaded dynamically when first expanded using
// an Ajax call.
//
// This code may be used in compiled form in any way you desire.  This file
// may be redistributed unmodified by any means PROVIDING it is not sold
// for profit without the author's written consent.  This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// This code is provided "as is" with no warranty either express or implied.
// The author accepts no liability for any damage or loss of business that
// this product may cause.
//
// Version     Date     Who  Comments
// ============================================================================
// 1.5.0.0  06/21/2007  EFW  Created the code
//=============================================================================

protected void Page_Load(object sender, EventArgs e)
{
    StringBuilder sb = new StringBuilder(10240);
    string id, url, target, title;

    XmlDocument toc = new XmlDocument();
    toc.Load(Server.MapPath("WebTOC.xml"));
    XmlNode root = toc.SelectSingleNode("//HelpTOC");

    foreach(XmlNode node in root.ChildNodes)
    {
        if(node.ChildNodes.Count != 0)
        {
            // Write out a parent TOC entry
            if(node.Attributes["Url"] == null)
            {
                id = node.Attributes["Id"].Value;
                title = node.Attributes["Title"].Value;
                url = "#";
                target = String.Empty;
            }
            else
            {
                id = node.Attributes["Id"].Value;
                title = node.Attributes["Title"].Value;
                url = node.Attributes["Url"].Value;
                target = " target=\"TopicContent\"";
            }

            sb.AppendFormat("<div class=\"TreeNode\">\r\n" +
                "<img class=\"TreeNodeImg\" " +
                "onclick=\"javascript: Toggle(this);\" " +
                "src=\"Collapsed.gif\"/><a class=\"UnselectedNode\" " +
                "onclick=\"javascript: return Expand(this);\" " +
                "href=\"{0}\"{1}>{2}</a>\r\n" +
                "<div id=\"{3}\" class=\"Hidden\"></div>\r\n</div>\r\n",
                url, target, HttpUtility.HtmlEncode(title), id);
        }
        else
        {
            title = node.Attributes["Title"].Value;

            if(node.Attributes["Url"] != null)
                url = node.Attributes["Url"].Value.Replace('\\', '/');
            else
                url = "about:blank";

            // Write out a TOC entry that has no children
            sb.AppendFormat("<div class=\"TreeItem\">\r\n" +
                "<img src=\"Item.gif\"/>" +
                "<a class=\"UnselectedNode\" " +
                "onclick=\"javascript: return SelectNode(this);\" " +
                "href=\"{0}\" target=\"TopicContent\">{1}</a>\r\n" +
                "</div>\r\n", url, HttpUtility.HtmlEncode(title));
        }
    }

    lcTOC.Text = sb.ToString();
}
</script>
