namespace Transacciones.Request
{
    public class Transaccion
    {

        public int IdCartera { get; set; }
        public decimal MontoTransaccion { get; set; }
        public string Descripcion { get; set; }
        public string TipoTransaccion { get; set; }

    }
}
