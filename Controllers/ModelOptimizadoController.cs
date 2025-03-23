using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Runtime.InteropServices;
using WSOptimizerAves.App_Data;
using WSOptimizerAves.Models;

namespace WSOptimizerAves.Controllers
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
        public RespOptimizerModel GetOptimizerModelN([FromBody] RequestOptimizerModel objReqDataIni)
        {

            RespOptimizerModel objRespModel = new RespOptimizerModel();
            string strSQLFor = "SELECT * FROM CatOptimizerP_Formula ";
            dtFor = Database.execQuery(strSQLFor);

            string strSQLRef = "SELECT * FROM CatOptimizerP_Referencias ";
            dtRef = Database.execQuery(strSQLRef);

            string strSQLConst = "SELECT * FROM CatOptimizerP_Constantes ";
            dtConst = Database.execQuery(strSQLConst);

            string strSQLParam = "SELECT * FROM CatOptimizerP_ParametrosEconomicos ";
            dtParam = Database.execQuery(strSQLParam);
            Console.WriteLine(objReqDataIni);
            //valido ractopamina

            ProductoModel pModTmpV = objReqDataIni.Productos.Find(p => p.Costo == 0 && (p.DuracionMin > 0 || p.DuracionMax > 0));
            if (pModTmpV != null)
            {
                throw new Exception("Existen etapas que no traen costo, coloque la duración en 0");
            }


            List<ResponseOptModel> lstResult = new List<ResponseOptModel>();
            DateTime date1 = DateTime.Now;

            objReq = objReqDataIni;
            int cveParametroSeleccionado = objReqDataIni.CveParametro;
            // obtengo las etapas inciiales
            List<ResponseOptimizerModel> respDataFull = getDataOptRegistro(objReqDataIni);
            if (respDataFull.Count == 0)
            {
                throw new Exception("No se encontró modelo optimizado");
            }
            List<ResponseOptimizerModel> respDataOpt = new List<ResponseOptimizerModel>();
            //respDataOpt.Add (getModelOptimizadoMinMax(respDataFull,7,"max"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 2, "min"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 3, "min"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 4, "min"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 5, "min"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 6, "max"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 7, "max"));
            respDataOpt.Add(getModelOptimizadoMinMax(respDataFull, 8, "max"));
            objRespModel.ResponseParametro = respDataOpt;


            if (objReqDataIni.CvePlan > 0)
                SaveData(objRespModel, objReqDataIni, cveParametroSeleccionado);

            return objRespModel;

        }

        private ResponseOptimizerModel getModelOptimizadoMinMax(List<ResponseOptimizerModel> respDataFull, int identificador, string tipo)
        {
            ResponseOptimizerModel respModel = new ResponseOptimizerModel();
            if (tipo.Equals("min"))
            {
                respModel = respDataFull.Find(p => p.Optimizer.Find(t => t.Orden == identificador).Valor == respDataFull.Min(r => r.Optimizer.Find(t => t.Orden == identificador).Valor)).DeepCopy();
            }
            else
            {
                respModel = respDataFull.Find(p => p.Optimizer.Find(t => t.Orden == identificador).Valor == respDataFull.Max(r => r.Optimizer.Find(t => t.Orden == identificador).Valor)).DeepCopy();
            }

            respModel.CveParametro = identificador;
            respModel.Parametro = GetParametroEcoVal(identificador, "NomParametro");
            respModel.Resultado = respModel.Resultado.DeepCopy();
            respModel.Resultado.CveParametro = identificador;
            return respModel.DeepCopy();



        }
        private List<ResponseOptimizerModel> getDataOptRegistro(RequestOptimizerModel objReqDataIni)
        {

            //obj
            int numInt = 6;
            double edadTope = objReqDataIni.EdadVenta - objReqDataIni.Productos.FindAll(t => t.CveProducto > 1).Sum(t => t.DuracionMin);
            ProductoModel prod = objReqDataIni.Productos.Find(r => r.Posicion == 1);
            double minEtapa = objReqDataIni.Productos.Find(r => r.Posicion == 1).DuracionMin;
            double maxEtapa = objReqDataIni.Productos.Find(r => r.Posicion == 1).DuracionMax;
            Boolean isExactDuracion = false;
            if (minEtapa == maxEtapa)
            {
                isExactDuracion = true;
                maxEtapa += 1;
            }
            double edadFinalMinima = 0;
            double edadFinalMaxima = 0;
            double edadFinal2Minima = 0;
            double edadFinal2Maxima = 0;
            double pesoMin1, pesoMin2, pesoMin3, pesoMin4, pesoMin5 = 0;
            double pesoMax1, pesoMax2, pesoMax3, pesoMax4, pesoMax5 = 0;


            List<double> pesoArray1 = GetArrPesoEtapa(PESO_NACIMIENTO, 1, objReqDataIni, 0);


            List<TablaModel> result;
            double duracionObjetivo = 0;
            double duracionRestante = 0;
            double durEtap1 = 0;
            double durEtap2 = 0;
            double durEtap3 = 0;
            double durEtap4 = 0;
            ResponseOptModel objR = new ResponseOptModel();
            List<ResponseOptimizerModel> lstResp = new List<ResponseOptimizerModel>();

            foreach (double peso1 in pesoArray1)
            {
                prod = objReqDataIni.Productos.Find(r => r.Posicion == 1);
                objReqDataIni.IsOptimizar = false;
                objReqDataIni.Productos.Find(p => p.CveProducto == 1).PesoFinal = peso1;
                TablaModel tModel = GetDataRegistro(prod, objReqDataIni);
                durEtap1 = tModel.Duracion_Etapa;
                duracionObjetivo = tModel.Duracion_Etapa;
                if ((int)duracionObjetivo > objReqDataIni.EdadVenta)
                {
                    break;
                }
                duracionRestante = objReqDataIni.Productos.FindAll(t => t.CveProducto >= 1).Sum(t => t.DuracionMax);
                if ((int)(duracionRestante + durEtap1) < (int)objReqDataIni.EdadVenta)
                {
                    break;
                }
                List<double> pesoArray2 = GetArrPesoEtapa(peso1, 2, objReqDataIni, duracionObjetivo);

                foreach (double peso2 in pesoArray2)
                {
                    objReqDataIni.IsOptimizar = true;
                    objReqDataIni.PesoInicialTmp = peso1;
                    prod = objReqDataIni.Productos.Find(r => r.Posicion == 2);
                    objReqDataIni.Productos.Find(p => p.CveProducto == 2).PesoFinal = peso2;
                    TablaModel tModel2 = GetDataRegistro(prod, objReqDataIni);
                    durEtap2 = tModel.Duracion_Etapa + tModel2.Duracion_Etapa;
                    duracionObjetivo = tModel.Duracion_Etapa + tModel2.Duracion_Etapa;
                    if ((int)duracionObjetivo > objReqDataIni.EdadVenta)
                    {
                        break;
                    }

                    duracionRestante = objReqDataIni.Productos.FindAll(t => t.CveProducto >= 2).Sum(t => t.DuracionMax);
                    if ((int)(duracionRestante + durEtap2) < (int)objReqDataIni.EdadVenta)
                    {
                        break;
                    }
                    List<double> pesoArray3 = GetArrPesoEtapa(peso2, 3, objReqDataIni, duracionObjetivo);

                    foreach (double peso3 in pesoArray3)
                    {
                        objReqDataIni.PesoInicialTmp = peso2;
                        prod = objReqDataIni.Productos.Find(r => r.Posicion == 3);
                        objReqDataIni.Productos.Find(p => p.CveProducto == 3).PesoFinal = peso3;
                        TablaModel tModel3 = GetDataRegistro(prod, objReqDataIni);
                        durEtap3 = tModel.Duracion_Etapa + tModel2.Duracion_Etapa + tModel3.Duracion_Etapa;
                        duracionObjetivo = tModel.Duracion_Etapa + tModel2.Duracion_Etapa + tModel3.Duracion_Etapa;
                        if ((int)duracionObjetivo > objReqDataIni.EdadVenta)
                        {
                            break;
                        }

                        duracionRestante = objReqDataIni.Productos.FindAll(t => t.CveProducto >= 3).Sum(t => t.DuracionMax);
                        if ((int)(duracionRestante + durEtap2) < (int)objReqDataIni.EdadVenta)
                        {
                            break;
                        }
                        List<double> pesoArray4 = GetArrPesoEtapa(peso3, 4, objReqDataIni, duracionObjetivo);

                        foreach (double peso4 in pesoArray4)
                        {
                            objReqDataIni.PesoInicialTmp = peso3;
                            prod = objReqDataIni.Productos.Find(r => r.Posicion == 4);


                            objReqDataIni.Productos.Find(p => p.CveProducto == 4).PesoFinal = peso4;
                            TablaModel tModel4 = GetDataRegistro(prod, objReqDataIni);


                            durEtap4 = tModel.Duracion_Etapa + tModel2.Duracion_Etapa + tModel3.Duracion_Etapa + tModel4.Duracion_Etapa;
                            duracionObjetivo = tModel.Duracion_Etapa + tModel2.Duracion_Etapa + tModel3.Duracion_Etapa + tModel4.Duracion_Etapa;

                            if ((int)duracionObjetivo > objReqDataIni.EdadVenta)
                            {
                                break;
                            }

                            duracionRestante = objReqDataIni.Productos.FindAll(t => t.CveProducto >= 4).Sum(t => t.DuracionMax);
                            
                            if ((int)(duracionRestante + durEtap3) < (int)objReqDataIni.EdadVenta)
                            {
                                break;
                            }
                            List<double> pesoArray5 = GetArrPesoEtapa(peso4, 5, objReqDataIni, durEtap4);
                            if (pesoArray5==null) {
                                break;
                            }
                            foreach (double peso5 in pesoArray5)
                            {
                                objReqDataIni.PesoInicialTmp = peso4;
                                prod = objReqDataIni.Productos.Find(r => r.Posicion == 5);
                                if ((int)(Math.Round(prod.DuracionMax + durEtap4,0)) < (int)objReqDataIni.EdadVenta)
                                {
                                    break;
                                }
                                objReqDataIni.Productos.Find(p => p.CveProducto == 5).PesoFinal = peso5;
                                TablaModel tModel5 = GetDataRegistro(prod, objReqDataIni);

                                duracionObjetivo = tModel.Duracion_Etapa + tModel2.Duracion_Etapa + tModel3.Duracion_Etapa + tModel4.Duracion_Etapa + tModel5.Duracion_Etapa;

                                if ((int)Math.Round(duracionObjetivo) == (int)objReqDataIni.EdadVenta)
                                {
                                    ResponseOptimizerModel respOpt = new ResponseOptimizerModel();
                                    List<TablaModel> lstProd = new List<TablaModel>();
                                    lstProd.Add(tModel);
                                    lstProd.Add(tModel2);
                                    lstProd.Add(tModel3);
                                    lstProd.Add(tModel4);
                                    lstProd.Add(tModel5);
                                    respOpt.Data = lstProd;
                                    respOpt.CveParametro = objReqDataIni.CveParametro;
                                    respOpt.Parametro = GetParametroEcoVal(objReqDataIni.CveParametro, "NomParametro");
                                    respOpt.Optimizer = GetOptimizer(objReqDataIni, lstProd);
                                    respOpt.Resultado = GetResultado(objReqDataIni, lstProd, respOpt.Optimizer);
                                    lstResp.Add(respOpt);



                                }
                                else {
                                    string entra = "aui";
                                }

                                if ((int)duracionObjetivo > (int)objReqDataIni.EdadVenta)
                                {
                                    break;
                                }



                            }

                        }
                    }
                }
            }





            return lstResp;


        }

        private ResultadoOptimizerModel GetResultado(RequestOptimizerModel objReq, List<TablaModel> tablaOpt, List<OptimizerModel> optimizer)
        {

            ResultadoOptimizerModel resultado = new ResultadoOptimizerModel();
            resultado.CveParametro = objReq.CveParametro;
            resultado.Seleccionado = objReq.CveParametro;

            List<TablaModel> tablaFilter = tablaOpt.FindAll(t => t.Costo > 0 && t.Duracion_Etapa > 0);
            int numRegistros = tablaFilter.Count;

            resultado.KilosProducidos = optimizer.Find(o => o.Orden == 0).Valor;

            resultado.PrecioVenta = objReq.PrecioVenta;
            resultado.Presupuesto = tablaFilter.Sum(t => t.PresupuestoAlimento);
            //resultado.Ca = resultado.Presupuesto / resultado.KilosProducidos;
            resultado.DuracionTotal = tablaFilter.Sum(t => t.Duracion_Etapa);
            //resultado.Gdp = resultado.KilosProducidos / resultado.DuracionTotal;
            resultado.Cda = resultado.Presupuesto / resultado.DuracionTotal;
            resultado.Costo = tablaFilter.Sum(t => t.Costo);



            //resultado.EdadVenta = tablaFilter.Max(t => t.Edad_Final); //objReq.EdadVenta ;
            resultado.PesoVenta = tablaFilter.Max(t => t.Peso_Final);

            resultado.CostoPonderado = optimizer.Find(o => o.Orden == 2).Valor;
            resultado.CostoTotalAlimento = optimizer.Find(o => o.Orden == 3).Valor;
            resultado.Ca = optimizer.Find(o => o.Orden == 4).Valor;
            resultado.CostoKiloProducido = optimizer.Find(o => o.Orden == 5).Valor;
            resultado.Utilidad = optimizer.Find(o => o.Orden == 6).Valor;
            resultado.Roi = optimizer.Find(o => o.Orden == 7).Valor;
            resultado.Gdp = optimizer.Find(o => o.Orden == 8).Valor;

            tablaFilter.ForEach(t => resultado.Presupuestos.Add(new EtapaResModel(t.Identificador, t.PresupuestoAlimento)));




            return resultado;
        }

        private List<double> GetArrPesoEtapa(double iniPeso, int identificador, RequestOptimizerModel objReqDataIni, double duracionAnterior)
        {
            objReqDataIni.IsOptimizar = true;
            objReqDataIni.PesoInicialTmp = iniPeso;
            ProductoModel prod=objReqDataIni.Productos.Find(r => r.Posicion == identificador);
            double duraRestanteMax=objReqDataIni.Productos.FindAll(t => t.CveProducto >= identificador ).Sum(t => t.DuracionMax);
            double duraRestanteMin = objReqDataIni.Productos.FindAll(t => t.CveProducto >= identificador).Sum(t => t.DuracionMin);
            List<double> pesoArray = new List<double>();

            double duracionTope = (int) Math.Round(objReqDataIni.EdadVenta - duracionAnterior,0);
            double duracionTopeIni = (int)Math.Round(objReqDataIni.EdadVenta - duracionAnterior, 0)- objReqDataIni.Productos.FindAll(t => t.CveProducto > identificador).Sum(t => t.DuracionMax); 
            if (identificador==5 &&  prod.DuracionMax < duracionTope) {
                return pesoArray;
            }
            if (duraRestanteMax < duracionTope) {
                return pesoArray;
            }
            
            double duraIni = prod.DuracionMin< duracionTopeIni? duracionTopeIni:prod.DuracionMin ;
            double duraFin = prod.DuracionMax>duracionTope ?duracionTope:prod.DuracionMax ;
            if (identificador == 5) {
                duraIni = duracionTope;
            }


            double dif = duraFin - duraIni;
            int stepV = 1;
            if (dif > 9) stepV = 2;
            if (dif > 19) stepV = 4;
            if (dif > 29) stepV = 6;
            if (dif > 39) stepV = 8;
            for (var x = duraIni; x <= duraFin; x += stepV)
            {
                double promedio = 10;
                TablaModel tModel = GetDataRegistroDuracion(prod, objReqDataIni, (int)x, promedio);
                promedio = Utileria.GetPromedio(tModel.Peso_Final, tModel.Peso_Inicial);
                int contador = 0;
                int contadorMax = 100;
                while (!Utileria.IsEqualDouble(promedio, tModel.Peso_Medio))
                {
                    tModel = GetDataRegistroDuracion(prod, objReqDataIni, (int)x, promedio);
                    promedio = Utileria.GetPromedio(tModel.Peso_Final, tModel.Peso_Inicial);
                    contador++;
                    if (contador > contadorMax) break;
                }

                pesoArray.Add(tModel.Peso_Final);
            }


            return pesoArray;
        }


        private double getMinPesoEtapa(double iniPeso, ProductoModel? prod, RequestOptimizerModel objReqDataIni)
        {
            objReqDataIni.IsOptimizar = true;
            objReqDataIni.PesoInicialTmp = iniPeso;
            for (var x = iniPeso; x <= 10; x += 0.05)
            {
                prod.PesoFinal = (double)x;
                TablaModel tModel = GetDataRegistro(prod, objReqDataIni);
                if (tModel.Duracion_Minima == tModel.Duracion_Maxima)
                {
                    if (tModel.Duracion_Maxima == 0) { return iniPeso; }
                    if (tModel.Duracion_Maxima > 0)
                    {
                        if ((int)(Math.Round(tModel.Duracion_Etapa)) == tModel.Duracion_Minima || tModel.Duracion_Etapa > tModel.Duracion_Maxima)
                        {
                            return prod.PesoFinal;
                        }
                    }
                }
                if (tModel.Duracion_Etapa >= tModel.Duracion_Minima && tModel.Duracion_Etapa <= tModel.Duracion_Maxima)
                {
                    return prod.PesoFinal;
                }

            }
            return iniPeso;
        }

        private double getMaxPesoEtapa(double iniPeso, ProductoModel? prod, RequestOptimizerModel objReqDataIni, double durEtapasAnteriores)
        {
            double pesoMax = iniPeso;
            objReqDataIni.IsOptimizar = true;
            objReqDataIni.PesoInicialTmp = iniPeso;
            double durRestante = objReqDataIni.EdadVenta - durEtapasAnteriores;
            for (var x = iniPeso; x <= 10; x += 0.05)
            {
                prod.PesoFinal = (double)x;
                TablaModel tModel = GetDataRegistro(prod, objReqDataIni);

                if (tModel.Duracion_Minima == tModel.Duracion_Maxima)
                {
                    if (tModel.Duracion_Maxima == 0) { return iniPeso; }
                    if (tModel.Duracion_Maxima > 0)
                    {
                        if ((int)(Math.Round(tModel.Duracion_Etapa)) == tModel.Duracion_Minima || tModel.Duracion_Etapa > tModel.Duracion_Maxima)
                        {
                            return prod.PesoFinal;
                        }
                    }
                }


                if (tModel.Duracion_Etapa >= tModel.Duracion_Minima && tModel.Duracion_Etapa <= tModel.Duracion_Maxima + 1)
                {
                    pesoMax = prod.PesoFinal;
                }
                if (tModel.Duracion_Etapa > tModel.Duracion_Maxima || tModel.Duracion_Etapa > durRestante)
                {
                    return pesoMax;
                }

            }
            return pesoMax;
        }

        private void SaveData(RespOptimizerModel objResp, RequestOptimizerModel objReq, int cveParametroSeleccionado)
        {

            string strSQLParam = "DELETE [OptimizerP_PlanA_Resultado] WHERE CvePlan =" + objReq.CvePlan.ToString();
            Database.execNonQuery(strSQLParam);

            strSQLParam = "INSERT INTO [OptimizerP_PlanA_Resultado](CvePlan,  Request,Response,FecAct,UsuAct) ";
            string jsonResp = JsonConvert.SerializeObject(objResp);
            string jsonReq = JsonConvert.SerializeObject(objReq);
            strSQLParam += "VALUES(" + objReq.CvePlan.ToString() + ",'" + jsonReq + "','" + jsonResp + "',GETDATE(),'" + objReq.UsuAct + "') ";
            Database.execNonQuery(strSQLParam);

            SaveDataReporte(objResp, objReq, cveParametroSeleccionado);

        }

        private void SaveDataReporte(RespOptimizerModel objResp, RequestOptimizerModel objReq, int cveParametroSeleccionado)
        {
            string strSQLParam = "DELETE [OptimizerP_PlanA_Reporte] WHERE CvePlan =" + objReq.CvePlan.ToString();
            Database.execNonQuery(strSQLParam);

            objResp.ResponseParametro.ForEach(resp =>
            {
                strSQLParam = "INSERT INTO [OptimizerP_PlanA_Reporte](CvePlan,  CveParametro,Seleccionado,CDA,Presupuesto,GDP,CA,DuracionTotal,PrecioVenta,PesoVenta,EdadVenta,KilosProducidos,Costo_Ponderado,Costo_TotalAlimento,Costo_KiloProducido,Costo, Utilidad,ROI,GDPKG ";
                strSQLParam += ", Presupuesto_P1, Presupuesto_P2, Presupuesto_P3, Presupuesto_P4, Presupuesto_P5) ";

                strSQLParam += "VALUES(" + objReq.CvePlan.ToString() + ",'" + resp.CveParametro + "','" + (cveParametroSeleccionado == resp.Resultado.CveParametro ? 1 : 2) + "','" + resp.Resultado.Cda.ToString() + "','" + resp.Resultado.Presupuesto.ToString() + "','" + resp.Resultado.Gdp.ToString() + "','" + resp.Resultado.Ca.ToString() + "','" + resp.Resultado.DuracionTotal.ToString() + "','" + resp.Resultado.PrecioVenta.ToString() + "','" + resp.Resultado.PesoVenta.ToString() + "','" + resp.Resultado.EdadVenta.ToString() + "','" + resp.Resultado.KilosProducidos.ToString() + "','" + resp.Resultado.CostoPonderado.ToString() + "','" + resp.Resultado.CostoTotalAlimento.ToString() + "','" + resp.Resultado.CostoKiloProducido.ToString() + "','" + resp.Resultado.Costo.ToString() + "','" + resp.Resultado.Utilidad.ToString() + "','" + resp.Resultado.Roi.ToString() + "','" + resp.Resultado.Gdp.ToString() + "'";
                for (int i = 1; i < 6; i++)
                {
                    double valPresupuesto = 0.0;
                    EtapaResModel presupuestoEtapa = resp.Resultado.Presupuestos.Find(pr => pr.Clave == i);
                    if (presupuestoEtapa != null)
                    {
                        valPresupuesto = presupuestoEtapa.Valor;
                    }
                    strSQLParam += ",'" + valPresupuesto + "'";
                };

                strSQLParam += ") ";

                Database.execNonQuery(strSQLParam);
            }
            );


        }

        [HttpPost]
        [Route("api/optimizer")]
        public ResponseOptimizerModel GetDataOptizerService([FromBody] RequestOptimizerModel objReqData)
        {
            string json = JsonConvert.SerializeObject(objReqData);
            string strSQLFor = "SELECT * FROM CatOptimizerP_Formula ";
            dtFor = Database.execQuery(strSQLFor);

            string strSQLRef = "SELECT * FROM CatOptimizerP_Referencias ";
            dtRef = Database.execQuery(strSQLRef);

            string strSQLVar = "SELECT * FROM CatOptimizerP_Variables ";
            dtVar = Database.execQuery(strSQLVar);

            string strSQLConst = "SELECT * FROM CatOptimizerP_Constantes ";
            dtConst = Database.execQuery(strSQLConst);

            return GetDataOpt(objReqData);
        }
        public ResponseOptimizerModel GetDataOpt(RequestOptimizerModel objReqData)
        {
            this.objReq = objReqData;
            List<TablaModel> tablaOpt = GetTablaOptimizer(objReqData);

            ResponseOptimizerModel respCal = new ResponseOptimizerModel()
            {
                Data = tablaOpt,
                Optimizer = GetOptimizer(objReq, tablaOpt)
            };

            return respCal;
        }


        public List<OptimizerModel> GetOptimizer(RequestOptimizerModel objReq, List<TablaModel> tablaOpt)
        {
            List<OptimizerModel> objResp = new List<OptimizerModel>();
            int idMin = tablaOpt.Min(p => System.Convert.ToInt32(p.Identificador));
            int idMax = tablaOpt.Max(p => System.Convert.ToInt32(p.Identificador));

            double pesoInicial = tablaOpt.Find(p => System.Convert.ToInt32(p.Identificador) == idMin).Peso_Inicial;
            double pesoFinal = tablaOpt.Find(p => System.Convert.ToInt32(p.Identificador) == idMax).Peso_Final;
            double pesoFinalMin = tablaOpt.Find(p => System.Convert.ToInt32(p.Identificador) == idMin).Peso_Final;

            double sumDuracion = tablaOpt.Sum(p => p.Duracion_Etapa);

            double kilosProducidos = pesoFinal - pesoInicial;
            objResp.Add(new OptimizerModel("Kilos producidos", 0, kilosProducidos));
            double valorKilosProd = objReq.PrecioVenta * kilosProducidos;
            //objResp.Add(new OptimizerModel(GetParametroEcoVal(1, "NomParametro"), short.Parse(GetParametroEcoVal(1, "Posicion")), valorKilosProd));
            double sumProductoCostoxPresupuesto = tablaOpt.Sum(p => p.Costo * p.PresupuestoAlimento);
            double sumPresupuesto = tablaOpt.Sum(p => p.PresupuestoAlimento);
            double costoPonderado = sumProductoCostoxPresupuesto / sumPresupuesto;//1
            objResp.Add(new OptimizerModel(GetParametroEcoVal(2, "NomParametro"), short.Parse(GetParametroEcoVal(2, "Posicion")), costoPonderado));
            objResp.Add(new OptimizerModel(GetParametroEcoVal(3, "NomParametro"), short.Parse(GetParametroEcoVal(3, "Posicion")), sumProductoCostoxPresupuesto));

            double ca = tablaOpt.Sum(p => p.PresupuestoAlimento / pesoFinal - pesoInicial);
            objResp.Add(new OptimizerModel(GetParametroEcoVal(4, "NomParametro"), short.Parse(GetParametroEcoVal(4, "Posicion")), ca));
            double costoProd = costoPonderado * ca;
            objResp.Add(new OptimizerModel(GetParametroEcoVal(5, "NomParametro"), short.Parse(GetParametroEcoVal(5, "Posicion")), costoProd));

            //objResp.Add(new OptimizerModel(GetParametroEcoVal(4, "NomParametro"), short.Parse(GetParametroEcoVal(4, "Posicion")), costoPonderado * (sumPresupuesto / kilosProducidos)));
            double utilidadAlimento = (objReq.PrecioVenta - costoProd) * pesoFinal - pesoInicial;
            //double utilidadAlimento = valorKilosProd - sumProductoCostoxPresupuesto;
            objResp.Add(new OptimizerModel(GetParametroEcoVal(6, "NomParametro"), short.Parse(GetParametroEcoVal(6, "Posicion")), utilidadAlimento));
            objResp.Add(new OptimizerModel(GetParametroEcoVal(7, "NomParametro"), short.Parse(GetParametroEcoVal(7, "Posicion")), (utilidadAlimento / sumProductoCostoxPresupuesto) * 100));
            double gdpkg = (pesoFinal - pesoFinalMin) / sumDuracion;
            objResp.Add(new OptimizerModel(GetParametroEcoVal(8, "NomParametro"), short.Parse(GetParametroEcoVal(8, "Posicion")), gdpkg));
            return objResp;
        }



        public List<TablaModel> GetTablaOptimizer(RequestOptimizerModel reqModel)
        {
            List<TablaModel> data = new List<TablaModel>();
            try
            {
                reqModel.Productos.ForEach(p =>
                {
                    TablaModel prod = GetDataRegistro(p, reqModel);
                    data.Add(prod);
                });
                return data;
            }
            catch (Exception ex)
            {
                return data;
            }
        }


        public TablaModel GetDataRegistro(ProductoModel prod, RequestOptimizerModel reqModel)

        {
            TablaModel registro = new TablaModel();
            int Identificador = prod.CveProducto;
            registro.Identificador = Identificador;
            registro.Costo = prod.Costo;
            registro.EM_Alimento = prod.EM;
            registro.EM_Alimento = GetEMAlimento(Identificador);

            registro.Costo = GetCosto(Identificador);
            registro.Duracion_Minima = GetDurMin(Identificador);
            registro.Duracion_Maxima = GetDurMax(Identificador);
            registro.Peso_Inicial = GetPeso_Inicial(Identificador);
            registro.Peso_Final = GetPeso_Final(Identificador);
            registro.Peso_Medio = GetPesoMedio(Identificador);
            registro.Req_EM = GetReqEM(Identificador, registro.Peso_Medio);
            registro.Duracion_Etapa = GetDuracion_Etapa(Identificador, registro.Peso_Medio);
            registro.SIDLysGDP = GetLisinaGDP(Identificador, registro.Peso_Medio);
            registro.ConsumoDiario = GetConsumoEstimado(Identificador, registro.Peso_Medio);
            registro.AlimentoOfrecer = GetAlimentoOfrecer(Identificador, registro.Peso_Medio);
            registro.PresupuestoAlimento = GetPresupuestoAlimento(Identificador, registro.Peso_Medio, registro.Duracion_Etapa);
            registro.Lisina = GetLisinaPorc(Identificador);
            registro.GDPNutrientes = GetGDPNutrientes(Identificador, registro.Peso_Medio);
            registro.GDPEcuacion = GetGDPEcuacion(Identificador, registro.Peso_Medio);
            registro.GDPUtilizar = GetGDPUtilizar(Identificador, registro.Peso_Medio);
            registro.CA = GetCA(Identificador, registro.Peso_Medio);

            return registro;
        }

        public TablaModel GetDataRegistroDuracion(ProductoModel prod, RequestOptimizerModel reqModel, int duracionObjetivo, double promedio)

        {
            TablaModel registro = new TablaModel();
            int Identificador = prod.CveProducto;
            registro.Identificador = Identificador;
            registro.Costo = prod.Costo;
            registro.Duracion_Etapa = duracionObjetivo;
            registro.EM_Alimento = prod.EM;
            registro.EM_Alimento = GetEMAlimento(Identificador);
            registro.Req_EM = GetReqEM(Identificador, promedio);
            registro.Costo = GetCosto(Identificador);
            registro.Duracion_Minima = GetDurMin(Identificador);
            registro.Duracion_Maxima = GetDurMax(Identificador);

            registro.Peso_Inicial = GetPeso_Inicial(Identificador);
            registro.Peso_Medio = promedio;

            registro.SIDLysGDP = Utileria.GetLisinaGDP(dtFor, objReq.CveReferencia, promedio);
            registro.ConsumoDiario = GetConsumoEstimado(Identificador, promedio);
            registro.AlimentoOfrecer = GetAlimentoOfrecer(Identificador, promedio);
            registro.PresupuestoAlimento = GetPresupuestoAlimento(Identificador, promedio, duracionObjetivo);
            registro.Lisina = GetLisinaPorc(Identificador);
            registro.GDPNutrientes = GetGDPNutrientes(Identificador, promedio);
            registro.GDPEcuacion = GetGDPEcuacion(Identificador, promedio);
            registro.GDPUtilizar = GetGDPUtilizar(Identificador, promedio);
            registro.Peso_Final = duracionObjetivo * registro.GDPUtilizar + registro.Peso_Inicial;
            registro.CA = GetCA(Identificador, promedio);

            return registro;
        }

        public double GetEMAlimento(int Identificador)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return prod.EM;
        }
        public double GetReqEM(int Identificador, double promedio)
        {

            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return Utileria.GetEnergiaMetabolizante(dtFor, objReq.CveReferencia, promedio, GetGDPEcuacion(Identificador, promedio));

        }



        public double GetPesoMedio(int Identificador)
        {
            double U11 = GetPeso_Inicial(Identificador);
            double V11 = GetPeso_Final(Identificador);
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            double Promedio = (U11 + V11) / 2;
            return Promedio;
        }


        public double GetCosto(int Identificador)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return prod.Costo;
        }
        public double GetDurMin(int Identificador)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return prod.DuracionMin;
        }
        public double GetDurMax(int Identificador)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return prod.DuracionMax;
        }

        public double GetPeso_Inicial(int Identificador)
        {
            if (objReq.IsOptimizar)
                return objReq.PesoInicialTmp;
            else if (Identificador == 1)
                return PESO_NACIMIENTO;
            else
                return GetPeso_Final(Identificador - 1);
        }
        public double GetPeso_Final(int Identificador)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return prod.PesoFinal;
        }

        public double GetLisinaGDP(int Identificador, double promedio)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return Utileria.GetLisinaGDP(dtFor, objReq.CveReferencia, promedio);
        }

        public double GetConsumoEstimado(int Identificador, double promedio)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return GetReqEM(Identificador, promedio) / GetEMAlimento(Identificador);
        }

        public double GetAlimentoOfrecer(int Identificador, double promedio)
        {
            //= M14 / (1 - ($C$11 / 100))
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return GetConsumoEstimado(Identificador, promedio) / (1 - objReq.Desperdicio / 100);
        }

        public double GetPresupuestoAlimento(int Identificador, double promedio, double duracionEtapa)
        {
            //= N14 * H14
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return GetAlimentoOfrecer(Identificador, promedio) * duracionEtapa;
        }
        public double GetLisinaPorc(int Identificador)
        {
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return prod.Lisina;
        }


        public double GetGDPNutrientes(int Identificador, double promedio)
        {
            //=((P14*10)*M14)/L14
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return ((prod.Lisina * 10) * GetConsumoEstimado(Identificador, promedio)) / GetLisinaGDP(Identificador, promedio);
        }

        public double GetGDPEcuacion(int Identificador, double promedio)
        {
            //=((P14*10)*M14)/L14
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            return Utileria.GetGananciaDP(dtFor, objReq.CveReferencia, promedio);
        }

        public double GetGDPUtilizar(int Identificador, double promedio)
        {
            //= SI(Q14 < R14, Q14, R14)
            ProductoModel prod = objReq.Productos.Find(p => p.CveProducto == Identificador);
            if (GetGDPNutrientes(Identificador, promedio) < GetGDPEcuacion(Identificador, promedio))
            {
                return GetGDPNutrientes(Identificador, promedio);
            }
            return GetGDPEcuacion(Identificador, promedio);
        }
        public double GetCA(int Identificador, double promedio)
        {
            //= M14 / S14
            return GetConsumoEstimado(Identificador, promedio) / GetGDPUtilizar(Identificador, promedio);
        }

        public double GetDuracion_Etapa(int Identificador, double promedio)
        {
            double pesoFinal = GetPeso_Final(Identificador);
            double pesoInicial = GetPeso_Inicial(Identificador);
            double gdp = GetGDPUtilizar(Identificador, promedio);
            return (pesoFinal - pesoInicial) / gdp;

        }


        private double GetConstantes(int v, int clave)
        {
            return Utileria.GetConstantes(dtConst, v, clave);
        }


        private double GetFormulas(int referencia, int v1, string v2)
        {
            return Utileria.GetFormulas(dtFor, referencia, v1, v2);
        }

        public string GetParametroEcoVal(int param, string column)
        {
            if (dtParam != null)
            {
                foreach (DataRow dtR in dtParam.Rows)
                {
                    if (dtR["CveParametro"].Equals(param))
                    {
                        string? v = dtR[column].ToString();
                        return v;
                    }
                }
            }
            return "0";
        }

    }

}
