@ECHO OFF

REM Step 4.1 - Generate the website table of content XML file too
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"TocToWebContents.xsl" toc.xml /out:"WebTOC.xml"
