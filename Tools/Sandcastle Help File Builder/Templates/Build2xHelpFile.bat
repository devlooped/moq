@ECHO OFF

REM Step 6 - Build the HTML 2.x help file
cd .\Output
copy "..\{@HTMLHelpName}.hxt" . > NUL
copy ..\Help2x.hxc "{@HTMLHelpName}.hxc" > NUL
copy ..\Help2x.hxf "{@HTMLHelpName}.hxf" > NUL
copy ..\Help2x_a.hxk "{@HTMLHelpName}_a.hxk" > NUL
copy ..\Help2x_b.hxk "{@HTMLHelpName}_b.hxk" > NUL
copy ..\Help2x_f.hxk "{@HTMLHelpName}_f.hxk" > NUL
copy ..\Help2x_k.hxk "{@HTMLHelpName}_k.hxk" > NUL
copy ..\Help2x_n.hxk "{@HTMLHelpName}_n.hxk" > NUL
copy ..\Help2x_s.hxk "{@HTMLHelpName}_s.hxk" > NUL

"{@HXCompPath}hxcomp" -p "{@HTMLHelpName}.hxc" -l "{@HTMLHelpName}.log"

type "{@HTMLHelpName}.log"

cd ..

IF EXIST "{@OutputFolder}{@HTMLHelpName}.hx?" DEL "{@OutputFolder}{@HTMLHelpName}.hx?" > NUL
IF EXIST "{@OutputFolder}{@HTMLHelpName}_?.hx?" DEL "{@OutputFolder}{@HTMLHelpName}_?.hx?" > NUL
copy ".\Output\*.hx?" "{@OutputFolder}" > NUL
del "{@OutputFolder}\{@HTMLHelpName}.hxf" > NUL

REM Must remove these in case we are building a 1x file or website as well
IF EXIST ".\Output\{@HTMLHelpName}.log" del ".\Output\{@HTMLHelpName}.log" > NUL
del .\Output\*.hx? > NUL
