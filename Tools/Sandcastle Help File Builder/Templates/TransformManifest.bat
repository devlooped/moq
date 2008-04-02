@ECHO OFF

REM Step 2 - Transform the reflection output
if {%1} == {vs2005} (
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\ApplyVSDocModel.xsl","{@SandcastlePath}ProductionTransforms\AddGuidFilenames.xsl" reflection.org /out:reflection.xml /arg:IncludeAllMembersTopic=true /arg:IncludeInheritedOverloadTopics=true {@IncludeProjectNode}
) else if {%1} == {hana} (
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\ApplyVSDocModel.xsl","{@SandcastlePath}ProductionTransforms\AddGuidFilenames.xsl" reflection.org /out:reflection.xml /arg:IncludeAllMembersTopic=false /arg:IncludeInheritedOverloadTopics=true {@IncludeProjectNode}
) else (
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\ApplyPrototypeDocModel.xsl","{@SandcastlePath}ProductionTransforms\AddGuidFilenames.xsl" reflection.org /out:reflection.xml
)

REM Generate a topic manifest
"{@SandcastlePath}ProductionTools\XslTransform" /xsl:"{@SandcastlePath}ProductionTransforms\ReflectionToManifest.xsl" reflection.xml /out:manifest.xml
