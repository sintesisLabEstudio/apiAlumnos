package com.tnt.notas
import okhttp3.ResponseBody
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT

interface ApiService {
    // Devuelve todos los alumnos ordenados por código.
    @GET("api/alumnos/listar")
    suspend fun listarAlumnos(): Response<List<AlumnoDto>>

    // Guarda un alumno nuevo con código, nombres y apellidos.
    @POST("api/alumnos/grabar")
    suspend fun crearAlumno(@Body alumno: AlumnoDto): Response<ResponseBody>

    // Devuelve todas las asignaturas ordenadas por id.
    @GET("api/asignaturas/listar")
    suspend fun listarAsignaturas(): Response<List<AsignaturaDto>>

    // Guarda una asignatura nueva; el id lo genera la BD.
    @POST("api/asignaturas/grabar")
    suspend fun crearAsignatura(@Body asignatura: AsignaturaNueva): Response<ResponseBody>

    // Graba una nota (1 a 3) entre 1.0 y 7.0 para un alumno y asignatura.
    @POST("api/notas/grabar")
    suspend fun crearNota(@Body nota: NotaDto): Response<ResponseBody>

    // Modifica el valor de una nota existente.
    @PUT("api/notas/editar")
    suspend fun editarNota(@Body nota: NotaDto): Response<ResponseBody>

    // Devuelve alumnos con sus notas y promedio cuando están las tres.
    @GET("api/notas/listado")
    suspend fun listado(): Response<List<ListadoNota>>

    // Modifica nombres y apellidos de un alumno existente.
    @PUT("api/alumnos/editar")
    suspend fun editarAlumno(@Body alumno: AlumnoDto): Response<ResponseBody>

    // Modifica el nombre de una asignatura existente.
    @PUT("api/asignaturas/editar")
    suspend fun editarAsignatura(@Body asignatura: AsignaturaDto): Response<ResponseBody>
}
