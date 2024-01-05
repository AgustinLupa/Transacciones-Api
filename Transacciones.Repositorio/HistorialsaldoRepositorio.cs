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
    public class HistorialsaldoRepositorio : IHistorialsaldoRepositorio
    {
        ModelContext _context;

        public HistorialsaldoRepositorio(ModelContext context)
        {
            _context = context;
        }

        public async Task<int> CantidadDePaginas(int idCartera)
        {
            var parametroIdCartera = new OracleParameter("p_id_cartera", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = idCartera
            };

            var parametroTamanoPagina = new OracleParameter("p_tamano_pagina", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = 5
            };

            var parametroResultado = new OracleParameter("p_cantidad_paginas", OracleDbType.Int32, ParameterDirection.Output);

            await _context.Database.ExecuteSqlRawAsync("BEGIN ObtenerCantidadPaginas(:p_id_cartera, :p_tamano_pagina, :p_cantidad_paginas); END;",
                parametroIdCartera, parametroTamanoPagina, parametroResultado);

            OracleDecimal oracleDecimalValue = (OracleDecimal)parametroResultado.Value;

            decimal result = new decimal((double)oracleDecimalValue.Value);

            // Obtener el valor de salida del parámetro           

            return (int)result;

        }

        public async Task<IEnumerable<Historialsaldo>> GetHistorialsaldo(short id_cartera, int pagina)
        {
            var parametroIdCartera = new OracleParameter("p_id_cartera", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = id_cartera
            };

            var parametroPagina = new OracleParameter("p_pagina", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = pagina
            };

            var parametroTamanoPagina = new OracleParameter("p_tamano_pagina", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = 5
            };

            // Crear el parámetro de salida
            var parametroResultado = new OracleParameter("p_resultados", OracleDbType.RefCursor, ParameterDirection.Output);

            // Llamada a la stored procedure con paginación
            var historiales = await _context.Historialsaldos
                .FromSqlRaw("BEGIN ObtenerHistorialesSaldo(:p_id_cartera, :p_pagina, :p_tamano_pagina, :p_resultados); END;",
                    parametroIdCartera, parametroPagina, parametroTamanoPagina, parametroResultado)
                .ToListAsync();

            return historiales;
        }

        public async Task<dynamic> GetHistorialsaldosXFecha(short id_cartera, DateTime fecha_inicio, DateTime fecha_final)
        {


            try
            {
                var parametroIdCartera = new OracleParameter("p_id_cartera", OracleDbType.Int32, ParameterDirection.Input)
                {
                    Value = id_cartera
                };


                var parametroFechaInicio = new OracleParameter("p_fecha_inicio", OracleDbType.Date, ParameterDirection.Input)
                {
                    Value = fecha_inicio
                };

                var parametroFechaFin = new OracleParameter("p_fecha_fin", OracleDbType.Date, ParameterDirection.Input)
                {
                    Value = fecha_final
                };

                // Crear el parámetro de salida
                var parametroResultado = new OracleParameter("p_porcentaje_variacion", OracleDbType.Decimal, ParameterDirection.Output);

                // Llamada a la stored procedure

                await _context.Database.ExecuteSqlRawAsync("BEGIN CalcularVariacionSaldoPorFecha(:p_id_cartera, :p_fecha_inicio, :p_fecha_fin, :p_porcentaje_variacion); END;",
                parametroIdCartera, parametroFechaInicio, parametroFechaFin, parametroResultado);


                // Obtener el resultado del porcentaje de variación
                //var porcentajeVariacion = Convert.ToDecimal(parametroResultado.Value);
                OracleDecimal oracleDecimalValue = (OracleDecimal)parametroResultado.Value;

                // Convertir OracleDecimal a decimal
                decimal porcentajeVariacion = new decimal((double)oracleDecimalValue.Value);


                return porcentajeVariacion;

            }
            catch (Exception)
            {

                return 0;
            }
        }

        public async Task<IEnumerable<Historialsaldo>> GetHistorialsaldosXTipo(short id_cartera, string tipo_transaccion, int pagina)
        {
            var parametroIdCartera = new OracleParameter("p_id_cartera", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = id_cartera
            };

            var parametroTipo = new OracleParameter("p_tipo_transaccion", OracleDbType.Varchar2, ParameterDirection.Input)
            {
                Value = tipo_transaccion
            };

            var parametroPagina = new OracleParameter("p_numero_pagina", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = pagina
            };

            var parametroTamanoPagina = new OracleParameter("p_tamanio_pagina", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = 5
            };

            var parametroResultado = new OracleParameter("p_resultados", OracleDbType.RefCursor, ParameterDirection.Output);

            var historiales = await _context.Historialsaldos
                .FromSqlRaw("BEGIN ObtenerHistorialesConTipo(:p_id_cartera, :p_tipo_transaccion, :p_numero_pagina, :p_tamanio_pagina, :p_resultados); END;",
                    parametroIdCartera, parametroTipo, parametroPagina, parametroTamanoPagina, parametroResultado)
                .ToListAsync();
            
            return historiales;

        }

        public async Task RealizarTransaccion(int idCartera, decimal montoTransaccion, string descripcion, string tipoTransaccion)
        {
            var parametroIdCartera = new OracleParameter("p_id_cartera", OracleDbType.Int32, ParameterDirection.Input)
            {
                Value = idCartera
            };

            var parametroMontoTransaccion = new OracleParameter("p_monto_transaccion", OracleDbType.Decimal, ParameterDirection.Input)
            {
                Value = montoTransaccion
            };

            var parametroDescripcion = new OracleParameter("p_descripcion", OracleDbType.Varchar2, ParameterDirection.Input)
            {
                Value = descripcion
            };

            var parametroTipoTransaccion = new OracleParameter("p_tipo_transaccion", OracleDbType.Varchar2, ParameterDirection.Input)
            {
                Value = tipoTransaccion
            };

            await _context.Database.ExecuteSqlRawAsync("BEGIN RealizarTransaccion(:p_id_cartera, :p_monto_transaccion, :p_descripcion, :p_tipo_transaccion); END;",
                parametroIdCartera, parametroMontoTransaccion, parametroDescripcion, parametroTipoTransaccion);
        }
    }
}
