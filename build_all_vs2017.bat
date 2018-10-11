@echo off
set CONFIG=Debug
set VisualStudioVersion=15.0

for /f "usebackq tokens=*" %%i in (`vswhere -latest -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set INSTALLDIR=%%i
)

"%INSTALLDIR%\MSBuild\%VisualStudioVersion%\bin\MSBuild.exe" ".\moq.sln" /t:restore /t:build

if not %ERRORLEVEL%==0 goto fail 

echo %time%
echo.
color

pause
exit /b 0

:end

:fail
color 07

echo.
echo.
echo Failed
echo.
pause
exit /b 1