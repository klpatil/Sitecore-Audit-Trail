namespace SCBasics.AuditTrail.Commands
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;
    using System.Collections.Specialized;
    using System.Web;
    using System.Linq;

    public class SCAuditTrailForItemCommand : Command
    {
        public override void Execute([NotNull] CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                var parameters = new NameValueCollection();
                parameters["id"] = item.ID.ToString();
                parameters["language"] = item.Language.ToString();
                parameters["version"] = item.Version.ToString();
                Context.ClientPage.Start(this, "Run", parameters);
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            string str = args.Parameters["id"];
            string name = args.Parameters["language"];
            string str3 = args.Parameters["version"];
            var version = Sitecore.Data.Version.Parse(str3);

            Item item = Context.ContentDatabase.GetItem(str, Sitecore.Globalization.Language.Parse(name), version);
            Error.AssertItemFound(item);

            if (!SheerResponse.CheckModified()) return;
                      
            if (!args.IsPostBack)
            {
                string forItemPageURL = Sitecore.Configuration.Settings.GetSetting("SCBasics.AuditTrail.ForItemPageURL");

                if (!string.IsNullOrWhiteSpace(forItemPageURL))
                {
                    Sitecore.Text.UrlString url = new Sitecore.Text.UrlString(
                        forItemPageURL);
                    url.Append("id", HttpUtility.UrlEncode(args.Parameters["id"]));
                    url.Append("la", HttpUtility.UrlEncode(args.Parameters["language"]));
                    url.Append("ve", HttpUtility.UrlEncode(args.Parameters["version"]));
                    
                    Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(url.ToString(),
                        "990px", "570px",
                        "ForItemPageURL", true);
                    args.WaitForPostBack();
                }
                else
                {
                    SheerResponse.Alert("SCBasics.AuditTrail.ForItemPageURL is empty. Please verify your configurations", true);
                }
            }
            
        }

        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");

            if (!context.Items.Any())
            {
                return CommandState.Disabled;
            }          

            return base.QueryState(context);
        }
    }
}
