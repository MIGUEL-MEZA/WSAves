using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using WSOptimizerGallinas.App_Data;
using WSOptimizerGallinas.Models;

namespace WSOptimizerGallinas.Controllers
{
    public class PerfilNutrimentalController : Controller
    {
        private DataTable dtRef;
        private DataTable dtVar;
        private DataTable dtConst;
        private DataTable dtFor;

        [HttpPost]
        [Route("api/data")]
        public ResponseModel GetCalculo([FromBody] RequestModel objReq)
        {
            
            if (objReq == null) throw new Exception("Dato incorrecto en la entrada");

            string strSQLFor = "SELECT * FROM CatOptimizerG_Formula ";
            dtFor = Database.execQuery(strSQLFor);

            string strSQLRef = "SELECT * FROM CatOptimizerG_Referencias ";
            dtRef = Database.execQuery(strSQLRef);

            string strSQLVar = "SELECT * FROM CatOptimizerG_Variables ";
            dtVar = Database.execQuery(strSQLVar);

            string strSQLConst = "SELECT * FROM CatOptimizerG_Constantes ";
            dtConst = Database.execQuery(strSQLConst);

            ResponseModel objResp = new ResponseModel();

            objResp.Variables.Add(GetVariable1(objReq));
            objResp.Variables.Add(GetVariable2(objReq));

            double iTemHum = Utileria.GetITH(objReq.Temperatura , objReq.Humedad );
            
            objResp.IndiceTemHum = iTemHum;
            objResp.DescripcionTemHum = Utileria.GetITHDesVal(iTemHum ).Clave ;
            objResp.ProductividadITH = double.Parse(Utileria.GetITHDesVal(iTemHum).Valor);

            objResp.Variables.Add(GetVariable3(objResp, objReq));
            objResp.Variables.Add(GetVariable4(objResp, objReq));
            objResp.Variables.Add(GetVariable5(objResp, objReq));
            objResp.Variables.Add(GetVariable6(objResp, objReq));
            objResp.Variables.Add(GetVariable7(objResp, objReq));
            objResp.Variables.Add(GetVariable8(objResp, objReq));
            objResp.Variables.Add(GetVariable9(objResp, objReq));
            objResp.Variables.Add(GetVariable10(objResp, objReq));
            objResp.Variables.Add(GetVariable11(objResp, objReq));
            objResp.Variables.Add(GetVariable12(objResp, objReq));
            objResp.Variables.Add(GetVariable13(objResp, objReq));
            objResp.Variables.Add(GetVariable14(objResp, objReq));
            objResp.Variables.Add(GetVariable15(objResp, objReq));
            objResp.Variables.Add(GetVariable16(objResp, objReq));
            objResp.Variables.Add(GetVariable17(objResp, objReq));
            objResp.Variables.Add(GetVariable18(objResp, objReq));
            objResp.Variables.Add(GetVariable19(objResp, objReq));
            
            objResp.Variables.Add(GetVariable24(objResp, objReq));
            objResp.Variables.Add(GetVariable26(objResp, objReq));

            objResp.Variables.Add(GetVariable20(objResp, objReq));
            objResp.Variables.Add(GetVariable21(objResp, objReq));
            objResp.Variables.Add(GetVariable22(objResp, objReq));
            objResp.Variables.Add(GetVariable23(objResp, objReq));
            
            objResp.Variables.Add(GetVariable25(objResp, objReq));
            
            objResp.Variables.Add(GetVariable27(objResp, objReq));
            objResp.Variables.Add(GetVariable28(objResp, objReq));
            objResp.Variables.Add(GetVariable29(objResp, objReq));
            objResp.Variables.Add(GetVariable30(objResp, objReq));
            objResp.Variables.Add(GetVariable31(objResp, objReq));
            objResp.Variables.Add(GetVariable32(objResp, objReq));

            objResp.Variables.Add(GetVariable33(objResp, objReq));
            objResp.Variables.Add(GetVariable34(objResp, objReq));
            objResp.Variables.Add(GetVariable35(objResp, objReq));
            objResp.Variables.Add(GetVariable36(objResp, objReq));
            objResp.Variables.Add(GetVariable37(objResp, objReq));
            objResp.Variables.Add(GetVariable38(objResp, objReq));
            objResp.Variables.Add(GetVariable39(objResp, objReq));
            objResp.Variables.Add(GetVariable40(objResp, objReq));
            objResp.Variables.Add(GetVariable41(objResp, objReq));
            objResp.Variables.Add(GetVariable42(objResp, objReq));
            objResp.Variables.Add(GetVariable43(objResp, objReq));
            objResp.Variables.Add(GetVariable44(objResp, objReq));
            
            if (objReq.CvePerfilN > 0)
                SaveData(objResp, objReq);

            //string jsonString = JsonSerializer.Serialize<ResponseModel>(objResp);
            //string jsonString= JsonConvert.SerializeObject(objResp);
            //Console.WriteLine(jsonString);

            objResp.Variables = objResp.Variables.OrderBy(p => p.NoVariable).ToList();
            return objResp;
        }

    

