msbuild Source\Moq.csproj /p:Configuration=Release
"Tools\Sandcastle Help File Builder\SandcastleBuilderConsole.exe" Moq.shfb -include=Moq,MockBehavior -exclude=Moq,MockBehavior,Normal -exclude=Moq,MockBehavior,Relaxed
xcopy Help\Moq.chm Moq.chm /Y
