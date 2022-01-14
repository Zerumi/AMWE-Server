// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using AMWE_RealTime_Server.Controllers;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class UserToAdminChatHub : Hub
    {
        private readonly ILogger _logger;

        public UserToAdminChatHub(ILogger<UserToAdminChatHub> logger)
        {
            _logger = logger;
        }

        private static readonly List<ChatState> chatStates = new List<ChatState>();

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

        private static uint ChatId = 0;

        [Authorize(Roles = Role.GlobalAdminRole)]
        public async Task<uint> OpenChat(uint ClientID)
        {
            _logger.LogInformation($"Вызван OpenChat от {Context.User.Identity.Name}");
            ChatState chatState = new ChatState()
            {
                ID = ChatId++,
                AdminConnectionID = Context.ConnectionId,
                User = AuthController.GlobalClientsList.Find(x => x.Id == ClientID),
                IsAccepted = false
            };
            chatStates.Add(chatState);
            await Clients.Group($"ID {chatState.User.Id}/" + chatState.User.Nameofpc).SendAsync("OpenChat", chatState.ID);
            _logger.LogInformation($"Создан ChatState с ID {ChatId} / {chatState.ID}");
            return chatState.ID;
        }

        [Authorize(Roles = Role.GlobalUserRole)]
        public async Task AcceptChat(uint ChatID)
        {
            _logger.LogInformation($"Вызван AcceptChat {ChatID} от {Context.User.Identity.Name}");
            var chat = chatStates.Find(x => x.ID == ChatID);
            _logger.LogInformation($"Найденный объект: {chat} в {chatStates.Count} {chatStates.First()}");
            if (uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/").Substring(3)) == chat.User.Id)
            {
                _logger.LogInformation($"Проверка пройдена, создается чат...");
                chat.IsAccepted = true;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat {ChatID}");
                await Groups.AddToGroupAsync(chat.AdminConnectionID, $"Chat {ChatID}");
                await Clients.Group($"Chat {ChatID}").SendAsync("AcceptChatID", ChatID);
            }
        }

        public async Task SendMessageToChat(uint ChatID, string Message)
        {
            _logger.LogInformation($"Отправлено сообщение в чат {ChatID} от {Context.User.Identity.Name} // {Message}");
            var chat = chatStates.Find(x => x.ID == ChatID);
            string user;
            bool check;
            if (Context.ConnectionId == chat.AdminConnectionID)
            {
                check = true;
                user = Context.User.Identity.Name;
            }
            else
            {
                check = uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/").Substring(3)) == chat.User.Id;
                user = chat.User.Nameofpc;
            }
            if (chat.IsAccepted && check)
            {
                await Clients.Group($"Chat {ChatID}").SendAsync("ReceiveMessage", ChatID, Message, user, DateTime.Now);
            }
        }

        public async Task CloseChat(uint ChatID)
        {
            _logger.LogInformation($"Вызван CloseChat от {Context.User.Identity.Name}");
            var chat = chatStates.Find(x => x.ID == ChatID);
            bool check;
            if (Context.ConnectionId == chat.AdminConnectionID)
                check = true;
            else
                check = uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/").Substring(3)) == chat.User.Id;
            if (check)
            {
                _logger.LogInformation($"CloseDeleteChat вызыван у чата {ChatID}");
                await Clients.Group($"Chat {ChatID}").SendAsync("CloseDeleteChat", ChatID);
                _ = chatStates.Remove(chat);
            }
        }
    }

    public static class Extensions
    {
        public static string GetUntilOrEmpty(this string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
    }
}
