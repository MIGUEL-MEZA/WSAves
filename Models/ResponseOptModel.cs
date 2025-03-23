namespace WSOptimizerAves.Models
{
    using System.Collections.Generic;

    public class ResponseOptModel
    {
        public List<ResponseOptimizerModel> ResponseOptimizer { get; set; } = new List<ResponseOptimizerModel>();

        public List<CatalogoModel> LstProducto { get; set; } = new List<CatalogoModel>();
        public double ValorObjetivo { get; set; }
        public List<OptimizerModel> Optimizer { get; set; }
    }

}
