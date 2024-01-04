using System;
using System.Collections.Generic;

namespace Transacciones.Datos.Modelos;

public partial class Historialsaldo
{
    public decimal IdHistorial { get; set; }

    public decimal? IdCartera { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Descripcion { get; set; }

    public string? TipoTransaccion { get; set; }

    public decimal? MontoTransaccion { get; set; }

    public decimal? SaldoPosterior { get; set; }

    public decimal? SaldoAnterior { get; set; }

    public virtual Cartera? IdCarteraNavigation { get; set; }
}
