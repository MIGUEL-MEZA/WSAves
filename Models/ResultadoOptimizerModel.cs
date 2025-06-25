namespace WSOptimizerGallinas.Models
{
    public class ResultadoOptimizerModel
    {
        private AnalisisProductivoTotal analisisProductivoTotal1;

        public int CveParametro { get; set; } = 0;
        public int Seleccionado { get; set; } = 2;

        public AnalisisProductivoTotal analisisProductivoTotal = new AnalisisProductivoTotal();
        public AnalisisProductivoCrianza analisisProductivoCrianza = new AnalisisProductivoCrianza();
        public AnalisisProductivoPostura analisisProductivoPostura= new AnalisisProductivoPostura();

    }


}
