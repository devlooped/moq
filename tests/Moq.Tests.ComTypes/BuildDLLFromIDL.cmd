midl /mktyplib203 /env win32 /tlb ComTypes.tlb ComTypes.idl
tlbimp ComTypes.tlb /out:Moq.Tests.ComTypes.dll /namespace:Moq.Tests.ComTypes /keyfile:..\..\Moq.snk /asmversion:4.0.0.0
peverify Moq.Tests.ComTypes.dll
del ComTypes.tlb
