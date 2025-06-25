namespace WSOptimizerGallinas.Models
{
    using System;

    public class ProductoModel : ICloneable
    {
        public int CveProducto { get; set; }
        public string NomProducto { get; set; }
        public int Posicion { get; set; }
        public double Costo { get; set; }
        public double EdadInicial { get; set; }
        public double EdadFinal { get; set; }
        public double Mortalidad { get; set; }
        public double ConsumoAlimento { get; set; }
        public double PesoHuevo { get; set; } = 0;
        public double Produccion { get; set; } = 0;
        public double TipoEtapa { get; set; } = 1;

        public object Clone()
        {
            ProductoModel prodNew = null;
            prodNew = new ProductoModel();
            prodNew.CveProducto = this.CveProducto;
            prodNew.NomProducto = this.NomProducto;
            prodNew.Posicion = this.Posicion;
            prodNew.Costo = this.Costo;
           
            return prodNew;
        }
    }

}
