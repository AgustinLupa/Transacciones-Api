using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transacciones.Datos.Modelos;
using Transacciones.Interface;

namespace Transacciones.Repositorio
{
    public class CarteraRepositorio : ICarteraRepositorio
    {
        ModelContext _context;

        public CarteraRepositorio(ModelContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Cartera>> GetCarteras(string nombreCuenta, int? idCartera = null)
        {

            var parametroNombreCuenta = new OracleParameter("p_nombre_cuenta", OracleDbType.Varchar2, ParameterDirection.Input)
            {
                Value = nombreCuenta ?? OracleString.Null
            };

            var parametroIdCartera = new OracleParameter("p_id_cartera", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = idCartera != 0 ? idCartera : OracleDecimal.Null
            };

            var parametroResultado = new OracleParameter("p_resultado", OracleDbType.RefCursor, ParameterDirection.Output);

            var carteras = await _context.Carteras
                .FromSqlRaw("BEGIN ObtenerCarteras(:p_nombre_cuenta, :p_id_cartera, :p_resultado); END;",
                    parametroNombreCuenta, parametroIdCartera, parametroResultado)                
                .ToListAsync();

            return carteras;

        }
    }
}
