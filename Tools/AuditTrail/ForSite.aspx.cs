using SCBasics.AuditTrail.DataAccess;
using Sitecore.Shell.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SC7.Web.Tools.AuditTrail
{
    public partial class ForSite : SecurePage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadSiteAuditTrailDetails();
        }

        private void LoadSiteAuditTrailDetails()
        {
            SCAuditLogsDataContext scAuditLogsDataContext =
                new SCAuditLogsDataContext();
            
            IOrderedQueryable<Log> logs =
                scAuditLogsDataContext.Logs.Where(x => 
                    x.SiteName == Sitecore.Context.Site.Name).
                OrderByDescending(x => x.Date);

            if (logs == null && !logs.Any())
            {
                ShowMessage("No audit trail records found, it seems that you haven't configured SCAuditTrail or no actions recorded yet.",
                    "Error");
                lblMessage.Visible = true;
                tblSCAuditTrailForSite.Visible = false;
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

                        
                        // ItemID
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.SCItemId,
                            SCBasics.AuditTrail.Utils.SourceTextType.Text,
                            aLog.SCItemPath);

                        //ItemLanguage
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.SCLanguage,
                            SCBasics.AuditTrail.Utils.SourceTextType.Text);

                        // ItemVersion
                        SCBasics.AuditTrail.Utils.UIControlUtil.AddTableCell(tableRow,
                            aLog.SCVersion,
                            SCBasics.AuditTrail.Utils.SourceTextType.Text);

                        tblSCAuditTrailForSite.Rows.Add(tableRow);
                    }

                }


                lblMessage.Visible = true;
                tblSCAuditTrailForSite.Visible = true;

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