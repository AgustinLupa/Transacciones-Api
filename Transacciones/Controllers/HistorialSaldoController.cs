using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using Transacciones.Datos.Modelos;
using Transacciones.Interface;
using Transacciones.Request;

namespace Transacciones.Controllers
{
    [Route("api")]
    [ApiController]
    public class HistorialSaldoController : ControllerBase
    {
        private IHistorialsaldoRepositorio _context;
        public HistorialSaldoController(IHistorialsaldoRepositorio context)
        {
            _context = context;
        }

        [HttpPost("ListarXcartera/{id}")]
        public async Task<IActionResult> ListarXCartera(short id, [FromBody] int pagina)
        {
            if (pagina == null || pagina <= 0)
            {
                return BadRequest("Los parámetros de paginación no son válidos.");
            }

            var result = _context.GetHistorialsaldo(id, pagina);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, "Ocurrió un error interno al obtener los historiales");
            }
        }
        

        [HttpPost("ListarXfecha/{id}")]
        public async Task<IActionResult> TraerPorcentajeXFecha(short id, [FromBody] FechaModel fechaModel)
        {
            return Ok(await _context.GetHistorialsaldosXFecha(id, fechaModel.fecha_inicio, fechaModel.fecha_fin));
        }

        [HttpPost("ListarXTipo")]
        public async Task<IActionResult> TraerHistorialXTipo([FromBody] HistorialSaldoXTipo historialSaldo)
        {
            return Ok(await _context.GetHistorialsaldosXTipo(historialSaldo.Id_cartera, historialSaldo.Tipo_Transaccion, historialSaldo.Pagina));
        }

        [HttpPost("RealizarTransaccion")]
        public async Task<IActionResult> RealizarTraccion([FromBody] Transaccion transaccion)
        {
            await _context.RealizarTransaccion(transaccion.IdCartera, transaccion.MontoTransaccion, transaccion.Descripcion, transaccion.TipoTransaccion);
            return Ok("se completo con exito");
        }

        [HttpGet("NumeroPaginas/{id}")]
        public async Task<IActionResult> ObtenerPaginas(int id)
        {
            return Ok(await _context.CantidadDePaginas(id));
        }
    }
}
