
# Sandcastle build script overrides for vs2005 doc model.

. $DxRoot\Presentation\Shared\SharedDocModel.ps1

function PostProcessReflectionData($sourceFile, $targetFile) {
	WriteInfo "Post processing reflection data."
    &$XslTransform $sourceFile `
        /xsl:$DxRoot\ProductionTransforms\ApplyVSDocModel.xsl `
        /xsl:$DxRoot\ProductionTransforms\AddFriendlyFilenames.xsl `
        /arg:IncludeAllMembersTopic=true `
        /arg:IncludeInheritedOverloadTopics=true `
        /out:$targetFile
}

function CreateToc {
    WriteInfo "Creating TOC."
    &$XslTransform $TempDir\ReflectionData\targets.xml `
        /xsl:$DxRoot\ProductionTransforms\createvstoc.xsl `
        /out:$TempDir\toc.xml
}

