@ECHO OFF

REM Step 5 - Generate an index for an HTML 1.x help file
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\ReflectionToChmIndex.xsl" reflection.xml /out:"{@HTMLHelpName}.hhk"
