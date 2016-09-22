using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PatientVault.Plugin.Entities;
using PatientVault.Plugin.Service;
using PatientVault.PluginDotNet40.Factory;

namespace PatientVault.Plugin.Examples.Authentication
{
    class Program
    {
        public static PatientVaultConfiguration GetConfiguration()
        {
            var configuration = new PatientVaultConfiguration();
            configuration.ApiRootUrl = @"https://test.neodecksoftware.com/pv/patientvaultapi";
            configuration.ApiKey = "";
            configuration.DevKey = "";
            configuration.CustomerKey = "";
            configuration.Culture = "en";
            configuration.TimeZone = "SA Western Standard Time";
            return configuration;
        }
        static void Main(string[] args)
        {
            var encryption = new EncryptionService();
            var configuration = GetConfiguration();
            var factory = new PatientVaultPostServiceDotNet40Factory();
            var client = new PatientVaultClient(encryption, configuration, factory);

            AccountExists(client);
            AccountCreate(client);
            AccountConnect(client);

            Console.ReadLine();

        }

        private static void AccountExists(PatientVaultClient client)
        {
            var request = new AccountExistsRequest();
            request.UserName = "johnsnow@example.com";
            AccountExistsResponse result = client.AccountExists(request);
            OutputResult(result);

            if (result.Exists)
            {
                // Code for connecting the account
            }

        }

        private static void AccountCreate(PatientVaultClient client)
        {
            var request = new AccountCreateRequest();
            request.Email = "johnsnow@example.com";
            request.FirstName = "John";
            request.MiddleName = "";
            request.LastName = "Snow";
            request.HomeCity = "New York";
            request.Pin = "1234";
            request.InstanceDescription = "GetWell Clinic";
            request.InstanceIdentifier = "123456789";
            var result = client.AccountCreate(request);
            OutputResult(result);

        }
        private static void AccountConnect(PatientVaultClient client)
        {
            var request = new AccountConnectRequest();
            request.UserName = "johnsnow@example.com";
            request.Pin = "1234";
            request.InstanceDescription = "GetWell Clinic";
            request.InstanceIdentifier = "123456789";
            var result = client.AccountConnect(request);
            OutputResult(result);
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
