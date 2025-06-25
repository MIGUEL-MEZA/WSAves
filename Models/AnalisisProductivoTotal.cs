namespace WSOptimizerGallinas.Models
{
    public class AnalisisProductivoTotal
    {

        public double costoPonderado { get; set; } = 0;
        public double consumoTotalAlimento { get; set; } = 0;

        public double costoProducidoHuevo { get; set; } = 0;
        public double conversionAlimenticia { get; set; } = 0;
        
        public double masaTotalHuevo { get; set; } = 0;
        public double costoProgramaAlimentacion { get; set; } = 0;
        public double consumoTotalAlimentoParvada { get; set; } = 0;
        public double costoProgramaParvada { get; set; } = 0;
        
        public double masaHuevoParvada { get; set; } = 0;
        public double ingresoHuevoParvada { get; set; } = 0;
        public double utilidadAlimentacionParvada { get; set; } = 0;
        public double roi { get; set; } = 0;

}
}