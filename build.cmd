:: Optional batch file to quickly build with some defaults.
:: Alternatively, this batch file can be invoked passing msbuild parameters, like: build.cmd /v:detailed /t:Rebuild

@ECHO OFF
SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
PUSHD "%~dp0" >NUL

SET CACHED_NUGET=%LocalAppData%\NuGet\NuGet.exe

:: Determine if MSBuild can be located. Allows for a better error message below.
where msbuild > %TEMP%\msbuild.txt
set /p msb=<%TEMP%\msbuild.txt

IF "%msb%"=="" (
    echo Please run %~n0 from a Visual Studio Developer Command Prompt.
    exit /b -1
)

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul
.nuget\nuget.exe update -self

:restore
:: Build script packages have no version in the path, so we install them to .nuget\packages to avoid conflicts with 
:: solution/project packages.
IF NOT EXIST packages.config goto run
.nuget\NuGet.exe install packages.config -OutputDirectory .nuget\packages -ExcludeVersion

:run
"%msb%" build.proj /v:normal %1 %2 %3 %4 %5 %6 %7 %8 %9

POPD >NUL
ENDLOCAL
ECHO ON