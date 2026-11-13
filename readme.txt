	API DE GESTION DE NOTAS DE ALUMNOS - ASP.NET CORE 10

PROYECTO: AlumnosApi
LENGUAJE: C# .NET 10
FRAMEWORK: ASP.NET Core Web API
BASE DE DATOS: SQL Server / LocalDB

DESCRIPCION DEL PROYECTO


Esta API REST implementa un sistema de gestión académica que permite:

1. REGISTRAR ALUMNOS
   - Código de alumno (único)
   - Nombres
   - Apellidos

2. REGISTRAR ASIGNATURAS
   - ID de asignatura (auto-generado)
   - Nombre de la asignatura

3. REGISTRAR NOTAS
   - Cada alumno puede tener hasta 3 notas por asignatura
   - Valores entre 1.0 y 7.0
   - Un decimal de precisión

4. CONSULTAR NOTAS CON PROMEDIO
   - Listar alumnos con todas sus notas y asignaturas
   - Promedio se calcula solo si están las 3 notas registradas
   - Formato especial: COD_ALUMNO, NOMBRE_COMPLETO, NOM_ASIGNATURA, 
					  N_NOTA_1, NOTA_1, N_NOTA_2, NOTA_2, N_NOTA_3, NOTA_3, PROMEDIO

INSTALACION Y CONFIGURACION

REQUISITOS:
- SQL Server o SQL Server Express instalado
- .NET 10 SDK instalado
- Visual Studio 2026 o superior (recomendado)

PASO 1: CREAR LA BASE DE DATOS
------------------------------
Opción A) Usando SQL Server Management Studio:
  1. Abre SQL Server Management Studio
  2. Conéctate a (LocalDB)\mssqllocaldb
  3. Abre el archivo setup.sql del proyecto
  4. Ejecuta el script (F5)

Opción B) Desde línea de comandos:
  sqlcmd -S (LocalDB)\mssqllocaldb -i setup.sql

PASO 2: RESTAURAR PAQUETES NUGET
---------------------------------
  dotnet restore

PASO 3: EJECUTAR LA APLICACION
------------------------------
  cd notasAlumnoAsignatura/AlumnosApi
  dotnet run

La API se ejecutará en:
  - HTTP: http://localhost:5000
  - HTTPS: https://localhost:5001

PASO 4: PROBAR LA API
---------------------
La documentación interactiva (Swagger) estará en:
  https://localhost:5001/swagger/index.html

Desde aquí puedes probar todos los endpoints sin necesidad de Postman.

ESTRUCTURA DEL PROYECTO

Controllers/
  ├── AlumnosController.cs       - Endpoints para gestionar alumnos
  ├── AsignaturasController.cs   - Endpoints para gestionar asignaturas
  └── NotasController.cs         - Endpoints para gestionar notas

Data/
  └── AlumnosContext.cs          - DbContext de Entity Framework Core

Models/
  ├── Alumno.cs
  ├── Asignatura.cs
  └── Nota.cs

BD/
  └── baseDatosAlumnos.mdf       - Base de datos

setup.sql                         - Script para crear la BD
Program.cs                        - Configuración de la aplicación
appsettings.json                 - Configuración de la cadena de conexión

ENDPOINTS DE LA API

ALUMNOS
-------
GET    /api/alumnos              - Listar todos los alumnos
POST   /api/alumnos              - Crear un nuevo alumno

ASIGNATURAS
-----------
GET    /api/asignaturas          - Listar todas las asignaturas
POST   /api/asignaturas          - Crear una nueva asignatura

NOTAS
-----
GET    /api/notas/listado        - Listar notas con formato especial
POST   /api/notas                - Ingresar una nueva nota
PUT    /api/notas                - Editar una nota existente

FORMAS DE PETICION (JSON)

CREAR ALUMNO (POST /api/alumnos)
{
  "codAlumno": 1,
  "nombres": "Juan",
  "apellidos": "Pérez"
}

