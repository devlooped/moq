@ECHO OFF

REM Step 4.3 - Generate a packaged table of content for an HTML 2.x help file
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\TocToHxSContents.xsl" toc.xml /out:"{@HTMLHelpName}.HxT"
