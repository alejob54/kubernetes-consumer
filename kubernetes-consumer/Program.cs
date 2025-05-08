// URL del endpoint
string url = "http://localhost:80/test";

// Número de tareas a crear
int numberOfTasks = 10;

// Número de peticiones por tarea
int requestsPerTask = 100000;

// Crear una lista para almacenar las tareas
List<Task> tasks = new List<Task>();

// Crear y ejecutar las tareas
using (HttpClient client = new HttpClient()) // Crear una instancia de HttpClient dentro del using para que se libere correctamente
{
    for (int i = 0; i < numberOfTasks; i++)
    {
        // Crear una nueva tarea para cada petición GET
        Task task = Task.Run(async () =>
        {
            try
            {
                for (int j = 0; j < requestsPerTask; j++)
                {
                    // Hacer la petición GET y esperar la respuesta
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // Lanzar una excepción si el código de estado no es exitoso
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Escribir la respuesta en la consola (incluyendo el ID de la tarea y el número de petición)
                    Console.WriteLine($"Task {Task.CurrentId}, Request {j + 1}: {responseBody}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Capturar errores específicos de HTTP
                Console.WriteLine($"Task {Task.CurrentId} - Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Capturar cualquier otra excepción
                Console.WriteLine($"Task {Task.CurrentId} - Unexpected Error: {ex.Message}");
            }
        });

        // Agregar la tarea a la lista
        tasks.Add(task);
    }

    // Esperar a que todas las tareas se completen
    await Task.WhenAll(tasks);

    Console.WriteLine("All tasks completed.");
}
// El HttpClient se libera automáticamente al salir del bloque using