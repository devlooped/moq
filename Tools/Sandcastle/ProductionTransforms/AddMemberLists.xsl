<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1">

	<xsl:output indent="yes" encoding="UTF-8" />

	<xsl:key name="index" match="/reflection/apis/api" use="@id" />

	<xsl:template match="/">
		<reflection>
			<xsl:apply-templates select="/reflection/assemblies" />
			<xsl:apply-templates select="/reflection/apis" />
		</reflection>
	</xsl:template>

	<xsl:template match="assemblies">
		<xsl:copy-of select="." />
	</xsl:template>

	<xsl:template match="apis">
		<apis>
			<xsl:apply-templates select="api" />
		</apis>
	</xsl:template>

	<xsl:template match="api">
		<xsl:copy-of select="." />
	</xsl:template>

	<xsl:template match="api[apidata/@group='type']">
		<xsl:copy-of select="." />
		<xsl:variable name="typeId" select="@id" />
		<xsl:variable name="methods" select="key('index',elements/element/@api)[apidata/@group='member' and apidata/@subgroup='method']" />
		<xsl:if test="count($methods) > 1">
			<api id="Methods:{$typeId}">
				<apidata group="type" subgroup="methods" />
				<elements>
					<xsl:for-each select="$methods">
						<element api="{@id}" />
					</xsl:for-each>
				</elements>
			</api>
		</xsl:if>
		<xsl:variable name="properties" select="key('index',elements/element/@api)[apidata/@group='member' and apidata/@subgroup='property']" />		
		<xsl:if test="count($properties) > 1">
			<api id="Properties:{$typeId}">
				<apidata group="type" subgroup="properties" />
				<elements>
					<xsl:for-each select="$properties">
						<element api="{@id}" />
					</xsl:for-each>
				</elements>
			</api>
		</xsl:if>		
	</xsl:template>

</xsl:stylesheet>