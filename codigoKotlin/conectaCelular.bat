@echo off
REM adb_reverse_50555.bat
REM Redirige el puerto 50555 del celular (USB) al puerto 50555 del PC, para que 127.0.0.1 llegue a la API.
REM Requisitos: celular conectado por cable, depuracion USB activada y API corriendo en el PC.
set "PUERTO=50555"
set "ADB=adb"
REM Busca adb en el PATH; si no esta, usa la ruta estandar del SDK de Android.
where adb >nul 2>&1
if not errorlevel 1 goto ejecutar
set "ADB=%LOCALAPPDATA%\Android\Sdk\platform-tools\adb.exe"
if exist "%ADB%" goto ejecutar
echo ERROR: no se encontro adb. Instalar Platform-Tools desde Android Studio (SDK Manager).
goto fin
:ejecutar
echo Dispositivos conectados:
"%ADB%" devices
REM Crea la redireccion: lo que el celular envie a 127.0.0.1:50555 llega a localhost:50555 del PC.
"%ADB%" reverse tcp:%PUERTO% tcp:%PUERTO%
if errorlevel 1 goto fallo
echo.
echo Redireccion activa:
"%ADB%" reverse --list
echo.
echo Listo. Abrir la app en el celular.
goto fin
:fallo
echo.
echo ERROR: no se pudo crear la redireccion.
echo Revisar que el celular este conectado por USB, con depuracion USB activada y autorizada.
echo En la lista de dispositivos debe aparecer la palabra device y no unauthorized ni offline.
:fin
pause