using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.ClientModel;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using System.Threading.Tasks;

namespace ObsidianKnockoff.Services
{
    internal class AiQueryHandlerService
    {
        //// properties
        //private IChatClient _chatClient;
        //private string _apiKey = Properties.Settings.Default.AiApiKey;
        //private string _modelEndpoint = Properties.Settings.Default.AiModelEndpoint;
        //private List<ChatMessage> _messageHistory = new List<ChatMessage>();

        //public List<ChatMessage> MessageHistory { get { return _messageHistory; } }

        //// constructors
        //public AiQueryHandlerService()
        //{
        //    // init model
        //    _chatClient = new ChatClient(
        //        "o4-mini",
        //        new ApiKeyCredential(_apiKey),
        //        new OpenAI.OpenAIClientOptions { Endpoint = new Uri(_modelEndpoint) }
        //    ).AsIChatClient();
        //}

        //// methods
        //public async Task<ChatMessage> QueryModel(ChatMessage userMessage)
        //{
        //    string sender = "Roggenrola";
        //    string message = "";

        //    // save user message to history
        //    _messageHistory.Add(userMessage);

        //    // query model and save response
        //    ChatMessage responseMessage = new ChatMessage();

        //    _messageHistory.Add(responseMessage);

        //    return responseMessage;
        //}
    }
}
