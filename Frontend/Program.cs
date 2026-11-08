#nullable enable
using System;
using System.Collections.Generic;

class Node<T>
{
    public T Value { get; set; }
    public Node<T>? Next { get; set; }
    public Node<T>? Prev { get; set; }

    public Node(T value)
    {
        Value = value;
    }
}

class DoublyLinkedList<T> where T : IComparable<T>
{
    private Node<T>? head;
    private Node<T>? tail;
    private bool _descending = false;

    public int Count { get; private set; }

    // Inserción ordenada. Si la lista está ascendente, inserta ascendente.
    // Si ya fue ordenada descendentemente, mantiene el orden descendente.
    public void Add(T value)
    {
        var newNode = new Node<T>(value);

        if (head == null)
        {
            head = tail = newNode;
            Count = 1;
            _descending = false;
            return;
        }

        if (!_descending)
        {
            // Orden ascendente
            Node<T>? current = head;
            while (current != null && current.Value.CompareTo(value) <= 0)
                current = current.Next;

            if (current == null)
                InsertAfter(tail!, newNode);
            else
                InsertBefore(current, newNode);
        }
        else
        {
            // Orden descendente
            Node<T>? current = head;
            while (current != null && current.Value.CompareTo(value) >= 0)
                current = current.Next;

            if (current == null)
                InsertAfter(tail!, newNode);
            else
                InsertBefore(current, newNode);
        }

        Count++;
    }

    private void InsertBefore(Node<T> node, Node<T> newNode)
    {
        newNode.Next = node;
        newNode.Prev = node.Prev;

        if (node.Prev != null)
            node.Prev.Next = newNode;
        else
            head = newNode;

        node.Prev = newNode;
    }

    private void InsertAfter(Node<T> node, Node<T> newNode)
    {
        newNode.Prev = node;
        newNode.Next = node.Next;

        if (node.Next != null)
            node.Next.Prev = newNode;
        else
            tail = newNode;

        node.Next = newNode;
    }

    public void ShowForward()
    {
        if (head == null)
        {
            Console.WriteLine("Lista vacía.");
            return;
        }

        var current = head;
        while (current != null)
        {
            Console.Write(current.Value);
            if (current.Next != null) Console.Write(" <-> ");
            current = current.Next;
        }
        Console.WriteLine();
    }

    public void ShowBackward()
    {
        if (tail == null)
        {
            Console.WriteLine("Lista vacía.");
            return;
        }

        var current = tail;
        while (current != null)
        {
            Console.Write(current.Value);
            if (current.Prev != null) Console.Write(" <-> ");
            current = current.Prev;
        }
        Console.WriteLine();
    }

    // Cambia físicamente el orden de la lista, no solo la muestra al revés.
    public void SortDescending()
    {
        if (_descending) return;

        Reverse();
        _descending = true;
    }

    private void Reverse()
    {
        var current = head;

        while (current != null)
        {
            var temp = current.Prev;
            current.Prev = current.Next;
            current.Next = temp;
            current = current.Prev;
        }

        var tempHead = head;
        head = tail;
        tail = tempHead;
    }

    public List<T> GetModes()
    {
        var modes = new List<T>();
        if (head == null) return modes;

        var current = head;
        int maxCount = 0;

        while (current != null)
        {
            int count = 1;
            var next = current.Next;

            while (next != null &&
                   EqualityComparer<T>.Default.Equals(current.Value, next.Value))
            {
                count++;
                next = next.Next;
            }

            if (count > maxCount)
            {
                maxCount = count;
                modes.Clear();
                modes.Add(current.Value);
            }
            else if (count == maxCount)
            {
                modes.Add(current.Value);
            }

            current = next;
        }

        return modes;
    }

    public void ShowModes()
    {
        var modes = GetModes();

        if (modes.Count == 0)
        {
            Console.WriteLine("Lista vacía. No hay moda.");
            return;
        }

        if (modes.Count == 1)
            Console.WriteLine($"La moda es {modes[0]}.");
        else
            Console.WriteLine("Las modas son: " + string.Join(", ", modes));
    }

    public void ShowGraph()
    {
        if (head == null)
        {
            Console.WriteLine("Lista vacía.");
            return;
        }

        var current = head;

        while (current != null)
        {
            int count = 1;
            var next = current.Next;

            while (next != null &&
                   EqualityComparer<T>.Default.Equals(current.Value, next.Value))
            {
                count++;
                next = next.Next;
            }

            Console.WriteLine($"{current.Value} {new string('*', count)}");
            current = next;
        }
    }

