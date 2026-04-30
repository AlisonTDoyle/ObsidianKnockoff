using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using ObsidianKnockoff.Classes;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianKnockoff.Services
{
    internal class AiQueryHandlerService
    {
        // properties
        private const string MODEL = "claude-sonnet-4-5";

        private AnthropicClient _client;
        private string _apiKey = Properties.Settings.Default.AiApiKey;
        private List<Message> _messageHistory = new List<Message>();

        public List<Message> MessageHistory { get { return _messageHistory; } }

        // constructors
        public AiQueryHandlerService()
        {
            _client = new AnthropicClient(_apiKey);
        }

        // methods
        public async Task<string> QueryWithRag(string userQuestion, List<Note> relevantNotes)
        {
            // build context from matching notes
            StringBuilder context = new StringBuilder();
            context.AppendLine("Here are some relevant notes from the user's notebook:");
            context.AppendLine();

            foreach (Note note in relevantNotes)
            {
                context.AppendLine($"--- {note.Title} ---");
                context.AppendLine(note.Content);
                context.AppendLine();
            }

            // combine context + question into a single prompt
            string prompt = $"{context}. Using the notes attached as context, answer this question: {userQuestion}. If the notes don't contain relevant information, say so.";

            // add user message to history
            _messageHistory.Add(new Message
            {
                Role = RoleType.User,
                Content = prompt
            });

            // send to AI
            MessageParameters parameters = new MessageParameters
            {
                Model = MODEL,
                MaxTokens = 1024,
                Messages = _messageHistory
            };

            MessageResponse response = await _client.Messages.GetClaudeMessageAsync(parameters);
            string reply = response.Content[0].Text;

            // store reply in history for multi-turn conversation
            _messageHistory.Add(new Message
            {
                Role = RoleType.Assistant,
                Content = reply
            });

            return reply;
        }
    }
}