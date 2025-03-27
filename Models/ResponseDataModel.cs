namespace WSOptimizerGallinas.Models
{
    using System.Collections.Generic;

    public class ResponseDataModel
    {
        public int NoVariable { get; set; }
        public string Variable { get; set; }
        public int Posicion { get; set; }
        public string MostrarCliente { get; set; }

        public List<EtapaResModel> Etapas { get; set; }
    }

}
