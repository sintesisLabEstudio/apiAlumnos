@echo off
setlocal EnableExtensions
set "SERVER=(localdb)\MSSQLLocalDB"
set "MDF=%~dp0ALUMNOS.mdf"
set "LDF=%~dp0ALUMNOS_log.ldf"
set "TMP=%TEMP%\ruta_viva_alumnos.txt"
set "EST=%TEMP%\alumnos_estado.txt"

echo ==========================================================
echo   DATOS DEL ARCHIVO DE BASE DE DATOS EN LA RUTA ACTUAL
echo ==========================================================
echo.
echo Ruta actual: %~dp0
echo Archivo:     %MDF%
echo.

sqlcmd -S "%SERVER%" -E -h -1 -W -Q "SET NOCOUNT ON; IF DB_ID('ALUMNOS') IS NULL SELECT 0 ELSE SELECT 1" > "%EST%" 2>nul
set "ESTADO="
for /f "usebackq delims=" %%e in ("%EST%") do set "ESTADO=%%e"
if not "%ESTADO%"=="1" echo Nota: la BD ALUMNOS no esta conectada; se lee el archivo de esta ruta.

sqlcmd -S "%SERVER%" -E -d ALUMNOS -h -1 -W -Q "SET NOCOUNT ON; SELECT physical_name FROM sys.database_files WHERE type_desc = 'ROWS'" > "%TMP%" 2>nul
set "RUTA_VIVA="
for /f "usebackq delims=" %%p in ("%TMP%") do set "RUTA_VIVA=%%p"

if /I "%RUTA_VIVA%"=="%MDF%" goto :usarvivo

sqlcmd -S "%SERVER%" -E -b -Q "IF DB_ID('ALUMNOS_RUTA_ACTUAL') IS NOT NULL BEGIN ALTER DATABASE ALUMNOS_RUTA_ACTUAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS_RUTA_ACTUAL'; END" >nul 2>&1
sqlcmd -S "%SERVER%" -E -b -Q "CREATE DATABASE ALUMNOS_RUTA_ACTUAL ON (FILENAME=N'%MDF%'), (FILENAME=N'%LDF%') FOR ATTACH;" >nul 2>&1
if errorlevel 1 goto :error
set "BD=ALUMNOS_RUTA_ACTUAL"
goto :consultar

:usarvivo
set "BD=ALUMNOS"

:consultar
sqlcmd -S "%SERVER%" -E -d %BD% -W -Q "SET NOCOUNT ON; SELECT physical_name AS ArchivoEnRutaActual FROM sys.database_files WHERE type_desc = 'ROWS'; SELECT name AS Tabla FROM sys.tables ORDER BY name; SELECT 'ALUMNO' AS Tabla, COUNT(*) AS Registros FROM ALUMNO UNION ALL SELECT 'ASIGNATURA', COUNT(*) FROM ASIGNATURA UNION ALL SELECT 'NOTAS', COUNT(*) FROM NOTAS; SELECT COD_ALUMNO, NOMBRES, APELLIDOS FROM ALUMNO ORDER BY COD_ALUMNO; SELECT ID_ASIGNATURA, NOM_ASIGNATURA FROM ASIGNATURA ORDER BY ID_ASIGNATURA; SELECT ID_NOTA, N_NOTA, NOTA, COD_ALUMNO, ID_ASIGNATURA FROM NOTAS ORDER BY COD_ALUMNO, ID_ASIGNATURA, N_NOTA; SELECT a.COD_ALUMNO, a.NOMBRES + ' ' + a.APELLIDOS AS NOMBRE_COMPLETO, s.NOM_ASIGNATURA, n.N_NOTA, n.NOTA FROM NOTAS n JOIN ALUMNO a ON n.COD_ALUMNO = a.COD_ALUMNO JOIN ASIGNATURA s ON n.ID_ASIGNATURA = s.ID_ASIGNATURA ORDER BY a.COD_ALUMNO, s.NOM_ASIGNATURA, n.N_NOTA"
if errorlevel 1 goto :error

if "%BD%"=="ALUMNOS_RUTA_ACTUAL" sqlcmd -S "%SERVER%" -E -b -Q "ALTER DATABASE ALUMNOS_RUTA_ACTUAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS_RUTA_ACTUAL';" >nul 2>&1

echo.
echo Datos tomados del archivo: %MDF%
echo.
pause
exit /b 0

:error
echo ERROR: no se pudieron leer los datos del archivo.
echo.
pause
exit /b 1