using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transacciones.Interface;
using Transacciones.Request;

namespace Transacciones.Controllers
{
    [Route("api")]
    [ApiController]
    public class CarteraController : ControllerBase
    {
        private ICarteraRepositorio _context;

        public CarteraController(ICarteraRepositorio cartera)
        {
            _context = cartera;
        }

        [HttpPost("listarCarteras")]
        public async Task<IActionResult> TraerCarteras([FromBody] CarteraRequest cartera)
        {
            var result = await _context.GetCarteras(cartera.NombreCuenta, cartera.IdCartera);
            return Ok(result);
        }
    }
}