        private void SaveData(ResponseModel objResp, RequestModel objReq)
        {
            string strSQLParam = "DELETE OptimizerP_PerfilN_Resultado WHERE CvePerfilN =" + objReq.CvePerfilN.ToString() + "";
            Database.execNonQuery(strSQLParam);

            strSQLParam = "INSERT INTO OptimizerP_PerfilN_Resultado(CvePerfilN,Request,Response,FecAct,UsuAct) ";
            string jsonResp = JsonConvert.SerializeObject(objResp);
            string jsonReq = JsonConvert.SerializeObject(objReq);
            strSQLParam += "VALUES(" + objReq.CvePerfilN.ToString() + ",'" + jsonReq + "','" + jsonResp + "',GETDATE(),'" + objReq.UsuAct + "') ";
            Database.execNonQuery(strSQLParam);
        }


        private ResponseDataModel GetVariable1(RequestModel objReq)
        {
            ResponseDataModel variable = new();
            variable.NoVariable = 1;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p => new EtapaResModel(p.Clave, p.PesoInicial)).ToList();
            return variable;
        }
        private ResponseDataModel GetVariable2(RequestModel objReq)
        {
            ResponseDataModel variable = new();
            variable.NoVariable = 2;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p => new EtapaResModel(p.Clave, p.PesoFinal)).ToList();
            return variable;
        }

        private ResponseDataModel GetVariable3(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 3;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor1 = variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor2 = variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;
                return new EtapaResModel(p.Clave, valor2 - valor1);
            }).ToList();

            return variable;
        }

        /// <summary>
        /// Peso Inicial (Kg) *Corregido por ITH*
        ///
        ///P = a * exp(-exp(-b * (t - c)))
        /// </summary>
        /// <param name="objResp">Salida</param>
        /// <param name="objReq">Entrada</param>
        /// <returns>_xlfn.IFS($C$6 =$I$9,$J$9 * EXP(-EXP(-$K$9 * (C15 -$L$9))),$C$6 =$I$10,$J$10 * EXP(-EXP(-$K$10 * (C15 -$L$10))),$C$6 =$I$11,$J$11 * EXP(-EXP(-$K$11 * (C15 -$L$11))),$C$6 =$I$12,$J$12 * EXP(-EXP(-$K$12 * (C15 -$L$12))),$C$6 =$I$13,$J$13 * EXP(-EXP(-$K$13 * (C15 -$L$13))),$C$6 =$I$14,$J$14 * EXP(-EXP(-$K$14 * (C15 -$L$14))))</returns>
        private ResponseDataModel GetVariable4(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);

            variable.NoVariable = 4;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");

            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor1 = variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double a = GetFormulas(objReq.Referencia, 1, "a");
                double b = GetFormulas(objReq.Referencia, 1, "b");
                double c = GetFormulas(objReq.Referencia, 1, "c");

                valor = a * Math.Exp(-Math.Exp(-b * (valor1 - c)));

                if (p.Clave > 1)
                {
                    valor += 0.014;
                    //suma 2 veces verificar
                    valor += 0.014;
                }
                if (p.Clave > 2)
                {
                    valor *= objResp.ProductividadITH;
                }


                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        
        /// <summary>
        /// Peso Final  (Kg) *Corregido por ITH*
        ///
        ///P = a * exp(-exp(-b * (t - c)))
        /// </summary>
        /// <param name="objResp">Salida</param>
        /// <param name="objReq">Entrada</param>
        /// <returns>_xlfn.IFS($C$6 =$I$9,$J$9 * EXP(-EXP(-$K$9 * (C15 -$L$9))),$C$6 =$I$10,$J$10 * EXP(-EXP(-$K$10 * (C15 -$L$10))),$C$6 =$I$11,$J$11 * EXP(-EXP(-$K$11 * (C15 -$L$11))),$C$6 =$I$12,$J$12 * EXP(-EXP(-$K$12 * (C15 -$L$12))),$C$6 =$I$13,$J$13 * EXP(-EXP(-$K$13 * (C15 -$L$13))),$C$6 =$I$14,$J$14 * EXP(-EXP(-$K$14 * (C15 -$L$14))))</returns>
        private ResponseDataModel GetVariable5(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 5;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");

            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor2 = variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double a = GetFormulas(objReq.Referencia, 1, "a");
                double b = GetFormulas(objReq.Referencia, 1, "b");
                double c = GetFormulas(objReq.Referencia, 1, "c");

                valor = a * Math.Exp(-Math.Exp(-b * (valor2 - c)));

                valor += 0.014;
                    //suma 2 veces verificar
                valor += 0.014;
                
                if (p.Clave > 1)
                {
                    valor *= objResp.ProductividadITH;
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }


        private ResponseDataModel GetVariable6(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable4 = Utileria.GetVariableByNum(objResp, 4);
            ResponseDataModel variable5 = Utileria.GetVariableByNum(objResp, 5);
            

            variable.NoVariable = 6;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor4 = variable4.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor5 = variable5.Etapas.Find(e => e.Clave == p.Clave).Valor;
                
                valor = Utileria.GetPromedio(valor4, valor5);
                
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable7(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable4 = Utileria.GetVariableByNum(objResp, 4);
            ResponseDataModel variable5 = Utileria.GetVariableByNum(objResp, 5);

            variable.NoVariable = 7;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor4 = variable4.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor5 = variable5.Etapas.Find(e => e.Clave == p.Clave).Valor;
                valor = valor5 - valor4;
                return new EtapaResModel(p.Clave, valor );
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable8(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable3 = Utileria.GetVariableByNum(objResp, 3);
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);

            variable.NoVariable = 8;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor3 = variable3.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;

                if (valor7 > 0) {
                    valor = valor7 / valor3;
                }
                
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>PRE - INICIO NUPIO    1.10    1.08    1.06    1.02    1.00 
        /// SIN NUPIO   1.00    1.00    1.00    1.00    1.00</returns>

        private ResponseDataModel GetVariable9(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable4 = Utileria.GetVariableByNum(objResp, 4);

            List<CatalogoModel> lstCat = new List<CatalogoModel>();

            lstCat.Add(new CatalogoModel("1", "1.00"));
            lstCat.Add(new CatalogoModel("2", "1.10"));
            lstCat.Add(new CatalogoModel("3", "1.08"));
            lstCat.Add(new CatalogoModel("4", "1.06"));
            lstCat.Add(new CatalogoModel("5", "1.02"));
            

            variable.NoVariable = 9;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor4 = variable4.Etapas.Find(e => e.Clave == p.Clave).Valor;
                valor = valor4;
                if (objReq.PreIniciadorNupio.Equals ("SI") ){
                    valor *= double.Parse(lstCat.Find(r => p.Clave.ToString().Equals(r.Clave)).Valor);
                }
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>PRE - INICIO NUPIO    1.10    1.08    1.06    1.02    1.00
        /// SIN NUPIO   1.00    1.00    1.00    1.00    1.00</returns>
        private ResponseDataModel GetVariable10(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable5 = Utileria.GetVariableByNum(objResp, 5);

            List<CatalogoModel> lstCat = new List<CatalogoModel>();
      
            lstCat.Add(new CatalogoModel("1", "1.10"));
            lstCat.Add(new CatalogoModel("2", "1.08"));
            lstCat.Add(new CatalogoModel("3", "1.06"));
            lstCat.Add(new CatalogoModel("4", "1.02"));
            lstCat.Add(new CatalogoModel("5", "1.00"));

            variable.NoVariable = 10;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor5 = variable5.Etapas.Find(e => e.Clave == p.Clave).Valor;
                valor = valor5;
                if (objReq.PreIniciadorNupio.Equals("SI"))
                {
                    valor *= double.Parse(lstCat.Find(r => p.Clave.ToString().Equals(r.Clave)).Valor);
                }
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable11(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable10 = Utileria.GetVariableByNum(objResp, 10);

            variable.NoVariable = 11;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor10 = variable10.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double promedio =Utileria.GetPromedio(valor9, valor10);
                double valor = promedio;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable12(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable10 = Utileria.GetVariableByNum(objResp, 10);

            variable.NoVariable = 12;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor10 = variable10.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor10-valor9;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable13(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable3 = Utileria.GetVariableByNum(objResp, 3);
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable10 = Utileria.GetVariableByNum(objResp, 10);

            variable.NoVariable = 13;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor3 = variable3.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor10 = variable10.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = (valor10 - valor9)/valor3;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable14(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 14;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = objReq.EtapasModel.Find(e => e.Clave == p.Clave).EMAlimento;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        /// <summary>
        /// Energia Metabolizable (Kcal/d) 			
        ///EM = (a* P ^ X )+ (b + c* P + d* P^2)*GDP
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>=_xlfn.IFS($C$6=$I$9,($T$9*C28^$S$9)+($U$9+$V$9*C28+$W$9*C28^2)*C30,$C$6=$I$10,($T$10*C28^$S$10)+($U$10+$V$10*C28+$W$10*C28^2)*C30,$C$6=$I$11,($T$11*C28^$S$11)+($U$11+$V$11*C28+$W$11*C28^2)*C30,$C$6=$I$12,($T$12*C28^$S$12)+($U$12+$V$12*C28+$W$12*C28^2)*C30,$C$6=$I$13,($T$13*C28^$S$13)+($U$13+$V$13*C28+$W$13*C28^2)*C30,$C$6=$I$14,($T$14*C28^$S$14)+($U$14+$V$14*C28+$W$14*C28^2)*C30)</returns>
        private ResponseDataModel GetVariable15(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);
            ResponseDataModel variable13 = Utileria.GetVariableByNum(objResp, 13);

            variable.NoVariable = 15;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor13 = variable13.Etapas.Find(e => e.Clave == p.Clave).Valor;
                
                double valor = Utileria.GetEnergiaMetabolizante(dtFor,objReq.Referencia, valor11, valor13);
                
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable16(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable14 = Utileria.GetVariableByNum(objResp, 14);
            ResponseDataModel variable15 = Utileria.GetVariableByNum(objResp, 15);

            variable.NoVariable = 16;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor;
                double valor14 = variable14.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor15 = variable15.Etapas.Find(e => e.Clave == p.Clave).Valor;
                valor = valor15 / valor14;

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        /// <summary>
        /// Consumo promedio diario (Kg/d) 
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>=C34/(1-($C$11/100))</returns>
        private ResponseDataModel GetVariable17(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            
            ResponseDataModel variable16 = Utileria.GetVariableByNum(objResp, 16);
            

            variable.NoVariable = 17;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor16 = variable16.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor16 / (1-(objReq.Desperdicio /100));
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable18(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable3 = Utileria.GetVariableByNum(objResp, 3);
            ResponseDataModel variable17 = Utileria.GetVariableByNum(objResp, 17);

            variable.NoVariable = 18;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor17 = variable17.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor3 = variable3.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor17 *valor3;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>C.A</returns>
        private ResponseDataModel GetVariable19(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable12 = Utileria.GetVariableByNum(objResp, 12);
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);

            variable.NoVariable = 19;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor12 = variable12.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor18/valor12;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }


        /// <summary>
        /// Relación SID LYS:EM
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>=(C46/C33)*1000</returns>
        private ResponseDataModel GetVariable20(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            
            ResponseDataModel variable15 = Utileria.GetVariableByNum(objResp, 15);
            ResponseDataModel variable24 = Utileria.GetVariableByNum(objResp, 24);
            
            variable.NoVariable = 20;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor15 = variable15.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor24 = variable24.Etapas.Find(e => e.Clave == p.Clave).Valor;
                
                double valor = (valor24/valor15)*1000;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable21(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable15 = Utileria.GetVariableByNum(objResp, 15);
            ResponseDataModel variable26 = Utileria.GetVariableByNum(objResp, 26);

            variable.NoVariable = 21;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor15 = variable15.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor26 = variable26.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor26 / valor15) * 1000;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable22(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 22;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 22, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>=C46/C30</returns>
        private ResponseDataModel GetVariable23(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable24 = Utileria.GetVariableByNum(objResp, 24);
            ResponseDataModel variable13 = Utileria.GetVariableByNum(objResp, 13);
            
            variable.NoVariable = 23;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor24 = variable24.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor13 = variable13.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor24 / valor13) ;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }


        /// <summary>
        /// SID Lisina (g/d)
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>Lisina Digestible (g/d)	 Lis.Dig. = (a* P ^ X ) + (b + c* GDP + d* GDP^2)*P</returns>
        private ResponseDataModel GetVariable24(ResponseModel objResp, RequestModel objReq)
        {
            //($K$40*C28^$J$40)+($L$40+$M$40*C30+$N$40*C30^2)*C28
            //($K$42*C28^$J$42)+($L$42+$M$42*C30+$N$42*C30^2)*C28
            //($K$45*C28^$J$45)+($L$45+$M$45*C30+$N$45*C30^2)*C28
            ResponseDataModel variable = new();
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);
            ResponseDataModel variable13 = Utileria.GetVariableByNum(objResp, 13);

            variable.NoVariable = 24;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double a = GetFormulas(objReq.Referencia, 5, "a");
                double b = GetFormulas(objReq.Referencia, 5, "b");
                double c = GetFormulas(objReq.Referencia, 5, "c");
                double d = GetFormulas(objReq.Referencia, 5, "d");
                double x = GetFormulas(objReq.Referencia, 5, "X");

                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor13 = variable13.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = (a * Math.Pow(valor11, x)) + (b + c * valor13 + d * Math.Pow(valor13, 2)) * valor11;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable25(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable24 = Utileria.GetVariableByNum(objResp, 24);
            ResponseDataModel variable16 = Utileria.GetVariableByNum(objResp, 16);


            variable.NoVariable = 25;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor24 = variable24.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor16 = variable16.Etapas.Find(e => e.Clave == p.Clave).Valor;


                double valor = (valor24 / valor16)/10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        /// <summary>
        /// Fósforo Disponibel (g/d)		
        /// P dis(g/d) = (a* PM)+(b+c* G+d*^2)		
        /// </summary>
        /// <param name="objResp"></param>
        /// <param name="objReq"></param>
        /// <returns>=_xlfn.IFS($C$6=$I$9,(($K$19*C28^$J$19)+($L$19+$M$19*C30+$N$19*C30^2)+$O$19*C30),$C$6=$I$10,(($K$20*C28^$J$20)+($L$20+$M$20*C30+$N$20*C30^2)),$C$6=$I$11,(($K$21*C28^$J$21)+($L$21+$M$21*C30+$N$21*C30^2)),$C$6=$I$12,(($K$22*C28^$J$22)+($L$22+$M$22*C30+$N$22*C30^2)),$C$6=$I$13,(($K$23*C28^$J$23)+($L$23+$M$23*C30+$N$23*C30^2)),$C$6=$I$14,(($K$24*C28^$J$24)+($L$24+$M$24*C30+$N$24*C30^2)))</returns>
        private ResponseDataModel GetVariable26(ResponseModel objResp, RequestModel objReq)
        {

            //(($K$19*C28^$J$19)+($L$19+$M$19*C30+$N$19*C30^2)+$O$19*C30)
            //(($K$21*C28^$J$21)+($L$21+$M$21*C30+$N$21*C30^2))
            //(($K$24*C28^$J$24)+($L$24+$M$24*C30+$N$24*C30^2))
            ResponseDataModel variable = new();
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);
            ResponseDataModel variable13 = Utileria.GetVariableByNum(objResp, 13);
            

            variable.NoVariable = 26;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor13 = variable13.Etapas.Find(e => e.Clave == p.Clave).Valor;

                
                double a = GetFormulas(objReq.Referencia, 4, "a");
                double b = GetFormulas(objReq.Referencia, 4, "b");
                double c = GetFormulas(objReq.Referencia, 4, "c");
                double d = GetFormulas(objReq.Referencia, 4, "d");
                double x = GetFormulas(objReq.Referencia, 4, "X");

                double valor =  a * Math.Pow (valor11 ,x) + (b + c * valor13 + d*Math.Pow (valor13,2));
                
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable27(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable26 = Utileria.GetVariableByNum(objResp, 26);
            ResponseDataModel variable16 = Utileria.GetVariableByNum(objResp, 16);


            variable.NoVariable = 27;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor26 = variable26.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor16 = variable16.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = (valor26 / valor16) / 10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable28(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable26 = Utileria.GetVariableByNum(objResp, 26);
            ResponseDataModel variable22 = Utileria.GetVariableByNum(objResp, 22);


            variable.NoVariable = 28;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor26 = variable26.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor22 = variable22.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor26 * valor22 ;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable29(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable28 = Utileria.GetVariableByNum(objResp, 28);
            ResponseDataModel variable16 = Utileria.GetVariableByNum(objResp, 16);


            variable.NoVariable = 29;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                
                double valor28 = variable28.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor16 = variable16.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = (valor28/valor16)/10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        

        private ResponseDataModel GetVariable30(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);
            
            //=$L$30 * C28 ^ 2 +$L$31 * C28 +$L$32
//            0.001, - 0.022, 0.233

            variable.NoVariable = 30;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;
                
                double c1 = GetConstantes( 30, 1);
                double c2 = GetConstantes( 30, 2);
                double c3 = GetConstantes( 30, 3);
                
                double valor = c1 * Math.Pow(valor11,2) + c2*valor11+c3;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable31(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);

            variable.NoVariable = 31;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double c1 = GetConstantes( 31, 1);
                double c2 = GetConstantes(31, 2);
                double c3 = GetConstantes(31, 3);

                double valor = c1 * Math.Pow(valor11, 2) + c2 * valor11 + c3;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
            return variable;
        }

        private ResponseDataModel GetVariable32(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);

            variable.NoVariable = 32;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double c1 = GetConstantes(32, 1);
                double c2 = GetConstantes( 32, 2);
                double c3 = GetConstantes( 32, 3);

                double valor = c1 * Math.Pow(valor11, 2) + c2 * valor11 + c3;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable33(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            
            variable.NoVariable = 33;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 33, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable34(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 34;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 34, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable35(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 35;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 35, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable36(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 36;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 36, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable37(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 37;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 37, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable38(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 38;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = GetConstantes( 38, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        

        private ResponseDataModel GetVariable39(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable33 = Utileria.GetVariableByNum(objResp, 33);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);

            variable.NoVariable = 39;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor33 = variable33.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor25* valor33;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable40(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable34 = Utileria.GetVariableByNum(objResp, 34);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);

            variable.NoVariable = 40;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor34 = variable34.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor25 * valor34;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable41(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable35 = Utileria.GetVariableByNum(objResp, 35);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);

            variable.NoVariable = 41;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor35 = variable35.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor25 * valor35;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable42(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable36 = Utileria.GetVariableByNum(objResp, 36);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);

            variable.NoVariable = 42;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor36 = variable36.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor25 * valor36;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable43(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable37 = Utileria.GetVariableByNum(objResp, 37);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);

            variable.NoVariable = 43;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor37 = variable37.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor25 * valor37;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable44(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable38 = Utileria.GetVariableByNum(objResp, 38);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);

            variable.NoVariable = 44;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor38 = variable38.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor25 * valor38;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private string GetVariable(int noVariable, string v)
        {
            return Utileria.GetVariable(dtVar, noVariable, v);
        }

        private double GetConstantes(int v, int clave)
        {
            return Utileria.GetConstantes(dtConst, v, clave);
        }

        private string GetVariable(int noVariable)
        {
            return Utileria.GetVariable(dtVar, noVariable);
        }

        private double GetFormulas(int referencia, int v1, string v2)
        {
            return Utileria.GetFormulas(dtFor, referencia,v1,v2);
        }


    }

}
