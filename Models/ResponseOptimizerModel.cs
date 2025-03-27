﻿namespace WSOptimizerGallinas.Models
{
    using System.Collections.Generic;

    public class ResponseOptimizerModel
    {
        public int CveParametro { get; set; }
        public string Parametro { get; set; }
        public List<TablaModel> Data { get; set; }
        public List<OptimizerModel> Optimizer { get; set; }

        public ResultadoOptimizerModel Resultado = new ResultadoOptimizerModel();

        public ResponseOptimizerModel DeepCopy()
        {
            ResponseOptimizerModel othercopy = (ResponseOptimizerModel)this.MemberwiseClone();
            return othercopy;
        }
    }

}
