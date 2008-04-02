@ECHO OFF

REM Step 6 - Build the HTML 1.x help file
cd .\Output
copy ..\*.hhp . > NUL
copy ..\*.hhc . > NUL
copy ..\*.hhk . > NUL

"{@HHCPath}hhc.exe" Help1x.hhp

cd ..

IF EXIST "{@OutputFolder}{@HTMLHelpName}.chm" DEL "{@OutputFolder}{@HTMLHelpName}.chm" > NUL
IF EXIST ".\Output\{@HTMLHelpName}.chm" COPY ".\Output\{@HTMLHelpName}.chm" "{@OutputFolder}" > NUL

REM Must remove these in case we are building a 2x file or website as well
del .\Output\*.hhp > NUL
del .\Output\*.hhc > NUL
del .\Output\*.hhk > NUL
del ".\Output\{@HTMLHelpName}.chm" > NUL
del ".\Output\{@HTMLHelpName}.log" > NUL
