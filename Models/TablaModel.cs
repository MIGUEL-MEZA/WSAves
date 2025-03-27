namespace WSOptimizerGallinas.Models
{
    public class TablaModel
    {
        public int Identificador { get; set; }
        public double EM_Alimento { get; set; }
        public double Req_EM { get; set; }
        public double Costo { get; set; }
        public double Duracion_Etapa { get; set; }
        public double Duracion_Minima { get; set; }
        public double Duracion_Maxima { get; set; }

        public double Peso_Inicial { get; set; }
        public double Peso_Final { get; set; }
        public double Peso_Medio { get; set; }
        public double SIDLysGDP { get; set; } 

        public double ConsumoDiario { get; set; }
        public double AlimentoOfrecer { get; set; }
        public double PresupuestoAlimento { get; set; }
        
        public double Lisina { get; set; }
        public double GDPNutrientes { get; set; }
        public double GDPEcuacion { get; set; }
        public double GDPUtilizar { get; set; }
        public double CA { get; set; }
        
        public double Edad_Inicial { get; set; }
        public double Edad_Final { get; set; }
        
    }

}
