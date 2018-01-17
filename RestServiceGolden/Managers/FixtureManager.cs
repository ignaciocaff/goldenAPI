using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Managers
{
    public class FixtureManager
    {
        List<Partido> fixture = new List<Partido>();
        public List<Partido> crearFixture()
        {
            for (int i = 0; i < 7; i++)
            {
                mostrar();
                combinar();   
            }
            asignarCancha();
            return fixture;
        }

        private String[] equipos = new String[8];
        private String[] canchas = new String[8];
        private String[] horarios = new String[6];

        public FixtureManager()
        {
            this.equipos[0] = "A";
            this.equipos[1] = "B";
            this.equipos[2] = "C";
            this.equipos[3] = "D";
            this.equipos[4] = "E";
            this.equipos[5] = "F";
            this.equipos[6] = "G";
            this.equipos[7] = "H";

            this.horarios[0] = "10:00 hs";
            this.horarios[1] = "11:30 hs";
            this.horarios[2] = "13:20 hs";
            this.horarios[3] = "15:00 hs";
            this.horarios[4] = "16:40 hs";
            this.horarios[5] = "18:20 hs";

            this.canchas[0] = "Cancha: 1";
            this.canchas[1] = "Cancha: 2";
            this.canchas[2] = "Cancha: 3";
            this.canchas[3] = "Cancha: 4";
            this.canchas[4] = "Cancha: 5";
            this.canchas[5] = "Cancha: 6";
            this.canchas[6] = "Cancha: 7";
            this.canchas[7] = "Cancha: 8";
        }

        public void asignarCancha()
        {

            foreach(Partido p in fixture)
            {
            }
            
        }

        public void combinar()
        {
            String buffer = equipos[equipos.Length - 1];

            for (int i = equipos.Length - 1; i > 1; i--)
            {
                equipos[i] = equipos[i - 1];
            }
            equipos[1] = buffer;

        }

        public void mostrar()
        {
            for (int i = 0, j = equipos.Length - 1; i < j; i++, j--)
            {
                Partido partido = new Partido();
                partido.local = equipos[i];
                partido.visitante = equipos[j];

                fixture.Add(partido);
            }
        }
    }
}