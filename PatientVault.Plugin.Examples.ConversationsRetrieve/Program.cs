using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PatientVault.Plugin.Entities;
using PatientVault.Plugin.Service;
using PatientVault.PluginDotNet40.Factory;

namespace PatientVault.Plugin.Examples.ConversationsRetrieve
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

            RetrieveConversations(client);
            RetrieveConversationSubjects(client);
            RetrieveConversationSubjectMessages(client);
            RetrieveMessageAttachmentRequest(client);

            Console.ReadLine();
        }

        #region Properties

        /// <summary>
        ///  Access Token Received from Authentication
        /// </summary>
        private static string _accessToken = "";

        /// <summary>
        ///  Conversation Group Id that was obtained from Conversation Create Group
        /// </summary>
        private static Guid _conversationGroupId = Guid.Parse("");

        /// <summary>
        ///  Conversation Id of the First Conversation that will be obtained from the Conversations List
        /// </summary>
        private static Guid? _conversationId;

        /// <summary>
        ///  Conversation Subject Id of the First Subject that was obtained from Conversations
        /// </summary>
        private static Guid? _conversationSubjectId;

        /// <summary>
        ///  Message Id of a Conversation Message that contains an attachment
        /// </summary>
        private static Guid? _conversationMessageId = Guid.Parse("");
        #endregion

        #region Retrieve Conversations

        private static void RetrieveConversations(PatientVaultClient client)
        {
            Console.WriteLine("\n Retrieving Conversations...");

            var request = new ConversationRetrieveConversationsRequest();
            request.AccessToken = _accessToken;
            if (_conversationGroupId == null)
            {
                Console.WriteLine("\n You need a Conversation Group Id...");
                return;
            }
            request.ConversationGroupId = _conversationGroupId;

            var result = client.ConversationRetrieveConversations(request);
            OutputResult(result);
            if (result.Success)
            {
                // Conversations List
                var conversationsList = result.Conversations;

                var firstConversation = conversationsList.FirstOrDefault();

                _conversationId = firstConversation?.ConversationId;
            }
        }
        #endregion

        #region Retrieve Conversation Subjects

        private static void RetrieveConversationSubjects(PatientVaultClient client)
        {
            Console.WriteLine("\n Retrieving Subjects...");

            var request = new ConversationRetrieveSubjectsRequest();
            request.ConversationGroupId = _conversationGroupId;
            if (_conversationId == null)
            {
                Console.WriteLine("\n You need a Conversation Id...");
                return;
            }
            request.ConversationId = (Guid)_conversationId;
            request.SingleRecord = false;
            request.UnreceivedOnly = false;

            var result = client.ConversationRetrieveSubjects(request);
            OutputResult(result);
            if (result.Success)
            {
                // Get the list of Subjects
                var listOfConversationSubjects = result.Subjects;
                // Select the First Subject
                var firstSubject = listOfConversationSubjects.FirstOrDefault();
                // Set the Conversation Subject Id from first subject
                _conversationSubjectId = firstSubject?.ConversationSubjectId;
            }
        }
        #endregion

        #region Retrieve Conversation Subject Messages

        private static void RetrieveConversationSubjectMessages(PatientVaultClient client)
        {
            Console.WriteLine("\n Retrieving Messages...");

            var request = new ConversationRetrieveMessagesRequest();
            request.ConversationGroupId = _conversationGroupId;
            if (_conversationSubjectId == null)
            {
                Console.WriteLine("\n You need a Conversation Subject Id...");
                return;
            }
            request.ConversationSubjectId = (Guid)_conversationSubjectId;

            var result = client.ConversationRetrieveMessages(request);
            OutputResult(result);
            if (result.Success)
            {
                // Get list of messages
                var listOfMessages = result.Messages;
            }
        }
        #endregion

        #region Retrieve Message Attachment

        private static void RetrieveMessageAttachmentRequest(PatientVaultClient client)
        {
            Console.WriteLine("\n Retrieving Message Attachment...");
            var request = new ConversationRetrieveMessageAttachmentRequest();
            request.ConversationGroupId = _conversationGroupId;
            if (_conversationMessageId == null)
            {
                Console.WriteLine("\n You need a Conversation Subject Message Id...");
                return;
            }
            request.ConversationSubjectMessageId = (Guid)_conversationMessageId;

            var result = client.ConversationRetrieveMessageAttachment(request);
            OutputResult(result);
            if (result.Success)
            {
                if (result.FileNotFound != null && (bool)result.FileNotFound) return;

                var base64EncodedFile = result.EncodedFile;
                var fileHash = result.EncodedFileHash;
                var fileName = result.FileName;
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
