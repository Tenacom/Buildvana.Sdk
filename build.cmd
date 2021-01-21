@echo off & setlocal enableextensions
pushd "%~dp0"

:: Configuration values
set _SOLUTION_NAME=
set _DEFAULT_TASK=
set _MSBUILD_CONFIGURATION=
set _MSBUILD_VERBOSITY=
set _MSBUILD_OPTIONS=
set _VS_MSBUILD_EXE=

:: Load configuration
if exist build-config.cmd call build-config.cmd

:: Default for solution name is the same name as containing folder, including extension
if "%_SOLUTION_NAME%" == "" call :F_SetToCurrentDirectoryName _SOLUTION_NAME
set _SOLUTION_FILE=%_SOLUTION_NAME%.sln
if not exist "%_SOLUTION_FILE%" (
    echo *** Solution file '%_SOLUTION_FILE%' not found.
    exit /B 1
)

:: Other default configuration values
if "%_DEFAULT_TASK%"=="" set _DEFAULT_TASK=All
if "%_MSBUILD_CONFIGURATION%"=="" set _MSBUILD_CONFIGURATION=Release
if "%_MSBUILD_VERBOSITY%"=="" set _MSBUILD_VERBOSITY=normal
if "%_MSBUILD_OPTIONS%"=="" set _MSBUILD_OPTIONS=
if "%_VS_MSBUILD_EXE%"=="" set _VS_MSBUILD_EXE="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"

:: Use VisualStudio's MSBuild if specified
if /I "%1" equ "VS" (
    set _VS=1
    shift
)

:: Task to run
set _TASK=%1
if "%_TASK%"=="" set _TASK=%_DEFAULT_TASK%

:: Define task dependencies
if /I "%_TASK%"=="Clean" (
    call :F_Run_Tasks Clean
) else if /I "%_TASK%"=="Tools" (
    call :F_Run_Tasks Clean Tools
) else if /I "%_TASK%"=="Restore" (
    call :F_Run_Tasks Clean Tools Restore
) else if /I "%_TASK%"=="Inspect" (
    call :F_Run_Tasks Clean Tools Restore Inspect
) else if /I "%_TASK%"=="Build" (
    call :F_Run_Tasks Clean Tools Restore Inspect Build
) else if /I "%_TASK%"=="Test" (
    call :F_Run_Tasks Clean Tools Restore Inspect Build Test
) else if /I "%_TASK%"=="Pack" (
    call :F_Run_Tasks Clean Tools Restore Inspect Build Test Pack
) else if /I "%_TASK%"=="All" (
    call :F_Run_Tasks Clean Tools Restore Inspect Build Test Pack
) else (
    echo *** Unknown task '%_TASK%'
)

popd
exit /B %ERRORLEVEL%

:: RUN TASKS

:F_Run_Tasks

set _LOGS_DIR=logs
mkdir %_LOGS_DIR% >nul 2>&1

set _LOGFILE=%_LOGS_DIR%\build.log
if exist %_LOGFILE% del %_LOGFILE%

call :F_Timestamp

:L_Run_Tasks_Loop
if "%1"=="" exit /B 0
if "%_VS%" == "" ( call :T_%1 ) else ( call :T_VS_%1 )
if errorlevel 1 exit /B %ERRORLEVEL%
shift
goto :L_Run_Tasks_Loop

:: TASKS

:T_Clean
:T_VS_Clean
call :F_Label Clean output directories
call :F_Exec call :F_Zap
exit /B %ERRORLEVEL%

:T_Tools
call :F_Label Restore .NET CLI tools
call :F_Exec dotnet tool restore
exit /B %ERRORLEVEL%

:T_VS_Tools
exit /B 0

:T_Inspect
call :F_Label Inspect code with ReSharper tools
call :F_Exec dotnet jb inspectcode "%_SOLUTION_FILE%" --output=%_LOGS_DIR%\inspect.log --format=Text
call :F_Exec dotnet jb dupfinder "%_SOLUTION_FILE%" --output=%_LOGS_DIR%\dupfinder.log.xml
exit /B %ERRORLEVEL%

:T_VS_Inspect
exit /B 0

:T_Restore
call :F_Label Restore dependencies
call :F_Exec dotnet restore --verbosity %_MSBUILD_VERBOSITY% %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_VS_Restore
call :F_Label Restore dependencies
call :F_Exec %_VS_MSBUILD_EXE% -t:restore -v:%_MSBUILD_VERBOSITY% %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_Build
call :F_Label Build solution
call :F_Exec dotnet build -c %_MSBUILD_CONFIGURATION% --verbosity %_MSBUILD_VERBOSITY% --no-restore -maxCpuCount:1 %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_VS_Build
call :F_Label Build solution
call :F_Exec %_VS_MSBUILD_EXE% -t:build -p:Configuration=%_MSBUILD_CONFIGURATION% -v:%_MSBUILD_VERBOSITY% -restore:False -maxCpuCount:1 %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_Test
call :F_Label Run unit tests
call :F_Exec dotnet test -c %_MSBUILD_CONFIGURATION% --verbosity %_MSBUILD_VERBOSITY% --no-build %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_VS_Test
exit /B 0

:T_Pack
call :F_Label Prepare for distribution
call :F_Exec dotnet pack -c %_MSBUILD_CONFIGURATION% --verbosity %_MSBUILD_VERBOSITY% --no-restore -maxCpuCount:1 %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_VS_Pack
call :F_Label Prepare for distribution
call :F_Exec %_VS_MSBUILD_EXE% -t:pack -p:Configuration=%_MSBUILD_CONFIGURATION% -v:%_MSBUILD_VERBOSITY% -restore:False -maxCpuCount:1 %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:: SUB-ROUTINES

:F_Zap
rmdir /S /Q artifacts >nul 2>&1
rmdir /S /Q _ReSharper.Caches >nul 2>&1
for /F "tokens=*" %%G in ('dir /B /AD /S bin ^& dir /B /AD /S obj') do rmdir /S /Q "%%G"
exit /B 0

:F_Exec
echo --- %*
echo --- %* >>%_LOGFILE% 2>&1
%* >>%_LOGFILE% 2>&1
set _EL=%ERRORLEVEL%
echo. >>%_LOGFILE% 2>&1
call :F_Display_Errorlevel %_EL%
exit /B %_EL%

:F_Timestamp
call :F_Timestamp_Core
call :F_Timestamp_Core >>%_LOGFILE% 2>&1
exit /B 0

:F_Timestamp_Core
echo.
echo ===^>^>^> '%_SOLUTION_NAME%'   %DATE% %TIME%
exit /B 0

:F_Label
call :F_Label_Core %*
call :F_Label_Core %* >>%_LOGFILE% 2>&1
exit /B 0

:F_Label_Core
echo.
echo ^>^>^> %*
exit /B 0

:F_Display_Errorlevel
if "%1%"=="0" exit /B 0
call :F_Display_Errorlevel_Core %1
call :F_Display_Errorlevel_Core %1 >>%_LOGFILE% 2>&1
exit /B 0

:F_Display_Errorlevel_Core
echo ^*^*^* ERRORLEVEL = %1
echo.
exit /B 0

:F_SetToCurrentDirectoryName
call :F_SetToCurrentDirectoryName_Core %1 "%CD%"
exit /B 0

:F_SetToCurrentDirectoryName_Core
set %1=%~nx2
exit /B 0

:: EOF
