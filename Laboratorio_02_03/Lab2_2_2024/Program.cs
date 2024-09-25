using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Ingrese la ruta del archivo CSV:");
        string archivo1 = Console.ReadLine()?.Trim('\"');
        Datos datos = new Datos();

        if (File.Exists(archivo1))
        {
            using (var reader = new StreamReader(archivo1))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(new[] { ';' }, 2);
                    if (parts.Length == 2)
                    {
                        string action = parts[0].Trim().ToUpper();
                        string json = parts[1].Trim();

                        if (!IsValidJson(json))
                        {
                            Console.WriteLine("Error: JSON inválido.");
                            continue;
                        }

                        switch (action)
                        {
                            case "INSERT":
                                datos.Insert(json);
                                break;
                            case "PATCH":
                                datos.Patch(json);
                                break;
                            case "DELETE":
                                datos.Delete(json);
                                break;
                            default:
                                Console.WriteLine($"Acción no reconocida: {action}");
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Error: El archivo no existe.");
            return;
        }

        Console.WriteLine("\nIngrese la ruta del archivo de búsqueda:");
        string archivo2 = Console.ReadLine()?.Trim('\"');

        if (File.Exists(archivo2))
        {
            string outputFile = Path.ChangeExtension(archivo2, ".txt");
            using (var reader = new StreamReader(archivo2))
            using (var writer = new StreamWriter(outputFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(new[] { ';' }, 2);
                    if (parts.Length == 2 && parts[0].Trim().ToUpper() == "SEARCH")
                    {
                        datos.Search(parts[1].Trim(), writer);
                    }
                }

                datos.PrintResults(writer);
            }

            Process.Start(new ProcessStartInfo(outputFile) { UseShellExecute = true });
        }
        else
        {
            Console.WriteLine("Error: El archivo no existe.");
        }

        Console.ReadLine();
    }

    private static bool IsValidJson(string json)
    {
        try
        {
            JToken.Parse(json);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
}






