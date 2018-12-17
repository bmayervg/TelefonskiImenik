using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using OfficeOpenXml;

namespace TelefonskiImenik.Business
{
    public class ExcelUtils
    {

        /// <summary>
        /// Maksimalna duzina naziva sheeta je 25 znakova
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        public static void ExportDataTableToExcel2007(DataTable tbl, string fileName, string sheetName)
        {
            using (ExcelPackage pck = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add((sheetName.Length >= 25 ? sheetName.Substring(0, 25) : sheetName));

                ws.Cells["A1"].LoadFromDataTable(tbl, true);

                // bojanje zaglavlja
                for (int c = 1; c <= tbl.Columns.Count; c++)
                {
                    ws.Cells[1, c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[1, c].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[1, c].Style.Font.Bold = true;

                    // da datumi i izgledaju kao datumi
                    if (tbl.Columns[c - 1].DataType == typeof(DateTime))
                    {
                        ws.Column(c).Style.Numberformat.Format = "dd.mm.yyyy.";
                    }
                }

                pck.Save();
            }
        }

        public static void ExportDataTableToExcel2007(DataTable tbl, string fileName)
        {
            ExportDataTableToExcel2007(tbl, fileName, "sheet1");
        }

        public static void ExportCSVtoXLSX(string fileNameCSV, string filename = "", bool firstRowIsHeader = false, string sheetName = "Sheet1", char delimiter = ';')
        {
            if (filename == "") fileNameCSV = filename.Replace(".csv", ".xlsx");

            var format = new ExcelTextFormat();
            format.Delimiter = delimiter;
            format.EOL = "\r";              // DEFAULT IS "\r\n";
            // format.TextQualifier = '"';

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filename)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add((sheetName.Length >= 25 ? sheetName.Substring(0, 25) : sheetName));
                worksheet.Cells["A1"].LoadFromText(new FileInfo(fileNameCSV), format, OfficeOpenXml.Table.TableStyles.Medium27, firstRowIsHeader);

                package.Save();
            }
        }

        public static void ExportDataTablesToExcel2007SheetsFile(string fileName, bool PrintHeaders, params DataTable[] dataTables)
        {
            ExcelPackage pck = ExportDataTablesToExcel2007Sheets(PrintHeaders, dataTables);
            pck.File = new FileInfo(fileName);
            pck.Save();
        }

        /// <summary>
        /// Eksportira DataTablice u Excel (.xlsx) sheetove i vraca byte[]
        /// 
        /// ExtendedProperties:
        ///     DataColumn:
        ///         ["columnName"]    - Za naziv kolone
        ///         ["columnVisible"] - Trebali kolona biti vidljiva (true/false)
        ///     DataTable:
        ///         ["sheetName"]     - Za ime sheeta u Excelu
        ///         ["sheetColor"]    - Za boju kartice (System.Drawing.Color)
        ///         ["printHeadersOverride"]    - Override-a ulazni parametar PrintHeaders
        ///     
        /// 
        /// Primjer pozivanja:
        ///     ExportDataTablesToExcel2007Sheets(
        ///         "C:\output.xlsx",
        ///         dtTablica1,
        ///         dtTablica2,
        ///         dtTablica3,
        ///         dtTablica4,
        ///         ...
        ///     );
        ///     
        /// </summary>
        public static byte[] ExportDataTablesToExcel2007SheetsWeb(bool PrintHeaders, params DataTable[] dataTables)
        {
            ExcelPackage pck = ExportDataTablesToExcel2007Sheets(PrintHeaders, dataTables);
            return pck.GetAsByteArray();
        }

        private static ExcelPackage ExportDataTablesToExcel2007Sheets(bool PrintHeaders, params DataTable[] dataTables)
        {
            ExcelPackage pck = new ExcelPackage();
            bool printHeaders;

            int i = 0;
            foreach (DataTable dt in dataTables)
            {
                List<string> UkloniKolone = new List<string>();

                // Učitaj popis kolona koje treba ukloniti.
                foreach (DataColumn dc in dt.Columns)
                    if (dc.ExtendedProperties["columnVisible"] != null && Convert.ToBoolean(dc.ExtendedProperties["columnVisible"]) == false)
                        UkloniKolone.Add(dc.ColumnName);

                foreach (string str in UkloniKolone)
                {
                    dt.Columns.Remove(dt.Columns[str]);
                    dt.AcceptChanges();
                }

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dt.ExtendedProperties["sheetName"] == null ? "Sheet" + i : dt.ExtendedProperties["sheetName"].ToString());

                // Boja kartice
                if (dt.ExtendedProperties["sheetColor"] != null)
                    ws.TabColor = (Color)dt.ExtendedProperties["sheetColor"];

                if (dt.ExtendedProperties["printHeadersOverride"] != null)
                {
                    if (dt.ExtendedProperties["printHeadersOverride"].ToString() == "true")
                    {
                        printHeaders = true;
                    }
                    else if (dt.ExtendedProperties["printHeadersOverride"].ToString() == "false")
                    {
                        printHeaders = false;
                    }
                    else
                    {
                        printHeaders = PrintHeaders;
                    }
                }
                else
                {
                    printHeaders = PrintHeaders;
                }

                // Ispis tablice
                ws.Cells["A1"].LoadFromDataTable(dt, printHeaders);

                for (int c = 1; c <= dt.Columns.Count; c++)
                {
                    // Formatiranje zaglavlja
                    if (printHeaders)
                    {
                        ws.Cells[1, c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[1, c].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        ws.Cells[1, c].Style.Font.Bold = true;

                        // Ispravak imena kolone
                        if (dt.Columns[c - 1].ExtendedProperties["columnName"] != null)
                            ws.Cells[1, c].Value = dt.Columns[c - 1].ExtendedProperties["columnName"].ToString();

                    }

                    // Da datumi i izgledaju kao datumi
                    if (dt.Columns[c - 1].DataType == typeof(DateTime))
                        ws.Column(c).Style.Numberformat.Format = "dd.mm.yyyy.";

                    // Automatski prilagodi veličinu kolone da sav tekst bude vidljiv
                    ws.Column(c).AutoFit();
                }

                i++;
            }

            return pck;
        }

