using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Plugin
{
    public class PluginCSCode : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            int stageNumber = 0;
            tracingService.Trace("Stage {0}",++stageNumber);
            var targetentity = (Entity)context.InputParameters["Target"];
            string accountName = "";
            string accountFax = "";

            tracingService.Trace("Stage {0}", ++stageNumber);
            if (targetentity.Attributes.ContainsKey("name"))
                accountName = (string)targetentity["name"];
            if(targetentity.Attributes.ContainsKey("fax"))
                accountFax = (string)targetentity["fax"];
             
            var newAccount = new Entity("cr8af_accountcopy");
             
            newAccount["cr8af_name"] = accountName + " Copy";             
            newAccount["cr8af_fax"] = accountFax + " Copy";
             
            // Create
            //Guid accountId = service.Create(newAccount);

            // Create with CreateRequest and CreateResponse
            var request = new CreateRequest() { Target = newAccount };
            var response = (CreateResponse) service.Execute(request);
            Guid accountId = response.id;

            tracingService.Trace("Stage {0} {1}", ++stageNumber,accountId);
            
            // Update
            // var existingAccount = new Entity("cr8af_accountcopy", accountId);
            // var updateAccount = new Entity("cr8af_accountcopy");
            // updateAccount.Id=existingAccount.Id;
            // updateAccount["cr8af_fax"] = accountFax + " Copy" + " Update";
            // service.Update(updateAccount);

            // Delete
            //service.Delete("cr8af_accountcopy", accountId);
            
            // Retrieve with Guid
            //var retrieveAccountCopy = service.Retrieve("cr8af_accountcopy", accountId, new ColumnSet(true));//retrieve all columns
            var retrieveAccountCopy = service.Retrieve("cr8af_accountcopy", accountId, new ColumnSet("cr8af_name", "cr8af_fax"));

            // update after retrieve
            var updateAccount2 = new Entity("cr8af_accountcopy");
            updateAccount2.Id = accountId;
            updateAccount2["cr8af_name"] = retrieveAccountCopy["cr8af_name"] + " Update";
            updateAccount2["cr8af_fax"] = retrieveAccountCopy["cr8af_fax"] + " Update";
            service.Update(updateAccount2);

            //Retrieve with alternative key
            //RetrieveRequest retrieveRequest = new RetrieveRequest()
            //{
            //    ColumnSet = new ColumnSet("name", "fax"),
            //    Target = new EntityReference("account","name", "new account plug in test9")
            //};
            //var retrieveResponse = (RetrieveResponse) service.Execute(retrieveRequest);
            //Entity entityRetrieve = retrieveResponse.Entity;

            //tracingService.Trace("Stage {0} {1} {2}", ++stageNumber, entityRetrieve["name"],entityRetrieve["fax"]);
             
            // Retrieve with compound alternative keys (KeyAttributeCollection) /more than one key
            var keyS = new KeyAttributeCollection();
            keyS.Add("name", "new account plug in test10");
            //keyS.Add("key2", "value2");
            tracingService.Trace("Stage {0} keys", ++stageNumber);
            RetrieveRequest retrieveRequest2 = new RetrieveRequest()
            {
                ColumnSet = new ColumnSet("name", "fax"),
                Target = new EntityReference("account", keyS)
            };
            tracingService.Trace("Stage {0}  retrieve request2", ++stageNumber);
            string name = "Nothing";
            string fax = "Nothing";
            try
            {
                var retrieveResponse2 = (RetrieveResponse)service.Execute(retrieveRequest2);
                
                if (retrieveResponse2 != null)
                {
                    tracingService.Trace("Stage {0}  retrieve response2", ++stageNumber);
                    Entity entityRetrieve2 = retrieveResponse2.Entity;

                    tracingService.Trace("Stage {0}   entityRetrieve2", ++stageNumber);


                    if (entityRetrieve2.Attributes.Contains("name"))
                    {
                        name = (string)entityRetrieve2["name"];
                    }


                    if (entityRetrieve2.Attributes.Contains("fax"))
                    {
                        fax = (string)entityRetrieve2["fax"];
                    }
                }
            }
            catch  
            {

            }
            
            tracingService.Trace("Stage {0} {1} {2}", ++stageNumber, name, fax);

            //Retrieve Multiple

            //Condition Expression
            ConditionExpression conditionExp = new ConditionExpression();
            conditionExp.AttributeName = "name";
            conditionExp.Operator = ConditionOperator.BeginsWith;
            conditionExp.Values.Add("new account");

            //Filter Expression
            FilterExpression filterExp    = new FilterExpression();
            filterExp.AddCondition(conditionExp);

            //Query Expression
            QueryExpression queryExp = new QueryExpression("account");
            //qE.ColumnSet.AddColumn("name");
            queryExp.ColumnSet.AllColumns = true;
            queryExp.Criteria.AddFilter(filterExp);

            EntityCollection entityCollection = service.RetrieveMultiple(queryExp);

            foreach (Entity entity in entityCollection.Entities)
            {
                string fax1 = "No fax";
                if (entity.Attributes.Contains("fax"))
                {
                    fax1 = (string)entity["fax"];
                }
                string name1 = "";
                if (entity.Attributes.Contains("name"))
                {
                    name1 = (string)entity["name"];
                }

                tracingService.Trace("Stage {0} {1} {2}", ++stageNumber, name1, fax1);

            }

        }
    }
}
