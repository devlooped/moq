@ECHO OFF

REM Step 4.2 - Generate a table of content for an HTML 1.x help file
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\TocToChmContents.xsl" toc.xml /out:"{@HTMLHelpName}.hhc"