CREAR ASIGNATURA (POST /api/asignaturas)
{
  "nomAsignatura": "Matemáticas"
}

INGRESAR NOTA (POST /api/notas)
{
  "codAlumno": 1,
  "idAsignatura": 1,
  "nNota": 1,
  "valor": 6.5
}

EDITAR NOTA (PUT /api/notas)
{
  "codAlumno": 1,
  "idAsignatura": 1,
  "nNota": 1,
  "valor": 7.0
}

VALIDACIONES

ALUMNOS:
- Código debe ser mayor a 0
- Nombres y apellidos son obligatorios
- El código de alumno NO puede repetirse

ASIGNATURAS:
- El nombre es obligatorio
- NO puede haber dos asignaturas con el mismo nombre

NOTAS:
- N_NOTA debe ser 1, 2 o 3 (identificador de cuál nota es)
- Valor debe estar entre 1.0 y 7.0
- Solo se permite 1 decimal (ej: 6.5)
- NO puede ingresar dos veces la misma nota para el mismo alumno/asignatura
- El alumno y la asignatura deben existir en la BD

PROMEDIO:
- Se calcula SOLO si están las 3 notas registradas
- Fórmula: (NOTA_1 + NOTA_2 + NOTA_3) / 3
- Se redondea a 1 decimal

CODIGOS DE RESPUESTA HTTP

200 OK              - Operación exitosa
201 Created         - Recurso creado exitosamente
400 Bad Request     - Error en los datos enviados
404 Not Found       - Recurso no encontrado
409 Conflict        - Conflicto (ej: código duplicado)
500 Server Error    - Error interno del servidor

ARCHIVOS IMPORTANTES

setup.sql           - REQUERIDO: Ejecutar PRIMERO para crear la BD
appsettings.json    - Cadena de conexión a la BD (LocalDB)
README.md           - Documentación completa del proyecto

NOTAS IMPORTANTES PARA EL PROFESOR

1. La base de datos se debe crear ANTES de ejecutar la aplicación
   Usa el script setup.sql proporcionado

2. La API usa LocalDB por defecto. Si necesitas usar otra instancia de SQL:
   - Modifica la cadena de conexión en appsettings.json
   - La cadena actual es: Server=(LocalDB)\mssqllocaldb;Database=baseDatosAlumnos;Integrated Security=True;TrustServerCertificate=True;

3. Para ver la BD creada:
   - Abre SQL Server Management Studio
   - Conéctate a (LocalDB)\mssqllocaldb
   - Verás la BD "baseDatosAlumnos" con las 3 tablas

4. Si necesitas resetear la BD:
   - Elimina la BD en SQL Server Management Studio
   - Vuelve a ejecutar setup.sql

5. La aplicación usa Entity Framework Core pero NO usa migraciones automáticas
   - La estructura se define en el archivo setup.sql
   - Los modelos en C# simplemente mapean a las tablas

RESPECTO A REQUERIMIENTOS

✓ Todos los requisitos se implementaron correctamente:
  - Ingresar alumnos, asignaturas y notas
  - Calcular promedio solo con las 3 notas
  - Listar con el formato especificado
  - Editar notas
  - Validaciones (nota 1.0 a 7.0)
  - N_NOTA de 1 a 3

✓ Se removió la carpeta Dtos para simplificar
✓ Se usa setup.sql para reproducibilidad

SOPORTE / ERRORES COMUNES

Error: "Cannot attach file ... baseDatosAlumnos.mdf"
Solución: Ejecuta el setup.sql tal como se indica

Error: "Login failed for user"
Solución: Verifica que estés usando Windows Authentication en LocalDB

Error: "Port 5001 already in use"
Solución: La aplicación anterior aún está corriendo. Ciérrala o cambia el puerto
		 en launchSettings.json

Error: "Database does not exist"
Solución: Ejecuta setup.sql ANTES de iniciar la aplicación

