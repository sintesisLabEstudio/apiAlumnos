@echo off
setlocal EnableExtensions
title Copiar base de datos ALUMNOS a la carpeta BD
set "SERVER=(localdb)\MSSQLLocalDB"
set "TMP=%TEMP%\copiar_bd_archivos.txt"

echo ==========================================================
echo   COPIAR LA BD ORIGINAL A LA CARPETA BD
echo ==========================================================
echo.
echo Las copias en BD se sobrescriben. La BD en vivo no se mueve.
echo Si la API esta corriendo, detenela antes con Ctrl+C.
echo.

echo 1/5 Verificando LocalDB...
call :asegurar_localdb
if errorlevel 1 goto :fallo

echo 2/5 Consultando la ruta de los archivos originales...
sqlcmd -S "%SERVER%" -E -h -1 -W -s "|" -Q "SET NOCOUNT ON; SELECT type_desc, physical_name FROM sys.master_files WHERE database_id = DB_ID('ALUMNOS') ORDER BY type_desc DESC" > "%TMP%"
for /f "usebackq tokens=1,2 delims=|" %%t in ("%TMP%") do (
    if "%%t"=="ROWS" set "MDF=%%u"
    if "%%t"=="LOG"  set "LDF=%%u"
)
if not defined MDF goto :sinbd
if not defined LDF goto :sinbd
for %%i in ("%~dp0..\..\BD") do set "CARPETA_BD=%%~fi"
echo     Original MDF: %MDF%
echo     Original LDF: %LDF%
echo     Destino:      %CARPETA_BD%
echo.

echo 3/5 Desconectando la BD (sin borrar archivos)...
sqlcmd -S "%SERVER%" -E -b -Q "IF DB_ID('ALUMNOS_RUTA_ACTUAL') IS NOT NULL BEGIN ALTER DATABASE ALUMNOS_RUTA_ACTUAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS_RUTA_ACTUAL'; END" >nul 2>&1
sqlcmd -S "%SERVER%" -E -b -Q "IF DB_ID('ALUMNOS') IS NOT NULL BEGIN ALTER DATABASE ALUMNOS SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS'; END"
if errorlevel 1 goto :fallo

echo 4/5 Copiando los archivos (sobrescribiendo)...
if not exist "%CARPETA_BD%" mkdir "%CARPETA_BD%"
if /I "%MDF%"=="%CARPETA_BD%\ALUMNOS.mdf" (
    echo     El .mdf ya esta en BD; no se copia.
) else (
    copy /Y "%MDF%" "%CARPETA_BD%\ALUMNOS.mdf" >nul
    if errorlevel 1 goto :fallo
)
if /I "%LDF%"=="%CARPETA_BD%\ALUMNOS_log.ldf" (
    echo     El .ldf ya esta en BD; no se copia.
) else (
    copy /Y "%LDF%" "%CARPETA_BD%\ALUMNOS_log.ldf" >nul
    if errorlevel 1 goto :fallo
)

echo 5/5 Volviendo a conectar la BD en su ruta original...
sqlcmd -S "%SERVER%" -E -b -Q "CREATE DATABASE ALUMNOS ON (FILENAME=N'%MDF%'), (FILENAME=N'%LDF%') FOR ATTACH;"
if errorlevel 1 goto :fallo
sqlcmd -S "%SERVER%" -E -b -Q "ALTER DATABASE ALUMNOS SET MULTI_USER;" >nul 2>&1

echo.
echo Copias actualizadas en la carpeta BD:
dir "%CARPETA_BD%\ALUMNOS.mdf" "%CARPETA_BD%\ALUMNOS_log.ldf"
echo.
echo LISTO. La BD en vivo sigue en su ruta original.
echo.
pause
exit /b 0

:asegurar_localdb
sqllocaldb info MSSQLLocalDB >nul 2>&1
if errorlevel 1 sqllocaldb create MSSQLLocalDB >nul 2>&1
sqllocaldb info MSSQLLocalDB 2>nul | findstr /I "State:.*Running" >nul
if not errorlevel 1 exit /b 0
sqllocaldb start MSSQLLocalDB >nul 2>&1
sqllocaldb info MSSQLLocalDB 2>nul | findstr /I "State:.*Running" >nul
if not errorlevel 1 exit /b 0
echo El motor quedo desincronizado; recuperando la instancia...
powershell -NoProfile -Command "Get-CimInstance Win32_Process | Where-Object { $_.Name -eq 'sqlservr.exe' -and $_.CommandLine -like '*MSSQLLocalDB*' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }"
sqllocaldb stop MSSQLLocalDB >nul 2>&1
sqllocaldb start MSSQLLocalDB >nul 2>&1
sqllocaldb info MSSQLLocalDB 2>nul | findstr /I "State:.*Running" >nul
if errorlevel 1 exit /b 1
exit /b 0

:sinbd
echo ERROR: la BD ALUMNOS no esta conectada. Ejecuta cargarBaseDatos.bat.
echo.
pause
exit /b 1

:fallo
echo ERROR: no se pudo completar la copia.
echo.
pause
exit /b 1