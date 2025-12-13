// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AMWE_RealTime_Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class UserToAdminChatHub : Hub
    {
        private readonly ILogger _logger;

        private readonly ApplicationContext _context;

        public UserToAdminChatHub(ILogger<UserToAdminChatHub> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        private static readonly List<ChatState> ChatStates = new List<ChatState>();

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            }
            else if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.IsInRole(Role.GlobalUserRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            }
            else if (Context.User.IsInRole(Role.GlobalAdminRole))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Role.GlobalAdminGroup);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public string GetTransportType()
        {
            return Context.Features.Get<IHttpTransportFeature>().TransportType.ToString();
        }

        private static uint s_chatId = 0;

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task<uint> OpenChat(uint clientID)
        {
            _logger.LogInformation($"Вызван OpenChat от {Context.User.Identity.Name}");
            ChatState chatState = new ChatState()
            {
                ID = s_chatId++,
                AdminConnectionID = Context.ConnectionId,
                User = _context.GlobalClientsList.First(x => x.Id == clientID),
                IsAccepted = false
            };
            ChatStates.Add(chatState);
            await Clients.Group($"ID {chatState.User.Id}/" + chatState.User.Nameofpc).SendAsync("OpenChat", chatState.ID);
            _logger.LogInformation($"Создан ChatState с ID {s_chatId} / {chatState.ID}");
            return chatState.ID;
        }

        [Authorize(Roles = Role.GlobalUserRole)]
        public async Task AcceptChat(uint chatID)
        {
            _logger.LogInformation($"Вызван AcceptChat {chatID} от {Context.User.Identity.Name}");
            ChatState chat = ChatStates.Find(x => x.ID == chatID);
            _logger.LogInformation($"Найденный объект: {chat} в {ChatStates.Count} {ChatStates.First()}");
            if (uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/")[3..]) == chat.User.Id)
            {
                _logger.LogInformation($"Проверка пройдена, создается чат...");
                chat.IsAccepted = true;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat {chatID}");
                await Groups.AddToGroupAsync(chat.AdminConnectionID, $"Chat {chatID}");
                await Clients.Group($"Chat {chatID}").SendAsync("AcceptChatID", chatID);
            }
        }

        public async Task SendMessageToChat(uint chatID, string message)
        {
            _logger.LogInformation($"Отправлено сообщение в чат {chatID} от {Context.User.Identity.Name} // {message}");
            ChatState chat = ChatStates.Find(x => x.ID == chatID);
            string user;
            bool check;
            if (Context.ConnectionId == chat.AdminConnectionID)
            {
                check = true;
                user = Context.User.Identity.Name;
            }
            else
            {
                check = uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/")[3..]) == chat.User.Id;
                user = chat.User.Nameofpc;
            }
            if (chat.IsAccepted && check)
            {
                await Clients.Group($"Chat {chatID}").SendAsync("ReceiveMessage", chatID, message, user, DateTime.Now);
            }
        }

        public async Task CloseChat(uint chatID)
        {
            _logger.LogInformation($"Вызван CloseChat от {Context.User.Identity.Name}");
            ChatState chat = ChatStates.Find(x => x.ID == chatID);
            bool check = Context.ConnectionId == chat.AdminConnectionID
|| uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/")[3..]) == chat.User.Id;
            if (check)
            {
                _logger.LogInformation($"CloseDeleteChat вызыван у чата {chatID}");
                await Clients.Group($"Chat {chatID}").SendAsync("CloseDeleteChat", chatID);
                _ = ChatStates.Remove(chat);
            }
        }
    }

    public static class Extensions
    {
        public static string GetUntilOrEmpty(this string text, string stopAt = "-")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text[..charLocation];
                }
            }

            return string.Empty;
        }
    }
}