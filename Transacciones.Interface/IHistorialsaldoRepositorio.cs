using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Datos.Modelos;

namespace Transacciones.Interface
{
    public interface IHistorialsaldoRepositorio
    {
        public Task<IEnumerable<Historialsaldo>> GetHistorialsaldo(short id_cartera, int paguina);
        public Task<IEnumerable<Historialsaldo>> GetHistorialsaldosXTipo(short id_cartera, string tipo_transaccion, int pagina);

        public Task<dynamic> GetHistorialsaldosXFecha(short id_cartera, DateTime fecha_inicio, DateTime fecha_final);

        public Task RealizarTransaccion(int idCartera, decimal montoTransaccion, string descripcion, string tipoTransaccion);
    }
}
