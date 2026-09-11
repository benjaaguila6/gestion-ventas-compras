using BE.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Modelos
{
    public class BitacoraEventosModelo530BA
    {
        public int IdBitacora { get; set; }
        public string DNI { get; set; }
        public string Evento { get; set; }
        public int Criticidad { get; set; }
        public DateTime FechaHora { get; set; }
        public Modulos530BA Modulo { get; set; }

        //ctor para bd
        public BitacoraEventosModelo530BA(int idBitacora, string dNI, string evento, int criticidad, DateTime fechaHora, Modulos530BA modulo)
        {
            IdBitacora = idBitacora;
            DNI = dNI;
            Evento = evento;
            Criticidad = criticidad;
            FechaHora = fechaHora;
            Modulo = modulo;
        }

        //ctor para hacer new
        public BitacoraEventosModelo530BA(string dNI, string evento, int criticidad, DateTime fechaHora, Modulos530BA modulo)
        {
            DNI = dNI;
            Evento = evento;
            Criticidad = criticidad;
            FechaHora = fechaHora;
            Modulo = modulo;
        }
    }
}
