package com.tnt.notas
import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.EditText
import android.widget.ListView
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
class ListaAlumnosActivity : AppCompatActivity() {
    var alumnos: List<AlumnoDto> = emptyList()
    var seleccionado: AlumnoDto? = null
    var modoEdicion: Boolean = false
    lateinit var lvAlumnos: ListView
    lateinit var tvTitulo: TextView
    lateinit var tvCodigo: TextView
    lateinit var etNombres: EditText
    lateinit var etApellidos: EditText
    lateinit var tvResultado: TextView
    lateinit var btnEditarAlumno: Button
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lista_alumnos)
        findViewById<Button>(R.id.btnHome).setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            startActivity(intent)
            finish()
        }
        modoEdicion = intent.getBooleanExtra("modoEdicion", false)
        lvAlumnos = findViewById(R.id.lvAlumnos)
        tvTitulo = findViewById(R.id.tvTitulo)
        tvCodigo = findViewById(R.id.tvCodigo)
        etNombres = findViewById(R.id.etNombres)
        etApellidos = findViewById(R.id.etApellidos)
        tvResultado = findViewById(R.id.tvResultado)
        btnEditarAlumno = findViewById(R.id.btnEditarAlumno)
        if (modoEdicion) {
            tvTitulo.text = "Editar alumno (toque uno para editar)"
            tvCodigo.visibility = View.VISIBLE
            etNombres.visibility = View.VISIBLE
            etApellidos.visibility = View.VISIBLE
            btnEditarAlumno.visibility = View.VISIBLE
            lvAlumnos.setOnItemClickListener { _, _, posicion, _ ->
                val a = alumnos[posicion]
                seleccionado = a
                tvCodigo.text = "Codigo: " + a.codAlumno
                etNombres.setText(a.nombres)
                etApellidos.setText(a.apellidos)
                tvResultado.text = "Alumno seleccionado: " + a.codAlumno
            }
            btnEditarAlumno.setOnClickListener { editar() }
        } else {
            tvTitulo.text = "Listado de alumnos"
            tvCodigo.visibility = View.GONE
            etNombres.visibility = View.GONE
            etApellidos.visibility = View.GONE
            btnEditarAlumno.visibility = View.GONE
            lvAlumnos.setOnItemClickListener(null)
        }
        findViewById<Button>(R.id.btnActualizar).setOnClickListener { cargar() }
        cargar()
    }
    fun cargar() {
        lifecycleScope.launch {
            try {
                val r = withContext(Dispatchers.IO) { RetrofitClient.api.listarAlumnos() }
                if (!r.isSuccessful) {
                    tvResultado.text = "Error " + r.code()
                    return@launch
                }
                alumnos = r.body() ?: emptyList()
                val textos = alumnos.map { it.codAlumno.toString() + " - " + it.nombres + " " + it.apellidos }
                lvAlumnos.adapter = ArrayAdapter(this@ListaAlumnosActivity, android.R.layout.simple_list_item_1, textos)
                if (alumnos.isEmpty()) {
                    tvResultado.text = "Aun no hay alumnos ingresados"
                } else {
                    tvResultado.text = "Alumnos: " + alumnos.size
                }
            } catch (e: Exception) {
                tvResultado.text = "No se pudo conectar con la API: " + e.message
            }
        }
    }
    fun editar() {
        if (!modoEdicion) {
            return
        }
        val a = seleccionado
        if (a == null) {
            tvResultado.text = "Seleccione un alumno de la lista"
            return
        }
        val nombres = etNombres.text.toString().trim()
        val apellidos = etApellidos.text.toString().trim()
        if (nombres == "" || apellidos == "") {
            tvResultado.text = "Complete nombres y apellidos"
            return
        }
        lifecycleScope.launch {
            tvResultado.text = "Enviando..."
            val mensaje = RetrofitClient.guardar {
                RetrofitClient.api.editarAlumno(AlumnoDto(a.codAlumno, nombres, apellidos))
            }
            tvResultado.text = mensaje
            if (mensaje == "Guardado correctamente") {
                seleccionado = null
                tvCodigo.text = "Codigo: -"
                etNombres.text.clear()
                etApellidos.text.clear()
                cargar()
            }
        }
    }
}
