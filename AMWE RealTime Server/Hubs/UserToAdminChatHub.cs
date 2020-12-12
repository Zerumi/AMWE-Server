// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using AMWE_RealTime_Server.Controllers;
using AMWE_RealTime_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Hubs
{
    [Authorize]
    public class UserToAdminChatHub : Hub
    {
        private readonly List<ChatState> chatStates = new List<ChatState>();

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

        private uint ChatId = 0;

        [Authorize(Roles=Role.GlobalAdminRole)]
        public async Task<uint> OpenChat(uint ClientID)
        {
            ChatState chatState = new ChatState()
            {
                ID = ChatId++,
                AdminConnectionID = Context.ConnectionId,
                User = AuthController.GlobalUsersList.Find(x => x.Id == ClientID),
                IsAccepted = false
            };
            chatStates.Add(chatState);
            await Clients.Group($"ID {chatState.User.Id} / " + chatState.User.Nameofpc).SendAsync("OpenChat", chatState.ID);
            return chatState.ID;
        }

        [Authorize(Roles=Role.GlobalUserRole)]
        public async Task AcceptChat(uint ChatID)
        {
            var chat = chatStates.Find(x => x.ID == ChatID);
            if (uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/").Substring(3)) == chat.User.Id)
            {
                chat.IsAccepted = true;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat {ChatID}");
                await Groups.AddToGroupAsync(chat.AdminConnectionID, $"Chat {ChatID}");
                await Clients.Group($"Chat {ChatID}").SendAsync("AcceptChatID", ChatID);
            }
        }

        public async Task SendMessageToChat(uint ChatID, string Message)
        {
            var chat = chatStates.Find(x => x.ID == ChatID);
            if (chat.IsAccepted && (uint.Parse(Context.User.Identity.Name.GetUntilOrEmpty("/").Substring(3)) == chat.User.Id || Context.ConnectionId == chat.AdminConnectionID))
            {
                await Clients.Group($"Chat {ChatID}").SendAsync("ReceiveMessage", ChatID, Message);
            }
        }

        [Authorize(Roles=Role.GlobalAdminRole)]
        public async Task CloseChat(uint ChatID)
        {
            var chat = chatStates.Find(x => x.ID == ChatID);
            if (chat.IsAccepted && Context.ConnectionId == chat.AdminConnectionID)
            {
                await Clients.Group($"Chat {ChatID}").SendAsync("CloseDeleteChat", ChatID);
                chatStates.Remove(chat);
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