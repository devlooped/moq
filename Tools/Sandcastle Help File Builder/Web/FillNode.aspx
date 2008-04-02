<%@ Page Language="C#" EnableViewState="False" %>

<script runat="server">
//=============================================================================
// System  : Sandcastle Help File Builder
// File    : FillNode.aspx
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 06/21/2007
// Note    : Copyright 2007, Eric Woodruff, All rights reserved
// Compiler: Microsoft C#
//
// This file contains the code used to dynamically load a parent node with its
// child table of content nodes when first expanded.
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

protected override void Render(HtmlTextWriter writer)
{
    StringBuilder sb = new StringBuilder(10240);
    string id, url, target, title;

    XmlDocument toc = new XmlDocument();
    toc.Load(Server.MapPath("WebTOC.xml"));

    // The ID to use should be passed in the query string
    XmlNode root = toc.SelectSingleNode("//HelpTOCNode[@Id='" +
        this.Request.QueryString["Id"] + "']");

    if(root == null)
    {
        writer.Write("<b>TOC node not found!</b>");
        return;
    }

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
            url = node.Attributes["Url"].Value;

            // Write out a TOC entry that has no children
            sb.AppendFormat("<div class=\"TreeItem\">\r\n" +
                "<img src=\"Item.gif\"/>" +
                "<a class=\"UnselectedNode\" " +
                "onclick=\"javascript: return SelectNode(this);\" " +
                "href=\"{0}\" target=\"TopicContent\">{1}</a>\r\n" +
                "</div>\r\n", url, HttpUtility.HtmlEncode(title));
        }
    }

    writer.Write(sb.ToString());
}
</script>
