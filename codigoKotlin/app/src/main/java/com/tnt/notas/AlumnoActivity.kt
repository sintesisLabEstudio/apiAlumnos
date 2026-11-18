package com.tnt.notas
import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.launch
class AlumnoActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_alumno)
        findViewById<Button>(R.id.btnHome).setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            startActivity(intent)
            finish()
        }
        val etCodigo = findViewById<EditText>(R.id.etCodigo)
        val etNombres = findViewById<EditText>(R.id.etNombres)
        val etApellidos = findViewById<EditText>(R.id.etApellidos)
        val btnGuardar = findViewById<Button>(R.id.btnGuardar)
        val tvResultado = findViewById<TextView>(R.id.tvResultado)
        btnGuardar.setOnClickListener {
            val codigo = etCodigo.text.toString().toIntOrNull()
            val nombres = etNombres.text.toString().trim()
            val apellidos = etApellidos.text.toString().trim()
            if (codigo == null || nombres == "" || apellidos == "") {
                tvResultado.text = "Complete codigo, nombres y apellidos"
                return@setOnClickListener
            }
            val alumno = AlumnoDto(codigo, nombres, apellidos)
            lifecycleScope.launch {
                tvResultado.text = "Enviando..."
                val mensaje = RetrofitClient.guardar { RetrofitClient.api.crearAlumno(alumno) }
                tvResultado.text = mensaje
            }
        }
    }
}
