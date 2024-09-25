using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using CompresionAritmetica;
using Lab2_2_2024;
using System.Linq;

public class Datos
{
    
        private HashSet<Libro> librosCollection = new HashSet<Libro>();
        private HuffmanEncoding huffmanEncoding = new HuffmanEncoding();

        public void Insert(string json)
        {
            var jsonObject = JObject.Parse(json);
            string isbn = jsonObject["isbn"].ToString();

            var nuevoLibro = new Libro(
                isbn,
                jsonObject["name"]?.ToString(),
                jsonObject["author"]?.ToString(),
                jsonObject["category"]?.ToString(),
                jsonObject["price"]?.ToString(),
                jsonObject["quantity"]?.ToString()
            );

            
            if (!librosCollection.Add(nuevoLibro))
            {
                //Console.WriteLine($"Error: El libro ya existe.");
            }
        }

        public void Patch(string json)
        {
            var patchObject = JObject.Parse(json);
            string patchIsbn = patchObject["isbn"].ToString();

        var libroExistente = librosCollection.FirstOrDefault(l => l.Isbn == patchIsbn);
            
        if (libroExistente != null)
            {
                
                libroExistente.Name = patchObject["name"]?.ToString() ?? libroExistente.Name;
                libroExistente.Author = patchObject["author"]?.ToString() ?? libroExistente.Author;
                libroExistente.Category = patchObject["category"]?.ToString() ?? libroExistente.Category;
                libroExistente.Price = patchObject["price"]?.ToString() ?? libroExistente.Price;
                libroExistente.Quantity = patchObject["quantity"]?.ToString() ?? libroExistente.Quantity;
            }
            else
            {
                //Console.WriteLine("Error: No se encontró el libro para hacer el cambio.");
            }
        }

        public void Delete(string json)
        {
            string deleteIsbn = JObject.Parse(json)["isbn"].ToString();
            var libroAEliminar = librosCollection.FirstOrDefault(l => l.Isbn == deleteIsbn);
            
            if (libroAEliminar != null)
            {
                librosCollection.Remove(libroAEliminar);
            }
            else
            {
               // Console.WriteLine("Error: No se encontró el libro para eliminar.");
            }
        }

        private int equalCount = 0;
        private int decompressCount = 0;
        private int huffmanCount = 0;
        private int arithmeticCount = 0;

        public void Search(string queryJson, StreamWriter writer)
        {
            bool found = false;
            string queryName = JObject.Parse(queryJson)["name"]?.ToString();

            if (queryName == null)
            {
                writer.WriteLine("El nombre de búsqueda no se especificó correctamente.");
                return;
            }

            foreach (var libro in librosCollection)
            {
                if (libro.Name == queryName)
                {
                    var result = new JObject
                    {
                        ["isbn"] = libro.Isbn,
                        ["name"] = libro.Name,
                        ["author"] = libro.Author,
                        ["category"] = libro.Category,
                        ["price"] = libro.Price,
                        ["quantity"] = libro.Quantity
                    };

                    
                    int nameSizeBytes = System.Text.Encoding.Unicode.GetByteCount(libro.Name);

                    
                    int nameSizeHuffmanBits = huffmanEncoding.GetHuffmanEncodedSize(libro.Name);

                    
                    ArithmeticCompression arithmeticCompression = new ArithmeticCompression(libro.Name);
                    double nameSizeArithmeticBytes = arithmeticCompression.GetCompressedSize();

                    
                    result["namesize"] = nameSizeBytes.ToString(); 
                    result["namesizehuffman"] = nameSizeHuffmanBits.ToString(); 
                    result["namesizearithmetic"] = nameSizeArithmeticBytes.ToString(); 

                    writer.WriteLine(result.ToString(Formatting.None));
                    found = true;

                    
                    double nameSizeHuffmanBytes = (double)nameSizeHuffmanBits / 8.0; 

                    if (nameSizeBytes <= nameSizeHuffmanBytes && nameSizeBytes <= nameSizeArithmeticBytes)
                    {
                        equalCount++;
                    }
                    else if (nameSizeHuffmanBytes >= nameSizeBytes && nameSizeArithmeticBytes >= nameSizeBytes)
                    {
                        decompressCount++;
                    }
                    else if (nameSizeHuffmanBytes <= nameSizeBytes && nameSizeHuffmanBytes <= nameSizeArithmeticBytes)
                    {
                        huffmanCount++; 
                    }
                    else if (nameSizeArithmeticBytes <= nameSizeBytes && nameSizeArithmeticBytes <= nameSizeHuffmanBytes)
                    {
                        arithmeticCount++;
                    }
                }
            }

            if (!found)
            {
                writer.WriteLine($"No se encontraron libros con el nombre exacto: {queryName}.");
            }
        }
        public void PrintResults(StreamWriter writer)
        {
            writer.WriteLine($"Equal: {equalCount}");
            writer.WriteLine($"Decompress: {decompressCount}");
            writer.WriteLine($"Huffman: {huffmanCount}");
            writer.WriteLine($"Arithmetic: {arithmeticCount}");
        }
    }










