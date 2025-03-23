namespace WSOptimizerAves.Models
{
    public class ResponseModel
    {
        public double IndiceTemHum { get; set; }
        public string DescripcionTemHum { get; set; }
        public double ProductividadITH { get; set; }

        public List<ResponseDataModel> Variables { get; set; } = new List<ResponseDataModel>();

    }
}