        public static byte[] ExportDataTableToWeb(DataTable tbl, string porukaZaPrviRed = null)
        {
            ExcelPackage pck = new ExcelPackage();

            int start = 1;

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
            if (!string.IsNullOrEmpty(porukaZaPrviRed))
            {
                ws.Cells["A1:E1"].Merge = true;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["A1"].Value = porukaZaPrviRed;
                ws.Cells["A2"].LoadFromDataTable(tbl, true);
                start = 2;
            }
            else
            {
                ws.Cells["A1"].LoadFromDataTable(tbl, true);
            }

            // bojanje zaglavlja
            for (int c = 1; c <= tbl.Columns.Count; c++)
            {
                ws.Cells[start, c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[start, c].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells[start, c].Style.Font.Bold = true;

                // da datumi i izgledaju kao datumi
                //ws.Column(c).Style.Numberformat.Format = "dd.mm.yyyy.";

                // prepravljeno da se i vrijeme eksportira ako postoji
                // moglo se također i samo sforsati da si datumi prikazuju i vrijeme sa formatiranjem "dd.mm.yyyy. hh:mm:ss";
                if (tbl.Columns[c - 1].DataType == typeof(DateTime))
                {
                    for (int r = 1; r <= tbl.Rows.Count; r++)
                    {
                        DateTime DatumVrijeme;
                        DateTime.TryParse((tbl.Rows[r - 1][c - 1] ?? "").ToString(), out DatumVrijeme);

                        ws.Cells[r + start, c].Style.Numberformat.Format = (DatumVrijeme.Hour > 0 || DatumVrijeme.Minute > 0 || DatumVrijeme.Second > 0) ? "dd.mm.yyyy. hh:mm:ss" : "dd.mm.yyyy.";
                    }
                }

                if (tbl.Columns[c - 1].ExtendedProperties["FormatAsNumber"] != null && Convert.ToBoolean(tbl.Columns[c - 1].ExtendedProperties["FormatAsNumber"]))
                {

                    if (tbl.Columns[c - 1].DataType == typeof(decimal))
                    {
                        ws.Column(c).Style.Numberformat.Format = "#,##0.00";
                    }

                    if (tbl.Columns[c - 1].DataType == typeof(int))
                    {
                        ws.Column(c).Style.Numberformat.Format = "#,##0";
                    }
                }
            }

            return pck.GetAsByteArray();

            //MemoryStream mem = new MemoryStream();
            //ZipOutputStream zipOut = new ZipOutputStream(mem);
            //byte[] bytes = null;


            //StringBuilder sb = generateXLSXContentTypes();
            //ZipEntry entry = new ZipEntry(@"[Content_Types].xml");
            //byte[] buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXRels();
            //entry = new ZipEntry(@"_rels\.rels");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXAPP();
            //entry = new ZipEntry(@"docProps\app.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXCORE();
            //entry = new ZipEntry(@"docProps\core.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXWorkbookRels();
            //entry = new ZipEntry(@"xl\_rels\workbook.xml.rels");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXSheetRels();
            //entry = new ZipEntry(@"xl\worksheets\_rels\sheet1.xml.rels");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXWorkbook();
            //entry = new ZipEntry(@"xl\workbook.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXTheme();
            //entry = new ZipEntry(@"xl\theme\theme1.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXStyles();
            //entry = new ZipEntry(@"xl\styles.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //List<string> sharedStrings = new List<string>();

            //StringBuilder sbDataXML = generateDataXML(tbl, sharedStrings);
            //entry = new ZipEntry(@"xl\worksheets\sheet1.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sbDataXML.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //sb = generateXLSXSharedStrings(sharedStrings);
            //entry = new ZipEntry(@"xl\sharedStrings.xml");
            //buff = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            //entry.Size = buff.Length;
            //zipOut.PutNextEntry(entry);
            //zipOut.Write(buff, 0, buff.Length);

            //zipOut.Flush();
            //zipOut.Finish();

            //mem.Position = 0;
            //mem.Close();
            //zipOut.Close();
            //bytes = mem.ToArray();
            //mem.Dispose();

            //return bytes;
        }
    }
}
