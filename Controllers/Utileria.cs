namespace WSOptimizerGallinas.Controllers
{
    using System;
    using System.Data;
    using System.Diagnostics.Eventing.Reader;
    using System.Runtime.CompilerServices;
    using System.Runtime.Intrinsics.Arm;
    using WSOptimizerGallinas.Models;

    public class Utileria
    {
        public static ResponseDataModel GetVariableByNum(ResponseModel objResp, int noEtapa)
        {
            try
            {
                ResponseDataModel objData = objResp.Variables.Find(p => p.NoVariable == noEtapa);
                return objData;
            }
            catch (Exception ex)
            {
                throw new Exception("No se encontró la etapa ");
            }
        }

        public static double GetPromedioLst(List<double> datos)
        {
            return datos.Average();
        }
        public static double GetPromedio(double val1, double val2)
        {
            List<double> datos = new List<double>();
            datos.Add(val1);
            datos.Add(val2);
            return datos.Average();
        }

        public static string GetVariables(DataTable dtVar, int cveVariable, string cveColumna)
        {
            foreach (DataRow dtR in dtVar.Rows)
            {
                if (dtR["CveVariable"].Equals(cveVariable))
                    return dtR[cveColumna].ToString();
            }
            return "";
        }

        public static double GetConstantes(DataTable dtConst, int cveVariable, int cveConstante)
        {
            foreach (DataRow dtR in dtConst.Rows)
            {
                if (dtR["CveVariable"].ToString().Equals(cveVariable.ToString()) & dtR["CveConstante"].ToString().Equals(cveConstante.ToString()))
                {
                    return double.Parse((string)dtR["Valor"].ToString());
                }
            }
            return 0;
        }

        public static double GetReferencias(DataTable dtRef, int cveReferencia, string cveColumna)
        {
            if (dtRef != null)
            {
                foreach (DataRow dtR in dtRef.Rows)
                {
                    if (dtR["CveReferencia"].Equals(cveReferencia))
                    {
                        return double.Parse((string)dtR[cveColumna]);
                    }

                }
            }
            return 0;
        }

        public static double GetFormulas(DataTable dtFor, int cveReferencia, int tipoFormula, string cveFormula)
        {
            if (dtFor != null)
            {
                foreach (DataRow dtR in dtFor.Rows)
                {
                    if (dtR["CveReferencia"].Equals(cveReferencia) && dtR["TipoFormula"].Equals(tipoFormula) && dtR["CveFormula"].ToString().Trim().Equals(cveFormula.Trim()))
                    {
                        return double.Parse(dtR["Valor"].ToString());
                    }

                }
            }
            return 0;
        }


        public static string GetVariable(DataTable dtVar, int cveVariable, string columna = "NomVariable")
        {
            if (dtVar != null)
            {
                foreach (DataRow dtR in dtVar.Rows)
                {
                    if (dtR["CveVariable"].Equals(cveVariable))
                    {
                        string? v = dtR[columna].ToString();
                        return v;
                    }
                }
            }
            return "";
        }

        public static double GetEdadProdPeso(DataTable dtFor, int cveReferencia, int edad, int tipo)
        {
            if (dtFor != null)
            {
                foreach (DataRow dtR in dtFor.Rows)
                {
                    if (dtR["CveReferencia"].Equals(cveReferencia) && dtR["edad"].Equals(edad) && dtR["tipo"].Equals(tipo))
                    {
                        return double.Parse(dtR["Valor"].ToString());
                    }

                }
            }
            return 0;
        }


        public static double GetITH(double temperatura, double humedad)
        {
            return 0.81 * temperatura + (temperatura - 14.4) * humedad / 100 + 46.4;
        }

        public static CatalogoModel GetITHDesVal(double iTemHum)
        {
            if (iTemHum >= 81.1)
            {
                return new CatalogoModel("Severo estrés por calor", "0.81");
            }
            else if (iTemHum >= 75.1)
            {
                return new CatalogoModel("Estrés por calor", "0.90");
            }
            else if (iTemHum >= 74)
            {
                return new CatalogoModel("Ligero estrés por calor", "0.95");
            }
            else if (iTemHum <= 73.1)
            {
                return new CatalogoModel("Zona termoneutral", "1.00");
            }
            return new CatalogoModel("", "1.00");

        }
        public static bool IsNumeric(string text)
        {
            double test;
            return double.TryParse(text, out test);
        }


        public static double GetFormulaData(DataTable dt, int cveReferencia, double pesoMedio, double gdp, int valorTipo, double masaHuevo = 0)
        {
            double n = GetFormulas(dt, cveReferencia, valorTipo, "n");
            double a = GetFormulas(dt, cveReferencia, valorTipo, "a");
            double b = GetFormulas(dt, cveReferencia, valorTipo, "b");
            double c = GetFormulas(dt, cveReferencia, valorTipo, "c");
            double d = GetFormulas(dt, cveReferencia, valorTipo, "d");
            double e = GetFormulas(dt, cveReferencia, valorTipo, "e");
            int[] array = { 4, 7, 10, 13, 16 };
            if (Array.Exists(array, e => e == valorTipo))
            {
                //EM = (a * P ^ n )+ (b + c*MH + d*MH^2)+ e*GDP					
                return (a * Math.Pow(pesoMedio, n)) + (b + c * masaHuevo + d * Math.Pow(masaHuevo, 2)) + e * gdp;
            }
            else
            {
                return (a * Math.Pow(pesoMedio, n)) + (b * Math.Pow(pesoMedio, 3) + c * Math.Pow(pesoMedio, 2) + d * pesoMedio + e) * gdp;
            }

        }

        public static double GetFormulaDataLisina(DataTable dt, int cveReferencia, string estatusConfort, int idEtapa, double pesoMedio, double gdp, int valorTipo, double masaHuevo = 0)
        {
            double estautusVal = GetFormulas(dt, 99, idEtapa, estatusConfort);
            double n = GetFormulas(dt, cveReferencia, valorTipo, "n");
            double a = GetFormulas(dt, cveReferencia, valorTipo, "a");
            double b = GetFormulas(dt, cveReferencia, valorTipo, "b");
            double c = GetFormulas(dt, cveReferencia, valorTipo, "c");
            double d = GetFormulas(dt, cveReferencia, valorTipo, "d");
            double e = GetFormulas(dt, cveReferencia, valorTipo, "e");
            int[] array = { 4, 7, 10, 13, 16 };
            if (Array.Exists(array, e => e == valorTipo))
            {
                //EM = (a * P ^ n )+ (b + c*MH + d*MH^2)+ e*GDP					
                return estautusVal * ((a * Math.Pow(pesoMedio, n)) + (b + c * masaHuevo + d * Math.Pow(masaHuevo, 2)) + e * gdp);
            }
            else
            {
                return estautusVal * ((a * Math.Pow(pesoMedio, n)) + (b * Math.Pow(pesoMedio, 3) + c * Math.Pow(pesoMedio, 2) + d * pesoMedio + e) * gdp);
            }

        }

        public static double GetFormulaDataCalcio(DataTable dt, int cveReferencia, string estatusConfort, int idEtapa, double pesoMedio, double gdp, int valorTipo, double masaHuevo = 0)
        {
            double estautusVal = GetFormulas(dt, 99, idEtapa, estatusConfort);
            double n = GetFormulas(dt, cveReferencia, valorTipo, "n");
            double a = GetFormulas(dt, cveReferencia, valorTipo, "a");
            double b = GetFormulas(dt, cveReferencia, valorTipo, "b");
            double c = GetFormulas(dt, cveReferencia, valorTipo, "c");
            double d = GetFormulas(dt, cveReferencia, valorTipo, "d");
            double e = GetFormulas(dt, cveReferencia, valorTipo, "e");
            int[] array = { 4, 7, 10, 13, 16 };
            if (Array.Exists(array, e => e == valorTipo))
            {
                //EM = (a * P ^ n )+ (b + c*MH + d*MH^2)+ e*GDP					
                return ((a * Math.Pow(pesoMedio, n)) + (b + c * masaHuevo + d * Math.Pow(masaHuevo, 2)) + e);
            }
            else
            {
                return ((a * Math.Pow(pesoMedio, n)) + (b * Math.Pow(pesoMedio, 3) + c * Math.Pow(pesoMedio, 2) + d * pesoMedio + e) * gdp);
            }

        }

        public static double GetTN(double pesoMedio)
        {
            return Constantes.TN_A * Math.Pow(pesoMedio, 5) + Constantes.TN_B * Math.Pow(pesoMedio, 4) + Constantes.TN_C * Math.Pow(pesoMedio, 3) + Constantes.TN_D * Math.Pow(pesoMedio, 2) + Constantes.TN_E * pesoMedio + Constantes.TN_F;

        }

        public static bool IsEqualDouble(double value1, double value2, int precision = 3)
        {
            var dif = Math.Abs(Math.Round(value1, precision) - Math.Round(value2, precision));
            while (precision > 0)
            {
                dif *= 10;
                precision--;
            }

            return dif < 1;
        }
    }

}
