using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models
{
    /*
     * 
     * Rojo - # Blanco - # Negro - # Amarillo
    Blanco - # Negro - # Amarillo
    Azul - # Blanco
    Amarillo  # Negro - # Amarillo
    Verde - # Blanco
    Negro - # Amarillo
    Naranja - # Negro
     */
    public class Colores
    {
        public int Numero { get; set; }
        public string Color { get; set; }

        public List<Colores> GetColores()
        {
            var result = new List<Colores>();

            result.Add(new Colores { Numero = 1, Color = "Rojo - # Blanco - # Negro - # Amarillo" });
            result.Add(new Colores { Numero = 2, Color = "Blanco - # Negro - # Amarillo" });
            result.Add(new Colores { Numero = 3, Color = "Azul - # Blanco" });
            result.Add(new Colores { Numero = 4, Color = "Amarillo  # Negro - # Amarillo" });
            result.Add(new Colores { Numero = 5, Color = "Verde - # Blanco" });
            result.Add(new Colores { Numero = 6, Color = "Negro - # Amarillo" });
            result.Add(new Colores { Numero = 7, Color = "Naranja - # Negro" });
            result.Add(new Colores { Numero = 8, Color = "Rosa - # Negro" });
            result.Add(new Colores { Numero = 9, Color = "Turquesa - # Negro" });
            result.Add(new Colores { Numero = 10, Color = "Purpura - # Blanco" });
            result.Add(new Colores { Numero = 11, Color = "Gris - # Rojo" });
            result.Add(new Colores { Numero = 12, Color = "Lima - # Negro" });
            result.Add(new Colores { Numero = 13, Color = "Marron - # Blanco" });
            result.Add(new Colores { Numero = 14, Color = "Vinotino - # Amarillo" });
            result.Add(new Colores { Numero = 15, Color = "Caqui - # Negro" });
            result.Add(new Colores { Numero = 16, Color = "Azul Claro - # Rojo" });
            result.Add(new Colores { Numero = 17, Color = "Azul Marino - # Blanco" });
            result.Add(new Colores { Numero = 18, Color = "Verde Militar Oscuro - # Amarillo" });
            result.Add(new Colores { Numero = 19, Color = "Azul Francia - # Rojo" });
            result.Add(new Colores { Numero = 20, Color = "Fucsia - # Amarillo" });
            result.Add(new Colores { Numero = 21, Color = "Purpura Claro - # Navy" });
            

            return result;
        }
    }
}
