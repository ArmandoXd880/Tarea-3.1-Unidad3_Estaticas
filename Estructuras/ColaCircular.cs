using System;

namespace Unidad3.Estructuras;

// Cola FIFO circular: los índices vuelven a cero al llegar al final.
public sealed class ColaCircular : IEstructuraEstatica
{
    private readonly int[] datos;
    private int front = 0;
    private int rear = -1;
    private int cantidad = 0; // Distingue lleno y vacío sin reservar una celda.
    public int Front => front;
    public int Rear => rear;
    public int Capacidad => datos.Length;
    public int Cantidad => cantidad;
    public string Indices => $"front = {front}   rear = {rear}";

    public ColaCircular(int capacidad)
    {
        if (capacidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidad));
        datos = new int[capacidad];
    }

    public bool IsEmpty() => cantidad == 0;
    public bool IsFull() => cantidad == Capacidad;

    public void Enqueue(int valor)
    {
        if (IsFull()) throw new InvalidOperationException("Cola circular llena.");
        rear = (rear + 1) % Capacidad; // Por ejemplo, (4 + 1) % 5 = 0.
        datos[rear] = valor;
        cantidad++;
    }

    public int Dequeue()
    {
        if (IsEmpty()) throw new InvalidOperationException("Cola circular vacía.");
        int valor = datos[front];
        datos[front] = 0;
        front = (front + 1) % Capacidad;
        cantidad--;
        // Normalizar el estado vacío facilita su lectura en la pantalla.
        if (cantidad == 0) { front = 0; rear = -1; }
        return valor;
    }

    public int Peek()
    {
        if (IsEmpty()) throw new InvalidOperationException("Cola circular vacía.");
        return datos[front];
    }

    public void Limpiar()
    {
        for (int i = 0; i < Capacidad; i++) datos[i] = 0;
        front = 0; rear = -1; cantidad = 0;
    }

    public void Insertar(int valor) => Enqueue(valor);
    public int Retirar() => Dequeue();
    public bool Ocupada(int indice)
    {
        ValidarIndice(indice);
        // Distancia circular desde front: pertenece si está dentro de cantidad.
        return (indice - front + Capacidad) % Capacidad < cantidad;
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
