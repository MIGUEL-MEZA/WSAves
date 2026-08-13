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

        // DebugInfo: opcional, mapea ClaveEtapa -> detalle de cálculos cuando RequestModel.Debug = true
        public System.Collections.Generic.Dictionary<int, string> DebugInfo { get; set; }
    }

}
