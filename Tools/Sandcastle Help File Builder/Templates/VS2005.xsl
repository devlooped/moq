<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.1" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
    xmlns:mshelp="http://msdn.microsoft.com/mshelp"
    xmlns:ddue="http://ddue.schemas.microsoft.com/authoring/2003/5"
    xmlns:xlink="http://www.w3.org/1999/xlink"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt">

<!--
// System  : Sandcastle Help File Builder Utilities
// File    : VS2005.xsl
// Author  : Eric Woodruff
// Updated : 10/19/2007
// Note    : Copyright 2007, Eric Woodruff, All rights reserved
//
// This is used to convert *.topic additional content files into *.html files
// that have the same appearance as API topics using the VS2005 presentation
// style.
-->

  <xsl:output method="xml" omit-xml-declaration="yes" encoding="utf-8" />

  <!-- This parameter, if specified, defines the path to the root folder -->
  <xsl:param name="pathToRoot" select="string('')" />

  <!-- Main template for the topic -->
  <xsl:template match="/topic">
<html>
<head>
<title><xsl:value-of select="title"/></title>
<link rel="stylesheet" type="text/css" href="{$pathToRoot}styles/presentation.css" />
<link rel="stylesheet" type="text/css" href="ms-help://Hx/HxRuntime/HxLink.css" />
<script type="text/javascript" src="{$pathToRoot}scripts/EventUtilities.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/SplitScreen.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/Dropdown.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/script_loc.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/script_manifold.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/script_feedBack.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/CheckboxMenu.js"> </script>
<script type="text/javascript" src="{$pathToRoot}scripts/CommonUtilities.js"> </script>
<META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=UTF-8" />
<META NAME="save" CONTENT="history" />

<!-- Stylesheet and script for colorized code blocks -->
<link type="text/css" rel="stylesheet" href="styles/highlight.css" />
<script type="text/javascript" src="scripts/highlight.js"></script>

<!-- Include the XML data island for HTML Help 2.0 if present -->
<xsl:if test="xml">
    <xsl:copy-of select="xml"/>
</xsl:if>

</head>

<body>
<input type="hidden" id="userDataCache" class="userDataStyle" />
<input type="hidden" id="hiddenScrollOffset" />
<img id="collapseImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/collapse_all.gif" alt="Collapse image" />
<img id="expandImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/expand_all.gif" alt="Expand Image" />
<img id="collapseAllImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/collapse_all.gif" />
<img id="expandAllImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/expand_all.gif" />
<img id="dropDownImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/dropdown.gif" />
<img id="dropDownHoverImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/dropdownHover.gif" />
<img id="copyImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/copycode.gif" alt="Copy image" />
<img id="copyHoverImage" style="display:none; height:0; width:0;" src="{$pathToRoot}icons/copycodeHighlight.gif" alt="CopyHover image" />

<div id="header">
<table id="topTable" cellpadding="0" cellspacing="0">
  <!-- Include the logo if present -->
  <xsl:if test="logoFile">
      <xsl:apply-templates select="logoFile"/>
  </xsl:if>
  <tr id="headerTableRow1">
    <!-- The product title is replaced with the project's HTML encoded HelpTitle value -->
    <td align="left"><span id="runningHeaderText"><@HtmlEncHelpTitle/></span></td>
  </tr>
  <tr id="headerTableRow2">
    <td align="left"><span id="nsrTitle"><xsl:value-of select="title"/></span></td>
  </tr>
  <tr id="headerTableRow3">
    <td align="left"><span id="headfb" class="feedbackhead" /></td>
  </tr>
</table>
<table id="gradientTable">
  <tr>
    <td class="nsrBottom" background="{$pathToRoot}icons/gradient.gif" />
  </tr>
</table>
</div>

<div id="mainSection">
<div id="mainBody">
<div id="allHistory" class="saveHistory" onsave="saveAll()" onload="loadAll()" />

<!-- Process the body text -->
<xsl:apply-templates select="bodyText" />
</div>
</div>

<div id="footer">
<div class="footerLine"><img width="100%" height="3px" src="{$pathToRoot}icons/footer.gif" alt="Footer image"/></div>

<!-- This includes the footer item from the shared content -->
<include item="footer"/>
<include item="feedback_script_block"/>
</div>

</body>
</html>
  </xsl:template>

  <!-- Pass through html tags from the body -->
  <xsl:template match="p|ol|ul|li|dl|dt|dd|table|tr|th|td|h1|h2|h3|h4|h5|h6|hr|br|pre|blockquote|div|span|a|img|b|i|strong|em|del|sub|sup|abbr|acronym|u|font|link|script|code">
    <xsl:copy>
      <xsl:copy-of select="@*" />
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>

  <!-- Add the logo if defined -->
  <xsl:template match="logoFile">
    <!-- TODO: support placement and alignment -->
    <tr>
      <td rowspan="4" align="center" style="width: 1px; padding: 0px"><img>
        <xsl:attribute name="src">
          <xsl:value-of select="$pathToRoot"/>
          <xsl:value-of select="@filename"/>
        </xsl:attribute>
        <xsl:attribute name="altText">
          <xsl:value-of select="@altText"/>
        </xsl:attribute>
        <xsl:if test="@height">
          <xsl:attribute name="height">
            <xsl:value-of select="@height"/>
          </xsl:attribute>
        </xsl:if>
        <xsl:if test="@width">
          <xsl:attribute name="width">
            <xsl:value-of select="@width"/>
          </xsl:attribute>
        </xsl:if>
      </img></td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
