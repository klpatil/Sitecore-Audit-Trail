using Sitecore.Data.Items;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;

using Sitecore.Sites;

namespace SCBasics.AuditTrail.ExtensionMethods
{
    /// <summary>
    /// Thanks to : 
    /// </summary>
    public static class ItemExtension
    {
        public static SiteContext GetContextSite(this Item item)
        {
            SiteInfo siteInfo = GetSiteInfo(item);

            if (siteInfo != null)
            {
                return Sitecore.Configuration.Factory.GetSite(siteInfo.Name);
            }

            // fallback and assume context site
            return Sitecore.Context.Site;
        }

        //Copied from mercury
        public static SiteInfo GetSiteInfo(this Item item)
        {
            List<SiteInfo> sites = SiteContextFactory.Sites;

            // There is no default way of getting the related site of an item.
            // Here we find the related site by checking if its root path is a prefix of the item path.
            // For robustness we take the most specific one if multiple sites match.
            return sites.Where(site => !string.IsNullOrWhiteSpace(site.RootPath))
                        .OrderByDescending(site => site.RootPath.Length)
                        .FirstOrDefault(
                            site => item.Paths.FullPath.StartsWith(site.RootPath, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
