namespace WSOptimizerGallinas.Controllers
{
    using System;
    using System.Data;
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

        public static double GetReferencias(DataTable dtRef,int cveReferencia, string cveColumna)
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

        public static string GetVariable(DataTable dtVar,int cveVariable, string columna = "NomVariable")
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

        public static double GetITH(double temperatura, double humedad) {
            return  0.81 * temperatura + (temperatura - 14.4) * humedad / 100 + 46.4;
        }

        public static CatalogoModel GetITHDesVal(double iTemHum)
        {
            if (iTemHum >= 81.1)
            {
                return new CatalogoModel("Severo estrés por calor", "0.80");
            }
            else if (iTemHum >= 75.1)
            {
                return new CatalogoModel("Estrés por calor", "0.90");
            }
            else if (iTemHum >= 69.1)
            {
                return new CatalogoModel("Ligero estrés por calor", "0.95");
            }
            else if (iTemHum <= 69)
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


        public static double GetEnergiaMetabolizante(DataTable dt,int cveReferencia, double pesoMedio, double gdp)
        {
            double a = GetFormulas(dt,cveReferencia, 3, "a");
            double b = GetFormulas(dt,cveReferencia, 3, "b");
            double c = GetFormulas(dt, cveReferencia, 3, "c");
            double d = GetFormulas(dt, cveReferencia, 3, "d");
            double x = GetFormulas(dt, cveReferencia, 3, "X");

            
            return (a * Math.Pow(pesoMedio, x)) + (b + c * pesoMedio + d * Math.Pow(pesoMedio, 2)) * gdp;
            
        }

        public static double GetLisinaGDP(DataTable dt, int cveReferencia, double pesoMedio)
        {
            //g Lis Dig/GDP = a*P^3 + b*P^2 + c*P + d
            double a = GetFormulas(dt, cveReferencia, 6, "a");
            double b = GetFormulas(dt, cveReferencia, 6, "b");
            double c = GetFormulas(dt, cveReferencia, 6, "c");
            double d = GetFormulas(dt, cveReferencia, 6, "d");

            return (a * Math.Pow(pesoMedio, 3)) + (b * Math.Pow(pesoMedio, 2)+(c*pesoMedio ) +d);

        }

        public static double GetGananciaDP(DataTable dt, int cveReferencia, double pesoMedio)
        {
            //GDP = a + b*P + c*P^2 + d*P^3			
            double a = GetFormulas(dt, cveReferencia, 2, "a");
            double b = GetFormulas(dt, cveReferencia, 2, "b");
            double c = GetFormulas(dt, cveReferencia, 2, "c");
            double d = GetFormulas(dt, cveReferencia, 2, "d");

            return (a +b* pesoMedio +c* Math.Pow(pesoMedio, 2)) + (d * Math.Pow(pesoMedio, 3) );

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
