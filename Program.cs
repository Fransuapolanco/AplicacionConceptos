using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Interfaz para el gestor de tareas
public interface ITaskManager
{
    void AddTask(string task);
    void CompleteTask(string task);
    IEnumerable<string> GetPendingTasks();
    IEnumerable<string> GetCompletedTasks();
}

// Implementación del gestor de tareas
public class TaskManager : ITaskManager
{
    private readonly ConcurrentQueue<string> pendingTasks = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> completedTasks = new ConcurrentQueue<string>();

    public void AddTask(string task)
    {
        pendingTasks.Enqueue(task);
        Console.WriteLine($"Tarea '{task}' agregada.");
    }

    public void CompleteTask(string task)
    {
        if (pendingTasks.Contains(task))
        {
            pendingTasks.TryDequeue(out string result);
            completedTasks.Enqueue(result);
            Console.WriteLine($"Tarea '{task}' completada.");
        }
        else
        {
            Console.WriteLine($"Tarea '{task}' no encontrada en tareas pendientes.");
        }
    }

    public IEnumerable<string> GetPendingTasks()
    {
        return pendingTasks.ToArray();
    }

    public IEnumerable<string> GetCompletedTasks()
    {
        return completedTasks.ToArray();
    }
}

// Clase principal de la aplicación
class Program
{
    private static ITaskManager taskManager = new TaskManager();
    private static bool running = true;

    static void Main(string[] args)
    {
        while (running)
        {
            MostrarMenuPrincipal();
            var choice = Console.ReadLine();
            ProcesarOpcion(choice);
        }
    }

    static void MostrarMenuPrincipal()
    {
        Console.WriteLine("\n--- Menú de Gestión de Tareas ---");
        Console.WriteLine("1. Agregar Tarea");
        Console.WriteLine("2. Completar Tarea");
        Console.WriteLine("3. Ver Tareas Pendientes");
        Console.WriteLine("4. Ver Tareas Completadas");
        Console.WriteLine("5. Salir");
        Console.Write("Seleccione una opción: ");
    }

    static void ProcesarOpcion(string choice)
    {
        switch (choice)
        {
            case "1":
                MostrarSubMenu("Agregar Tarea");
                AgregarTarea();
                break;
            case "2":
                MostrarSubMenu("Completar Tarea");
                CompletarTarea();
                break;
            case "3":
                MostrarSubMenu("Ver Tareas Pendientes");
                VerTareasPendientes();
                break;
            case "4":
                MostrarSubMenu("Ver Tareas Completadas");
                VerTareasCompletadas();
                break;
            case "5":
                running = false;
                Console.WriteLine("Saliendo de la aplicación...");
                break;
            default:
                Console.WriteLine("Opción inválida, por favor intente nuevamente.");
                break;
        }
    }

    static void MostrarSubMenu(string titulo)
    {
        Console.WriteLine($"\n--- {titulo} ---");
    }

    static void AgregarTarea()
    {
        Console.Write("Ingrese la descripción de la tarea: ");
        var task = Console.ReadLine();
        Task.Run(() => taskManager.AddTask(task)).Wait();
    }

    static void CompletarTarea()
    {
        Console.Write("Ingrese la tarea a completar: ");
        var taskToComplete = Console.ReadLine();
        Task.Run(() => taskManager.CompleteTask(taskToComplete)).Wait();
    }

    static void VerTareasPendientes()
    {
        Task.Run(() =>
        {
            var pendingTasks = taskManager.GetPendingTasks();
            Console.WriteLine("\nTareas Pendientes:");
            if (pendingTasks.Any())
            {
                foreach (var t in pendingTasks)
                {
                    Console.WriteLine($"- {t}");
                }
            }
            else
            {
                Console.WriteLine("No hay tareas pendientes.");
            }
        }).Wait();
    }

    static void VerTareasCompletadas()
    {
        Task.Run(() =>
        {
            var completedTasks = taskManager.GetCompletedTasks();
            Console.WriteLine("\nTareas Completadas:");
            if (completedTasks.Any())
            {
                foreach (var t in completedTasks)
                {
                    Console.WriteLine($"- {t}");
                }
            }
            else
            {
                Console.WriteLine("No hay tareas completadas.");
            }
        }).Wait();
    }
}

