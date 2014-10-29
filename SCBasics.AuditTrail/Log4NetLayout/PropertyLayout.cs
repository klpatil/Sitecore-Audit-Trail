using log4net.Layout;
using log4net.spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCBasics.AuditTrail.Log4NetLayout
{
    /// <summary>
    /// Thanks : http://sitecoreblog.alexshyba.com/2010/07/sitecore-logging-part-3-adding-custom.html
    /// </summary>
    public class PropertyLayout : LayoutSkeleton
    {
        // Fields
        protected const int BUF_SIZE = 0x100;
        private StringBuilder m_sbuf = new StringBuilder(0x100);
        protected const int MAX_CAPACITY = 0x400;
        private string m_propertyName;

        public string PropertyName
        {

            get
            {
                return m_propertyName;
            }
            set
            {
                m_propertyName = value;
            }
        }

        public override bool IgnoresException
        {
            get
            {
                return false;
            }
        }

        public override void ActivateOptions()
        {
        }

        public override string Format(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }


            if (m_sbuf.Capacity > 0x400)
            {                
                m_sbuf = new StringBuilder(0x100);
            }
            else
            {
                m_sbuf.Length = 0;
            }

            if (!String.IsNullOrEmpty(PropertyName))
            {
                m_sbuf.Append(loggingEvent.Properties[PropertyName]);
            }

            return m_sbuf.ToString();
        }

    }
}

