using log4net.Appender;
using log4net.spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCBasics.AuditTrail.ExtensionMethods;
namespace SCBasics.AuditTrail.Appender
{
    public class SitecoreDatabaseLogAppender : ADONetAppender
    {
        /// <summary>
        /// E.g. "AUDIT (sitecore\admin): Login";        
        /// </summary>
        private const string AUDIT_UNFORMAT_PATTERN1 = "AUDIT ({0}): {1}";
        /// <summary>
        /// E.g. AUDIT (sitecore\admin): Restore: archive: recyclebin, id: e232f06d-ac70-4912-908e-91b284991cab
        /// </summary>
        private const string AUDIT_UNFORMAT_PATTERN2 = "AUDIT ({0}): {1}: archive: recyclebin, id: {2}";
        /// <summary>
        /// E.g. "AUDIT (sitecore\admin): Save item: master:/sitecore/content/Home, language: en, version: 3, id: {110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"
        /// </summary>
        private const string AUDIT_UNFORMAT_PATTERN3 = "AUDIT ({0}): {1}: {2}:{3}, language: {4}, version: {5}, id: {6}";
        /// <summary>
        /// E.g. "AUDIT (sitecore\admin): Rename item : master:/sitecore/content/Home/P1 D New, language: en, version: 1, id: {54CFD505-A8B2-4A8A-A0C4-A358D9C21D06} to P1 D New Test"
        /// </summary>
        private const string AUDIT_UNFORMAT_PATTERN4 = "AUDIT ({0}): {1}: {2}:{3}, language: {4}, version: {5}, id: {6} to {7}";
        /// <summary>
        /// E.g. AUDIT (sitecore\admin): Execute workflow command. Item: master:/sitecore/content/Home/KP Demo, language: ar-SY, version: 2, id: {C4311D97-3651-4776-AAED-82CF4C16034F}, command: /sitecore/system/Workflows/Sample Workflow/Awaiting Approval/Reject, previous state: Awaiting Approval, next state: /sitecore/system/Workflows/Sample Workflow/Draft, user: sitecore\admin
        /// </summary>
        private const string AUDIT_UNFORMAT_PATTERN5 = "AUDIT ({0}): {1}. Item: {2}:{3}, language: {4}, version: {5}, id: {6}, command: {7}, previous state: {8}, next state: {9}, user: {10}";

        private const string SEPERATOR = "|";

        protected override void Append(log4net.spi.LoggingEvent loggingEvent)
        {
            try
            {


                // We are not checking LogLevel -- That has been added at FilterLevel
                // in Configuration
                // Thanks : http://sitecoreblog.alexshyba.com/2010/07/sitecore-logging-part-3-adding-custom.html#more
                if (Sitecore.Context.Site != null)
                {
                    var properties = loggingEvent.Properties;

                    // By Default Everything comes as a Shell
                    // So, Here we are string RAW Message parsing it using
                    // http://blogs.msdn.com/b/simonince/archive/2009/07/09/string-unformat-i-ve-created-a-monster.aspx


                    // User is perfect!
                    if (Sitecore.Context.User != null)
                    {
                        properties["scuser"] = Sitecore.Context.User.Name;
                    }

                    if (!string.IsNullOrWhiteSpace(loggingEvent.RenderedMessage))
                    {
                        string auditRAWMessage = loggingEvent.RenderedMessage;

                        object[] ParsedValues = AuditUnformatItem(@auditRAWMessage);
                        if (ParsedValues != null && ParsedValues.Any())
                        {
                            // 0 - UserName, 1 - Action, 2 - DatabaseName
                            // 3 - ItemPath, 4 - Language, 5 - Version, 6 - ItemID
                            // 7 - Misc e.g. When you rename item you get new item name
                            // WORKFLOW : 7 - Command, 8 - Previous State
                            // 9 - Next State, 10 - User

                            SetPropertyValue(properties, ParsedValues, "scaction", 1);

                            if (properties["scaction"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(properties["scaction"]))
                                && Convert.ToString(properties["scaction"]) == "Restore")
                                return; // We don't process Restore as we don't have all details

                            SetPropertyValue(properties, ParsedValues, "scitemid", 6);

                            if (ParsedValues.Length > 2)
                            {
                                string databaseName = Convert.ToString(ParsedValues.GetValue(2));
                                if (!string.IsNullOrWhiteSpace(databaseName))
                                {
                                    Sitecore.Data.Database database =
                                        Sitecore.Configuration.Factory.GetDatabase(databaseName);
                                    if (database != null && properties["scitemid"] != null)
                                    {
                                        Sitecore.Data.Items.Item item =
                                            database.GetItem(Convert.ToString(properties["scitemid"]));
                                        if (item != null)
                                        {
                                            properties["sitename"] = item.GetContextSite().Name;
                                        }                                        
                                        
                                    }
                                }
                            }

                            SetPropertyValue(properties, ParsedValues, "scitempath", 3);
                            SetPropertyValue(properties, ParsedValues, "sclanguage", 4);
                            SetPropertyValue(properties, ParsedValues, "scversion", 5);
                            SetPropertyValue(properties, ParsedValues, "scmisc", 7);

                            // Workflow                        
                            SetPropertyValue(properties, ParsedValues, "scmisc", 8, true);
                            SetPropertyValue(properties, ParsedValues, "scmisc", 9, true);
                            SetPropertyValue(properties, ParsedValues, "scmisc", 10, true);
                        }
                        
                    }

                }
                base.Append(loggingEvent);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Append method failed to record information", ex, this);
            }
        }

        private static void SetPropertyValue(log4net.helpers.PropertiesCollection properties,
            object[] ParsedValues, string propertyKey, int indexToFetch, bool isAppend = false)
        {

            if (ParsedValues.Length > indexToFetch)
                if (isAppend)
                {
                    properties[propertyKey] += SEPERATOR + Convert.ToString(ParsedValues.GetValue(indexToFetch));
                }
                else
                {
                    properties[propertyKey] = Convert.ToString(ParsedValues.GetValue(indexToFetch));
                }
        }

        private static object[] AuditUnformatItem(string auditString)
        {
            object[] pattern1Result = auditString.Unformat(AUDIT_UNFORMAT_PATTERN1);
            object[] pattern2Result = auditString.Unformat(AUDIT_UNFORMAT_PATTERN2);
            object[] pattern3Result = auditString.Unformat(AUDIT_UNFORMAT_PATTERN3);
            object[] pattern4Result = auditString.Unformat(AUDIT_UNFORMAT_PATTERN4);
            object[] pattern5Result = auditString.Unformat(AUDIT_UNFORMAT_PATTERN5);

            /*
             * The Logic here is we start fom MAX COUNT
             * Last Pattern will have least results
             */
            if (pattern5Result.Count() > pattern4Result.Count())
            {
                return pattern5Result;
            }
            if (pattern4Result.Count() > pattern3Result.Count())
            {
                return pattern4Result;
            }
            else if (pattern3Result.Count() > pattern2Result.Count())
            {
                return pattern3Result;
            }
            else if (pattern2Result.Count() > pattern1Result.Count())
                return pattern2Result;
            else
                return pattern1Result;

        }
    }
}
