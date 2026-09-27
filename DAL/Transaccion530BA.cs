using System;
using System.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// Unidad de trabajo (Unit of Work) de la capa DAL: UNA conexión y UNA
    /// transacción local que comparten todas las operaciones de negocio.
    ///
    /// Existe para sacar la transacción del BLL sin perder atomicidad. El BLL
    /// sigue orquestando y sigue decidiendo el commit, pero ya no habla
    /// SqlConnection ni SqlTransaction: abre la unidad de trabajo, invoca los
    /// métodos del DAL y termina con Confirmar o Revertir. El BLL queda así
    /// libre de ADO.NET, y la frontera transaccional queda en la capa que
    /// entiende de la base de datos.
    ///
    /// La conexión se abre UNA vez y se comparte. Eso mantiene la transacción
    /// LOCAL: si cada llamada del DAL abriera su propia conexión, la atomicidad
    /// sólo podría garantizarse con TransactionScope, y al tocar la segunda
    /// conexión el ámbito la promovería a transacción DISTRIBUIDA (promotion to
    /// distributed transaction), que es justamente lo que se descarta. Un Unit
    /// of Work explícito da la misma atomicidad sin depender de un ámbito de
    /// transacción ambiental.
    /// </summary>
    public class Transaccion530BA : IDisposable
    {
        private readonly SqlConnection _conexion;
        private readonly SqlTransaction _transaccion;

        // true cuando la transacción ya terminó por commit o por rollback.
        // Es lo que vuelve idempotentes Confirmar, Revertir y Dispose: no se
        // puede revertir ni volver a confirmar una transacción ya cerrada.
        private bool _transaccionTerminada;

        private bool _liberado;

        // La conexión y la transacción que el DAL necesita para sumar su
        // sentencia a esta misma unidad de trabajo. El DALAcceso las recibe
        // por parámetro justamente para no abrir conexiones propias.
        public SqlConnection Conexion
        {
            get { return _conexion; }
        }

        public SqlTransaction Transaccion
        {
            get { return _transaccion; }
        }

        public Transaccion530BA()
        {
            DALAcceso530BA acceso = new DALAcceso530BA();

            _conexion = new SqlConnection(acceso.CadenaConexion);
            _conexion.Open();
            _transaccion = _conexion.BeginTransaction();
        }

        // Confirma todo lo que se hizo en la unidad de trabajo. Si el commit
        // falla, la excepción sube y queda en manos de Dispose el rollback
        // automático, así que no se deja una transacción abierta.
        public void Confirmar()
        {
            if (_transaccionTerminada)
            {
                return;
            }

            _transaccion.Commit();
            _transaccionTerminada = true;
        }

        // Revierte todo lo que se hizo en la unidad de trabajo.
        //
        // Es best-effort a propósito: se llama desde un catch que va a relanzar
        // la excepción original, y un fallo del rollback NO debe tapar ese
        // error. Si el rollback fallara, el cierre de conexión de Dispose deja
        // la transacción sin confirmar igual, porque SQL Server revierte solo al
        // cerrar la conexión que tenía la transacción abierta.
        public void Revertir()
        {
            if (_transaccionTerminada)
            {
                return;
            }

            try
            {
                _transaccion.Rollback();
            }
            catch (Exception)
            {
                // se ignora a propósito: ver el comentario de arriba. Un rollback
                // que falla no puede convertir el error de negocio en algo peor.
            }
            finally
            {
                _transaccionTerminada = true;
            }
        }

        // Cierra la unidad de trabajo liberando la transacción y la conexión.
        public void Dispose()
        {
            if (_liberado)
            {
                return;
            }

            _liberado = true;

            // Rollback automático: un return temprano, o una excepción que no
            // pasó por Revertir, no pueden dejar un cobro a medio escribir.
            // Revertir es no-op si ya se confirmó o ya se revirtió.
            Revertir();

            try
            {
                _transaccion.Dispose();
            }
            catch (Exception)
            {
                // liberar recursos nunca debe tapar la excepción de negocio
            }

            try
            {
                _conexion.Dispose();
            }
            catch (Exception)
            {
                // ídem
            }
        }
    }
}
