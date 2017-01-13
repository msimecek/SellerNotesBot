using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SellerNotesBot.Services;

namespace SellerNotesBot.Dialogs
{
    [Serializable]
    public class LoginDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var token = context.GetAccessToken();
            if (String.IsNullOrEmpty(token))
            {
                // login prompt
                string loginUrl = LoginService.GenerateLoginUrl();
                PromptDialog.Text(context, AfterLoginCodeEntered, $"Pro pokračování je nutné se přihlásit. Přejděte prosím do systému přes [tento odkaz]({loginUrl}) a přihlaste se. Následně prosím zadejte kód z webu.");
            }
            else
            {
                // přihlášeno, pokračovat
                context.Done(true);
            }

        }

        private async Task AfterLoginCodeEntered(IDialogContext context, IAwaitable<string> result)
        {
            var code = await result;
            //TODO: ověření kódu

            var loginService = new LoginService();
            var loginResult = await loginService.LoginAsync("a", "a");

            context.UserData.SetValue("UserToken", loginResult.id_token);

            await context.PostAsync("Díky, vše je v pořádku a můžete pokračovat.");
            context.Done(true);
        }
    }
}