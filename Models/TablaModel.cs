namespace WSOptimizerGallinas.Models
{
    public class TablaModel
    {
        public int Identificador { get; set; }
        public double Costo { get; set; }
        public double EdadInicial { get; set; }
        public double EdadFinal { get; set; }
        public double Semanas { get; set; }
        public double Dias { get; set; }
        public double Mortalidad { get; set; }

        public double NoAves { get; set; }
        public double ConsumoAlimento { get; set; }
        public double ConsumoAlimentoTotal { get; set; }
        public double PesoHuevo { get; set; } 

        public double Produccion { get; set; }
        public double MasaHuevo { get; set; }
        public double ConversionAlimenticia { get; set; }
        public double HuevoProducido { get; set; }
        
    }

}
