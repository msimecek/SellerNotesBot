using Microsoft.Bot.Builder.Dialogs;
using System;

namespace SellerNotesBot
{
    public static class ContextExtensions
    {
        public static string GetAccessToken(this IDialogContext context)
        {
            string result;
            if (context.UserData.TryGetValue("UserToken", out result))
            {
                return result; // TODO: renew token
            }

            return String.Empty;
        }
    }
}