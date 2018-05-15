using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestServiceGolden.Utilidades
{
    public class Logger
    {
        private readonly string archivo;
        private StreamWriter sw;
        public List<string> mensajes { get; set; }


        public Logger(string archivo)
        {
            this.archivo = "log-" + archivo + ".txt";
            this.mensajes = new List<string>();

        }

        public void AgregarMensaje(string tipo, string mensaje)
        {
            mensajes.Add("{\"TIPO\":\"" + tipo + "\",\"MENSAJE\":\"" + mensaje + "\"}");
        }

        public void EscribirLog()
        {
            try
            {

                sw = new StreamWriter("h:\\root\\home\\jigcaffaratti-001\\www\\logs\\" + archivo, true);
                sw.WriteLine("             . " + DateTime.Now.ToString("dd/MM HH:mm"));
                foreach (var mensaje in mensajes)
                {

                    sw.WriteLine(mensaje);
                }

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

        }
    }
}

