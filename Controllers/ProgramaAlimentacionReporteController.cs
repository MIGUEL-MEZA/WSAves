using ClosedXML.Excel;
using ExpertPdf.HtmlToPdf;
using Newtonsoft.Json;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WSOptimizerGallinas.App_Data;
using WSOptimizerGallinas.Models;

namespace WSOptimizerGallinas.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class ProgramaAlimentacionReporteController : Controller
    {
        private static readonly XLColor ExcelDarkBlue = XLColor.FromHtml("#0b2e57");
        private static readonly XLColor ExcelLightBlue = XLColor.FromHtml("#6084d7");
        private static readonly XLColor ExcelGridBlue = XLColor.FromHtml("#d6deed");
        private static readonly XLColor ExcelCategoryBlue = XLColor.FromHtml("#dce9f5");
        private static readonly XLColor ExcelAlternateRow = XLColor.FromHtml("#eef2f8");
        private static readonly CultureInfo ReportNumberCulture = CultureInfo.InvariantCulture;

        private readonly IConfiguration configuration;

        public ProgramaAlimentacionReporteController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("api/reportes/programaalimentacion/{id}/excel")]
        public IActionResult GetProgramaAlimentacionExcel(long id, [FromQuery] string? seccion = null)
        {
            try
            {
                ProgramaReporteModel reporte = GetReporte(id, seccion);
                byte[] bytes = GenerateExcelBytes(reporte);
                string fileName = $"ProgramaAlimentacion_{id}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest("Error generando el archivo Excel: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("api/reportes/programaalimentacion/{id}/pdf")]
        public IActionResult GetProgramaAlimentacionPdf(long id, [FromQuery] string? seccion = null)
        {
            try
            {
                ProgramaReporteModel reporte = GetReporte(id, seccion);
                byte[] bytes = GeneratePdfBytes(reporte);
                string fileName = $"ProgramaAlimentacion_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                return File(bytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest("Error generando el archivo PDF: " + ex.Message);
            }
        }

        private ProgramaReporteModel GetReporte(long id, string? seccion)
        {
            if (id <= 0)
            {
                throw new Exception("El id del programa no es valido.");
            }

            string seccionNormalizada = NormalizeSeccion(seccion);
            PlanAContextModel contexto = GetPlanAContext(id, seccionNormalizada);

            return string.Equals(seccionNormalizada, "comparativo", StringComparison.OrdinalIgnoreCase)
                ? BuildComparativoReporte(contexto)
                : BuildPresupuestoReporte(contexto);
        }

        private static string NormalizeSeccion(string? seccion)
        {
            if (string.IsNullOrWhiteSpace(seccion))
            {
                return "presupuesto";
            }

            string value = seccion.Trim().ToLowerInvariant();
            return value == "comparativo" ? "comparativo" : "presupuesto";
        }

        private static PlanAContextModel GetPlanAContext(long id, string seccion)
        {
            DataRow rowResultado = GetPlanAResultadoRow(id);
            DataRow rowPlanA = GetPlanARow(id);
            DataRow? rowCliente = GetClienteRow(rowPlanA);
            ResponseOptimizerModel response = GetResponse(rowResultado);
            List<PlanAEtapaModel> etapas = GetPlanAEtapas(rowPlanA, id);

            return new PlanAContextModel
            {
                CvePlan = id,
                Seccion = seccion,
                PlanARow = rowPlanA,
                ResultadoRow = rowResultado,
                ClienteRow = rowCliente,
                Response = response,
                Etapas = etapas
            };
        }

        private static DataRow GetPlanAResultadoRow(long id)
        {
            string sql = "SELECT * FROM [OptimizerG_PlanA_Resultado] WHERE CvePlan = " + id.ToString(CultureInfo.InvariantCulture);

            DataTable dt = Database.execQuery(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                throw new Exception("No se encontraron datos del resultado para el programa indicado.");
            }

            return dt.Rows[0];
        }

        private static DataRow GetPlanARow(long id)
        {
            string sql =
                "SELECT PA.*, " +
                "P.FolioR AS FolioRPN, " +
                "R.NomReferencia " +
                "FROM [OptimizerG_PlanA] PA " +
                "INNER JOIN [OptimizerG_PerfilN] P ON P.CvePerfilN = PA.CvePerfilN " +
                "INNER JOIN [CatOptimizerG_Referencias] R ON R.CveReferencia = PA.CveReferencia " +
                "WHERE PA.CvePlan = " + id.ToString(CultureInfo.InvariantCulture);

            DataTable dt = Database.execQuery(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                throw new Exception("No se encontraron datos del programa indicado.");
            }

            return dt.Rows[0];
        }

        private static DataRow? GetClienteRow(DataRow rowPrograma)
        {
            if (!rowPrograma.Table.Columns.Contains("CodCliente") || rowPrograma["CodCliente"] == DBNull.Value)
            {
                return null;
            }

            string codCliente = rowPrograma["CodCliente"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(codCliente))
            {
                return null;
            }

            string sql = "SELECT * FROM [Clientes] WHERE CodCliente = '" + codCliente.Replace("'", "''") + "'";
            DataTable dt = Database.execQuery(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return dt.Rows[0];
        }

        private static ResponseOptimizerModel GetResponse(DataRow row)
        {
            string responseJson = row["Response"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(responseJson))
            {
                throw new Exception("El programa no contiene informacion en Response.");
            }

            ResponseOptimizerModel? response = JsonConvert.DeserializeObject<ResponseOptimizerModel>(responseJson);
            if (response == null)
            {
                throw new Exception("No fue posible leer el Response del programa.");
            }

            return response;
        }

        private static string GetClienteReporte(DataRow rowPrograma, DataRow? rowCliente)
        {
            string nombreClienteReporte = GetTrimmedValue(rowCliente, "NomClienteR");
            if (!string.IsNullOrWhiteSpace(nombreClienteReporte))
            {
                return nombreClienteReporte;
            }

            string nombreClienteA = GetTrimmedValue(rowCliente, "NomClienteA");
            string nombreCliente = GetTrimmedValue(rowCliente, "NomCliente");

            if (string.IsNullOrWhiteSpace(nombreClienteA))
            {
                return !string.IsNullOrWhiteSpace(nombreCliente)
                    ? nombreCliente
                    : GetTrimmedValue(rowPrograma, "NomCliente");
            }

            if (string.IsNullOrWhiteSpace(nombreCliente))
            {
                return nombreClienteA;
            }

            if (string.Equals(nombreClienteA, nombreCliente, StringComparison.OrdinalIgnoreCase))
            {
                return nombreClienteA;
            }

            return nombreClienteA + " (" + nombreCliente + ")";
        }

        private static string GetTrimmedValue(DataRow? row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return (row[columnName]?.ToString() ?? string.Empty).Trim();
        }

        private static double GetDoubleValue(DataRow? row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return 0d;
            }

            return double.TryParse(Convert.ToString(row[columnName], CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out double value)
                ? value
                : 0d;
        }

        private static ProgramaReporteModel BuildPresupuestoReporte(PlanAContextModel contexto)
        {
            ProgramaReporteModel reporte = CreateBaseReporte(contexto);
            ResponseOptimizerModel parametro = contexto.Response;

            List<PlanAEtapaModel> etapasAplicadas = contexto.Etapas
                .Where(e => string.Equals(e.Aplica, "S", StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.OrdenVisual)
                .ToList();

            List<TablaModel> datosOrdenados = parametro.Data
                .OrderBy(d => d.Identificador)
                .ToList();

            int coincidenciasPorOrdenVisual = datosOrdenados.Count(d => etapasAplicadas.Any(e => e.OrdenVisual == d.Identificador));
            bool usarMapeoSecuencial = coincidenciasPorOrdenVisual < Math.Min(etapasAplicadas.Count, datosOrdenados.Count);

            List<TablaModel> datosPresupuesto = usarMapeoSecuencial
                ? datosOrdenados.Take(etapasAplicadas.Count).ToList()
                : datosOrdenados.Where(d => etapasAplicadas.Any(e => e.OrdenVisual == d.Identificador)).ToList();

            reporte.PresupuestoFilas = datosPresupuesto
                .Select((d, index) =>
                {
                    PlanAEtapaModel? etapa = usarMapeoSecuencial
                        ? etapasAplicadas.ElementAtOrDefault(index)
                        : etapasAplicadas.FirstOrDefault(e => e.OrdenVisual == d.Identificador);

                    return new ProgramaPresupuestoFilaModel
                    {
                        CveEtapa = etapa?.CveEtapa ?? d.Identificador,
                        NomEtapa = ResolveProgramaStageName(etapa),
                        Costo = d.Costo,
                        EdadInicial = d.EdadInicial,
                        EdadFinal = d.EdadFinal,
                        Mortalidad = d.Mortalidad,
                        NoAves = d.NoAves,
                        ConsumoAlimento = d.ConsumoAlimento,
                        PesoHuevo = d.PesoHuevo,
                        Produccion = d.Produccion,
                        MasaHuevo = d.MasaHuevo,
                        ConversionAlimenticia = d.ConversionAlimenticia
                    };
                })
                .ToList();

            reporte.PresupuestoTotales = new ProgramaPresupuestoTotalesModel
            {
                ConsumoAlimento = parametro.Resultado.analisisProductivoTotal.consumoTotalAlimento,
                MasaHuevo = parametro.Resultado.analisisProductivoTotal.masaTotalHuevo,
                ConversionAlimenticia = parametro.Resultado.analisisProductivoTotal.conversionAlimenticia
            };

            reporte.PresupuestoResumenTotal = new List<ProgramaResumenItemModel>
            {
                new ProgramaResumenItemModel("COSTO PROGRAMA DE ALIMENTACIÓN TOTAL, $/AVE", parametro.Resultado.analisisProductivoTotal.costoProgramaAlimentacion, true, "N2"),
                new ProgramaResumenItemModel("COSTO PONDERADO DEL ALIMENTO TOTAL, $", parametro.Resultado.analisisProductivoTotal.costoPonderado, true, "N2"),
                new ProgramaResumenItemModel("COSTO POR KG PRODUCIDO, $/KG HUEVO", parametro.Resultado.analisisProductivoTotal.costoProducidoHuevo, true, "N2"),
                new ProgramaResumenItemModel("MASA DE HUEVO, KG/PARVADA", parametro.Resultado.analisisProductivoTotal.masaHuevoParvada, "N2"),
                new ProgramaResumenItemModel("PRECIO VENTA ($/Kg huevo)", GetDoubleValue(contexto.PlanARow, "PrecioVentaH"), true, "N2"),
                new ProgramaResumenItemModel("INGRESO POR VENTA DE HUEVO, $/PARVADA", parametro.Resultado.analisisProductivoTotal.ingresoHuevoParvada, true, "N2"),
                new ProgramaResumenItemModel("UTILIDAD POR CONCEPTO DE ALIMENTACIÓN, $/PARVADA", parametro.Resultado.analisisProductivoTotal.utilidadAlimentacionParvada, true, "N2"),
                new ProgramaResumenItemModel("ROI, %", parametro.Resultado.analisisProductivoTotal.roi, true, "N2")
            };

            reporte.PresupuestoResumenCrianza = new List<ProgramaResumenItemModel>
            {
                new ProgramaResumenItemModel("COSTO PONDERADO DEL ALIMENTO CRIANZA, $", parametro.Resultado.analisisProductivoCrianza.costoPonderadoCrianza, true, "N2"),
                new ProgramaResumenItemModel("CONSUMO DE ALIMENTO CRIANZA, KG/AVE", parametro.Resultado.analisisProductivoCrianza.consumoAlimentoCrianza, "N2"),
                new ProgramaResumenItemModel("COSTO PROGRAMA DE ALIMENTACIÓN CRIANZA, $/POLLITA", parametro.Resultado.analisisProductivoCrianza.costoProgramaCrianza, true, "N2")
            };

            reporte.PresupuestoResumenPostura = new List<ProgramaResumenItemModel>
            {
                new ProgramaResumenItemModel("COSTO PONDERADO DEL ALIMENTO POSTURA, $/KG", parametro.Resultado.analisisProductivoPostura.costoPonderadoPostura, true, "N2"),
                new ProgramaResumenItemModel("CONSUMO TOTAL DE ALIMENTO POSTURA, KG/AVE", parametro.Resultado.analisisProductivoPostura.consumoAlimentoPostura, "N2"),
                new ProgramaResumenItemModel("COSTO POR KG PRODUCIDO, $/KG HUEVO", parametro.Resultado.analisisProductivoPostura.costoProducidoPostura, true, "N2"),
                new ProgramaResumenItemModel("CONVERSIÓN ALIMENTICIA", parametro.Resultado.analisisProductivoPostura.conversionAlimenticiaPostura, "N2"),
                new ProgramaResumenItemModel("COSTO PROGRAMA DE ALIMENTACIÓN POSTURA, $/AVE", parametro.Resultado.analisisProductivoPostura.costoProgramaPostura, true, "N2"),
                new ProgramaResumenItemModel("MASA DE HUEVO TOTAL, KG/AVE", parametro.Resultado.analisisProductivoPostura.masaHuevoPostura, "N2"),
                new ProgramaResumenItemModel("INGRESO POR VENTA DE HUEVO, $/AVE", parametro.Resultado.analisisProductivoPostura.ingresoVentaHuevo, true, "N2"),
                new ProgramaResumenItemModel("UTILIDAD POR CONCEPTO DE ALIMENTACIÓN, $", parametro.Resultado.analisisProductivoPostura.utilidadAlimentacion, true, "N2"),
                new ProgramaResumenItemModel("ROI, %", parametro.Resultado.analisisProductivoPostura.roiPostura, true, "N2")
            };

            return reporte;
        }

        private static ProgramaReporteModel BuildComparativoReporte(PlanAContextModel contexto)
        {
            ProgramaReporteModel reporte = CreateBaseReporte(contexto);
            List<ProgramaComparativoColumnaModel> columnas = GetComparativoColumnas(contexto.CvePlan);
            List<PlanAEtapaModel> etapasAplicadas = contexto.Etapas
                .Where(e => string.Equals(e.Aplica, "S", StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.OrdenVisual)
                .ToList();

            reporte.ComparativoColumnas = columnas
                .Select((c, index) => index == 0 ? string.Empty : c.Titulo)
                .ToList();

            reporte.ComparativoPresupuestos = GetComparativoDatos(31, contexto.CvePlan, columnas)
                .Select((row, index) => new ProgramaComparativoFilaModel
                {
                    Etiqueta = etapasAplicadas.ElementAtOrDefault(index) is PlanAEtapaModel etapa
                        ? ResolveProgramaStageName(etapa)
                        : row[0],
                    Valores = FormatComparativoValores(
                        etapasAplicadas.ElementAtOrDefault(index) is PlanAEtapaModel etapaValores
                            ? ResolveProgramaStageName(etapaValores)
                            : row[0],
                        row.Skip(1).ToList(),
                        reporte.ComparativoColumnas.Skip(1).ToList(),
                        "presupuestos"),
                    Visible = etapasAplicadas.ElementAtOrDefault(index) != null
                        && HasVisibleComparativoRow(row[0], row.Skip(1).ToList())
                })
                .Where(r => r.Visible)
                .ToList();

            reporte.ComparativoPresupuestosTotales = BuildComparativoTotales(
                reporte.ComparativoPresupuestos,
                reporte.ComparativoColumnas.Skip(1).ToList(),
                "presupuestos");

            reporte.ComparativoVariables = GetComparativoDatos(3, contexto.CvePlan, columnas)
                .Select((row, index) => new ProgramaComparativoFilaModel
                {
                    Etiqueta = row[0],
                    Valores = FormatComparativoValores(
                        row[0],
                        row.Skip(1).ToList(),
                        reporte.ComparativoColumnas.Skip(1).ToList(),
                        "variables"),
                    Visible = HasVisibleComparativoRow(row[0], row.Skip(1).ToList())
                })
                .Where(r => r.Visible)
                .ToList();

            return reporte;
        }

        private static ProgramaReporteModel CreateBaseReporte(PlanAContextModel contexto)
        {
            return new ProgramaReporteModel
            {
                CvePlan = contexto.CvePlan,
                Seccion = contexto.Seccion,
                Folio = contexto.PlanARow.Table.Columns.Contains("FolioR")
                    ? contexto.PlanARow["FolioR"]?.ToString() ?? string.Empty
                    : string.Empty,
                Referencia = contexto.PlanARow.Table.Columns.Contains("NomReferencia")
                    ? contexto.PlanARow["NomReferencia"]?.ToString() ?? string.Empty
                    : string.Empty,
                Cliente = GetClienteReporte(contexto.PlanARow, contexto.ClienteRow),
                FechaEmision = DateTime.Now
            };
        }

        private static List<PlanAEtapaModel> GetPlanAEtapas(DataRow rowPrograma, long id)
        {
            string codCliente = GetTrimmedValue(rowPrograma, "CodCliente").Replace("'", "''");
            long cvePerfil = rowPrograma.Table.Columns.Contains("CvePerfilN") && rowPrograma["CvePerfilN"] != DBNull.Value
                ? Convert.ToInt64(rowPrograma["CvePerfilN"], CultureInfo.InvariantCulture)
                : 0;

            string sql =
                "SELECT " +
                "ISNULL(PP.CvePlan, 0) AS CvePlan, " +
                "tbl.CveEtapa, " +
                "Et.NomEtapa, " +
                "CASE WHEN PP.CvePlan IS NULL THEN 'N' ELSE ISNULL(PP.Aplica, 'N') END AS Aplica, " +
                "ISNULL(PP.Costo, 0) AS Costo, " +
                "PP.FecAct, " +
                "ISNULL(PP.UsuAct, '') AS UsuAct, " +
                "ISNULL(OG.NomOpcion, '') AS NomAplica, " +
                "ISNULL(Et.CodALLIX, '') AS CodALLIX, " +
                "ISNULL(U.NomUsuario, '') AS NomUsuAct " +
                "FROM CatOptimizerG_Etapas tbl " +
                "LEFT JOIN OptimizerG_PlanA P ON P.CvePlan = " + id.ToString(CultureInfo.InvariantCulture) + " AND P.CodCliente = '" + codCliente + "' " +
                "LEFT JOIN OptimizerG_PlanA_Etapas PP ON PP.CveEtapa = tbl.CveEtapa AND PP.CvePlan = " + id.ToString(CultureInfo.InvariantCulture) + " " +
                "INNER JOIN OptimizerG_PerfilN_Etapas Et ON Et.CvePerfilN = " + cvePerfil.ToString(CultureInfo.InvariantCulture) + " AND Et.CveEtapa = tbl.CveEtapa " +
                "LEFT JOIN CatOpcionesGenerales OG ON OG.CveOpcion = PP.Aplica AND OG.Categoria = 1 " +
                "LEFT JOIN Usuarios U ON U.CodUsuario = PP.UsuAct " +
                "ORDER BY tbl.CveEtapa";

            DataTable dt = Database.execQuery(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<PlanAEtapaModel>();
            }

            return dt.AsEnumerable()
                .Select((r, index) => new PlanAEtapaModel
                {
                    CveEtapa = Convert.ToInt32(r["CveEtapa"]),
                    NomEtapa = r["NomEtapa"]?.ToString() ?? string.Empty,
                    Aplica = r["Aplica"]?.ToString() ?? string.Empty,
                    OrdenVisual = index + 1
                })
                .ToList();
        }

        private static List<ProgramaComparativoColumnaModel> GetComparativoColumnas(long id)
        {
            string sql = BuildReportesColumnasSql(43, 3, id);
            DataTable dt = Database.execQuery(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<ProgramaComparativoColumnaModel>();
            }

            IEnumerable<DataRow> orderedRows = dt.AsEnumerable();
            if (dt.Columns.Contains("Posicion"))
            {
                orderedRows = orderedRows.OrderBy(r => SafeToInt(r["Posicion"]));
            }
            else if (dt.Columns.Contains("CveControl"))
            {
                orderedRows = orderedRows.OrderBy(r => SafeToInt(r["CveControl"]));
            }

            return orderedRows
                .Select((r, index) => new ProgramaComparativoColumnaModel
                {
                    Campo = GetStringColumn(r, "Campo", string.Empty),
                    Titulo = index == 0 ? string.Empty : GetStringColumn(r, "Titulo", GetStringColumn(r, "Campo", string.Empty)),
                    Posicion = dt.Columns.Contains("Posicion") ? SafeToInt(r["Posicion"]) : index + 1
                })
                .ToList();
        }

        private static List<List<string>> GetComparativoDatos(int cveMenu, long id, List<ProgramaComparativoColumnaModel> columnas)
        {
            string sql = BuildReportesDatosSql(42, cveMenu, id.ToString(CultureInfo.InvariantCulture));
            DataTable dt = Database.execQuery(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<List<string>>();
            }

            List<ProgramaComparativoColumnaModel> columnasVisibles = columnas
                .Where(c => !string.IsNullOrWhiteSpace(c.Campo))
                .ToList();

            return dt.AsEnumerable()
                .Select(row => columnasVisibles
                    .Select(col => dt.Columns.Contains(col.Campo)
                        ? FormatDataValue(row[col.Campo], col.Campo)
                        : string.Empty)
                    .ToList())
                .ToList();
        }

        private static List<string> BuildComparativoTotales(
            List<ProgramaComparativoFilaModel> filas,
            List<string> columnas,
            string seccion)
        {
            if (filas.Count == 0)
            {
                return new List<string>();
            }

            int totalColumnas = filas.Max(f => f.Valores.Count);
            List<string> totales = new List<string>();
            for (int i = 0; i < totalColumnas; i++)
            {
                double total = 0;
                foreach (ProgramaComparativoFilaModel fila in filas)
                {
                    if (i < fila.Valores.Count && TryParseDisplayNumber(fila.Valores[i], out double valor))
                    {
                        total += valor;
                    }
                }

                bool esMoneda = i < columnas.Count && IsComparativoCurrency(seccion, string.Empty, columnas[i]);
                totales.Add(FormatDisplayNumber(total, esMoneda));
            }

            return totales;
        }

        private static bool HasVisibleComparativoRow(string etiqueta, List<string> valores)
        {
            if (string.IsNullOrWhiteSpace(etiqueta))
            {
                return false;
            }

            return valores.Any(v => !string.IsNullOrWhiteSpace(v));
        }

        private static List<string> FormatComparativoValores(
            string etiqueta,
            List<string> valores,
            List<string> columnas,
            string seccion)
        {
            List<string> resultado = new List<string>();
            for (int i = 0; i < valores.Count; i++)
            {
                string columna = i < columnas.Count ? columnas[i] : string.Empty;
                bool esMoneda = IsComparativoCurrency(seccion, etiqueta, columna);

                if (TryParseDisplayNumber(valores[i], out double numero))
                {
                    resultado.Add(FormatDisplayNumber(numero, esMoneda));
                }
                else
                {
                    resultado.Add(valores[i]);
                }
            }

            return resultado;
        }

        private static bool IsComparativoCurrency(string seccion, string etiqueta, string columna)
        {
            string seccionNormalizada = (seccion ?? string.Empty).Trim().ToLowerInvariant();
            string etiquetaNormalizada = NormalizeToken(etiqueta);
            string columnaNormalizada = NormalizeToken(columna);

            if (seccionNormalizada == "presupuestos")
            {
                return columnaNormalizada.Contains("COSTO");
            }

            return etiquetaNormalizada == "PRECIOVENTA"
                || etiquetaNormalizada == "COSTOTOTALALIMENTO"
                || etiquetaNormalizada == "COSTOPONDERADO"
                || etiquetaNormalizada == "COSTOKILOPRODUCIDO";
        }

        private static string NormalizeToken(string? value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .ToUpperInvariant();
        }

        private static bool TryParseDisplayNumber(string? value, out double numero)
        {
            string limpio = (value ?? string.Empty)
                .Replace("$", string.Empty)
                .Replace(",", string.Empty)
                .Trim();

            return double.TryParse(
                limpio,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out numero);
        }

        private static string FormatDisplayNumber(double value, bool asCurrency)
        {
            return asCurrency
                ? FormatCurrency(value)
                : value.ToString("N2", ReportNumberCulture);
        }

        private static string BuildReportesColumnasSql(int cvePlataforma, int cveMenu, long id)
        {
            return " DECLARE @CvePlataforma int=" + cvePlataforma.ToString(CultureInfo.InvariantCulture) +
                   " DECLARE @CveMenu int=" + cveMenu.ToString(CultureInfo.InvariantCulture) +
                   " DECLARE @Id bigint=" + id.ToString(CultureInfo.InvariantCulture) +
                   " DECLARE @Estatus int=0" +
                   " DECLARE @Mensaje varchar(250)=''" +
                   " EXEC spp_Reportes_Columnas @CvePlataforma,@CveMenu,@Id,@Estatus Output,@Mensaje Output";
        }

        private static string BuildReportesDatosSql(int cvePlataforma, int cveMenu, string filtros)
        {
            return " DECLARE @CvePlataforma int=" + cvePlataforma.ToString(CultureInfo.InvariantCulture) +
                   " DECLARE @CveMenu int=" + cveMenu.ToString(CultureInfo.InvariantCulture) +
                   " DECLARE @Filtros varchar(MAX) ='" + filtros + "'" +
                   " DECLARE @Estatus int=0" +
                   " DECLARE @Mensaje varchar(250)=''" +
                   " EXEC spp_Reportes_Datos @CvePlataforma,@CveMenu,@Filtros,@Estatus Output,@Mensaje Output";
        }

        private static string FormatDataValue(object value, string columnName)
        {
            if (value == DBNull.Value || value == null)
            {
                return string.Empty;
            }

            if (!double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
            {
                return value.ToString() ?? string.Empty;
            }

            return columnName switch
            {
                "Titulo" => value.ToString() ?? string.Empty,
                _ => numericValue.ToString(CultureInfo.InvariantCulture)
            };
        }

        private byte[] GenerateExcelBytes(ProgramaReporteModel reporte)
        {
            string templatePath = GetDesignPath("Nuptimizer-PerfilNutricional.xlsx");
            if (!System.IO.File.Exists(templatePath))
            {
                throw new Exception("No se encontro la plantilla base de Excel.");
            }

            using XLWorkbook workbook = new XLWorkbook(templatePath);
            IXLWorksheet worksheet = workbook.Worksheet(1);

            if (string.Equals(reporte.Seccion, "comparativo", StringComparison.OrdinalIgnoreCase))
            {
                BuildComparativoExcel(worksheet, reporte);
            }
            else
            {
                BuildPresupuestoExcel(worksheet, reporte);
            }

            using MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] GeneratePdfBytes(ProgramaReporteModel reporte)
        {
            string licenseKey = configuration["ExpertPdf:LicenseKey"] ?? throw new Exception("No se encontro la licencia de ExpertPdf en la configuracion.");
            PdfConverter pdf = new PdfConverter
            {
                LicenseKey = licenseKey
            };

            pdf.PdfDocumentOptions.EmbedFonts = true;
            pdf.PdfDocumentOptions.GenerateSelectablePdf = true;
            pdf.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            pdf.PdfDocumentOptions.FitWidth = true;
            pdf.PdfDocumentOptions.FitHeight = false;
            pdf.PdfDocumentOptions.TopMargin = 5;
            pdf.PdfDocumentOptions.BottomMargin = 5;
            pdf.PdfDocumentOptions.LeftMargin = 10;
            pdf.PdfDocumentOptions.RightMargin = 10;
            pdf.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Landscape;
            pdf.PdfDocumentOptions.ShowHeader = true;
            pdf.PdfDocumentOptions.ShowFooter = true;
            pdf.PdfHeaderOptions.DrawHeaderLine = false;
            pdf.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(
                BuildHeaderHtml(reporte),
                GetTemplatePath("perfil_nutricional_header.html"));
            pdf.PdfHeaderOptions.HeaderHeight = 115;
            pdf.PdfFooterOptions.DrawFooterLine = false;
            pdf.PdfFooterOptions.HtmlToPdfArea = new HtmlToPdfArea(
                BuildFooterHtml(),
                GetTemplatePath("perfil_nutricional_footer.html"));
            pdf.PdfFooterOptions.FooterHeight = 55;
            pdf.PdfFooterOptions.FooterTextColor = Color.Black;
            pdf.PdfFooterOptions.FooterTextFontType = PdfFontType.Helvetica;
            pdf.PdfFooterOptions.FooterTextFontSize = 8;
            pdf.PdfFooterOptions.ShowPageNumber = true;
            pdf.PdfFooterOptions.PageNumberText = "Pagina";
            pdf.PdfFooterOptions.PageNumberTextColor = Color.Black;
            pdf.PdfFooterOptions.PageNumberTextFontType = PdfFontType.Helvetica;
            pdf.PdfFooterOptions.PageNumberTextFontSize = 8;
            pdf.PdfFooterOptions.PageNumberYLocation = 6;

            return pdf.GetPdfBytesFromHtmlString(BuildPdfHtml(reporte));
        }

        private void BuildPresupuestoExcel(IXLWorksheet worksheet, ProgramaReporteModel reporte)
        {
            int lastColumn = 11;
            BuildExcelHeader(worksheet, reporte, "PROGRAMA DE ALIMENTACIÓN", lastColumn);
            int row = 4;

            string[] headers =
            {
                string.Empty,
                "COSTO\n($/kg)",
                "EDAD INICIAL\n(SEM)",
                "EDAD FINAL\n(SEM)",
                "MORTALIDAD ACUMULADA\n(%)",
                "NO. AVES AJUSTADO\nA MORTALIDAD",
                "CONSUMO ALIMENTO\n(Kg AVE/FASE)",
                "PESO HUEVO\n(gr)",
                "PRODUCCIÓN\n(%)",
                "MASA DE HUEVO\n(gr)",
                "CONVERSIÓN\nALIMENTICIA"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                IXLCell cell = worksheet.Cell(row, i + 1);
                cell.Value = headers[i];
                ApplyHeaderCellStyle(cell, ExcelDarkBlue);
            }

            row++;
            bool alternate = false;
            foreach (ProgramaPresupuestoFilaModel fila in reporte.PresupuestoFilas)
            {
                ApplyPresupuestoRow(worksheet, row, fila, alternate);
                alternate = !alternate;
                row++;
            }

            worksheet.Range(row, 1, row, 5).Merge();
            ApplyFooterBand(worksheet.Range(row, 1, row, 5));            
            ApplyFooterBand(worksheet.Range(row, 6, row, 6));
            worksheet.Cell(row, 7).Value = reporte.PresupuestoTotales.ConsumoAlimento.ToString("N2", CultureInfo.InvariantCulture);
            worksheet.Range(row, 7, row, 8).Merge();
            ApplyFooterBand(worksheet.Range(row, 7, row, 8));
            worksheet.Cell(row, 9).Value = string.Empty;
            ApplyFooterBand(worksheet.Range(row, 9, row, 9));
            worksheet.Cell(row, 10).Value = reporte.PresupuestoTotales.MasaHuevo.ToString("N2", CultureInfo.InvariantCulture);
            ApplyFooterBand(worksheet.Range(row, 10, row, 10));
            worksheet.Cell(row, 11).Value = reporte.PresupuestoTotales.ConversionAlimenticia.ToString("N2", CultureInfo.InvariantCulture);
            ApplyFooterBand(worksheet.Range(row, 11, row, 11));

            row += 2;
            row = WriteSummarySection(worksheet, row, "ANÁLISIS ECONONÓMICO TOTAL", reporte.PresupuestoResumenTotal);
            row += 2;
            row = WriteSummarySection(worksheet, row, "ANÁLISIS ECONÓMICO PRODUCTIVO CRIANZA", reporte.PresupuestoResumenCrianza);
            row += 2;
            WriteSummarySection(worksheet, row, "ANÁLISIS ECONÓMICO PRODUCTIVO POSTURA", reporte.PresupuestoResumenPostura);

            worksheet.Columns().AdjustToContents();
        }

        private static int WriteSummarySection(IXLWorksheet worksheet, int row, string titulo, List<ProgramaResumenItemModel> items)
        {
            worksheet.Range(row, 1, row, 2).Merge();
            worksheet.Cell(row, 1).Value = titulo;
            ApplyHeaderCellStyle(worksheet.Cell(row, 1), ExcelDarkBlue);
            row++;

            foreach (ProgramaResumenItemModel item in items)
            {
                worksheet.Cell(row, 1).Value = item.Etiqueta;
                worksheet.Cell(row, 2).Value = FormatSummaryValue(item);
                ApplySummaryLabelStyle(worksheet.Cell(row, 1));
                ApplySummaryValueStyle(worksheet.Cell(row, 2));
                row++;
            }

            return row;
        }

        private void BuildComparativoExcel(IXLWorksheet worksheet, ProgramaReporteModel reporte)
        {
            int lastColumn = Math.Max(2, reporte.ComparativoColumnas.Count);
            BuildExcelHeader(worksheet, reporte, "PROGRAMA DE ALIMENTACIÓN", lastColumn);
            int row = 4;

            row = WriteComparativoSection(worksheet, row, "PRESUPUESTO POR POLLO", reporte.ComparativoColumnas, reporte.ComparativoPresupuestos, reporte.ComparativoPresupuestosTotales);
            row += 2;
            WriteComparativoSection(worksheet, row, "VARIABLES ECONÓMICAS", reporte.ComparativoColumnas, reporte.ComparativoVariables, null);

            worksheet.Columns().AdjustToContents();
        }

        private static int WriteComparativoSection(IXLWorksheet worksheet, int startRow, string titulo, List<string> columnas, List<ProgramaComparativoFilaModel> filas, List<string>? totales)
        {
            worksheet.Range(startRow, 1, startRow, columnas.Count).Merge();
            worksheet.Cell(startRow, 1).Value = titulo;
            ApplyCategoryBand(worksheet.Range(startRow, 1, startRow, columnas.Count));
            startRow++;
            startRow++;

            for (int i = 0; i < columnas.Count; i++)
            {
                IXLCell cell = worksheet.Cell(startRow, i + 1);
                cell.Value = columnas[i];
                ApplyHeaderCellStyle(cell, ExcelDarkBlue);
            }

            startRow++;
            bool alternate = false;
            foreach (ProgramaComparativoFilaModel fila in filas)
            {
                worksheet.Cell(startRow, 1).Value = fila.Etiqueta;
                ApplyRowLabelStyle(worksheet.Cell(startRow, 1), alternate);

                for (int i = 0; i < fila.Valores.Count; i++)
                {
                    worksheet.Cell(startRow, i + 2).Value = fila.Valores[i];
                    ApplyBodyStyle(worksheet.Cell(startRow, i + 2), alternate);
                }

                alternate = !alternate;
                startRow++;
            }

            if (totales != null && totales.Count > 0)
            {
                worksheet.Cell(startRow, 1).Value = string.Empty;
                ApplyFooterBand(worksheet.Range(startRow, 1, startRow, 1));
                for (int i = 0; i < totales.Count; i++)
                {
                    worksheet.Cell(startRow, i + 2).Value = totales[i];
                    ApplyFooterBand(worksheet.Range(startRow, i + 2, startRow, i + 2));
                }

                startRow++;
            }

            return startRow;
        }

        private static void BuildExcelHeader(IXLWorksheet worksheet, ProgramaReporteModel reporte, string titulo, int lastColumn)
        {
            worksheet.Range(1, 2, 2, 6).Clear(XLClearOptions.Contents);
            worksheet.Range(1, 2, 1, 6).Merge();
            worksheet.Range(2, 2, 2, 6).Merge();

            ApplyExcelHeaderBandStyle(worksheet, lastColumn);
            ApplyExcelSpacerRowStyle(worksheet, lastColumn);

            worksheet.Cell(1, 2).Value = titulo;
            ApplyExcelHeaderTitleStyle(worksheet.Cell(1, 2));
            ApplyExcelHeaderDetail(worksheet.Cell(2, 2), reporte);

            worksheet.Row(1).Height = Math.Max(worksheet.Row(1).Height, 34d);
            worksheet.Row(2).Height = Math.Max(worksheet.Row(2).Height, 40d);
            worksheet.Row(3).Height = Math.Max(worksheet.Row(3).Height, 10d);
            worksheet.Column(1).Width = Math.Max(worksheet.Column(1).Width, 10d);
        }

        private static void ApplyPresupuestoRow(IXLWorksheet worksheet, int row, ProgramaPresupuestoFilaModel fila, bool alternate)
        {
            worksheet.Cell(row, 1).Value = fila.NomEtapa;
            ApplyRowLabelStyle(worksheet.Cell(row, 1), alternate);

            string[] values =
            {
                FormatCurrency(fila.Costo),
                fila.EdadInicial.ToString("N0", CultureInfo.InvariantCulture),
                fila.EdadFinal.ToString("N0", CultureInfo.InvariantCulture),
                fila.Mortalidad.ToString("N2", CultureInfo.InvariantCulture),
                fila.NoAves.ToString("N0", CultureInfo.InvariantCulture),
                fila.ConsumoAlimento.ToString("N2", CultureInfo.InvariantCulture),
                ShouldShowPosturaValue(fila.PesoHuevo) ? fila.PesoHuevo.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                ShouldShowPosturaValue(fila.Produccion) ? fila.Produccion.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                ShouldShowPosturaValue(fila.MasaHuevo) ? fila.MasaHuevo.ToString("N2", CultureInfo.InvariantCulture) : string.Empty,
                ShouldShowPosturaValue(fila.ConversionAlimenticia) ? fila.ConversionAlimenticia.ToString("N2", CultureInfo.InvariantCulture) : string.Empty
            };

            for (int i = 0; i < values.Length; i++)
            {
                worksheet.Cell(row, i + 2).Value = values[i];
                ApplyBodyStyle(worksheet.Cell(row, i + 2), alternate);
            }
        }

        private static void ApplyHeaderCellStyle(IXLCell cell, XLColor background)
        {
            cell.Style.Fill.BackgroundColor = background;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.White;
            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.InsideBorderColor = XLColor.White;
        }

        private static void ApplyRowLabelStyle(IXLCell cell, bool alternate)
        {
            cell.Style.Fill.BackgroundColor = alternate ? ExcelAlternateRow : XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.FromHtml("#1f2937");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = ExcelGridBlue;
            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.InsideBorderColor = ExcelGridBlue;
        }

        private static void ApplyBodyStyle(IXLCell cell, bool alternate)
        {
            cell.Style.Fill.BackgroundColor = alternate ? ExcelAlternateRow : XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = ExcelGridBlue;
            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.InsideBorderColor = ExcelGridBlue;
        }

        private static void ApplyFooterBand(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = ExcelDarkBlue;
            range.Style.Font.FontColor = XLColor.White;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = XLColor.White;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorderColor = XLColor.White;
        }

        private static void ApplyCategoryBand(IXLRange range)
        {
            range.Style.Fill.BackgroundColor = XLColor.FromHtml("#003d7a");
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = ExcelGridBlue;
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorderColor = ExcelGridBlue;
        }

        private static void ApplySummaryLabelStyle(IXLCell cell)
        {
            cell.Style.Fill.BackgroundColor = XLColor.White;
            cell.Style.Font.FontColor = XLColor.FromHtml("#1f2937");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = ExcelGridBlue;
        }

        private static void ApplySummaryValueStyle(IXLCell cell)
        {
            cell.Style.Fill.BackgroundColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = ExcelGridBlue;
        }

        private string BuildPdfHtml(ProgramaReporteModel reporte)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html><head><meta charset=\"utf-8\" />");
            builder.AppendLine("<style>");
            builder.AppendLine("body{font-family:Helvetica,Arial,sans-serif;font-size:10pt;color:#1f2937;margin:0;padding:10px;}");
            builder.AppendLine("table{width:100%;border-collapse:collapse;margin-bottom:18px;}");
            builder.AppendLine("thead{display:table-header-group;}");
            builder.AppendLine("tfoot{display:table-row-group;}");
            builder.AppendLine("th{background:#0b2e57;color:#fff;font-weight:400;border:1px solid #d6deed;padding:8px 6px;text-align:center;}");
            builder.AppendLine("td{border:1px solid #d6deed;padding:6px 8px;text-align:center;}");
            builder.AppendLine(".row-label{text-align:left;font-weight:bold;}");
            builder.AppendLine(".category{background:#003d7a;color:#fff;font-weight:bold;text-align:left;}");
            builder.AppendLine(".section-gap{height:10px;background:#fff;border:none;padding:0;}");
            builder.AppendLine(".alt{background:#eef2f8;}");
            builder.AppendLine(".summary th{background:#0b2e57;color:#fff;text-align:left;}");
            builder.AppendLine(".summary td{text-align:left;}");
            builder.AppendLine(".section-block{page-break-inside:avoid;break-inside:avoid-page;margin-bottom:18px;}");
            builder.AppendLine("</style></head><body>");

            if (string.Equals(reporte.Seccion, "comparativo", StringComparison.OrdinalIgnoreCase))
            {
                AppendComparativoPdf(builder, reporte);
            }
            else
            {
                AppendPresupuestoPdf(builder, reporte);
            }

            builder.AppendLine("</body></html>");
            return builder.ToString();
        }

        private string BuildHeaderHtml(ProgramaReporteModel reporte)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html><head><meta charset=\"utf-8\" />");
            builder.AppendLine("<style>");
            builder.AppendLine("body{margin:0;padding:0;font-family:Helvetica,Arial,sans-serif;}");
            builder.AppendLine(".header{background:#0b2e57;color:#fff;border-radius:10px;padding:16px 24px;display:flex;justify-content:space-between;align-items:center;gap:18px;}");
            builder.AppendLine(".header-left{display:flex;align-items:center;gap:18px;min-width:0;}");
            builder.AppendLine(".header-left img{display:block;max-height:56px;width:auto;}");
            builder.AppendLine(".header-right img{display:block;max-height:52px;width:auto;}");
            builder.AppendLine(".header-text{min-width:0;}");
            builder.AppendLine(".header h1{margin:0;font-size:24pt;font-weight:400;line-height:1.05;}");
            builder.AppendLine(".folio{color:#61a7ff;font-size:12pt;margin-top:6px;}");
            builder.AppendLine(".cliente{font-weight:bold;font-size:12pt;margin-top:6px;}");
            builder.AppendLine("</style></head><body>");
            builder.AppendLine("<div class=\"header\">");
            builder.AppendLine("<div class=\"header-left\">");
            builder.AppendLine("<img src=\"" + EscapeHtml(GetPdfImageUri(GetDesignPath("Icono-PerfilNutricional.png"))) + "\" alt=\"Perfil\" />");
            builder.AppendLine("<div class=\"header-text\">");
            builder.AppendLine("<h1>PROGRAMA DE ALIMENTACIÓN</h1>");
            builder.AppendLine("<div class=\"folio\">" + EscapeHtml(BuildFolioReferencia(reporte)) + "</div>");
            builder.AppendLine("<div class=\"cliente\">" + EscapeHtml(reporte.Cliente) + "</div>");
            builder.AppendLine("</div>");
            builder.AppendLine("</div>");
            builder.AppendLine("<div class=\"header-right\"><img src=\"" + EscapeHtml(GetPdfImageUri(GetDesignPath("Logo_Nuptimizer.png"))) + "\" alt=\"Nuptimizer\" /></div>");
            builder.AppendLine("</div>");
            builder.AppendLine("</body></html>");
            return builder.ToString();
        }

        private static void AppendPresupuestoPdf(StringBuilder builder, ProgramaReporteModel reporte)
        {
            builder.AppendLine("<table>");
            builder.AppendLine("<tr><th>ETAPA</th><th>COSTO FÓRMULA<br/>($/Kg)</th><th>EDAD INICIAL<br/>(SEM)</th><th>EDAD FINAL<br/>(SEM)</th><th>MORTALIDAD ACUMULADA<br/>(%)</th><th>NO. AVES AJUSTADO<br/>A MORTALIDAD</th><th>CONSUMO ALIMENTO<br/>(Kg AVE/FASE)</th><th>PESO HUEVO<br/>(gr)</th><th>PRODUCCIÓN<br/>(%)</th><th>MASA DE HUEVO<br/>(gr)</th><th>CONVERSIÓN<br/>ALIMENTICIA</th></tr>");
            bool alternate = false;
            foreach (ProgramaPresupuestoFilaModel fila in reporte.PresupuestoFilas)
            {
                string rowClass = alternate ? " class=\"alt\"" : string.Empty;
                builder.AppendLine("<tr" + rowClass + ">");
                builder.AppendLine("<td class=\"row-label\">" + EscapeHtml(fila.NomEtapa) + "</td>");
                builder.AppendLine("<td>" + FormatCurrency(fila.Costo) + "</td>");
                builder.AppendLine("<td>" + fila.EdadInicial.ToString("N0", CultureInfo.InvariantCulture) + "</td>");
                builder.AppendLine("<td>" + fila.EdadFinal.ToString("N0", CultureInfo.InvariantCulture) + "</td>");
                builder.AppendLine("<td>" + fila.Mortalidad.ToString("N2", CultureInfo.InvariantCulture) + "</td>");
                builder.AppendLine("<td>" + fila.NoAves.ToString("N0", CultureInfo.InvariantCulture) + "</td>");
                builder.AppendLine("<td>" + fila.ConsumoAlimento.ToString("N2", CultureInfo.InvariantCulture) + "</td>");
                builder.AppendLine("<td>" + EscapeHtml(ShouldShowPosturaValue(fila.PesoHuevo) ? fila.PesoHuevo.ToString("N2", CultureInfo.InvariantCulture) : string.Empty) + "</td>");
                builder.AppendLine("<td>" + EscapeHtml(ShouldShowPosturaValue(fila.Produccion) ? fila.Produccion.ToString("N2", CultureInfo.InvariantCulture) : string.Empty) + "</td>");
                builder.AppendLine("<td>" + EscapeHtml(ShouldShowPosturaValue(fila.MasaHuevo) ? fila.MasaHuevo.ToString("N2", CultureInfo.InvariantCulture) : string.Empty) + "</td>");
                builder.AppendLine("<td>" + EscapeHtml(ShouldShowPosturaValue(fila.ConversionAlimenticia) ? fila.ConversionAlimenticia.ToString("N2", CultureInfo.InvariantCulture) : string.Empty) + "</td>");
                builder.AppendLine("</tr>");
                alternate = !alternate;
            }

            builder.AppendLine("<tr><th colspan=\"5\"></th><th></th><th>" + reporte.PresupuestoTotales.ConsumoAlimento.ToString("N2", CultureInfo.InvariantCulture) + "</th><th></th><th></th><th>" + reporte.PresupuestoTotales.MasaHuevo.ToString("N2", CultureInfo.InvariantCulture) + "</th><th>" + reporte.PresupuestoTotales.ConversionAlimenticia.ToString("N2", CultureInfo.InvariantCulture) + "</th></tr>");
            builder.AppendLine("</table>");
            AppendSummaryPdfTable(builder, "ANÁLISIS ECONONÓMICO TOTAL", reporte.PresupuestoResumenTotal);
            AppendSummaryPdfTable(builder, "ANÁLISIS ECONÓMICO PRODUCTIVO CRIANZA", reporte.PresupuestoResumenCrianza);
            AppendSummaryPdfTable(builder, "ANÁLISIS ECONÓMICO PRODUCTIVO POSTURA", reporte.PresupuestoResumenPostura);
        }

        private static void AppendSummaryPdfTable(StringBuilder builder, string titulo, List<ProgramaResumenItemModel> items)
        {
            builder.AppendLine("<table class=\"summary\" style=\"width:50%; page-break-inside:avoid; break-inside:avoid-page;\">");
            builder.AppendLine("<tr><th colspan=\"2\">" + EscapeHtml(titulo) + "</th></tr>");
            foreach (ProgramaResumenItemModel item in items)
            {
                builder.AppendLine("<tr><td class=\"row-label\">" + EscapeHtml(item.Etiqueta) + "</td><td>" + EscapeHtml(FormatSummaryValue(item)) + "</td></tr>");
            }

            builder.AppendLine("</table>");
        }

        private static void AppendComparativoPdf(StringBuilder builder, ProgramaReporteModel reporte)
        {
            AppendComparativoPdfSection(builder, "PRESUPUESTO POR POLLO", reporte.ComparativoColumnas, reporte.ComparativoPresupuestos, reporte.ComparativoPresupuestosTotales);
            AppendComparativoPdfSection(builder, "VARIABLES ECONÓMICAS", reporte.ComparativoColumnas, reporte.ComparativoVariables, null);
        }

        private static void AppendComparativoPdfSection(StringBuilder builder, string titulo, List<string> columnas, List<ProgramaComparativoFilaModel> filas, List<string>? totales)
        {
            builder.AppendLine("<div class=\"section-block\">");
            builder.AppendLine("<table>");
            builder.AppendLine("<tr><td class=\"category\" colspan=\"" + columnas.Count + "\">" + EscapeHtml(titulo) + "</td></tr>");
            builder.AppendLine("<tr><td class=\"section-gap\" colspan=\"" + columnas.Count + "\"></td></tr>");
            builder.AppendLine("<tr>");
            for (int i = 0; i < columnas.Count; i++)
            {
                builder.AppendLine("<th style=\"background:#0b2e57;\">" + EscapeHtml(columnas[i]) + "</th>");
            }

            builder.AppendLine("</tr>");
            bool alternate = false;
            foreach (ProgramaComparativoFilaModel fila in filas)
            {
                string rowClass = alternate ? " class=\"alt\"" : string.Empty;
                builder.AppendLine("<tr" + rowClass + ">");
                builder.AppendLine("<td class=\"row-label\">" + EscapeHtml(fila.Etiqueta) + "</td>");
                foreach (string valor in fila.Valores)
                {
                    builder.AppendLine("<td>" + EscapeHtml(valor) + "</td>");
                }

                builder.AppendLine("</tr>");
                alternate = !alternate;
            }

            if (totales != null && totales.Count > 0)
            {
                builder.AppendLine("<tr><th></th>");
                foreach (string total in totales)
                {
                    builder.AppendLine("<th>" + EscapeHtml(total) + "</th>");
                }

                builder.AppendLine("</tr>");
            }

            builder.AppendLine("</table>");
            builder.AppendLine("</div>");
        }

        private static string BuildFolioReferencia(ProgramaReporteModel reporte)
        {
            if (!string.IsNullOrWhiteSpace(reporte.Folio))
            {
                return "FOLIO: " + reporte.Folio + " | " + reporte.Referencia;
            }

            return reporte.Referencia;
        }

        private static string FormatCurrency(double value)
        {
            return "$" + value.ToString("N2", ReportNumberCulture);
        }

        private static string FormatSummaryValue(ProgramaResumenItemModel item)
        {
            return item.EsMoneda
                ? FormatCurrency(item.Valor)
                : item.Valor.ToString(item.Formato, ReportNumberCulture);
        }

        private static string GetPdfImageUri(string filePath)
        {
            return new Uri(filePath).AbsoluteUri;
        }

        private string BuildFooterHtml()
        {
            return GetTemplate("perfil_nutricional_footer.html")
                .Replace("@@PieTexto", EscapeHtml("AV. DEL MARQUES NO.32, FRACC. IND. BERNARDO QUINTANA, 76246, EL MARQUES, QRO.  |  T.+52 (442) 196 0100  |  www.gponutec.com"));
        }

        private static string GetDesignPath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Diseno", fileName);
        }

        private static string GetTemplate(string templateName)
        {
            string path = GetTemplatePath(templateName);
            if (!System.IO.File.Exists(path))
            {
                throw new Exception("No se encontro la plantilla " + templateName + ".");
            }

            return System.IO.File.ReadAllText(path, Encoding.UTF8);
        }

        private static string GetTemplatePath(string templateName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Views", "Template", templateName);
        }

        private static string ResolveProgramaStageName(PlanAEtapaModel? etapa)
        {
            return etapa?.NomEtapa?.Trim() ?? string.Empty;
        }

        private static bool ShouldShowPosturaValue(double value)
        {
            return Math.Abs(value) > 0.0000001d;
        }

        private static int SafeToInt(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return int.MaxValue;
            }

            return int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out int result)
                ? result
                : int.MaxValue;
        }

        private static string GetStringColumn(DataRow row, string columnName, string defaultValue)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return defaultValue;
            }

            return row[columnName]?.ToString() ?? defaultValue;
        }

        private static void ApplyExcelHeaderBandStyle(IXLWorksheet worksheet, int lastColumn)
        {
            IXLRange band = worksheet.Range(1, 1, 2, lastColumn);
            band.Style.Fill.BackgroundColor = ExcelDarkBlue;
            band.Style.Font.FontColor = XLColor.White;
            band.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            band.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            band.Style.Alignment.WrapText = true;
            band.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            band.Style.Border.OutsideBorderColor = ExcelDarkBlue;
            band.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            band.Style.Border.InsideBorderColor = ExcelDarkBlue;
        }

        private static void ApplyExcelSpacerRowStyle(IXLWorksheet worksheet, int lastColumn)
        {
            IXLRange spacer = worksheet.Range(3, 1, 3, lastColumn);
            spacer.Clear(XLClearOptions.Contents);
            spacer.Style.Fill.BackgroundColor = XLColor.White;
            spacer.Style.Border.OutsideBorder = XLBorderStyleValues.None;
            spacer.Style.Border.InsideBorder = XLBorderStyleValues.None;
        }

        private static void ApplyExcelHeaderTitleStyle(IXLCell cell)
        {
            cell.Style.Font.FontSize = 20d;
            cell.Style.Font.Bold = false;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
        }

        private static void ApplyExcelHeaderDetail(IXLCell cell, ProgramaReporteModel reporte)
        {
            cell.Value = string.Empty;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            cell.Style.Font.FontSize = 11d;

            IXLRichText richText = cell.RichText;
            richText.ClearText();
            richText
                .AddText(BuildFolioReferencia(reporte))
                .SetFontColor(XLColor.FromHtml("#61a7ff"))
                .SetFontSize(11d);

            if (!string.IsNullOrWhiteSpace(reporte.Cliente))
            {
                richText
                    .AddText(Environment.NewLine + reporte.Cliente)
                    .SetFontColor(XLColor.White)
                    .SetBold(true)
                    .SetFontSize(11d);
            }
        }

        private static string EscapeHtml(string? text)
        {
            return System.Net.WebUtility.HtmlEncode(text ?? string.Empty);
        }

        private sealed class ProgramaReporteModel
        {
            public long CvePlan { get; set; }
            public string Seccion { get; set; } = "presupuesto";
            public string Folio { get; set; } = string.Empty;
            public string Cliente { get; set; } = string.Empty;
            public string Referencia { get; set; } = string.Empty;
            public DateTime FechaEmision { get; set; }
            public List<ProgramaPresupuestoFilaModel> PresupuestoFilas { get; set; } = new();
            public ProgramaPresupuestoTotalesModel PresupuestoTotales { get; set; } = new();
            public List<ProgramaResumenItemModel> PresupuestoResumenTotal { get; set; } = new();
            public List<ProgramaResumenItemModel> PresupuestoResumenCrianza { get; set; } = new();
            public List<ProgramaResumenItemModel> PresupuestoResumenPostura { get; set; } = new();
            public List<string> ComparativoColumnas { get; set; } = new();
            public List<ProgramaComparativoFilaModel> ComparativoPresupuestos { get; set; } = new();
            public List<string> ComparativoPresupuestosTotales { get; set; } = new();
            public List<ProgramaComparativoFilaModel> ComparativoVariables { get; set; } = new();
        }

        private sealed class PlanAContextModel
        {
            public long CvePlan { get; set; }
            public string Seccion { get; set; } = "presupuesto";
            public DataRow PlanARow { get; set; } = null!;
            public DataRow ResultadoRow { get; set; } = null!;
            public DataRow? ClienteRow { get; set; }
            public ResponseOptimizerModel Response { get; set; } = null!;
            public List<PlanAEtapaModel> Etapas { get; set; } = new();
        }

        private sealed class ProgramaPresupuestoFilaModel
        {
            public int CveEtapa { get; set; }
            public string NomEtapa { get; set; } = string.Empty;
            public double Costo { get; set; }
            public double EdadInicial { get; set; }
            public double EdadFinal { get; set; }
            public double Mortalidad { get; set; }
            public double NoAves { get; set; }
            public double ConsumoAlimento { get; set; }
            public double PesoHuevo { get; set; }
            public double Produccion { get; set; }
            public double MasaHuevo { get; set; }
            public double ConversionAlimenticia { get; set; }
        }

        private sealed class ProgramaPresupuestoTotalesModel
        {
            public double ConsumoAlimento { get; set; }
            public double MasaHuevo { get; set; }
            public double ConversionAlimenticia { get; set; }
        }

        private sealed class ProgramaResumenItemModel
        {
            public ProgramaResumenItemModel(string etiqueta, double valor, string formato)
            {
                Etiqueta = etiqueta;
                Valor = valor;
                Formato = formato;
            }

            public ProgramaResumenItemModel(string etiqueta, double valor, bool esMoneda, string formato)
                : this(etiqueta, valor, formato)
            {
                EsMoneda = esMoneda;
            }

            public string Etiqueta { get; }
            public double Valor { get; }
            public string Formato { get; }
            public bool EsMoneda { get; }
        }

        private sealed class ProgramaComparativoFilaModel
        {
            public string Etiqueta { get; set; } = string.Empty;
            public List<string> Valores { get; set; } = new();
            public bool Visible { get; set; }
        }

        private sealed class ProgramaComparativoColumnaModel
        {
            public string Campo { get; set; } = string.Empty;
            public string Titulo { get; set; } = string.Empty;
            public int Posicion { get; set; }
        }

        private sealed class PlanAEtapaModel
        {
            public int CveEtapa { get; set; }
            public string NomEtapa { get; set; } = string.Empty;
            public string Aplica { get; set; } = string.Empty;
            public int OrdenVisual { get; set; }
        }
    }
}

