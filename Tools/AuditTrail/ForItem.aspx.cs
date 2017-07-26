using SCBasics.AuditTrail.DataAccess;
using Sitecore.Shell.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;


namespace SC7.Web.Tools.AuditTrail
{
    public partial class ForItem : SecurePage
    {
              
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    LoadItemAuditTrailDetails(HttpUtility.UrlDecode(Request.QueryString["id"]));
            }
        }
        
        private void LoadItemAuditTrailDetails(string ItemID)
        {
            SCAuditLogsDataContext scAuditLogsDataContext =
                new SCAuditLogsDataContext(ConfigurationManager.ConnectionStrings["SCAuditTrailConnectionString"].ConnectionString);
            
            IOrderedQueryable<Log> logs =
                scAuditLogsDataContext.Logs.Where(x => 
                    x.SCItemId == ItemID).
                OrderByDescending(x => x.Date);

            if (logs == null && !logs.Any())
            {
                ShowMessage("No audit trail records found, it seems that you haven't configured SCAuditTrail or no actions recorded yet.",
                    "Error");
                lblMessage.Visible = true;
                tblSCAuditTrailForItem.Visible = false;
            }
            else
            {

                foreach (Log aLog in logs)
                {

                    if (aLog != null)
                    {

                        TableRow tableRow = new TableRow();
                        tableRow.ID = "row" + aLog.ID.ToString();
                        tableRow.TableSection = TableRowSection.TableBody;


                        //Action
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.SCAction,
                            SCBasics.AuditTrail.Utils.SourceTextType.Text);


                        //UserName
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                             aLog.SCUser,
                             SCBasics.AuditTrail.Utils.SourceTextType.Text);

                        //Date
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.Date.ToUniversalTime().ToString(),
                            SCBasics.AuditTrail.Utils.SourceTextType.DateTime);


                        //ItemLanguage
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.SCLanguage,
                            SCBasics.AuditTrail.Utils.SourceTextType.Text);

                        // ItemVersion
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.SCVersion,
                            SCBasics.AuditTrail.Utils.SourceTextType.Text);

                        tblSCAuditTrailForItem.Rows.Add(tableRow);
                    }

                }


                lblMessage.Visible = true;
                tblSCAuditTrailForItem.Visible = true;

            }
        }

        private void Page_Error(object sender, EventArgs e)
        {
            // Get last error from the server
            Exception exc = Server.GetLastError();
            Response.Write("Oops something went wrong : " + exc.Message);
            Sitecore.Diagnostics.Log.Error("SC Audit Trail For Item : " + exc, this);
            Server.ClearError();
        }

        /// <summary>
        /// This function will be used
        /// to show message
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="messageType">Message type to show</param>
        private void ShowMessage(string message, string messageType)
        {
            lblMessage.Visible = true;
            lblMessage.Text = message;
            if (messageType == "Error")
            {
                lblMessage.CssClass = "alert alert-danger";

            }
            else
            {
                lblMessage.CssClass = "alert alert-success";
            }
        }

    }
}