using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PatientVault.Plugin.Entities;
using PatientVault.Plugin.Service;
using PatientVault.PluginDotNet40.Factory;

namespace PatientVault.Plugin.Examples.ConversationsCreate
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

            ConversationCreateGroup(client);
            ConversationCreate(client);
            ConversationCreateMessage(client);

            Console.ReadLine();
        }
        #region Properties

        /// <summary>
        ///  Access Token Received from Authentication
        /// </summary>
        private static string accessToken = "";

        /// <summary>
        ///  Conversation Group Id that was obtained from Conversation Create Group
        /// </summary>
        private static Guid? _conversationGroupId;

        /// <summary>
        ///  Conversation Subject Id that was obtained from Conversation Create
        /// </summary>
        private static Guid? _conversationSubjectId;

        #endregion

        #region Conversation Create Group

        private static void ConversationCreateGroup(PatientVaultClient client)
        {
            Console.WriteLine("\n Creating Conversation Group...");
            var request = new ConversationCreateGroupRequest();
            request.OwnerName = "OWNER";
            request.Title = "OWNER TITLE";
            request.DeveloperApplicationNote = "Sample Note";
            request.PrimaryIdentifier = "NPI";
            request.PrimaryIdentifierType = "123456789";
            request.SecondaryIdentifier = "License";
            request.SecondaryIdentifierType = "123456789";

            var result = client.ConversationCreateGroup(request);
            OutputResult(result);
            if (result.Success)
            {
                // Conversation Group Id
                _conversationGroupId = result.ConversationGroupId;
            }
        }
        #endregion

        #region Conversation Create

        private static void ConversationCreate(PatientVaultClient client)
        {
            Console.WriteLine("\n Creating a Conversation...");
            var request = new ConversationCreateRequest();
            request.AccessToken = accessToken;
            if (_conversationGroupId == null)
            {
                Console.WriteLine("\n You need a Conversation Group Id...");
                return;
            }
            request.ConversationGroupId = _conversationGroupId.Value;
            request.Title = "Get Well Clinic";
            request.Subject = "Your visit today";
            request.Message = "Thanks for visiting our practice today!";
            request.MessageHash = "";
            request.AuthorTitle = "Dr. John Smith";
            request.AuthorSubTitle = "Your doctor";

            var result = client.ConversationCreate(request);
            OutputResult(result);
            if (result.Success)
            {
                // Conversation Id
                var conversationId = result.ConversationId;
                // Conversation Subject Id
                _conversationSubjectId = result.ConversationSubjectId;
            }
        }
        #endregion

        #region Conversation Create Message

        private static void ConversationCreateMessage(PatientVaultClient client)
        {
            Console.WriteLine("\n Sending a Conversation Message with an attachment...");
            var request = new ConversationCreateMessageRequest();
            if (_conversationGroupId == null)
            {
                Console.WriteLine("\n You need a Conversation Group Id...");
                return;
            }
            request.ConversationGroupId = _conversationGroupId.Value;
            if (_conversationSubjectId == null)
            {
                Console.WriteLine("\n You need a Conversation Subjects Id...");
                return;
            }
            request.ConversationSubjectId = _conversationSubjectId.Value;
            request.AuthorIdentifier = "12345";
            request.AuthorIdentifierType = "UserID";
            request.AuthorTitle = "Dr. John Smith";
            request.AuthorSubTitle = "Your doctor";
            request.Message = "Hello, here are your lab results!";
            request.MessageHash = "";
            // If you wish to include an attachment in the message, we do so
            request.FileName = "ECG.PDF";
            request.EncodedFile = Convert.ToBase64String(Files.ECG);
            request.FileHash = ""; // This is automatically generated by the plugin

            var result = client.ConversationCreateMessage(request);
            OutputResult(result);
            if (result.Success)
            {
                // Created Message Id
                var messageId = result.ConversationSubjectMessageId;
            }
        }
        #endregion

        #region Output Result

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
        #endregion

    }
}
