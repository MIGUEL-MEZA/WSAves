using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using WSOptimizerGallinas.App_Data;
using WSOptimizerGallinas.Models;

namespace WSOptimizerGallinas.Controllers
{
    public class ModelOptimizadoController : Controller
    {

        public double PESO_NACIMIENTO = 0.0450000033352087;


        public RequestOptimizerModel objReq;
        public DataTable dtPdmax;
        public DataTable dtMD;
        public DataTable dtParam;
        public DataTable dtRef;
        public DataTable dtConst;
        public DataTable dtFor;
        public DataTable dtVar;



        [HttpPost]
        [Route("api/optimizado")]
        public ResponseOptimizerModel GetOptimizerModelN([FromBody] RequestOptimizerModel objReqDataIni)
        {

            ResponseOptimizerModel objRespModel = new ResponseOptimizerModel();

            int numEtapa = 0;
            List<TablaModel> lstTablaModel = new List<TablaModel>();
            for (int x = 1; x < 12; x++)
            {
                lstTablaModel.Add(GetRegistro(objReqDataIni, x));
            }

            //costo ponderado alimento
            double costoPonderado = lstTablaModel.Sum(x => (x.Costo * x.ConsumoAlimento)) / lstTablaModel.Sum(x => x.ConsumoAlimento);
            double consumoTotalAlimento = lstTablaModel.Sum(x => x.ConsumoAlimento);
            double masaTotalHuevo = lstTablaModel.Sum(x => x.MasaHuevo);
            double conversionAlimenticia = consumoTotalAlimento / masaTotalHuevo;
            double costoProducidoHuevo = costoPonderado * conversionAlimenticia;
            double costoProgramaAlimentacion = costoPonderado * consumoTotalAlimento;
            double consumoTotalAlimentoParvada = lstTablaModel.Sum(x => x.ConsumoAlimentoTotal);
            double costoProgramaParvada = lstTablaModel.Sum(x => (x.Costo * x.ConsumoAlimentoTotal));
            double masaHuevoParvada = lstTablaModel.Sum(x => (x.NoAves * x.MasaHuevo));
            double ingresoHuevoParvada = masaHuevoParvada * objReqDataIni.PrecioVenta;
            double utilidadAlimentacionParvada = ingresoHuevoParvada - costoProgramaParvada;
            double roi = (utilidadAlimentacionParvada / costoProgramaParvada)*100;

            //anlisiseconomico
            double consumoAlimentoCrianza = lstTablaModel.Where(p => objReqDataIni.Productos.Exists(r => r.CveProducto == p.Identificador && r.TipoEtapa == 1)).Sum(x => x.ConsumoAlimento);
            double costoPonderadoCrianza = lstTablaModel.Where(p => objReqDataIni.Productos.Exists(r => r.CveProducto == p.Identificador && r.TipoEtapa == 1)).Sum(x => (x.Costo * x.ConsumoAlimento)) / consumoAlimentoCrianza;
            double costoProgramaCrianza = consumoAlimentoCrianza * costoPonderadoCrianza;

            //analisiseconomicopostura
            double consumoAlimentoPostura = lstTablaModel.Where(p => objReqDataIni.Productos.Exists(r => r.CveProducto == p.Identificador && r.TipoEtapa > 1)).Sum(x => x.ConsumoAlimento);
            double costoPonderadoPostura = lstTablaModel.Where(p => objReqDataIni.Productos.Exists(r => r.CveProducto == p.Identificador && r.TipoEtapa > 1)).Sum(x => (x.Costo * x.ConsumoAlimento)) / consumoAlimentoPostura;
            double masaHuevoPostura = lstTablaModel.Where(p => objReqDataIni.Productos.Exists(r => r.CveProducto == p.Identificador && r.TipoEtapa > 1)).Sum(x => (x.MasaHuevo));
            double costoProgramaPostura = consumoAlimentoPostura * costoPonderadoPostura;
            double conversionAlimenticiaPostura = consumoAlimentoPostura / masaHuevoPostura;
            double costoProducidoPostura = costoPonderadoPostura * conversionAlimenticiaPostura;

            double ingresoVentaHuevo = objReqDataIni.PrecioVenta * masaHuevoPostura;
            double utilidadAlimentacion = ingresoVentaHuevo - costoProgramaPostura;
            double roiPostura = (utilidadAlimentacion / costoProgramaPostura)*100;

            objRespModel.Data = lstTablaModel;
            objRespModel.CveParametro = 1;
            objRespModel.Resultado.CveParametro = objReqDataIni.CveParametro;
            objRespModel.Resultado.analisisProductivoTotal.costoPonderado = costoPonderado;
            objRespModel.Resultado.analisisProductivoTotal.consumoTotalAlimento  = consumoTotalAlimento ;
            objRespModel.Resultado.analisisProductivoTotal.masaTotalHuevo  = masaTotalHuevo ;
            objRespModel.Resultado.analisisProductivoTotal.conversionAlimenticia  = conversionAlimenticia ;
            objRespModel.Resultado.analisisProductivoTotal.costoProducidoHuevo  = costoProducidoHuevo ;
            objRespModel.Resultado.analisisProductivoTotal.costoProgramaAlimentacion  = costoProgramaAlimentacion ;
            objRespModel.Resultado.analisisProductivoTotal.consumoTotalAlimentoParvada  = consumoTotalAlimentoParvada ;
            objRespModel.Resultado.analisisProductivoTotal.costoProgramaParvada = costoProgramaParvada ;
            objRespModel.Resultado.analisisProductivoTotal.masaHuevoParvada  = masaHuevoParvada ;
            objRespModel.Resultado.analisisProductivoTotal.ingresoHuevoParvada  = ingresoHuevoParvada ;
            objRespModel.Resultado.analisisProductivoTotal.utilidadAlimentacionParvada  = utilidadAlimentacionParvada ;
            objRespModel.Resultado.analisisProductivoTotal.roi  = roi ;

            objRespModel.Resultado.analisisProductivoCrianza.consumoAlimentoCrianza = consumoAlimentoCrianza;
            objRespModel.Resultado.analisisProductivoCrianza.costoPonderadoCrianza  = costoPonderadoCrianza ;
            objRespModel.Resultado.analisisProductivoCrianza.costoProgramaCrianza = costoProgramaCrianza;

            objRespModel.Resultado.analisisProductivoPostura.consumoAlimentoPostura = consumoAlimentoPostura;
            objRespModel.Resultado.analisisProductivoPostura.costoPonderadoPostura = costoPonderadoPostura;
            objRespModel.Resultado.analisisProductivoPostura.masaHuevoPostura = masaHuevoPostura;
            objRespModel.Resultado.analisisProductivoPostura.costoProgramaPostura = costoProgramaPostura;
            objRespModel.Resultado.analisisProductivoPostura.conversionAlimenticiaPostura = conversionAlimenticiaPostura;
            objRespModel.Resultado.analisisProductivoPostura.costoProducidoPostura = costoProducidoPostura;
            
            objRespModel.Resultado.analisisProductivoPostura.ingresoVentaHuevo = ingresoVentaHuevo;
            objRespModel.Resultado.analisisProductivoPostura.utilidadAlimentacion = utilidadAlimentacion;
            objRespModel.Resultado.analisisProductivoPostura.roiPostura = roiPostura;


            if (objReqDataIni.CvePlan > 0)
                SaveData(objRespModel, objReqDataIni);

            return objRespModel;

        }
        private void SaveData(ResponseOptimizerModel objResp, RequestOptimizerModel objReq)
        {

            string strSQLParam = "DELETE [OptimizerG_PlanA_Resultado] WHERE CvePlan =" + objReq.CvePlan.ToString();
            Database.execNonQuery(strSQLParam);

            strSQLParam = "INSERT INTO [OptimizerG_PlanA_Resultado](CvePlan,  Request,Response,FecAct,UsuAct) ";
            string jsonResp = JsonConvert.SerializeObject(objResp);
            string jsonReq = JsonConvert.SerializeObject(objReq);
            strSQLParam += "VALUES(" + objReq.CvePlan.ToString() + ",'" + jsonReq + "','" + jsonResp + "',GETDATE(),'" + objReq.UsuAct + "') ";
            Database.execNonQuery(strSQLParam);

            SaveDataReporte(objResp, objReq);

        }

