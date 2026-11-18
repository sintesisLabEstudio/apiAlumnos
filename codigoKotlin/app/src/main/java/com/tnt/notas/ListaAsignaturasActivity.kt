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
class ListaAsignaturasActivity : AppCompatActivity() {
    var asignaturas: List<AsignaturaDto> = emptyList()
    var seleccionada: AsignaturaDto? = null
    var modoEdicion: Boolean = false
    lateinit var lvAsignaturas: ListView
    lateinit var tvTitulo: TextView
    lateinit var tvId: TextView
    lateinit var etNombre: EditText
    lateinit var tvResultado: TextView
    lateinit var btnEditarAsignatura: Button
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lista_asignaturas)
        findViewById<Button>(R.id.btnHome).setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            startActivity(intent)
            finish()
        }
        modoEdicion = intent.getBooleanExtra("modoEdicion", false)
        lvAsignaturas = findViewById(R.id.lvAsignaturas)
        tvTitulo = findViewById(R.id.tvTitulo)
        tvId = findViewById(R.id.tvId)
        etNombre = findViewById(R.id.etNombre)
        tvResultado = findViewById(R.id.tvResultado)
        btnEditarAsignatura = findViewById(R.id.btnEditarAsignatura)
        if (modoEdicion) {
            tvTitulo.text = "Editar asignatura (toque una para editar)"
            tvId.visibility = View.VISIBLE
            etNombre.visibility = View.VISIBLE
            btnEditarAsignatura.visibility = View.VISIBLE
            lvAsignaturas.setOnItemClickListener { _, _, posicion, _ ->
                val s = asignaturas[posicion]
                seleccionada = s
                tvId.text = "Id: " + s.idAsignatura
                etNombre.setText(s.nomAsignatura)
                tvResultado.text = "Asignatura seleccionada: " + s.idAsignatura
            }
            btnEditarAsignatura.setOnClickListener { editar() }
        } else {
            tvTitulo.text = "Listado de asignaturas"
            tvId.visibility = View.GONE
            etNombre.visibility = View.GONE
            btnEditarAsignatura.visibility = View.GONE
            lvAsignaturas.setOnItemClickListener(null)
        }
        findViewById<Button>(R.id.btnActualizar).setOnClickListener { cargar() }
        cargar()
    }
    fun cargar() {
        lifecycleScope.launch {
            try {
                val r = withContext(Dispatchers.IO) { RetrofitClient.api.listarAsignaturas() }
                if (!r.isSuccessful) {
                    tvResultado.text = "Error " + r.code()
                    return@launch
                }
                asignaturas = r.body() ?: emptyList()
                val textos = asignaturas.map { it.idAsignatura.toString() + " - " + it.nomAsignatura }
                lvAsignaturas.adapter = ArrayAdapter(this@ListaAsignaturasActivity, android.R.layout.simple_list_item_1, textos)
                if (asignaturas.isEmpty()) {
                    tvResultado.text = "Aun no hay asignaturas ingresadas"
                } else {
                    tvResultado.text = "Asignaturas: " + asignaturas.size
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
        val s = seleccionada
        if (s == null) {
            tvResultado.text = "Seleccione una asignatura de la lista"
            return
        }
        val nombre = etNombre.text.toString().trim()
        if (nombre == "") {
            tvResultado.text = "Ingrese el nombre de la asignatura"
            return
        }
        lifecycleScope.launch {
            tvResultado.text = "Enviando..."
            val mensaje = RetrofitClient.guardar {
                RetrofitClient.api.editarAsignatura(AsignaturaDto(s.idAsignatura, nombre))
            }
            tvResultado.text = mensaje
            if (mensaje == "Guardado correctamente") {
                seleccionada = null
                tvId.text = "Id: -"
                etNombre.text.clear()
                cargar()
            }
        }
    }
}
