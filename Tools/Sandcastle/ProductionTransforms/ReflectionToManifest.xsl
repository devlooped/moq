<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1">

	<xsl:output indent="yes" encoding="UTF-8" />

	<xsl:key name="index" match="/reflection/apis/api" use="@id" />

	<xsl:template match="/">
		<topics>
			<xsl:apply-templates select="/reflection/apis/api" />
		</topics>
	</xsl:template>

	<!-- namespace and member topics -->
	<xsl:template match="api">
    <xsl:if test="not(topicdata/@notopic)">
      <topic id="{@id}" />
    </xsl:if>
	</xsl:template>

</xsl:stylesheet>
