<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match ="/">
    <root>
      <Types>
        <xsl:for-each select="Types/SearchTypes">
          <xsl:element name="Type"> 
            <xsl:attribute name="Key"><xsl:value-of select="value"/></xsl:attribute>
            <xsl:attribute name="Name"><xsl:value-of select="name"/></xsl:attribute>
            <xsl:value-of select="text"/>
          </xsl:element>
        </xsl:for-each>
      </Types>
    </root>
  </xsl:template>

</xsl:stylesheet>
