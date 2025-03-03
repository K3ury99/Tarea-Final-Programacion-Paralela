using System;
using System.Threading.Tasks;

namespace EjercicioParaleloAgradable
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            PrintHeader();

            ProcesarOperaciones();

            PrintFooter();
            Console.WriteLine("Presiona ENTER para salir...");
            Console.ReadLine();
        }

        // Diseño de consola por: Keury Ramirez
        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=============================================================");
            Console.WriteLine("       Bienvenido al ejercicio de Procesamiento Paralelo      ");
            Console.WriteLine("=============================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void PrintFooter()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================");
            Console.WriteLine("      Fin del procesamiento de tareas          ");
            Console.WriteLine("===============================================");
            Console.ResetColor();
        }

        static void ProcesarOperaciones()
        {
            // Task.Run para iniciar la operacion A.
            var operacionA = Task.Run(async () =>
            {
                PrintMessage("[Operación A]", "Iniciando procesamiento con Task.Run...", ConsoleColor.Yellow);
                await Task.Delay(2000); 
                PrintMessage("[Operación A]", "Procesamiento completado.", ConsoleColor.Green);
            });

            // 2Task.Factory.StartNew para iniciar la operacion B.
            var operacionB = Task.Factory.StartNew(async () =>
            {
                PrintMessage("[Operación B]", "Iniciando procesamiento con Task.Factory.StartNew...", ConsoleColor.Yellow);
                await Task.Delay(3000); 
                PrintMessage("[Operación B]", "Procesamiento completado.", ConsoleColor.Green);
            }).Unwrap();

            // Tarea padre que agrupa dos tareas hijas (C1 y C2)
            var operacionC = Task.Factory.StartNew(() =>
            {
                PrintMessage("[Operación C - Padre]", "Iniciando tareas hijo...", ConsoleColor.Yellow);

                // Tarea hija C1
                var operacionC1 = Task.Factory.StartNew(async () =>
                {
                    PrintMessage("[Operación C1]", "Iniciando procesamiento...", ConsoleColor.Yellow);
                    await Task.Delay(1000); 
                    PrintMessage("[Operación C1]", "Procesamiento completado.", ConsoleColor.Green);
                }, TaskCreationOptions.AttachedToParent).Unwrap();

                // Tarea hija C2
                var operacionC2 = Task.Factory.StartNew(async () =>
                {
                    PrintMessage("[Operación C2]", "Iniciando procesamiento...", ConsoleColor.Yellow);
                    await Task.Delay(1500); 
                    PrintMessage("[Operación C2]", "Procesamiento completado.", ConsoleColor.Green);
                }, TaskCreationOptions.AttachedToParent).Unwrap();

                // Esperamos que ambas tareas hijas se completen.
                Task.WaitAll(operacionC1, operacionC2);
                PrintMessage("[Operación C - Padre]", "Todas las tareas hijo han finalizado.", ConsoleColor.Green);
            });

            // Coordinamos las tareas con Task.WhenAny
            // Esto se ejecuta una acción cuando alguna de las operaciones principales finaliza.
            Task.WhenAny(operacionA, operacionB, operacionC)
                .ContinueWith(t =>
                {
                    PrintMessage("[Task.WhenAny]", "Al menos una operación principal ha finalizado.", ConsoleColor.Magenta);
                });

            // Continuación en caso de exito de la Operación C - Tarea Padre
            operacionC.ContinueWith(t =>
            {
                PrintMessage("[Continuación]", "Operación C finalizó exitosamente.", ConsoleColor.Green);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            // Continuación en caso de error o cancelación en la Operación C
            operacionC.ContinueWith(t =>
            {
                PrintMessage("[Continuación]", "Operación C fue cancelada o produjo un error.", ConsoleColor.Red);
            }, TaskContinuationOptions.OnlyOnCanceled);

            // Espera a que todas las tareas finalicen
            try
            {
                Task.WaitAll(new Task[] { operacionA, operacionB, operacionC });
                PrintMessage("[Final]", "Todas las operaciones han finalizado correctamente.", ConsoleColor.Green);
            }
            catch (AggregateException ae)
            {
                PrintMessage("[Final]", "Se produjo al menos un error durante el procesamiento:", ConsoleColor.Red);
                foreach (var ex in ae.InnerExceptions)
                {
                    PrintMessage("[Error]", ex.Message, ConsoleColor.Red);
                }
            }
        }

        // Método para imprimir mansajito con colore chevere hahah, se me ocurrio hacer esto para que sea mas organizado a la vista, espero le guste!
        static void PrintMessage(string tag, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{tag,-20}: {message}");
            Console.ResetColor();
        }
    }
}
