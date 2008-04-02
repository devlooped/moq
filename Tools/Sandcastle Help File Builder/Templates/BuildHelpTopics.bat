@ECHO OFF

REM Step 3 - Build the help topics
"{@SandcastlePath}ProductionTools\BuildAssembler" /config:sandcastle.config manifest.xml
