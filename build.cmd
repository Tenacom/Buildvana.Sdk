@echo off & setlocal enableextensions

pushd "%~dp0"

set _TASK=%1
if "%_TASK%"=="" set _TASK=All

if /I "%_TASK%"=="Clean" (
    call :F_Run_Tasks Clean
) else if /I "%_TASK%"=="Restore" (
    call :F_Run_Tasks Clean Restore
) else if /I "%_TASK%"=="Build" (
    call :F_Run_Tasks Clean Restore Build
) else if /I "%_TASK%"=="Test" (
    call :F_Run_Tasks Clean Restore Build Test
) else if /I "%_TASK%"=="Pack" (
    call :F_Run_Tasks Clean Restore Build Test Pack
) else if /I "%_TASK%"=="All" (
    call :F_Run_Tasks Clean Restore Build Test Pack
) else (
    echo *** Unknown task '%1'
)

popd
exit /B %ERRORLEVEL%

:: RUN TASKS

:F_Run_Tasks

set _LOGFILE=build.log
if exist %_LOGFILE% del %_LOGFILE%

set _MSBUILD_OPTIONS=--verbosity normal
set _CONFIGURATION=Release

call :F_Timestamp

:L_Run_Tasks_Loop
if "%1"=="" exit /B 0
call :T_%1
if errorlevel 1 exit /B %ERRORLEVEL%
shift
goto :L_Run_Tasks_Loop

:: TASKS

:T_Clean
call :F_Label Clean output directories
call :F_Exec call :F_Zap
exit /B %ERRORLEVEL%

:T_Restore
call :F_Label Restore dependencies
call :F_Exec dotnet restore %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_Build
call :F_Label Build solution
call :F_Exec dotnet build --no-restore -MaxCpuCount:1 -c %_CONFIGURATION% %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_Test
call :F_Label Run tests
call :F_Exec dotnet test --no-build -c %_CONFIGURATION% %_MSBUILD_OPTIONS%
exit /B %ERRORLEVEL%

:T_Pack
call :F_Label Prepare NuGet packages
call :F_Exec dotnet pack --no-build -c %_CONFIGURATION% %_MSBUILD_OPTIONS%
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
echo ===^>^>^> %DATE% %TIME%
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

:: EOF
