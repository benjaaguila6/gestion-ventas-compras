using DAL;
using Services.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLPatente
    {
        DALPatente dal = new DALPatente();
        public List<PermisoModelo530BA> obtenerTodos()
        {
            DataTable dt = dal.obtenerTodos();

            List<PermisoModelo530BA> lista = new List<PermisoModelo530BA>();

            foreach (DataRow row in dt.Rows)
            {
                var patente = new PermisoModelo530BA
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = row["Nombre"].ToString()
                };

                lista.Add(patente);
            }

            return lista;
        }

        

        
    }
}
