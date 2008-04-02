<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" 
				xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
        xmlns:mshelp="http://msdn.microsoft.com/mshelp"
				xmlns:ddue="http://ddue.schemas.microsoft.com/authoring/2003/5"
				xmlns:xlink="http://www.w3.org/1999/xlink"
        xmlns:msxsl="urn:schemas-microsoft-com:xslt"
        >

  <xsl:template name="autogenSeeAlsoLinks">

    <!-- a link to the containing type on all list and member topics -->
    <xsl:if test="$group='member' or $group='list'">
      <div class="seeAlsoStyle">
      <include item="SeeAlsoTypeLinkText">
        <parameter>
          <referenceLink target="{$typeId}" />
          <!--<xsl:value-of select="$typeName"/> -->
        </parameter>
        <parameter>
          <xsl:value-of select="/document/reference/containers/type/apidata/@subgroup"/>
        </parameter>
      </include>
      </div>
    </xsl:if>

    <!-- a link to the namespace topic -->
    <xsl:if test="normalize-space($namespaceId)">
      <div class="seeAlsoStyle">
      <include item="SeeAlsoNamespaceLinkText">
        <parameter>
          <referenceLink target="{$namespaceId}" />
          <!--<xsl:value-of select="/document/reference/containers/namespace/apidata/@name"/> -->
        </parameter>
      </include>
      </div>
    </xsl:if>

  </xsl:template>

  <xsl:variable name="typeId">
    <xsl:choose>
      <xsl:when test="/document/reference/topicdata[@group='api'] and /document/reference/apidata[@group='type']">
        <xsl:value-of select="$key"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="/document/reference/containers/type/@api"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>

  <xsl:variable name="namespaceId">
    <xsl:value-of select="/document/reference/containers/namespace/@api"/>
  </xsl:variable>

  <!-- indent by 2*n spaces -->
  <xsl:template name="indent">
    <xsl:param name="count" />
    <xsl:if test="$count &gt; 1">
      <xsl:text>&#160;&#160;</xsl:text>
      <xsl:call-template name="indent">
        <xsl:with-param name="count" select="$count - 1" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  <!-- Gets the substring after the last occurence of a period in a given string -->
  <xsl:template name="subString">
    <xsl:param name="name" />

    <xsl:choose>
      <xsl:when test="contains($name, '.')">
        <xsl:call-template name="subString">
          <xsl:with-param name="name" select="substring-after($name, '.')" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$name" />
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template name="codeSection">
        
        <table width="100%" cellspacing="0" cellpadding="0">
          <tr>
            <th>
              <span class="copyCode" onclick="CopyCode(this)" onkeypress="CopyCode_CheckKey(this, event)" onmouseover="ChangeCopyCodeIcon(this)" onmouseout="ChangeCopyCodeIcon(this)" tabindex="0">
                <img class="copyCodeImage" name="ccImage" align="absmiddle">
                  <includeAttribute name="title" item="copyImage" />
                  <includeAttribute name="src" item="iconPath">
                    <parameter>copycode.gif</parameter>
                  </includeAttribute>
                </img>
                <include item="copyCode"/>
              </span>
            </th>
          </tr>
          <tr>
            <td colspan="2">
              <pre><xsl:text/><xsl:copy-of select="node()"/><xsl:text/></pre>
            </td>
          </tr>
        </table>
     
  </xsl:template>

  <xsl:template name="languageCheck">
    <xsl:param name="codeLanguage"/>

    <xsl:if test="$languages != 'false'">
      <xsl:if test="count($languages/language) &gt; 0">
        <xsl:for-each select="$languages/language">
          <xsl:if test="$codeLanguage = @name">
            <xsl:value-of select="@style"/>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>