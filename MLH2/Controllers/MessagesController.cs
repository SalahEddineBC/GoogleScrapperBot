using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
namespace MLH2
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<ProfileForm> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(ProfileForm.BuildForm));
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                StateClient sc = activity.GetStateClient();
                BotData userData = sc.BotState.GetPrivateConversationData(
                     activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                var boolProfileComplete = userData.GetProperty<bool>("ProfileComplete");
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));


                if (!boolProfileComplete)
                {
                    // Call our FormFlow by calling MakeRootDialog
                    await Conversation.SendAsync(activity, MakeRootDialog);
                }
                else
                {
                    var Name = userData.GetProperty<string>("Name");
                    var V = userData.GetProperty<int>("Valeur");
                    var centreInteret = userData.GetProperty<string>("centreInteret");
                    var conn = new DBConnect();
                    //var insert = "INSERT INTO members VALUES (11, '" + Name + "', '" + Email + "', '" + WhatWeEarn + "', '" + Motivation + "',' " + centreInteret + "',' " + Gender + "',' " + Choice + "',' " + Studie + "', NULL, NULL)";
                    try
                    {
                        //conn.Insert(insert);
                    }
                    finally
                    {

                    }
                    // Tell the user their profile is complete
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("Votre Reponse est enregistré\n\n");

                    // Create final reply
                    Activity replyMessage = activity.CreateReply(sb.ToString());
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}