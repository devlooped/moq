<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1">

	<xsl:output indent="yes" encoding="UTF-8" />

	<xsl:key name="index" match="/*/apis/api" use="@id" />

	<xsl:template match="/">
		<reflection>
			<xsl:apply-templates select="/*/assemblies" />
			<xsl:apply-templates select="/*/apis" />
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

	<!-- ignore EII apis -->
	<xsl:template match="api[memberdata/@visibility='private' and proceduredata/@virtual='true']" />

	<!-- remove EII apis from element lists -->
	<xsl:template match="api[apidata/@group='type']">
		<api id="{@id}">
			<xsl:for-each select="*">
				<xsl:choose>
					<xsl:when test="local-name()='elements'">
						<elements>
							<xsl:for-each select="element">
                <xsl:choose>
                  <xsl:when test="(memberdata/@visibility='private' and proceduredata/@virtual='true') or key('index',@api)[memberdata/@visibility='private' and proceduredata/@virtual='true']">
                    <!-- ignore eii elements -->
                  </xsl:when>
                  <xsl:otherwise>
                    <!-- copy non-eii elements -->
                    <xsl:copy-of select="." />
                  </xsl:otherwise>
                </xsl:choose>
                <!--
								<xsl:if test="not() and not(key('index',@api)[memberdata/@visibility='private' and proceduredata/@virtual='true'])">
									<element api="{@api}">
										<xsl:if test="boolean(@display-api)">
											<xsl:attribute name="display-api"><xsl:value-of select="@display-api" /></xsl:attribute>
										</xsl:if>
									</element>
								</xsl:if>
                -->
							</xsl:for-each>
						</elements>
					</xsl:when>
					<xsl:otherwise>
						<xsl:copy-of select="." />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</api>
	</xsl:template>

	<!-- copy all other apis -->
	<xsl:template match="api">
		<xsl:copy-of select="." />
	</xsl:template>

</xsl:stylesheet>
