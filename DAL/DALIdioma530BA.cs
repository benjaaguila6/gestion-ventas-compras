using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALIdioma530BA
    {
        DALAcceso530BA acceso = new DALAcceso530BA();

        public DataTable obtenerTodos()
        {
            string query = "SELECT Id, Nombre FROM Idioma";
            return acceso.executeDataTable(query);
        }
    }
}
