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
        public double EdadVenta { get; set; }
        public double DiasPigmento { get; set; }
        public double PrecioVenta { get; set; }
        public double Desperdicio { get; set; }
        
        public double CostoAlimentacion { get; set; }
        
        public List<ProductoModel> Productos { get; set; }
        public bool IsOptimizar { get; set; } = false;
        public double EdadFinalTmp { get; set; }
        public double EdadInicialTmp { get; set; }
        public double PesoInicialTmp { get; set; }

        public object Clone()
        {
            RequestOptimizerModel requestNew = null;
            requestNew = new RequestOptimizerModel();
            // cargar los valores de las variables miembro 
            // en el objeto nuevo 
            requestNew.DiasPigmento = this.DiasPigmento;
            requestNew.Desperdicio = this.Desperdicio;
            requestNew.CostoAlimentacion = this.CostoAlimentacion ;
            requestNew.EdadVenta = this.EdadVenta;
            requestNew.EdadFinalTmp = this.EdadFinalTmp;
            requestNew.CveParametro = this.CveParametro;
            requestNew.CvePlan = this.CvePlan;
            requestNew.CveReferencia = this.CveReferencia;
            requestNew.Desperdicio = this.Desperdicio;
            requestNew.EdadInicialTmp = this.EdadInicialTmp;
            requestNew.IsOptimizar = this.IsOptimizar;
            requestNew.PesoInicialTmp = this.PesoInicialTmp;
            requestNew.PrecioVenta = this.PrecioVenta;
            requestNew.Productos = this.Productos.Select(p => (ProductoModel)p.Clone()).ToList();
            requestNew.UsuAct = this.UsuAct;
            // devolver la referencia del objeto creado
            return requestNew;
        }
    }

}
