using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Datos.Modelos;

namespace Transacciones.Interface
{
    public interface ICarteraRepositorio
    {
        public Task<IEnumerable<Cartera>> GetCarteras(string nombreCuenta, int? idCartera = null);
    }
}
