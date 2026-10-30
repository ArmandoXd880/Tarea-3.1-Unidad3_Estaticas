using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Unidad3.Estructuras;

namespace Unidad3.Aplicacion;

// La vista no almacena los elementos de las estructuras: consulta las clases propias.
// Las colecciones de Controls/Rows pertenecen al framework gráfico, no al algoritmo.
public sealed class FormPrincipal : Form
{
    private readonly TabControl pestañas = new TabControl();

    public FormPrincipal()
    {
        Text = "Unidad 3 | Pilas y colas estáticas";
        ClientSize = new Size(1080, 760);
        MinimumSize = new Size(1000, 740);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.White;
        AutoScaleMode = AutoScaleMode.Dpi;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        var titulo = new Label
        {
            Text = "Pilas y colas con arreglos\nTarea 3.1 · Capacidad fija de 5 enteros",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Dock = DockStyle.Fill, Padding = new Padding(16, 10, 0, 0)
        };
        pestañas.Dock = DockStyle.Fill;
        Agregar("Pila", new PilaEstatica(5), true,
            "LIFO: sale el último valor que entró. top señala la cima.");
        Agregar("Cola simple", new ColaSimple(5), false,
            "FIFO lineal: no reutiliza huecos al inicio hasta quedar vacía.");
        Agregar("Cola circular", new ColaCircular(5), false,
            "FIFO circular: (índice + 1) % capacidad permite reutilizar los huecos.");
        layout.Controls.Add(titulo, 0, 0);
        layout.Controls.Add(pestañas, 0, 1);
        layout.Controls.Add(new Label
        {
            Text = "Jesús Armando Zendejo Altamirano | Estructuras de Datos | Unidad 3",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter
        }, 0, 2);
        Controls.Add(layout);
    }

    private void Agregar(string nombre, IEstructuraEstatica estructura, bool pila, string ayuda)
    {
        var pagina = new TabPage(nombre) { BackColor = Color.White };
        pagina.Controls.Add(new PanelEstructura(estructura, pila, ayuda));
        pestañas.TabPages.Add(pagina);
    }
}

// Cada pestaña mantiene su propia estructura y un registro visible de operaciones.
internal sealed class PanelEstructura : UserControl
{
    private readonly IEstructuraEstatica estructura;
    private readonly bool esPila;
    private readonly TextBox entrada = new TextBox { Width = 120 };
    private readonly Label estado = new Label { Dock = DockStyle.Fill };
    private readonly Label mensaje = new Label { Dock = DockStyle.Fill };
    private readonly DataGridView tabla = new DataGridView();
    private readonly TextBox registro = new TextBox();
    private int paso;

    public PanelEstructura(IEstructuraEstatica estructura, bool esPila, string ayuda)
    {
        this.estructura = estructura;
        this.esPila = esPila;
        Dock = DockStyle.Fill;
        Padding = new Padding(12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 8, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 212));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.Controls.Add(new Label { Text = ayuda, Dock = DockStyle.Fill }, 0, 0);

        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        acciones.Controls.Add(new Label { Text = "Entero:", AutoSize = true, Margin = new Padding(0, 8, 5, 0) });
        entrada.AccessibleName = "Valor entero que se insertará";
        acciones.Controls.Add(entrada);
        Boton(acciones, esPila ? "Push" : "Enqueue", InsertarEntrada);
        Boton(acciones, esPila ? "Pop" : "Dequeue", () => Ejecutar("Retirar", () => "Sale " + estructura.Retirar()));
        Boton(acciones, "Peek", () => Ejecutar("Peek", () => "Siguiente: " + estructura.Peek()));
        Boton(acciones, "Limpiar", () => Ejecutar("Limpiar", () => { estructura.Limpiar(); return "Estructura vacía"; }));
        layout.Controls.Add(acciones, 0, 1);

        estado.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        layout.Controls.Add(estado, 0, 2);
        tabla.Dock = DockStyle.Fill;
        tabla.ReadOnly = true;
        tabla.AllowUserToAddRows = false;
        tabla.AllowUserToDeleteRows = false;
        tabla.AllowUserToResizeRows = false;
        tabla.RowHeadersVisible = false;
        tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tabla.BackgroundColor = Color.White;
        tabla.BorderStyle = BorderStyle.FixedSingle;
        tabla.ColumnHeadersHeight = 32;
        tabla.RowTemplate.Height = 32;
        tabla.Columns.Add("Indice", "Índice físico");
        tabla.Columns.Add("Valor", "Valor almacenado");
        tabla.Columns.Add("Marca", "Puntero de índice");
        tabla.Columns.Add("Ocupacion", "Ocupación");
        foreach (DataGridViewColumn columna in tabla.Columns)
            columna.SortMode = DataGridViewColumnSortMode.NotSortable;
        layout.Controls.Add(tabla, 0, 3);
        layout.Controls.Add(mensaje, 0, 4);
        layout.Controls.Add(new Label { Text = "Registro de operaciones", Dock = DockStyle.Fill }, 0, 5);
        registro.Multiline = true;
        registro.ReadOnly = true;
        registro.ScrollBars = ScrollBars.Vertical;
        registro.Dock = DockStyle.Fill;
        registro.Font = new Font("Consolas", 9F);
        layout.Controls.Add(registro, 0, 6);

