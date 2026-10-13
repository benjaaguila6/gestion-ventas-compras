using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLIdioma530BA
    {
        DALIdioma530BA dal = new DALIdioma530BA();

        public List<Idioma530BA> obtenerTodos()
        {
            DataTable dt = dal.obtenerTodos();
            List<Idioma530BA> lista = new List<Idioma530BA>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new Idioma530BA
                {
                    Id = System.Convert.ToInt32(row["Id"]),
                    Nombre = row["Nombre"].ToString()
                });
            }

            return lista;
        }
    }
}
