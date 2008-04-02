<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.1"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:ddue="http://ddue.schemas.microsoft.com/authoring/2003/5">

<!--
// System  : Sandcastle Help File Builder Utilities
// File    : TocToWebContents.xsl
// Author  : Eric Woodruff
// Updated : 06/21/2007
// Note    : Copyright 2007, Eric Woodruff, All rights reserved
//
// This is used to generate the website table of content XML file.
//
-->

  <msxsl:script language="C#" implements-prefix="ddue">
    <msxsl:using namespace="System.Xml" />
    <msxsl:using namespace="System.Xml.XPath" />
    <![CDATA[
    // Get the title from the HTML file
    public static string getTitle(string fileName)
    {
        XPathDocument doc = new XPathDocument(fileName);
        XPathNavigator node = doc.CreateNavigator().SelectSingleNode("/html/head/title");

        if(node != null)
            return node.Value;

        return String.Empty;
    }

    // Create a unique ID for nodes with children
    public static string getGUID()
    {
        return Guid.NewGuid().ToString();
    }
  ]]>
  </msxsl:script>

  <xsl:output indent="yes" encoding="UTF-8" />

  <xsl:param name="html" select="string('Output/html')"/>

  <xsl:template match="/">
    <HelpTOC DTDVersion="1.0">
        <xsl:apply-templates select="/topics" />
    </HelpTOC>
  </xsl:template>

  <xsl:template match="topic">
    <HelpTOCNode>
      <xsl:if test="count(*) > 0">
        <xsl:attribute name="Id">
          <xsl:value-of select="ddue:getGUID()"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="@file">
          <xsl:attribute name="Title">
            <xsl:value-of select="ddue:getTitle(concat($html,'/', @file, '.htm'))"/>
          </xsl:attribute>
          <xsl:attribute name="Url">
            <xsl:value-of select="concat('html/',@file,'.htm')" />
          </xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="Title">
            <xsl:value-of select="@id" />
          </xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:apply-templates />
    </HelpTOCNode>
  </xsl:template>

</xsl:stylesheet>