    public bool Exists(T value)
    {
        var current = head;

        while (current != null)
        {
            int cmp = current.Value.CompareTo(value);

            if (cmp == 0) return true;
            if (!_descending && cmp > 0) return false;
            if (_descending && cmp < 0) return false;

            current = current.Next;
        }

        return false;
    }

    public bool RemoveOne(T value)
    {
        var current = head;

        while (current != null)
        {
            int cmp = current.Value.CompareTo(value);

            if (cmp == 0)
            {
                RemoveNode(current);
                return true;
            }

            if (!_descending && cmp > 0) return false;
            if (_descending && cmp < 0) return false;

            current = current.Next;
        }

        return false;
    }

    public int RemoveAll(T value)
    {
        int removed = 0;
        var current = head;

        while (current != null)
        {
            var next = current.Next;
            int cmp = current.Value.CompareTo(value);

            if (cmp == 0)
            {
                RemoveNode(current);
                removed++;
            }
            else if (!_descending && cmp > 0)
            {
                break;
            }
            else if (_descending && cmp < 0)
            {
                break;
            }

            current = next;
        }

        return removed;
    }

    private void RemoveNode(Node<T> node)
    {
        if (node.Prev != null)
            node.Prev.Next = node.Next;
        else
            head = node.Next;

        if (node.Next != null)
            node.Next.Prev = node.Prev;
        else
            tail = node.Prev;

        Count--;

        if (Count == 0)
            _descending = false;
    }
}

class Program
{
    static void Main()
    {
        // Lista genérica; en este ejemplo se usa int.
        var list = new DoublyLinkedList<int>();

        int opcion;

        do
        {
            Console.WriteLine();
            Console.WriteLine("===== MENÚ LISTA DOBLEMENTE LIGADA =====");
            Console.WriteLine("1. Adicionar");
            Console.WriteLine("2. Mostrar hacia adelante");
            Console.WriteLine("3. Mostrar hacia atrás");
            Console.WriteLine("4. Ordenar descendentemente");
            Console.WriteLine("5. Mostrar la(s) moda(s)");
            Console.WriteLine("6. Mostrar gráfico");
            Console.WriteLine("7. Existe");
            Console.WriteLine("8. Eliminar una ocurrencia");
            Console.WriteLine("9. Eliminar todas las ocurrencias");
            Console.WriteLine("0. Salir");

            opcion = LeerEntero("Seleccione una opción: ");

            switch (opcion)
            {
                case 1:
                    int valorAdd = LeerEntero("Valor a adicionar: ");
                    list.Add(valorAdd);
                    Console.WriteLine("Elemento adicionado.");
                    break;

                case 2:
                    Console.WriteLine("Lista hacia adelante:");
                    list.ShowForward();
                    break;

                case 3:
                    Console.WriteLine("Lista hacia atrás:");
                    list.ShowBackward();
                    break;

                case 4:
                    list.SortDescending();
                    Console.WriteLine("Lista ordenada descendentemente.");
                    break;

                case 5:
                    list.ShowModes();
                    break;

                case 6:
                    Console.WriteLine("Gráfico de ocurrencias:");
                    list.ShowGraph();
                    break;

                case 7:
                    int valorExiste = LeerEntero("Valor a buscar: ");
                    Console.WriteLine(
                        list.Exists(valorExiste)
                            ? $"El {valorExiste} sí existe."
                            : $"El {valorExiste} no existe."
                    );
                    break;

                case 8:
                    int valorEliminar = LeerEntero("Valor a eliminar (una ocurrencia): ");
                    Console.WriteLine(
                        list.RemoveOne(valorEliminar)
                            ? "Se eliminó una ocurrencia."
                            : "No se encontró el valor."
                    );
                    break;

                case 9:
                    int valorEliminarTodas = LeerEntero("Valor a eliminar (todas las ocurrencias): ");
                    int eliminadas = list.RemoveAll(valorEliminarTodas);
                    Console.WriteLine(
                        eliminadas > 0
                            ? $"Se eliminaron {eliminadas} ocurrencia(s)."
                            : "No se encontró el valor."
                    );
                    break;

                case 0:
                    Console.WriteLine("Saliendo...");
                    break;

                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

        } while (opcion != 0);
    }

    static int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out int valor))
                return valor;

            Console.WriteLine("Entrada inválida. Ingrese un número entero.");
        }
    }
}