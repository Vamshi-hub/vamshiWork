using astorWorkBackgroundService.Models;
using astorWorkDAO;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace astorWorkBackgroundService.EmailHelper
{
    static  class AttachmentHandler
    {
        public static string CreateDeliveryTblContent(IEnumerable<DeliveryMaterial> deliveryMaterials) {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<table border = '1' cellspacing = '0' cellpadding = '0' style = 'border-collapse:collapse; border: none; font-family:arial'>
                            <tr>");

            string[] colNames = new string[] { "Block", "Level", "Zone", "Marking No.", "Material Type", "Expected Delivery Date" };

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

        public static string CreateMRFTblContent(MRFMaster mrfMaster)
        {
            /*
            List<string> columnNames = new List<string>();
            columnNames.Add("Marking No.");
            columnNames.Add("Material Type");
            
            DataTable dataTable = _emailService.CreateDataTable("Materials", columnNames);

            // Create three new DataRow objects and add 
            // them to the DataTable
            DataRow row;
            foreach (MaterialMaster materialMaster in mrfMaster.Materials)
            {
                row = dataTable.NewRow();
                row[columnNames[0]] = materialMaster.MarkingNo;
                row[columnNames[1]] = materialMaster.MaterialType;
                dataTable.Rows.Add(row);
            }
            */
            StringBuilder sb = new StringBuilder();

            sb.Append(
            @"
            <table border = '1' cellspacing = '0' cellpadding = '0' style = 'border-collapse:collapse; border: none; font-family:arial'>
                <tr>");

            string[] colNames = new string[] { "Marking No.", "Block", "Level", "Zone", "Material Type" };

            for (int i = 0; i < colNames.Length; i++)
                sb.AppendFormat(string.Format(@"<td width = '250' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colNames[i]));

            sb.AppendFormat("</tr>");

            foreach (MaterialMaster materialMaster in mrfMaster.Materials)
            {
                string[] colData = new string[] { materialMaster.MarkingNo, materialMaster.Block, materialMaster.Level, materialMaster.Zone, materialMaster.MaterialType.Name };

                sb.AppendFormat(@"<tr>");

                for (int i = 0; i < colData.Length; i++)
                    sb.AppendFormat(string.Format(@"<td width = '200' valign = 'top' style = 'width:100.1pt;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt; font-size:10.0pt' >{0}</td>", colData[i]));

                sb.AppendFormat(@"<tr>");
            }

            sb.Append(@"</table>");

            return sb.ToString();
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
                column.Unique = false;
                if (i == 0)
                    column.ReadOnly = true;
                else
                    column.ReadOnly = false;

                // Add the Column to the DataColumnCollection.
                table.Columns.Add(column);
            }

            return table;
        }
    }
}
