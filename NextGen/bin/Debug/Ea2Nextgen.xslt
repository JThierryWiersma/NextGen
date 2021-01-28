<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:UML="omg.org/UML1.3" >
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
  <xsl:template match="XMI" mode="#all">
    <xsl:element name="results">
      <xsl:apply-templates select="./*"/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="//UML:Class|UML:UseCase">
    <xsl:text>
  </xsl:text>
    <xsl:element name="type">
      <xsl:attribute name="definition">
        <xsl:value-of select="translate(./UML:ModelElement.stereotype/UML:Stereotype/@name,' ','_')"></xsl:value-of>
      </xsl:attribute>
      <xsl:text>
    </xsl:text>
      <xsl:element name="name">
        <xsl:value-of select="translate(@name,' ', '_')"/>
      </xsl:element>
      <xsl:message>
        Zoek de baseclass van de class door een generalization met de huidige class als subtype te vinden.
        De naam van supertype is dan de baseclass
      </xsl:message>
      <xsl:variable name="xmiid">
        <xsl:value-of select="@xmi.id"/>
      </xsl:variable>
      <xsl:for-each select="//UML:Generalization[@subtype=$xmiid]">
        <xsl:text>
    </xsl:text>
        <xsl:element name="supertype">
          <xsl:call-template name="findName">
            <xsl:with-param name="id" select="@supertype"/>
          </xsl:call-template>
        </xsl:element>
      </xsl:for-each>

      <xsl:message>Doorloop alle tagged values van de class [@tag!=modeldocument]</xsl:message>
      <xsl:for-each select="./UML:ModelElement.taggedValue/UML:TaggedValue[@tag!='modeldocument']">
        <xsl:variable name="ttt">
          <xsl:value-of select="translate(@tag, '$ ABCDEFGHIJKLMNOPQRSTUVWXYZ', '__abcdefghijklmnopqrstuvwxyz')"/>
        </xsl:variable>
        <xsl:if test="string($ttt)!=''">
          <xsl:text>
      </xsl:text>
          <xsl:element name="{$ttt}">
            <xsl:value-of select="@value"/>
          </xsl:element>
        </xsl:if>
      </xsl:for-each>
      <xsl:text>
    </xsl:text>

      <xsl:message>Doorloop alle dependencies</xsl:message>
      <xsl:for-each select="//UML:Dependency[@client=$xmiid]">
        <xsl:text>
      </xsl:text>
        <xsl:comment>Dependency attribuut:</xsl:comment>
        <xsl:text>
      </xsl:text>
        <xsl:message>Maak een 'name' element aan</xsl:message>
        <xsl:variable name="depname">
          <xsl:call-template name="findName">
            <xsl:with-param name="id" select="@supplier"/>
          </xsl:call-template>
        </xsl:variable>
        <xsl:text>
      </xsl:text>
        <xsl:variable name="depstereotype">
          <xsl:call-template name="findStereotype">
            <xsl:with-param name="id" select="@supplier"/>
          </xsl:call-template>
        </xsl:variable>
        <xsl:if test="string($depstereotype)!=''">
          <xsl:text>
      </xsl:text>
          <xsl:element name="{$depstereotype}">
            <xsl:value-of select="$depname"/>
          </xsl:element>
        </xsl:if>
      </xsl:for-each>

      <xsl:element name="attributes">
        <xsl:variable name="ordering" select="position()"/>
        <xsl:message>Doorloop alle attributen van de class</xsl:message>
        <xsl:for-each select="./UML:Classifier.feature/UML:Attribute">
          <xsl:text>
      </xsl:text>
          <xsl:element name="attribute">
            <xsl:message>Maak een 'ordering' element aan</xsl:message>
            <xsl:text>
        </xsl:text>
            <xsl:element name="ordering">
              <xsl:value-of select="position()"/>
            </xsl:element>
            <xsl:message>Maak een 'name' element aan</xsl:message>
            <xsl:text>
        </xsl:text>
            <xsl:element name="name">
              <xsl:value-of select="translate(@name,' ', '_')"/>
            </xsl:element>
            <xsl:message>Doorloop alle tagged values van de class</xsl:message>
            <xsl:for-each select="UML:ModelElement.taggedValue/UML:TaggedValue">
              <xsl:variable name="ttt">
                <xsl:value-of select="translate(@tag, '$ ABCDEFGHIJKLMNOPQRSTUVWXYZ', '__abcdefghijklmnopqrstuvwxyz')"/>
              </xsl:variable>
              <xsl:if test="string($ttt)!=''">
                <xsl:text>
        </xsl:text>
                <xsl:element name="{$ttt}">
                  <xsl:value-of select="@value"/>
                </xsl:element>
              </xsl:if>
            </xsl:for-each>
          </xsl:element>
        </xsl:for-each>

        <xsl:message>Doorloop alle associaties </xsl:message>
        <xsl:for-each select="//UML:Association/UML:Association.connection/UML:AssociationEnd[@type=$xmiid]">
          <xsl:variable name="otherEnd" select="..//UML:AssociationEnd[@type!=$xmiid]"/>
          <xsl:text>
      </xsl:text>
          <xsl:comment>Associatie attribuut:</xsl:comment>
          <xsl:text>
      </xsl:text>
          <xsl:element name="attribute">
            <xsl:message>Maak een 'name' element aan</xsl:message>
            <xsl:text>
        </xsl:text>
            <xsl:element name="name">
              <xsl:if test="string(@name)=''">
                <xsl:text>anonymous</xsl:text>
                <xsl:value-of select="position()"/>
              </xsl:if>
              <xsl:value-of select="translate(@name, '$ ABCDEFGHIJKLMNOPQRSTUVWXYZ', '__abcdefghijklmnopqrstuvwxyz')"/>
            </xsl:element>
            <xsl:element name="ordering">
              <xsl:value-of select="99 + position()"/>
            </xsl:element>
            <xsl:message>Maak een 'type' element aan</xsl:message>
            <xsl:text>
        </xsl:text>
            <xsl:element name="otherEndName">
              <xsl:call-template name="findName">
                <xsl:with-param name="id" select="$otherEnd/@type"/>
              </xsl:call-template>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="type">
              <xsl:variable name="stereoname" select="../../UML:ModelElement.stereotype/UML:Stereotype/@name"/>
              <xsl:if test="string($stereoname)=''">
                <xsl:text>association</xsl:text>
              </xsl:if>
              <xsl:if test="string($stereoname)!=''">
                <xsl:value-of select="$stereoname"/>
              </xsl:if>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:variable name="otherEndStereotype">
              <xsl:call-template name="findStereotype">
                <xsl:with-param name="id" select="$otherEnd/@type"/>
              </xsl:call-template>
            </xsl:variable>
            <xsl:if test="string($otherEndStereotype)!=''" >
              <xsl:element name="{$otherEndStereotype}">
                <xsl:call-template name="findName">
                  <xsl:with-param name="id" select="$otherEnd/@type"/>
                </xsl:call-template>
              </xsl:element>
            </xsl:if>
            <xsl:text>
        </xsl:text>
            <xsl:element name="otherEndUMLType">
              <xsl:call-template name="findUmlType">
                <xsl:with-param name="id" select="$otherEnd/@type"/>
              </xsl:call-template>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="visibility">
              <xsl:value-of select="$otherEnd/@visibility"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="multiplicity">
              <xsl:value-of select="$otherEnd/@multiplicity"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="aggregation">
              <xsl:value-of select="$otherEnd/@aggregation"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="isOrdered">
              <xsl:value-of select="$otherEnd/@isOrdered"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="targetScope">
              <xsl:value-of select="$otherEnd/@targetScope"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="changeable">
              <xsl:value-of select="$otherEnd/@changeable"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="isNavigable">
              <xsl:value-of select="$otherEnd/@isNavigable"/>
            </xsl:element>
            <xsl:message>Doorloop alle tagged values van de class</xsl:message>
            <xsl:for-each select="UML:ModelElement.taggedValue/UML:TaggedValue">
              <xsl:variable name="ttt">
                <xsl:value-of select="translate(@tag, '$ ABCDEFGHIJKLMNOPQRSTUVWXYZ', '__abcdefghijklmnopqrstuvwxyz')"/>
              </xsl:variable>
              <xsl:if test="string($ttt)!=''">
                <xsl:text>
        </xsl:text>
                <xsl:element name="{$ttt}">
                  <xsl:value-of select="@value"/>
                </xsl:element>
              </xsl:if>
            </xsl:for-each>
          </xsl:element>
        </xsl:for-each>

      </xsl:element>
      <xsl:text>
      </xsl:text>
      <xsl:element name="sets">
        <xsl:for-each select="./UML:Classifier.feature/UML:Operation">
          <xsl:text>
      </xsl:text>
          <xsl:element name="set">
            <xsl:text>
        </xsl:text>
            <xsl:element name="name">
              <xsl:value-of select="@name"/>
            </xsl:element>
            <xsl:text>
        </xsl:text>
            <xsl:element name="index">
              <xsl:value-of select="position()"/>
            </xsl:element>
            <xsl:element name="stereotype">
              <xsl:value-of select="./UML:ModelElement.stereotype/UML:Stereotype/@name"/>
            </xsl:element>
            <xsl:message>Doorloop alle tagged values van de class</xsl:message>
            <xsl:for-each select="./UML:ModelElement.taggedValue/UML:TaggedValue">
              <xsl:variable name="ttt">
                <xsl:value-of select="translate(@tag, '$ ABCDEFGHIJKLMNOPQRSTUVWXYZ', '__abcdefghijklmnopqrstuvwxyz')"/>
              </xsl:variable>
              <xsl:if test="string($ttt)!=''">
                <xsl:text>
        </xsl:text>
                <xsl:element name="{$ttt}">
                  <xsl:value-of select="@value"/>
                </xsl:element>
              </xsl:if>
            </xsl:for-each>
            <xsl:text>
        </xsl:text>
            <xsl:element name="attributes">
              <xsl:for-each select="./UML:BehavioralFeature.parameter/UML:Parameter">
                <xsl:text>
          </xsl:text>
                <xsl:element name="attribute">
                  <xsl:text>
            </xsl:text>
                  <xsl:element name="name">
                    <xsl:choose>
                      <xsl:when test="string(@name)=''">
                        <xsl:text>anonymous</xsl:text>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="@name"/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:element>
                  <xsl:text>
            </xsl:text>
                  <xsl:element name="ordering">
                    <xsl:value-of select="position()"/>
                  </xsl:element>
                  <xsl:element name="kind">
                    <xsl:value-of select="@kind"/>
                  </xsl:element>
                  <xsl:text>
            </xsl:text>
                  <xsl:element name="visibility">
                    <xsl:value-of select="@visibility"/>
                  </xsl:element>
                  <xsl:text>
            </xsl:text>
                  <xsl:element name="defaultValue">
                    <xsl:value-of select="./UML:Parameter.defaultValue/UML:Expression/@body"/>
                  </xsl:element>
                </xsl:element>
              </xsl:for-each>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
    </xsl:element>
  </xsl:template>
  <xsl:template match="text()">
  </xsl:template>
  <xsl:template name="findName">
    <xsl:param name="id"/>
    <xsl:for-each select="//UML:Class[@xmi.id=$id]">
      <xsl:value-of select="translate(@name, ' ', '_')"/>
    </xsl:for-each>
    <xsl:for-each select="//UML:UseCase[@xmi.id=$id]">
      <xsl:value-of select="translate(@name, ' ', '_')"/>
    </xsl:for-each>
    <xsl:message> Kijk eventueel in de EA stubs voor externe referentie </xsl:message>
    <xsl:for-each select="//EAStub[@xmi.id=$id]">
      <xsl:value-of select="translate(@name, ' ', '_')"/>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="findStereotype">
    <xsl:param name="id"/>
    <xsl:for-each select="//UML:Class[@xmi.id=$id]/UML:ModelElement.stereotype/UML:Stereotype">
      <xsl:value-of select="translate(@name, ' ', '_')"/>
    </xsl:for-each>
    <xsl:for-each select="//UML:UseCase[@xmi.id=$id]/UML:ModelElement.stereotype/UML:Stereotype">
      <xsl:value-of select="translate(@name, ' ', '_')"/>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="findUmlType">
    <xsl:param name="id"/>
    <xsl:for-each select="//UML:Class[@xmi.id=$id]">
      <xsl:value-of select="Class"/>
    </xsl:for-each>
    <xsl:for-each select="//UML:UseCase[@xmi.id=$id]">
      <xsl:value-of select="UseCase"/>
    </xsl:for-each>
    <xsl:for-each select="//UML:Actor[@xmi.id=$id]">
      <xsl:value-of select="Actor"/>
    </xsl:for-each>
    <xsl:for-each select="//EAStub[@xmi.id=$id]">
      <xsl:value-of select="@UMLType"/>
    </xsl:for-each>

  </xsl:template>

</xsl:stylesheet>
