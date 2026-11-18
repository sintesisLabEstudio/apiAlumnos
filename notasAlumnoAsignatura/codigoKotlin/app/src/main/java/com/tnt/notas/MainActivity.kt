package com.tnt.notas
import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_menu)
        findViewById<Button>(R.id.btnAlumno).setOnClickListener {
            startActivity(Intent(this, AlumnoActivity::class.java))
        }
        findViewById<Button>(R.id.btnAsignatura).setOnClickListener {
            startActivity(Intent(this, AsignaturaActivity::class.java))
        }
        findViewById<Button>(R.id.btnNota).setOnClickListener {
            startActivity(Intent(this, NotaActivity::class.java))
        }
        findViewById<Button>(R.id.btnListado).setOnClickListener {
            startActivity(Intent(this, ListadoActivity::class.java))
        }
        findViewById<Button>(R.id.btnListaAlumnos).setOnClickListener {
            val intent = Intent(this, ListaAlumnosActivity::class.java)
            intent.putExtra("modoEdicion", false)
            startActivity(intent)
        }
        findViewById<Button>(R.id.btnEditarAlumno).setOnClickListener {
            val intent = Intent(this, ListaAlumnosActivity::class.java)
            intent.putExtra("modoEdicion", true)
            startActivity(intent)
        }
        findViewById<Button>(R.id.btnListaAsignaturas).setOnClickListener {
            val intent = Intent(this, ListaAsignaturasActivity::class.java)
            intent.putExtra("modoEdicion", false)
            startActivity(intent)
        }
        findViewById<Button>(R.id.btnEditarAsignatura).setOnClickListener {
            val intent = Intent(this, ListaAsignaturasActivity::class.java)
            intent.putExtra("modoEdicion", true)
            startActivity(intent)
        }
        findViewById<Button>(R.id.btnEditarNota).setOnClickListener {
            startActivity(Intent(this, EditarNotaActivity::class.java))
        }
        findViewById<Button>(R.id.btnResumen).setOnClickListener {
            startActivity(Intent(this, ResumenActivity::class.java))
        }
        findViewById<Button>(R.id.btnSalir).setOnClickListener {
            finishAffinity()
        }
    }
}