        var herramientas = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        Boton(herramientas, "Cargar demostración", Demostrar);
        Boton(herramientas, "Guardar captura PNG", GuardarCaptura);
        Boton(herramientas, "Guardar registro TXT", GuardarRegistro);
        layout.Controls.Add(herramientas, 0, 7);
        Controls.Add(layout);
        Refrescar();
        mensaje.Text = "Listo. Los guiones indican posiciones sin elemento.";
    }

    private static void Boton(FlowLayoutPanel panel, string texto, Action accion)
    {
        var boton = new Button { Text = texto, AutoSize = true, Height = 34, Padding = new Padding(5, 0, 5, 0) };
        boton.Click += (_, _) => accion();
        panel.Controls.Add(boton);
    }

    private void InsertarEntrada()
    {
        // TryParse acepta negativos y cero, y rechaza texto y valores fuera de int.
        if (!int.TryParse(entrada.Text, out int valor))
        {
            Informar("Entrada inválida: escribe un entero de 32 bits.", true);
            return;
        }
        Ejecutar("Insertar " + valor, () => { estructura.Insertar(valor); return "Insertado " + valor; });
        entrada.SelectAll();
        entrada.Focus();
    }

    private void Ejecutar(string operacion, Func<string> accion)
    {
        try { Informar(operacion + " -> " + accion(), false); }
        catch (InvalidOperationException ex) { Informar(operacion + " -> " + ex.Message, true); }
        Refrescar(); // También mostrar el estado preservado después de un rechazo.
    }

    private void Informar(string texto, bool error)
    {
        mensaje.ForeColor = error ? Color.DarkRed : Color.DarkGreen;
        mensaje.Text = texto;
        // Limitar el registro visual evita un crecimiento indefinido de texto.
        if (registro.TextLength > 16000) registro.Clear();
        registro.AppendText($"{++paso:00}. {texto} | {estructura.Indices} | cantidad={estructura.Cantidad}\r\n");
    }

    private void Refrescar()
    {
        estado.Text = $"{estructura.Indices}   |   Cantidad: {estructura.Cantidad}/{estructura.Capacidad}\n" +
            $"isEmpty: {estructura.IsEmpty()}   |   isFull: {estructura.IsFull()}";
        tabla.Rows.Clear();
        for (int i = 0; i < estructura.Capacidad; i++)
        {
            bool ocupada = estructura.Ocupada(i);
            int fila = tabla.Rows.Add(i, ocupada ? estructura.ValorEn(i).ToString() : "—",
                estructura.MarcaEn(i), ocupada ? "Ocupada" : "Libre");
            tabla.Rows[fila].DefaultCellStyle.BackColor = ocupada ? Color.FromArgb(225, 238, 250) : Color.White;
        }
        tabla.ClearSelection();
    }

    private void Demostrar()
    {
        // Una demostración reinicia exclusivamente la pestaña activa.
        estructura.Limpiar(); registro.Clear(); paso = 0;
        for (int valor = 10; valor <= (esPila ? 30 : 50); valor += 10)
        {
            int dato = valor;
            Ejecutar("Insertar " + dato, () => { estructura.Insertar(dato); return "Correcto"; });
        }
        Ejecutar("Retirar", () => "Sale " + estructura.Retirar());
        if (!esPila)
        {
            Ejecutar("Retirar", () => "Sale " + estructura.Retirar());
            Ejecutar("Insertar 60", () => { estructura.Insertar(60); return "Correcto"; });
            if (estructura is ColaCircular)
                Ejecutar("Insertar 70", () => { estructura.Insertar(70); return "Correcto"; });
        }
    }

    private void GuardarCaptura()
    {
        using var dialogo = new SaveFileDialog { Filter = "Imagen PNG|*.png", FileName = estructura.GetType().Name + ".png" };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            // Capturar el formulario real, después de cerrar el cuadro de guardado.
            Form formulario = FindForm()!;
            formulario.Refresh();
            using var imagen = new Bitmap(formulario.Width, formulario.Height);
            formulario.DrawToBitmap(imagen, new Rectangle(Point.Empty, formulario.Size));
            imagen.Save(dialogo.FileName, ImageFormat.Png);
            Informar("Captura guardada.", false);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Runtime.InteropServices.ExternalException)
        { Informar("No se pudo guardar la captura: " + ex.Message, true); }
    }

    private void GuardarRegistro()
    {
        using var dialogo = new SaveFileDialog { Filter = "Texto|*.txt", FileName = estructura.GetType().Name + "_registro.txt" };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        try { File.WriteAllText(dialogo.FileName, registro.Text); Informar("Registro guardado.", false); }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        { Informar("No se pudo guardar el registro: " + ex.Message, true); }
    }
}
