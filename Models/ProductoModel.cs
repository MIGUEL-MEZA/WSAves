namespace WSOptimizerGallinas.Models
{
    using System;

    public class ProductoModel : ICloneable
    {
        public int CveProducto { get; set; }
        public string NomProducto { get; set; }
        public int Posicion { get; set; }
        public double EM { get; set; }
        public double Costo { get; set; }
        public double Lisina { get; set; }
        public double PesoFinal { get; set; }
        public double DuracionMin { get; set; } = 0;
        public double DuracionMax { get; set; } = 0;

        public object Clone()
        {
            ProductoModel prodNew = null;
            prodNew = new ProductoModel();
            prodNew.CveProducto = this.CveProducto;
            prodNew.NomProducto = this.NomProducto;
            prodNew.Posicion = this.Posicion;
            prodNew.Costo = this.Costo;
            prodNew.EM = this.EM;
            prodNew.PesoFinal = this.PesoFinal;
            prodNew.DuracionMin = this.DuracionMin;
            prodNew.DuracionMax = this.DuracionMax;
            return prodNew;
        }
    }

}
