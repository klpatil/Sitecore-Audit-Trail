using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace SCBasics.AuditTrail.Utils
{
    public class UIControlUtil
    {
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/8kb3ddd4%28v=vs.110%29.aspx
        /// </summary>
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:MM:ss.fff";

        
        public static TableCell AddTableCell(TableRow tableRow,
            string value, SourceTextType fieldType, string toolTip = "")
        {
            TableCell tableCell1 = new TableCell();

            string valueToPrint = "NA";

            switch (fieldType)
            {
                case SourceTextType.DateTime:
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        DateTime createdDate = DateTime.Now;
                        createdDate = DateTime.Parse(value);
                        
                        valueToPrint = SCBasics.AuditTrail.Utils.DateTimeUtil.TimeAgo(createdDate);
                    }
                    else
                    {
                        valueToPrint = "NA";
                    }
                    break;
                case SourceTextType.Text:
                    valueToPrint = !string.IsNullOrWhiteSpace(value) ? value : "NA";
                    break;
                default:
                    valueToPrint = !string.IsNullOrWhiteSpace(value) ? value : "NA";
                    break;
            }

            tableCell1.Text = valueToPrint;
            tableCell1.ToolTip = toolTip;
            tableRow.Cells.Add(tableCell1);
            return tableCell1;
        }
    }

    public enum SourceTextType
    {
        DateTime,
        Text
    }
}
