namespace WSOptimizerAves.Models
{
    public class EtapaResModel
    {
        public int Clave { get; set; }
        public double Valor { get; set; }


        public EtapaResModel(int clave, double valor)
        {
            this.Clave = clave;
            this.Valor = valor;
        }
    }

}
