using astorWorkBackgroundService.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkBackgroundService.EmailHelper
{
    static  class AttachmentHandler
    {
        public static string CreateTblContent(IEnumerable<DeliveryMaterial> deliveryMaterials) {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<table border = '1' cellspacing = '0' cellpadding = '0' style = 'border-collapse:collapse; border: none; font-family:arial'>
                            <tr>");

            string[] colNames = new string[] { "Block", "Level", "Zone", "Marking No.", "Material Type", "Expected Delievery Date" };

            for (int i = 0; i < colNames.Length; i++)
                sb.AppendFormat(string.Format(@"<td width = '200' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colNames[i]));

            sb.AppendFormat("</tr>");

            foreach (DeliveryMaterial deliveryMaterial in deliveryMaterials)
            {
                string[] colData = new string[] { deliveryMaterial.Block, deliveryMaterial.Level, deliveryMaterial.Zone, deliveryMaterial.MarkingNo, deliveryMaterial.MaterialType, deliveryMaterial.ExpectedDeliveryDate.ToString("dd/MM/yyyy") };

                sb.AppendFormat(@"<tr>");

                for (int i = 0; i < colData.Length; i++)
                    sb.AppendFormat(string.Format(@"<td width = '200' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colData[i]));

                sb.AppendFormat(@"<tr>");
            }

            sb.Append(@"</table>");

            return sb.ToString();
        }

        public static PdfPTable CreatePDFTable(DataTable dt)
        {
            Font font5 = FontFactory.GetFont(FontFactory.HELVETICA, 5);

            int columnCount = dt.Columns.Count;

            PdfPTable table = new PdfPTable(columnCount);
            List<float> widths = new List<float>();

            for (int colIndex = 0; colIndex < columnCount; colIndex++)
                widths.Add(4f);

            table.SetWidths(widths.ToArray());
            table.WidthPercentage = 100;

            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = columnCount;

            foreach (DataColumn c in dt.Columns)
                table.AddCell(new Phrase(c.ColumnName, font5));

            foreach (DataRow row in dt.Rows)
                if (dt.Rows.Count > 0)
                    for (int colIndex = 0; colIndex < columnCount; colIndex++)
                        table.AddCell(new Phrase(row[colIndex].ToString(), font5));

            return table;
        }

        public static DataTable CreateDataTable(string tableName, List<string> columnNames)
        {
            // Create a new DataTable.
            DataTable table = new DataTable(tableName);
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;

            // Create new DataColumn, set DataType, 
            // ColumnName and add to DataTable.    
            for (int i = 0; i < columnNames.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = columnNames[i];
                if (i == 0)
                {
                    column.ReadOnly = true;
                    column.Unique = false;

                    // Make the ID column the primary key column.
                    //DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                    //PrimaryKeyColumns[0] = table.Columns[columnNames[i]];
                    //table.PrimaryKey = PrimaryKeyColumns;
                }
                else
                {
                    column.ReadOnly = false;
                    column.Unique = false;
                }

                // Add the Column to the DataColumnCollection.
                table.Columns.Add(column);
            }

            return table;
        }
    }
}
