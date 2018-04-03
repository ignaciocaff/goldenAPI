using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class IEquipoPlanilla
    {
        public int id_equipo { get; set; }
        public string nombre { get; set; }
        public List<IJugador> lsJugadores{ get; set; }
    }
}