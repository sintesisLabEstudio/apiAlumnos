# API de Gestión de Notas de Alumnos

API REST desarrollada en **C# con .NET 10** para gestionar **alumnos, asignaturas y notas**, con cálculo de promedio y una aplicación Android (Kotlin) que la consume.

## ✨ Características

- CRUD completo de alumnos, asignaturas y notas.
- Hasta **3 notas por alumno y asignatura** (`N_NOTA` = 1, 2 o 3).
- Notas entre **1.0 y 7.0**, con un solo decimal.
- **Promedio** calculado únicamente cuando están las 3 notas.
- Listado consolidado con el formato exigido por la consigna.
- Documentación interactiva con **Swagger / OpenAPI**.
- Cliente Android Kotlin con Retrofit (incluido en la entrega).

## 📋 Requisitos

- **.NET 10 SDK**
- **SQL Server LocalDB** (instancia `MSSQLLocalDB`)
- Opcional: **Android SDK** si se compila la app Kotlin.

## 📁 Estructura del proyecto

```text
notasAlumnoAsignatura/
├── AlumnosApi/
│   ├── Controllers/            Alumnos, Asignaturas y Notas
│   ├── Models/                 Entidades EF Core
│   ├── Data/
│   │   ├── ALUMNOS.sql         Script original de la base de datos
│   │   ├── AlumnosContext.cs   DbContext
│   │   └── comandos/           Scripts .bat de operación
│   ├── BD/                     Copias ALUMNOS.mdf y ALUMNOS_log.ldf
│   ├── Program.cs              Configuración (HTTP 50555 + Swagger)
│   └── appsettings.json        Cadena de conexión
└── codigoKotlin/               Aplicación Android (Retrofit)
```

## 🗄️ Base de datos

La base de datos se llama **`ALUMNOS`** y la crea el script `Data/ALUMNOS.sql`. El script solo crea estructura: **no inserta registros**; los datos se ingresan desde Swagger o desde la app Android.

### Tablas

| Tabla | Columna | Tipo | Observación |
|---|---|---|---|
| `ALUMNO` | `COD_ALUMNO` | int | PK, no autogenerado |
| | `NOMBRES` | varchar(50) | Obligatorio |
| | `APELLIDOS` | varchar(50) | Obligatorio |
| `ASIGNATURA` | `ID_ASIGNATURA` | int | PK, identity |
| | `NOM_ASIGNATURA` | varchar(50) | Obligatorio, único |
| `NOTAS` | `ID_NOTA` | int | PK, identity |
| | `NOTA` | decimal(2,1) | Valor 1.0 a 7.0 |
| | `N_NOTA` | int | 1, 2 o 3 |
| | `COD_ALUMNO` | int | FK a `ALUMNO` |
| | `ID_ASIGNATURA` | int | FK a `ASIGNATURA` |

### Crear o cargar la BD

```powershell
cd C:\desarrollo\tnt\notasAlumnoAsignatura\AlumnosApi\Data\comandos
.\cargarBaseDatos.bat
```

Opciones manuales:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i "Data\ALUMNOS.sql"
```

> Si los archivos físicos ya no existen, ejecuta `crearBaseDatosAlumnos.bat` para generar una BD vacía desde el script.

## ▶️ Ejecutar la API

```powershell
cd C:\desarrollo\tnt\notasAlumnoAsignatura\AlumnosApi
dotnet run --urls http://localhost:50555
```

O, más simple, doble clic en:

```text
Data\comandos\ejecutarAPI.bat
```

- API: **http://localhost:50555**
- Swagger: **http://localhost:50555/swagger**
- JSON de Swagger: **http://localhost:50555/swagger/v1/swagger.json**

## 🔌 Endpoints

### Alumnos

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/alumnos/listar` | Lista todos los alumnos ordenados por código |
| POST | `/api/alumnos/grabar` | Graba un alumno nuevo |
| PUT | `/api/alumnos/editar` | Edita nombres y apellidos |
| DELETE | `/api/alumnos/borrar/{codAlumno}` | Borra el alumno y sus notas |

### Asignaturas

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/asignaturas/listar` | Lista todas las asignaturas |
| POST | `/api/asignaturas/grabar` | Graba una asignatura nueva |
| PUT | `/api/asignaturas/editar` | Edita el nombre |
| DELETE | `/api/asignaturas/borrar/{idAsignatura}` | Borra la asignatura y sus notas |

### Notas

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/notas/listar` | Lista todas las notas |
| GET | `/api/notas/listado` | Listado con notas y promedio |
| POST | `/api/notas/grabar` | Graba una nota (1 a 3) |
| PUT | `/api/notas/editar` | Edita el valor de una nota |
| DELETE | `/api/notas/borrar/{idNota}` | Borra una nota por id |
| DELETE | `/api/notas/borrarPorAlumnoAsignatura/{codAlumno}/{idAsignatura}` | Borra todas las notas de un alumno en una asignatura |

## 📦 Ejemplos de peticiones

**POST `/api/alumnos/grabar`**

