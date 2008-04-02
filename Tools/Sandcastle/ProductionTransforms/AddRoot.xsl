<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1">

	<xsl:output indent="yes" encoding="UTF-8" />

	<xsl:param name="name" />

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
			<api id="R:{$name}">
				<apidata group="root" pseudo="true"/>
				<elements>
					<xsl:for-each select="/reflection/apis/api[apidata/@group='namespace']">
						<element api="{@id}" />
					</xsl:for-each>
				</elements>
			</api>
		</apis>
	</xsl:template>

	<xsl:template match="api">
		<xsl:copy-of select="." />
	</xsl:template>

</xsl:stylesheet>