        private void SaveDataReporte(ResponseOptimizerModel objResp, RequestOptimizerModel objReq)
        {
            string strSQLParam = "DELETE [OptimizerG_PlanA_Reporte] WHERE CvePlan =" + objReq.CvePlan.ToString();
            Database.execNonQuery(strSQLParam);

            AnalisisProductivoTotal total = objResp.Resultado.analisisProductivoTotal;
            AnalisisProductivoCrianza crianza = objResp.Resultado.analisisProductivoCrianza;
            AnalisisProductivoPostura postura = objResp.Resultado.analisisProductivoPostura;

            strSQLParam = "INSERT INTO [OptimizerG_PlanA_Reporte] ";
            strSQLParam += "([CvePlan], [CveParametro], [Seleccionado], [TotalCostoPonderado], [TotalConsumoTotalAlimento], [TotalMasaTotalHuevo], [TotalConversionAlimenticia] ";
            strSQLParam += ",[TotalCostoProducidoHuevo] ,[TotalCostoProgramaAlimentacion] ,[TotalConsumoAlimentoParvada] ,[TotalCostoProgramaParvada] ,[TotalMasaHuevoParvada] ";
            strSQLParam += ",[TotalIngresoHuevoParvada], [TotalUtilidadParvada],[TotalROI], [CrianzaConsumoAlimento], [CrianzaCostoPonderado],[CrianzaCostoPrograma],[PosturaConsumoAlimento] ";
            strSQLParam += ",[PosturaCostoPonderado],[PosturaMasaHuevo],[PosturaCostoPrograma],[PosturaConversionAlimenticia],[PosturaCostoProducido],[PosturaIngresoVentaHuevo]";
            strSQLParam += ",[PosturaUtilidadAlimentacion],[PosturaROI]) ";
            strSQLParam += "VALUES(";
            strSQLParam += objReq.CvePlan.ToString() + ",";
            strSQLParam += objResp.CveParametro.ToString() + ",1,";
            strSQLParam += total.costoPonderado.ToString() + ",";
            strSQLParam += total.consumoTotalAlimento.ToString() + ",";
            strSQLParam += total.masaTotalHuevo.ToString() + ",";
            strSQLParam += total.conversionAlimenticia.ToString() + ",";
            strSQLParam += total.costoProducidoHuevo.ToString() + ",";
            strSQLParam += total.costoProgramaAlimentacion.ToString() + ",";
            strSQLParam += total.consumoTotalAlimentoParvada.ToString() + ",";
            strSQLParam += total.costoProgramaParvada.ToString() + ",";
            strSQLParam += total.masaHuevoParvada.ToString() + ",";
            strSQLParam += total.ingresoHuevoParvada.ToString() + ",";
            strSQLParam += total.utilidadAlimentacionParvada.ToString() + ",";
            strSQLParam += total.roi.ToString() + ",";

            strSQLParam += crianza.consumoAlimentoCrianza.ToString() + ",";
            strSQLParam += crianza.costoPonderadoCrianza.ToString() + ",";
            strSQLParam += crianza.costoProgramaCrianza.ToString() + ",";

            strSQLParam += postura.consumoAlimentoPostura.ToString() + ",";
            strSQLParam += postura.costoPonderadoPostura.ToString() + ",";
            strSQLParam += postura.masaHuevoPostura.ToString() + ",";
            strSQLParam += postura.costoProgramaPostura.ToString() + ",";
            strSQLParam += postura.conversionAlimenticiaPostura.ToString() + ",";
            strSQLParam += postura.costoProducidoPostura.ToString() + ",";
            strSQLParam += postura.ingresoVentaHuevo.ToString() + ",";
            strSQLParam += postura.utilidadAlimentacion.ToString() + ",";
            strSQLParam += postura.roiPostura.ToString() ;
            strSQLParam +=  ")";
            Database.execNonQuery(strSQLParam);


        }
        private TablaModel  GetRegistro(RequestOptimizerModel objReqDataIni, int numEtapa)
        {

            TablaModel registro=new TablaModel();

            double costo = GetCosto(objReqDataIni, numEtapa);
            double edadInicial= GetEdadInicial(objReqDataIni, numEtapa);
            double edadFinal = GetEdadFinal(objReqDataIni, numEtapa);
            double semanas = GetSemanas(objReqDataIni, numEtapa);
            double dias = GetDias(objReqDataIni, numEtapa);
            double mortalidad = GetMortalidad(objReqDataIni, numEtapa);
            double noAves = GetNoAves(objReqDataIni,numEtapa );
            double consumoAlimento = GetConsumoAlimento(objReqDataIni, numEtapa);
            double consumoAlimentoTotal = noAves  * consumoAlimento;
            double pesoHuevo = 0;
            double produccion = 0;
            double masaHuevo = 0;
            double conversionAlimenticia = 0;
            double huevoProducido = 0;
            if (numEtapa > 5)
            {
                pesoHuevo = GetPesoHuevo(objReqDataIni, numEtapa);
                produccion = GetProduccion(objReqDataIni, numEtapa);
                masaHuevo = ((pesoHuevo * produccion) / 100000) * dias;
                conversionAlimenticia = consumoAlimento / masaHuevo;
                huevoProducido = (masaHuevo * noAves) / 1000;
            }
            registro.Identificador = numEtapa;
            registro.Costo = costo; 
            registro.EdadInicial = edadInicial;
            registro.EdadFinal = edadFinal;
            registro.Semanas  = semanas;
            registro.Dias  = dias ;
            registro.Mortalidad  = mortalidad;
            registro.NoAves  = noAves;
            registro.ConsumoAlimento  = consumoAlimento;
            registro.ConsumoAlimentoTotal  = consumoAlimentoTotal;
            registro.PesoHuevo = pesoHuevo  ;
            registro.Produccion  = produccion ;
            registro.MasaHuevo  = masaHuevo ;
            registro.ConversionAlimenticia  = conversionAlimenticia ;
            registro.HuevoProducido  = huevoProducido ;
            return registro;
        }

