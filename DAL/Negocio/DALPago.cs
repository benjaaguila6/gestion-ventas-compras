using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Negocio
{
    // Persistencia de Pago. Recibe la unidad de trabajo del llamador:
    // no abre conexiones propias para no partir la transacción de negocio.
    public class DALPago
    {
        DALAcceso530BA _dal = new DALAcceso530BA();

        // Inserta el pago y devuelve el Id generado por SCOPE_IDENTITY.
        // El DVH se deja en NULL a propósito: se calcula recién después del commit,
        // cuando ya se conoce el Id.
        public int RegistrarPago(int idFactura, Pago530BA pago)
        {
            string query = @"
                INSERT INTO Pago (IdFactura, MetodoPago, Ultimos4, NombreTitular, Vencimiento, Monto, Fecha, DVH)
                VALUES (@idFactura, @metodoPago, @ultimos4, @nombreTitular, @vencimiento, @monto, @fecha, NULL);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parametros = new Dictionary<string, object>
            {
                { "@idFactura", idFactura },
                { "@metodoPago", (int)pago.Metodo },
                { "@ultimos4", pago.Ultimos4 },
                { "@nombreTitular", pago.NombreTitular },
                { "@vencimiento", pago.Vencimiento },
                { "@monto", pago.Monto },
                { "@fecha", pago.Fecha }
            };

            return Convert.ToInt32(_dal.executeScalar(query, parametros));
        }

        public void ActualizarDVHPago(int idPago, long dvh)
        {
            string query = "UPDATE Pago SET DVH = @dvh WHERE Id = @id";

            var parametros = new Dictionary<string, object>
            {
                { "@id", idPago },
                { "@dvh", dvh }
            };

            _dal.executeNonQuery(query, parametros);
        }
    }
}
