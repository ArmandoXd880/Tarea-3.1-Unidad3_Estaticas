using System;

namespace Unidad3.Estructuras;

// Cola FIFO lineal: no desplaza datos ni reutiliza huecos mientras haya datos.
public sealed class ColaSimple : IEstructuraEstatica
{
    private readonly int[] datos;
    private int front = 0;
    private int rear = -1; // Última posición ocupada; no la próxima libre.
    public int Front => front;
    public int Rear => rear;
    public int Capacidad => datos.Length;
    public int Cantidad => rear - front + 1;
    public string Indices => $"front = {front}   rear = {rear}";

    public ColaSimple(int capacidad)
    {
        if (capacidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidad));
        datos = new int[capacidad];
    }

    public bool IsEmpty() => rear < front;
    // Llena para insertar: puede haber huecos anteriores a front.
    public bool IsFull() => rear == Capacidad - 1;

    public void Enqueue(int valor)
    {
        if (IsFull())
            throw new InvalidOperationException("Cola simple sin espacio al final.");
        rear++;
        datos[rear] = valor;
    }

    public int Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Cola simple vacía.");
        int valor = datos[front];
        datos[front] = 0;
        front++;
        // Al retirar el último dato, recuperar todo el arreglo sin desplazamientos.
        if (front > rear) { front = 0; rear = -1; }
        return valor;
    }

    public int Peek()
    {
        if (IsEmpty()) throw new InvalidOperationException("Cola simple vacía.");
        return datos[front];
    }

    public void Limpiar()
    {
        for (int i = 0; i < Capacidad; i++) datos[i] = 0;
        front = 0; rear = -1;
    }

    public void Insertar(int valor) => Enqueue(valor);
    public int Retirar() => Dequeue();
    public bool Ocupada(int indice)
    {
        ValidarIndice(indice);
        return indice >= front && indice <= rear;
    }
    public int ValorEn(int indice) { ValidarIndice(indice); return datos[indice]; }
    public string MarcaEn(int indice)
    {
        if (!Ocupada(indice)) return "";
        return (indice == front ? "front " : "") + (indice == rear ? "rear" : "");
    }
    private void ValidarIndice(int indice)
    {
        if (indice < 0 || indice >= Capacidad)
            throw new ArgumentOutOfRangeException(nameof(indice));
    }
}