```json
{ "codAlumno": 1, "nombres": "Juan", "apellidos": "Pérez" }
```

**POST `/api/asignaturas/grabar`**

```json
{ "nomAsignatura": "Matemáticas" }
```

**POST `/api/notas/grabar`** — el valor se envía como `nota`:

```json
{ "codAlumno": 1, "idAsignatura": 1, "nNota": 1, "nota": 6.5 }
```

**GET `/api/notas/listado`** — respuesta:

```json
[
  {
    "COD_ALUMNO": 1,
    "NOMBRE_COMPLETO": "Juan Pérez",
    "NOM_ASIGNATURA": "Matemáticas",
    "N_NOTA_1": 1, "NOTA_1": 6.5,
    "N_NOTA2": 2,  "NOTA_2": 7.0,
    "N_NOTA_3": 3, "NOTA_3": 5.8,
    "PROMEDIO": 6.4
  }
]
```

Los campos de notas no existentes se omiten y `PROMEDIO` solo aparece si están las tres notas.

## ✅ Validaciones

- Código de alumno > 0, nombres y apellidos obligatorios y sin duplicados.
- Nombre de asignatura obligatorio y único.
- `N_NOTA` debe ser 1, 2 o 3.
- Nota entre **1.0 y 7.0**, con máximo un decimal.
- No se puede repetir la misma nota para el mismo alumno/asignatura.
- Alumno y asignatura deben existir.
- Promedio: `(NOTA_1 + NOTA_2 + NOTA_3) / 3`, redondeado a un decimal, solo con las 3 notas.

Códigos de respuesta: `200 OK`, `400 Bad Request`, `404 Not Found`, `409 Conflict`, `500 Internal Server Error`.

## ⚙️ Comandos por lotes (.bat)

Ubicados en `AlumnosApi\Data\comandos` (el último en `AlumnosApi\BD`). Usan rutas relativas y funcionan en cualquier equipo.

| Comando | Qué hace |
|---|---|
| `crearBaseDatosAlumnos.bat` | Crea la BD vacía desde `ALUMNOS.sql` si no existe |
| `cargarBaseDatos.bat` | Carga la BD real desde la ruta del motor y la deja en `MULTI_USER` |
| `desconectarBase.bat` | Solo `sp_detach_db`; no copia, no mueve ni borra archivos |
| `borrarBaseDatosAlumnos.bat` | `DROP DATABASE`: borra la BD y sus archivos (cuidado) |
| `copiarBaseDatosAlumnosABD.bat` | Copia y sobrescribe los `.mdf`/`.ldf` en `BD\` |
| `respaldarBaseDatos.bat` | Alias del anterior |
| `ejecutarAPI.bat` | Ejecuta la API en `http://localhost:50555` |
| `verEstadoConexionBD.bat` | Diagnóstico: LocalDB, BD, registros, puerto y HTTP |
| `cargaSwagger.bat` | Abre Swagger si la API responde |
| `mostrarRutaMdfAlumnos.bat` | Muestra la ruta real del `.mdf` en vivo |
| `mostrarContenidoBD.bat` | Muestra tablas y registros de la BD en vivo |
| `BD\mostrarContenidoBDEnRutaActual.bat` | Muestra los datos del archivo `.mdf` de la carpeta `BD` |

### Orden recomendado

1. `cargarBaseDatos.bat`
2. `ejecutarAPI.bat`
3. `verEstadoConexionBD.bat` → debe decir `RESULTADO: TODO OK`
4. `cargaSwagger.bat`

## 🤖 Aplicación Android

El cliente Kotlin usa Retrofit con `http://127.0.0.1:50555/`. Con el celular por USB:

```powershell
cd C:\desarrollo\tnt\notasAlumnoAsignatura\codigoKotlin
.\conectaCelular.bat
adb reverse --list
```

Debe aparecer `UsbFfs tcp:50555 tcp:50555`. En emulador se usa `10.0.2.2` (detectado automáticamente).

## 🛠️ Solución de problemas

| Síntoma | Solución |
|---|---|
| `HTTP 000` en el diagnóstico | La API no está corriendo: ejecuta `ejecutarAPI.bat` |
| `Login timeout expired` / servidor no encontrado | LocalDB desincronizado: `cargarBaseDatos.bat` lo recupera |
| API responde 500 al consultar | BD no cargada o no en `MULTI_USER`: ejecuta `cargarBaseDatos.bat` |
| La API se cierra al compilar | El `.csproj` mata `AlumnosApi.exe` antes de compilar; vuelve a ejecutar `ejecutarAPI.bat` |
| `Scaffold-DbContext` no se reconoce | Instala `Microsoft.EntityFrameworkCore.Tools` 10.0.0 |

## 🧰 Tecnologías

.NET 10 · ASP.NET Core Web API · Entity Framework Core 10 · SQL Server LocalDB · Swagger/OpenAPI · Kotlin + Retrofit · Gradle

## 👥 Autores

Ignacio Gaete Caamaño · Jorge Vera Videla  
Asignatura: Taller de Nuevas Tecnologías — Prof. Roberto Cisterna Figueroa
