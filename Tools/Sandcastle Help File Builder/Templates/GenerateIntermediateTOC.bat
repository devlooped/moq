@ECHO OFF

REM Step 4 - Generate an intermediate table of content file
if {%1} == {prototype} (
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\CreatePrototypeToc.xsl" reflection.xml /out:toc.xml
) else (
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\CreateVSToc.xsl" reflection.xml /out:toc.xml
)
