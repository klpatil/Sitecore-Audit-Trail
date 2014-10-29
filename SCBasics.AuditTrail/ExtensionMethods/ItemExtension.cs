using Sitecore.Data.Items;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCBasics.AuditTrail.ExtensionMethods
{
    /// <summary>
    /// Thanks to : 
    /// </summary>
    public static class ItemExtension
    {
        public static SiteInfo GetSiteInfo(this Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList();

            return siteInfoList.FirstOrDefault(info => item.Paths.FullPath.StartsWith(info.RootPath + info.StartItem));
        }

        /// <summary>
        /// Thanks : http://jignesh-patel.com/get-siteinfo-or-site-name-from-sitecore-item/
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetSiteName(this Item item)
        {
            var info = item.GetSiteInfo();
            return info != null ? info.Name : string.Empty;
        }


        /// <summary>
        /// http://firebreaksice.com/sitecore-context-site-resolution/
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Sitecore.Sites.SiteContext GetContextSite(this Item item)
        {
            string[] systemSites = new string[] { "shell", "login", "admin", "service", "modules_shell", "modules_website", "scheduler", "system", "publisher" };
            var siteInfoList =
                Sitecore.Configuration.Factory.GetSiteInfoList().Where(x => !systemSites.Contains(x.Name));
            // loop through all configured sites
            foreach (var site in siteInfoList)
            {
                // get this site's home page item
                var homePage = item.Database.GetItem(site.RootPath + site.StartItem);

                // if the item lives within this site, this is our context site
                if (homePage != null && homePage.Axes.IsAncestorOf(item))
                {
                    return Sitecore.Configuration.Factory.GetSite(site.Name);
                }
            }

            // fallback and assume context site
            return Sitecore.Context.Site;
        }

    }
}
