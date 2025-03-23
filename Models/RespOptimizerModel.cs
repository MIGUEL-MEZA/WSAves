namespace WSOptimizerAves.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class RespOptimizerModel
    {
        public List<ResponseOptimizerModel> ResponseParametro { get; set; } = new List<ResponseOptimizerModel>();
    }

}
