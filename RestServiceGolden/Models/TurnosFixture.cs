using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class TurnosFixture
    {
        int id_turno_fixture { get; set; }
        Cancha cancha { get; set; }
        HorarioFijo horario { get; set; }
    }
}