        private double GetCosto(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.Costo;
            return valor;
        }

        private double GetEdadInicial(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor =etapaProducto.EdadInicial;
            return valor;
        }
        private double GetEdadFinal(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.EdadFinal;
            return valor;
        }

        private double GetSemanas(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto =objReqDataIni.Productos.First(p=> p.CveProducto==numEtapa);
            valor=etapaProducto.EdadFinal- etapaProducto.EdadInicial ;
            return valor;
        }
        private double GetDias(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.EdadFinal - etapaProducto.EdadInicial;
            return valor * 7;
        }
        private double GetMortalidad(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.Mortalidad;
            return valor;
        }
        private double GetNoAves(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            
            if (numEtapa==1 )
            {
                valor = objReqDataIni.NoPollitas * (1 - 0.0155);
            }else if (numEtapa == 6)
            {
                valor = objReqDataIni.NoGallinas * (1 - 0.011);
            }
            else
            {
                valor = (1 - (GetMortalidad(objReqDataIni, numEtapa) - GetMortalidad(objReqDataIni, numEtapa - 1)) / 100) * GetNoAves(objReqDataIni, numEtapa - 1);
            }
                return valor;
        }
        private double GetConsumoAlimento(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.ConsumoAlimento;
            return valor;
        }

        private double GetPesoHuevo(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.PesoHuevo ;
            return valor;
        }
        private double GetProduccion(RequestOptimizerModel objReqDataIni, int numEtapa)
        {
            double valor = 0;
            ProductoModel etapaProducto = objReqDataIni.Productos.First(p => p.CveProducto == numEtapa);
            valor = etapaProducto.Produccion;
            return valor;
        }
    }

}
