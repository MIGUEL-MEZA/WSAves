namespace WSOptimizerGallinas.Models
{
    using System.Collections.Generic;

    public class RequestModel
    {
        public int CvePerfilN { get; set; }
        public string UsuAct { get; set; }
        public int Referencia { get; set; }
        public string PreIniciadorNupio { get; set; }

        public double Temperatura { get; set; }
        public double Humedad { get; set; }
        public double DesperdicioCrianza { get; set; }
        public string EstatusConfort { get; set; }
        public string TipoInstalaciones { get; set; }
        
        public List<EtapaModel> EtapasModel { get; set; }

        // Si Debug = true, el servicio incluirá detalles de cálculo por etapa en ResponseDataModel.DebugInfo
        public bool Debug { get; set; } = false;
    }

}
