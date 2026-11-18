package com.tnt.notas
import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.EditText
import android.widget.Spinner
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
class EditarNotaActivity : AppCompatActivity() {
    var alumnos: List<AlumnoDto> = emptyList()
    var asignaturas: List<AsignaturaDto> = emptyList()
    var listadoNotas: List<ListadoNota> = emptyList()
    lateinit var spAlumno: Spinner
    lateinit var spAsignatura: Spinner
    lateinit var spNumero: Spinner
    lateinit var etNota: EditText
    lateinit var tvResultado: TextView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_editar_nota)
        findViewById<Button>(R.id.btnHome).setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            startActivity(intent)
            finish()
        }
        spAlumno = findViewById(R.id.spAlumno)
        spAsignatura = findViewById(R.id.spAsignatura)
        spNumero = findViewById(R.id.spNumero)
        etNota = findViewById(R.id.etNota)
        tvResultado = findViewById(R.id.tvResultado)
        val numeros = ArrayAdapter(this, android.R.layout.simple_spinner_item, listOf("1", "2", "3"))
        numeros.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        spNumero.adapter = numeros
        val listener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: AdapterView<*>?, view: View?, position: Int, id: Long) {
                mostrarNotaCorrespondiente()
            }
            override fun onNothingSelected(parent: AdapterView<*>?) {
            }
        }
        spAlumno.onItemSelectedListener = listener
        spAsignatura.onItemSelectedListener = listener
        spNumero.onItemSelectedListener = listener
        findViewById<Button>(R.id.btnEditarNota).setOnClickListener { editarNota() }
        cargarDatos()
    }
    fun cargarDatos() {
        lifecycleScope.launch {
            try {
                val respAlumnos = withContext(Dispatchers.IO) { RetrofitClient.api.listarAlumnos() }
                val respAsignaturas = withContext(Dispatchers.IO) { RetrofitClient.api.listarAsignaturas() }
                val respListado = withContext(Dispatchers.IO) { RetrofitClient.api.listado() }
                if (!respAlumnos.isSuccessful || !respAsignaturas.isSuccessful) {
                    tvResultado.text = "No se pudieron cargar alumnos y asignaturas"
                    return@launch
                }
                alumnos = respAlumnos.body() ?: emptyList()
                asignaturas = respAsignaturas.body() ?: emptyList()
                listadoNotas = if (respListado.isSuccessful) respListado.body() ?: emptyList() else emptyList()
                val textosAlumnos = alumnos.map { it.codAlumno.toString() + " - " + it.nombres + " " + it.apellidos }
                val textosAsignaturas = asignaturas.map { it.nomAsignatura }
                val adA = ArrayAdapter(this@EditarNotaActivity, android.R.layout.simple_spinner_item, textosAlumnos)
                adA.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                spAlumno.adapter = adA
                val adS = ArrayAdapter(this@EditarNotaActivity, android.R.layout.simple_spinner_item, textosAsignaturas)
                adS.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                spAsignatura.adapter = adS
                mostrarNotaCorrespondiente()
            } catch (e: Exception) {
                tvResultado.text = "No se pudo conectar con la API: " + e.message
            }
        }
    }
    fun mostrarNotaCorrespondiente() {
        if (alumnos.isEmpty() || asignaturas.isEmpty()) {
            return
        }
        val codAlumno = alumnos[spAlumno.selectedItemPosition].codAlumno
        val nomAsignatura = asignaturas[spAsignatura.selectedItemPosition].nomAsignatura
        val nNota = spNumero.selectedItem.toString().toInt()
        val fila = listadoNotas.find { it.codAlumno == codAlumno && it.nomAsignatura == nomAsignatura }
        val valor = when (nNota) {
            1 -> fila?.nota1
            2 -> fila?.nota2
            3 -> fila?.nota3
            else -> null
        }
        if (valor != null) {
            etNota.setText(valor.toString())
            tvResultado.text = "Nota actual cargada para la seleccion"
        } else {
            etNota.setText("")
            tvResultado.text = "No hay nota registrada para la seleccion"
        }
    }
    fun editarNota() {
        if (alumnos.isEmpty() || asignaturas.isEmpty()) {
            tvResultado.text = "Primero ingrese al menos un alumno y una asignatura"
            return
        }
        val valor = etNota.text.toString().replace(",", ".").toDoubleOrNull()
        if (valor == null || valor < 1.0 || valor > 7.0) {
            tvResultado.text = "La nota debe estar entre 1.0 y 7.0"
            return
        }
        val nota = NotaDto(
            alumnos[spAlumno.selectedItemPosition].codAlumno,
            asignaturas[spAsignatura.selectedItemPosition].idAsignatura,
            spNumero.selectedItem.toString().toInt(),
            valor
        )
        lifecycleScope.launch {
            tvResultado.text = "Enviando..."
            val mensaje = RetrofitClient.guardar {
                RetrofitClient.api.editarNota(nota)
            }
            tvResultado.text = mensaje
            if (mensaje == "Guardado correctamente") {
                val respListado = withContext(Dispatchers.IO) { RetrofitClient.api.listado() }
                if (respListado.isSuccessful) {
                    listadoNotas = respListado.body() ?: emptyList()
                    mostrarNotaCorrespondiente()
                }
            }
        }
    }
}
