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
        private DataTable dtEdad;



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

            string strSQLEdad = "SELECT * FROM CatOptimizerG_EdadProdPeso ";
            dtEdad = Database.execQuery(strSQLEdad);

            ResponseModel objResp = new ResponseModel();

            objResp.Variables.Add(GetVariable1(objReq));
            objResp.Variables.Add(GetVariable2(objReq));

            double iTemHum = Utileria.GetITH(objReq.Temperatura, objReq.Humedad);

            objResp.IndiceTemHum = iTemHum;
            objResp.DescripcionTemHum = Utileria.GetITHDesVal(iTemHum).Clave;
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
            objResp.Variables.Add(GetVariable18(objResp, objReq));
            objResp.Variables.Add(GetVariable19(objResp, objReq));
            objResp.Variables.Add(GetVariable17(objResp, objReq));
            objResp.Variables.Add(GetVariable20(objResp, objReq));
            objResp.Variables.Add(GetVariable21(objResp, objReq));
            objResp.Variables.Add(GetVariable22(objResp, objReq));
            objResp.Variables.Add(GetVariable23(objResp, objReq));
            objResp.Variables.Add(GetVariable24(objResp, objReq));
            objResp.Variables.Add(GetVariable25(objResp, objReq));
            objResp.Variables.Add(GetVariable26(objResp, objReq));
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

            objResp.Variables.Add(GetVariable39(objResp, objReq));
            objResp.Variables.Add(GetVariable40(objResp, objReq));
            objResp.Variables.Add(GetVariable37(objResp, objReq));
            objResp.Variables.Add(GetVariable38(objResp, objReq));
            objResp.Variables.Add(GetVariable41(objResp, objReq));
            objResp.Variables.Add(GetVariable42(objResp, objReq));
            objResp.Variables.Add(GetVariable43(objResp, objReq));
            objResp.Variables.Add(GetVariable44(objResp, objReq));
            objResp.Variables.Add(GetVariable45(objResp, objReq));
            objResp.Variables.Add(GetVariable46(objResp, objReq));
            objResp.Variables.Add(GetVariable47(objResp, objReq));
            objResp.Variables.Add(GetVariable48(objResp, objReq));
            objResp.Variables.Add(GetVariable49(objResp, objReq));
            objResp.Variables.Add(GetVariable50(objResp, objReq));
            objResp.Variables.Add(GetVariable51(objResp, objReq));
            objResp.Variables.Add(GetVariable52(objResp, objReq));
            objResp.Variables.Add(GetVariable53(objResp, objReq));
            objResp.Variables.Add(GetVariable54(objResp, objReq));
            objResp.Variables.Add(GetVariable55(objResp, objReq));
            objResp.Variables.Add(GetVariable56(objResp, objReq));
            objResp.Variables.Add(GetVariable57(objResp, objReq));
            objResp.Variables.Add(GetVariable58(objResp, objReq));
            objResp.Variables.Add(GetVariable59(objResp, objReq));
            objResp.Variables.Add(GetVariable60(objResp, objReq));
            objResp.Variables.Add(GetVariable61(objResp, objReq));
            objResp.Variables.Add(GetVariable62(objResp, objReq));

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
            string strSQLParam = "DELETE OptimizerG_PerfilN_Resultado WHERE CvePerfilN =" + objReq.CvePerfilN.ToString() + "";
            Database.execNonQuery(strSQLParam);

            strSQLParam = "INSERT INTO OptimizerG_PerfilN_Resultado(CvePerfilN,Request,Response,FecAct,UsuAct) ";
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
            variable.Etapas = objReq.EtapasModel.Select(p => new EtapaResModel(p.Clave, p.EdadInicial)).ToList();
            return variable;
        }
        private ResponseDataModel GetVariable2(RequestModel objReq)
        {
            ResponseDataModel variable = new();
            variable.NoVariable = 2;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p => new EtapaResModel(p.Clave, p.EdadFinal)).ToList();
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

        /// <sum mary>
        /// # de días
        ///
        /// </summary>
        /// <param name="objResp">Salida</param>
        /// <param name="objReq">Entrada</param>
        /// <returns><returns>
        private ResponseDataModel GetVariable4(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable3 = Utileria.GetVariableByNum(objResp, 3);


            variable.NoVariable = 4;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");

            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor1 = variable3.Etapas.Find(e => e.Clave == p.Clave).Valor;
                valor = valor1 * 7;

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
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 1);

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



                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }


        private ResponseDataModel GetVariable6(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);



            variable.NoVariable = 6;
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

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable7(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable5 = Utileria.GetVariableByNum(objResp, 5);
            ResponseDataModel variable6 = Utileria.GetVariableByNum(objResp, 6);

            variable.NoVariable = 7;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor5 = variable5.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor6 = variable6.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double[] valorArr = { valor5, valor6 };
                valor = valorArr.Average();
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable8(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable5 = Utileria.GetVariableByNum(objResp, 5);
            ResponseDataModel variable6 = Utileria.GetVariableByNum(objResp, 6);

            variable.NoVariable = 8;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor5 = variable5.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor6 = variable6.Etapas.Find(e => e.Clave == p.Clave).Valor;


                valor = valor6 - valor5;


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
            ResponseDataModel variable8 = Utileria.GetVariableByNum(objResp, 8);

            variable.NoVariable = 9;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor4 = variable4.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor8 = variable8.Etapas.Find(e => e.Clave == p.Clave).Valor;

                valor = valor8 / valor4;

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
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);

            variable.NoVariable = 10;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 1);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable11(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 11;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 1);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable12(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable10 = Utileria.GetVariableByNum(objResp, 10);
            ResponseDataModel variable11 = Utileria.GetVariableByNum(objResp, 11);

            variable.NoVariable = 12;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                double valor10 = variable10.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor11 = variable11.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double[] valorArr = { valor10, valor11 };
                valor = valorArr.Average();

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable13(ResponseModel objResp, RequestModel objReq)
        {

            ResponseDataModel variable = new();
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);

            variable.NoVariable = 13;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 2);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable14(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 14;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 2);
                }

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
            ResponseDataModel variable13 = Utileria.GetVariableByNum(objResp, 13);
            ResponseDataModel variable14 = Utileria.GetVariableByNum(objResp, 14);

            variable.NoVariable = 15;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor = 0;
                double valor13 = variable13.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor14 = variable14.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double[] valorArr = { valor13, valor14 };
                valor = valorArr.Average();

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable16(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 16;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor;
                valor = p.pesoObjetivo;

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable17(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);
            ResponseDataModel variable19 = Utileria.GetVariableByNum(objResp, 19);

            variable.NoVariable = 17;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor19 = variable19.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valConstante = 0.0145;
                if (p.Clave > 7)
                {
                    valConstante = 0.014;
                }
                double valor = (valor18 - valor19) * valConstante;
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
        private ResponseDataModel GetVariable18(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable12 = Utileria.GetVariableByNum(objResp, 12);
            ResponseDataModel variable15 = Utileria.GetVariableByNum(objResp, 15);


            variable.NoVariable = 18;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor12 = variable12.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor15 = variable15.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = (valor12 * valor15) / 100;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable19(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable12 = Utileria.GetVariableByNum(objResp, 12);
            ResponseDataModel variable16 = Utileria.GetVariableByNum(objResp, 16);


            variable.NoVariable = 19;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor12 = variable12.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor16 = variable16.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = (valor12 * valor16) / 100;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable20(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);

            variable.NoVariable = 20;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 3);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable21(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 21;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 3);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        

        private ResponseDataModel GetVariable22(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);

            variable.NoVariable = 22;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                    int edad = (int)variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, 99, edad, 5);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable23(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 23;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                    int edad = (int)variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, 99, edad, 5);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable24(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable1 = Utileria.GetVariableByNum(objResp, 1);

            variable.NoVariable = 24;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable1.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 4);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable25(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable2 = Utileria.GetVariableByNum(objResp, 2);

            variable.NoVariable = 25;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;

                if (p.tipoEtapa > 0)
                {
                    int edad = (int)variable2.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = Utileria.GetEdadProdPeso(dtEdad, objReq.Referencia, edad, 4);
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable26(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 26;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = p.EMAlimento;
                double factor = 0;
                if (objReq.TipoInstalaciones == "PISO")
                {
                    factor = (p.tipoEtapa > 1) ? Constantes.PISO_CRIANZA : Constantes.PISO_POSTURA;
                }else if (objReq.TipoInstalaciones == "AVIARIO")
                {
                    factor = (p.tipoEtapa > 1) ? Constantes.AVIARIO_CRIANZA : Constantes.AVIARIO_POSTURA;
                }
                else
                {
                    factor = (p.tipoEtapa > 1) ? Constantes.JAULA_CRIANZA : Constantes.JAULA_POSTURA;
                }

                  return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable27(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 27;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = p.EMAlimento;
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
        private ResponseDataModel GetVariable28(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);

            variable.NoVariable = 28;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                int tipoValor = Constantes.EM_CRIANZA;
                if (p.tipoEtapa == 2)
                {
                    tipoValor = Constantes.EM_PREPOSTURA;
                }
                else if (p.tipoEtapa == 3)
                {
                    tipoValor = Constantes.EM_POSTURA;
                }

                double valor = Utileria.GetFormulaData(dtFor, objReq.Referencia, valor7, valor9, tipoValor, valor18);

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }


        private ResponseDataModel GetVariable29(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable27 = Utileria.GetVariableByNum(objResp, 27);
            ResponseDataModel variable28 = Utileria.GetVariableByNum(objResp, 28);

            variable.NoVariable = 29;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor27 = variable27.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor28 = variable28.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor28 / valor27;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable30(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);

            variable.NoVariable = 30;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;


                double valor = Utileria.GetTN(valor7);
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
        /// <returns>=_xlfn.IFS($</returns>
        private ResponseDataModel GetVariable31(ResponseModel objResp, RequestModel objReq)
        {

            ResponseDataModel variable = new();
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);
            ResponseDataModel variable30 = Utileria.GetVariableByNum(objResp, 30);


            variable.NoVariable = 31;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor30 = variable30.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double tc1 = Constantes.TN_CRIANZA;
                double tc2 = Constantes.TN2_CRIANZA;
                if (p.Clave == Constantes.ETAPA_BOOSTER)
                {
                    tc1 = Constantes.TN_BOOSTER;
                    tc2 = Constantes.TN2_BOOSTER;
                }
                else if (p.tipoEtapa == 3)
                {
                    tc1 = Constantes.TN_POSTURA;
                    tc2 = Constantes.TN2_POSTURA;
                }

                double valor = tc1 * Math.Pow(valor7, tc2) * (valor30 - objReq.Temperatura);

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable32(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable28 = Utileria.GetVariableByNum(objResp, 28);
            ResponseDataModel variable31 = Utileria.GetVariableByNum(objResp, 31);


            variable.NoVariable = 32;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor28 = variable28.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor31 = variable31.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor28 + valor31;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable33(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable27 = Utileria.GetVariableByNum(objResp, 27);
            ResponseDataModel variable32 = Utileria.GetVariableByNum(objResp, 32);


            variable.NoVariable = 33;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor27 = variable27.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor32 = variable32.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor = valor32 / valor27;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable34(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable33 = Utileria.GetVariableByNum(objResp, 33);

            variable.NoVariable = 34;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor33 = variable33.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor33 / (1 - (objReq.DesperdicioCrianza / 100)));
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }



        private ResponseDataModel GetVariable35(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable4 = Utileria.GetVariableByNum(objResp, 4);
            ResponseDataModel variable33 = Utileria.GetVariableByNum(objResp, 33);
            ResponseDataModel variable34 = Utileria.GetVariableByNum(objResp, 34);

            variable.NoVariable = 35;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor4 = variable4.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor33 = variable33.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor34 = variable34.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = valor4 * valor34;
                if (p.tipoEtapa > 1)
                {
                    valor = valor4 * valor33;
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable36(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable8 = Utileria.GetVariableByNum(objResp, 8);
            ResponseDataModel variable24 = Utileria.GetVariableByNum(objResp, 24);
            ResponseDataModel variable25 = Utileria.GetVariableByNum(objResp, 25);
            ResponseDataModel variable35 = Utileria.GetVariableByNum(objResp, 35);

            variable.NoVariable = 36;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                if (p.tipoEtapa > 2) {
                    double valor24 = variable24.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    double valor25 = variable25.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    double valor35 = variable35.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = (valor35 / (valor25-valor24 ));
                }
                else { 
                    double valor8 = variable8.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    double valor35 = variable35.Etapas.Find(e => e.Clave == p.Clave).Valor;

                    valor = (valor35 / valor8);
                    }
                

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable37(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable39 = Utileria.GetVariableByNum(objResp, 39);

            variable.NoVariable = 37;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                if (p.tipoEtapa < 3) { 
                    double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor39 = variable39.Etapas.Find(e => e.Clave == p.Clave).Valor;

                valor = (valor39 / valor9);

            }
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable38(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);
            ResponseDataModel variable39 = Utileria.GetVariableByNum(objResp, 39);

            variable.NoVariable = 38;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                if (p.tipoEtapa > 2)
                {
                    double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    double valor39 = variable39.Etapas.Find(e => e.Clave == p.Clave).Valor;

                    valor = (valor39 / valor18) * 1000;
                }
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable39(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);

            variable.NoVariable = 39;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                int tipoValor = Constantes.LISINA_CRIANZA;
                if (p.tipoEtapa == 2)
                {
                    tipoValor = Constantes.LISINA_PREPOSTURA;
                }
                else if (p.tipoEtapa == 3)
                {
                    tipoValor = Constantes.LISINA_POSTURA;
                }

                double valor = Utileria.GetFormulaDataLisina(dtFor, objReq.Referencia, objReq.EstatusConfort, p.Clave, valor7, valor9, tipoValor, valor18);

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable40(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable33 = Utileria.GetVariableByNum(objResp, 33);
            ResponseDataModel variable39 = Utileria.GetVariableByNum(objResp, 39);

            variable.NoVariable = 40;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor33 = variable33.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor39 = variable39.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor39 / valor33) / 10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable41(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable17 = Utileria.GetVariableByNum(objResp, 17);
            ResponseDataModel variable39 = Utileria.GetVariableByNum(objResp, 39);

            variable.NoVariable = 41;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                if (p.tipoEtapa == 3 && p.Clave > 6)
                {
                    double valor17 = variable17.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    double valor39 = variable39.Etapas.Find(e => e.Clave == p.Clave).Valor;

                    valor = valor39 - valor17;
                }

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable42(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable33 = Utileria.GetVariableByNum(objResp, 33);
            ResponseDataModel variable41 = Utileria.GetVariableByNum(objResp, 41);

            variable.NoVariable = 42;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = 0;
                if (p.tipoEtapa == 3 && p.Clave > 6)
                {
                    double valor33 = variable33.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    double valor41 = variable41.Etapas.Find(e => e.Clave == p.Clave).Valor;
                    valor = (valor41 / valor33) / 10;
                }
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable43(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);

            variable.NoVariable = 43;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                int tipoValor = Constantes.PROTEINA_CRIANZA;
                if (p.tipoEtapa == 2)
                {
                    tipoValor = Constantes.PROTEINA_PREPOSTURA;
                }
                else if (p.tipoEtapa == 3)
                {
                    tipoValor = Constantes.PROTEINA_POSTURA;
                }

                double valor = Utileria.GetFormulaDataLisina(dtFor, objReq.Referencia, objReq.EstatusConfort, p.Clave, valor7, valor9, tipoValor, valor18);

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable44(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable29 = Utileria.GetVariableByNum(objResp, 29);
            ResponseDataModel variable43 = Utileria.GetVariableByNum(objResp, 43);

            variable.NoVariable = 44;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor29 = variable29.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor43 = variable43.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor43 / valor29) / 10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable45(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);

            variable.NoVariable = 45;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                int tipoValor = Constantes.FOSFORO_CRIANZA;
                if (p.tipoEtapa == 2)
                {
                    tipoValor = Constantes.FOSFORO_PREPOSTURA;
                }
                else if (p.tipoEtapa == 3)
                {
                    tipoValor = Constantes.FOSFORO_POSTURA;
                }

                double valor = Utileria.GetFormulaDataLisina(dtFor, objReq.Referencia, objReq.EstatusConfort, p.Clave, valor7, valor9, tipoValor, valor18);

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable46(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable29 = Utileria.GetVariableByNum(objResp, 29);
            ResponseDataModel variable45 = Utileria.GetVariableByNum(objResp, 45);

            variable.NoVariable = 46;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor29 = variable29.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor45 = variable45.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor45 / valor29) / 10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }



        private ResponseDataModel GetVariable47(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            // comparo variable 2 y variable1        
            ResponseDataModel variable7 = Utileria.GetVariableByNum(objResp, 7);
            ResponseDataModel variable9 = Utileria.GetVariableByNum(objResp, 9);
            ResponseDataModel variable18 = Utileria.GetVariableByNum(objResp, 18);

            variable.NoVariable = 47;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {

                double valor7 = variable7.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor9 = variable9.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor18 = variable18.Etapas.Find(e => e.Clave == p.Clave).Valor;
                int tipoValor = Constantes.CALCIO_CRIANZA;
                if (p.tipoEtapa == 2)
                {
                    tipoValor = Constantes.CALCIO_PREPOSTURA;
                }
                else if (p.tipoEtapa == 3)
                {
                    tipoValor = Constantes.CALCIO_POSTURA;
                }

                double valor = Utileria.GetFormulaDataCalcio(dtFor, objReq.Referencia, objReq.EstatusConfort, p.Clave, valor7, valor9, tipoValor, valor18);

                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable48(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();
            ResponseDataModel variable33 = Utileria.GetVariableByNum(objResp, 33);
            ResponseDataModel variable47 = Utileria.GetVariableByNum(objResp, 47);

            variable.NoVariable = 48;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor33 = variable33.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor47 = variable47.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = (valor33 / valor47) / 10;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable49(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();


            variable.NoVariable = 49;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable50(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();


            variable.NoVariable = 50;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable51(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 51;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable52(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 52;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable53(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();


            variable.NoVariable = 53;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable54(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();


            variable.NoVariable = 54;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable55(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();


            variable.NoVariable = 55;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable56(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();


            variable.NoVariable = 56;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable57(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            variable.NoVariable = 57;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor = Utileria.GetConstantes(dtConst, variable.NoVariable, p.Clave);
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable58(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable40 = Utileria.GetVariableByNum(objResp, 40);
            ResponseDataModel variable53 = Utileria.GetVariableByNum(objResp, 53);

            variable.NoVariable = 58;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor40 = variable40.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor53 = variable53.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = valor40 * valor53;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }
        private ResponseDataModel GetVariable59(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable40 = Utileria.GetVariableByNum(objResp, 40);
            ResponseDataModel variable54 = Utileria.GetVariableByNum(objResp, 54);

            variable.NoVariable = 59;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor40 = variable40.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor54 = variable54.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = valor40 * valor54;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable60(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable40 = Utileria.GetVariableByNum(objResp, 40);
            ResponseDataModel variable55 = Utileria.GetVariableByNum(objResp, 55);

            variable.NoVariable = 60;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor40 = variable40.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor55 = variable55.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = valor40 * valor55;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable61(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable40 = Utileria.GetVariableByNum(objResp, 40);
            ResponseDataModel variable56 = Utileria.GetVariableByNum(objResp, 56);

            variable.NoVariable = 61;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor40 = variable40.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor56 = variable56.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = valor40 * valor56;
                return new EtapaResModel(p.Clave, valor);
            }).ToList();

            return variable;
        }

        private ResponseDataModel GetVariable62(ResponseModel objResp, RequestModel objReq)
        {
            ResponseDataModel variable = new();

            ResponseDataModel variable40 = Utileria.GetVariableByNum(objResp, 40);
            ResponseDataModel variable57 = Utileria.GetVariableByNum(objResp, 57);

            variable.NoVariable = 62;
            variable.Variable = GetVariable(variable.NoVariable);
            variable.Posicion = int.Parse(GetVariable(variable.NoVariable, "Posicion"));
            variable.MostrarCliente = GetVariable(variable.NoVariable, "MostrarCliente");
            variable.Etapas = objReq.EtapasModel.Select(p =>
            {
                double valor40 = variable40.Etapas.Find(e => e.Clave == p.Clave).Valor;
                double valor57 = variable57.Etapas.Find(e => e.Clave == p.Clave).Valor;

                double valor = valor40 * valor57;
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
            return Utileria.GetFormulas(dtFor, referencia, v1, v2);
        }

        private double GetEdadProdPeso(int referencia, int v1, int v2)
        {
            return Utileria.GetEdadProdPeso(dtFor, referencia, v1, v2);
        }
    }

}
