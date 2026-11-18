@echo off
setlocal EnableExtensions
title Contenido de la base de datos ALUMNOS
set "SERVER=(localdb)\MSSQLLocalDB"
set "TMP=%TEMP%\contenido_bd_estado.txt"

echo ==========================================================
echo   CONTENIDO DE LA BASE DE DATOS ALUMNOS
echo ==========================================================
echo.

sqlcmd -S "%SERVER%" -E -h -1 -W -Q "SET NOCOUNT ON; IF DB_ID('ALUMNOS') IS NULL SELECT 0 ELSE SELECT 1" > "%TMP%" 2>nul
set "ESTADO_BD="
for /f "usebackq delims=" %%e in ("%TMP%") do set "ESTADO_BD=%%e"
if not "%ESTADO_BD%"=="1" goto :nodesconectada

echo Archivo en vivo segun el motor:
sqlcmd -S "%SERVER%" -E -d ALUMNOS -h -1 -W -Q "SET NOCOUNT ON; SELECT physical_name FROM sys.database_files WHERE type_desc = 'ROWS'"
echo.

echo 1) TABLAS
sqlcmd -S "%SERVER%" -E -d ALUMNOS -h -1 -W -Q "SET NOCOUNT ON; SELECT name FROM sys.tables ORDER BY name"
echo.

echo 2) REGISTROS POR TABLA
sqlcmd -S "%SERVER%" -E -d ALUMNOS -W -Q "SET NOCOUNT ON; SELECT 'ALUMNO' AS Tabla, COUNT(*) AS Registros FROM ALUMNO UNION ALL SELECT 'ASIGNATURA', COUNT(*) FROM ASIGNATURA UNION ALL SELECT 'NOTAS', COUNT(*) FROM NOTAS"
echo.

echo 3) ALUMNOS
sqlcmd -S "%SERVER%" -E -d ALUMNOS -W -Q "SET NOCOUNT ON; SELECT COD_ALUMNO, NOMBRES, APELLIDOS FROM ALUMNO ORDER BY COD_ALUMNO"
echo.

echo 4) ASIGNATURAS
sqlcmd -S "%SERVER%" -E -d ALUMNOS -W -Q "SET NOCOUNT ON; SELECT ID_ASIGNATURA, NOM_ASIGNATURA FROM ASIGNATURA ORDER BY ID_ASIGNATURA"
echo.

echo 5) NOTAS
sqlcmd -S "%SERVER%" -E -d ALUMNOS -W -Q "SET NOCOUNT ON; SELECT ID_NOTA, N_NOTA, NOTA, COD_ALUMNO, ID_ASIGNATURA FROM NOTAS ORDER BY COD_ALUMNO, ID_ASIGNATURA, N_NOTA"
echo.

echo 6) NOTAS CON ALUMNO Y ASIGNATURA
sqlcmd -S "%SERVER%" -E -d ALUMNOS -W -Q "SET NOCOUNT ON; SELECT a.COD_ALUMNO, a.NOMBRES + ' ' + a.APELLIDOS AS NOMBRE_COMPLETO, s.NOM_ASIGNATURA, n.N_NOTA, n.NOTA FROM NOTAS n JOIN ALUMNO a ON n.COD_ALUMNO = a.COD_ALUMNO JOIN ASIGNATURA s ON n.ID_ASIGNATURA = s.ID_ASIGNATURA ORDER BY a.COD_ALUMNO, s.NOM_ASIGNATURA, n.N_NOTA"
echo.

echo Fin del contenido.
echo.
pause
exit /b 0

:nodesconectada
echo La BD ALUMNOS no esta conectada.
echo Ejecuta cargarBaseDatos.bat (en Data\comandos) y vuelve a intentar.
echo.
pause
exit /b 1