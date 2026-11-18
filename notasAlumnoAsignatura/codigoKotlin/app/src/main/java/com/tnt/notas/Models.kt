package com.tnt.notas
import com.google.gson.annotations.SerializedName
data class AlumnoDto(
    val codAlumno: Int,
    val nombres: String,
    val apellidos: String
)
data class AsignaturaNueva(
    val nomAsignatura: String
)
data class AsignaturaDto(
    val idAsignatura: Int,
    val nomAsignatura: String
)
data class NotaDto(
    val codAlumno: Int,
    val idAsignatura: Int,
    val nNota: Int,
    val nota: Double
)
data class ListadoNota(
    @SerializedName("COD_ALUMNO") val codAlumno: Int?,
    @SerializedName("NOMBRE_COMPLETO") val nombreCompleto: String?,
    @SerializedName("NOM_ASIGNATURA") val nomAsignatura: String?,
    @SerializedName("NOTA_1") val nota1: Double?,
    @SerializedName("NOTA_2") val nota2: Double?,
    @SerializedName("NOTA_3") val nota3: Double?,
    @SerializedName("PROMEDIO") val promedio: Double?
)
