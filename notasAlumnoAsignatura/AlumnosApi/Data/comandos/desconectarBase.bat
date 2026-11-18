@echo off
setlocal
set "SERVER=(localdb)\MSSQLLocalDB"

echo ==========================================================
echo   DESCONECTAR LA BASE DE DATOS ALUMNOS DEL MOTOR SQL
echo ==========================================================
echo.
echo Unicamente desconecta la BD. No copia, no mueve y no borra nada.
echo.
sqlcmd -S "%SERVER%" -E -b -Q "IF DB_ID('ALUMNOS') IS NOT NULL BEGIN ALTER DATABASE ALUMNOS SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS'; PRINT 'BD ALUMNOS desconectada. Sus archivos siguen en su ruta.'; END ELSE PRINT 'La BD ALUMNOS no estaba conectada.';"
if errorlevel 1 goto :error

echo.
echo Fin. Los archivos .mdf y .ldf quedan donde estaban.
echo.
pause
exit /b 0

:error
echo ERROR: no se pudo desconectar la BD.
echo.
pause
exit /b 1