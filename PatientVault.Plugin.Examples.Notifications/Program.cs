using System;
using Bridges.Contract.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PatientVault.Plugin.Contract;
using PatientVault.Plugin.Entities.Events;
using PatientVault.PluginDotNet40.Factory;


namespace PatientVault.Plugin.Examples.Notifications
{

    class Program
    {
        private const string ApiTestUrl = @"https://test.neodecksoftware.com/pv/patientvaultapi";
        public static IPatientVaultNotificationClient CreateClient(string root = ApiTestUrl, string culture = "ES")
        {
            IPatientVaultNotificationClient client;
            var configuration = new PatientVaultConfiguration();
            configuration.ApiRootUrl = root;
            configuration.ApiKey = "";
            configuration.DevKey = "";
            configuration.CustomerKey = "";
            configuration.ConversationGroupId = Guid.Parse("");
            configuration.Culture = culture;
            configuration.TimeZone = "SA Western Standard Time";
            configuration.TimeOutInMilliSeconds = 15000;
            var factory = new PatientVaultNotificationServiceFactory();
            // Create the client
            client = new PatientVaultNotificationClient(factory, configuration);
            return client;
            
        }
        static void Main(string[] args)
        {
            var client = CreateClient();
            // Create the client
            try
            {
                client.ConversationMessageReceived += client_ConversationMessageReceived;
                client.ConversationMessageReceivedByOthers += ClientOnConversationMessageReceivedByOthers;
                client.Closed += () => Console.WriteLine("STATUS: Connection Closed");
                client.Reconnected += () => Console.WriteLine("STATUS: Connection reconnected");
                client.ConnectionError += exception => Console.WriteLine(string.Format("STATUS: Connection error: {0}", exception.Message));

                client.Connected += delegate(ConnectionResult result)
                {
                    if (result.Success)
                    {
                        
                        Console.WriteLine("STATUS: Waiting for events...");
                    }
                    else
                    {
                        Console.WriteLine("Error connecting:");
                        Console.WriteLine(result.ExceptionDetails);
                        Console.WriteLine(result.InnerExceptionDetails);
                    }
                };
                client.Start();
                Console.ReadLine();
                client.Start();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Stop();
            }
            
        }

        private static void ClientOnConversationMessageReceivedByOthers(ConversationMessageReceivedByOthers e)
        {
            OutputResult(e, "Message Received By Others:");
        }

        static void client_ConversationMessageReceived(ConversationMessage e)
        {
            OutputResult(e, "Message Received:");
        }

        private static void OutputResult(object result, string title = "")
        {
            Console.WriteLine(title);
            var json = JsonConvert.SerializeObject(result);
            JObject parsed = JObject.Parse(json);
            foreach (var pair in parsed)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
        }

    }
}
