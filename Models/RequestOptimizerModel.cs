namespace WSOptimizerGallinas.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RequestOptimizerModel : ICloneable
    {
        public int CvePlan { get; set; }
        public string UsuAct { get; set; }
        public int CveParametro { get; set; }
        public int CveReferencia { get; set; }
        public double NoGallinas { get; set; }
        public double NoPollitas { get; set; }
        public double PrecioVenta { get; set; }
        public double MasaHuevoTotal { get; set; }
        public double ConsumoAlimento { get; set; }
        
        public List<ProductoModel> Productos { get; set; }
      

        public object Clone()
        {
            RequestOptimizerModel requestNew = null;
            requestNew = new RequestOptimizerModel();
            // cargar los valores de las variables miembro 
            // en el objeto nuevo 
            
            requestNew.CveParametro = this.CveParametro;
            requestNew.CvePlan = this.CvePlan;
            requestNew.CveReferencia = this.CveReferencia;
            requestNew.PrecioVenta = this.PrecioVenta;
            requestNew.Productos = this.Productos.Select(p => (ProductoModel)p.Clone()).ToList();
            requestNew.UsuAct = this.UsuAct;
            // devolver la referencia del objeto creado
            return requestNew;
        }
    }

}
