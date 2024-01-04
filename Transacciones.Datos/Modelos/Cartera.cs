using System;
using System.Collections.Generic;

namespace Transacciones.Datos.Modelos;

public partial class Cartera
{
    public decimal IdCartera { get; set; }

    public string? NombreCuenta { get; set; }

    public decimal? SaldoActual { get; set; }

    public virtual ICollection<Historialsaldo> Historialsaldos { get; set; } = new List<Historialsaldo>();
}
