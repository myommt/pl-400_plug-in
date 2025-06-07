using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;

namespace Plugin
{
    public class _PluginCSCode : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Stage {0}","1");
            var entity = (Entity)context.InputParameters["Target"];
            var oldEntity = (Entity)context.PreEntityImages["PreImageAlias"];

            tracingService.Trace("Stage {0}", "2");
            if (entity.Attributes.ContainsKey("fax"))
            {
                string strfax = (string)entity["fax"];

                string strOldFax = "";
                if (oldEntity.Attributes.ContainsKey("fax"))
                {
                    strOldFax = (string)oldEntity["fax"];
                }
                else
                {
                    strOldFax = "Nothing";
                }

                tracingService.Trace("Stage {0}", "3");
                entity["address1_line3"] = "Old Data - " + strOldFax + "New Data - " + strfax;
                tracingService.Trace("Stage {0}", "4");
            }
            else
            {
                tracingService.Trace("Stage {0}", "5");
                entity["address1_line3"] = "There is no data here.";
                tracingService.Trace("Stage {0}", "6");
            }
        }
    }
}
