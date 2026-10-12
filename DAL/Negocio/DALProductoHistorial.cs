using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Negocio
{
    public class DALProductoHistorial
    {
        private DALAcceso530BA dal = new DALAcceso530BA();

        public DataTable ObtenerHistorialProducto(int codProducto)
        {
            string query = "SELECT * FROM ProductoHistorial WHERE codProducto = @codProducto ORDER BY FechaHora DESC";
            var parameters = new Dictionary<string, object>
            {
                { "@codProducto", codProducto }
            };
            
            return dal.executeDataTable(query, parameters);
        }

        public void RestaurarVersion(ProductoHistorial530BA version, string dni)
        {
            string query = @"
                EXEC sp_set_session_context 'UsuarioId', @UsuarioId;
                EXEC sp_set_session_context 'OperacionAudit', 'RESTORE';
                
                UPDATE Producto 
                SET Nombre = @nombre, 
                    Existencia = @existencia, 
                    PrecioUnitario = @precioUnitario, 
                    Activo = @Activo, 
                    DVH = @DVH
                WHERE codProducto = @codProducto;

                EXEC sp_set_session_context 'UsuarioId', NULL;
                EXEC sp_set_session_context 'OperacionAudit', NULL;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@UsuarioId", dni },
                { "@nombre", version.nombre },
                { "@existencia", version.existencia },
                { "@precioUnitario", version.precioUnitario },
                { "@Activo", version.Activo },
                { "@DVH", version.DVH },
                { "@codProducto", version.codProducto }
            };

            dal.executeNonQuery(query, parameters);
        }
    }
}
