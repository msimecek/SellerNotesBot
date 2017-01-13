using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SellerNotesBot.Model;
using SellerNotesBot.Services;
using SellerNotesBot.Models;

namespace SellerNotesBot.Dialogs
{
    [Serializable]
    public class SearchDialog : IDialog<Customer>
    {
        public enum SearchType
        {
            Name, Code, VAT
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Zadejte kód obchodního partnera, jeho DIČ nebo část názvu, ke kterému chcete připojit záznam o kontaktu.");
            context.Wait(MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;

            SearchType type = IdentifySearchType(message.Text);

            var token = context.GetAccessToken();

            var contactService = new ContactService(token);
            var customersResult = await GetCustomers(message, type, contactService);
            if (customersResult.StatusCode == System.Net.HttpStatusCode.Forbidden || customersResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Relogin(context);
                token = context.GetAccessToken();
                contactService = new ContactService(token);
                //zkusime znovu
                customersResult = await GetCustomers(message, type, contactService);
            }

            if (customersResult.IsSuccessStatusCode)
            {
                //varianty: 
                // nenalezeno 
                // nalezen prave jeden - vratim ho
                // nalezeno <= 5 - zobrazit volby
                // nalezeno > 5 - upresnit hledani
                if (customersResult.Customers.Count < 1)
                {
                    await context.PostAsync("Nenalezen žádný partner, prosím zkuste hledat znovu");
                    context.Done((Customer)null);
                }
                else if (customersResult.Customers.Count == 1)
                {
                    context.Done(customersResult.Customers.First());
                }
                else if (customersResult.Customers.Count <= 5)
                {
                    PromptDialog.Choice(context, ResumePrompt, customersResult.Customers,
                        "Nalezeno více partnerů, prosím vyberete jednoho:"
                        /*descriptions: partnersResult.Partners.Select(p=> p.Name)*/,
                        promptStyle: PromptStyle.Auto
                        );
                }
                else
                {
                    await context.PostAsync("Nalezeno příliš mnoho partnerů, upřesněte prosím hledaný výraz");
                    context.Done((Customer)null);
                }
            }
            else
            {
                await context.PostAsync("Nic jsme nenašli nebo se nepodařilo získat seznam partnerů.");
                context.Wait(MessageReceived);
            }
        }

        private static async Task Relogin(IDialogContext context)
        {
            //pravdepodobne vyprsel token
            var login = new LoginService();
            var res = await login.LoginAsync("a", "a");
            context.UserData.SetValue("UserToken", res.id_token);
        }

        private static async Task<GetCustomersResult> GetCustomers(IMessageActivity message, SearchType type, ContactService contactService)
        {
            GetCustomersResult customersResult;
            switch (type)
            {
                case SearchType.Code:
                    customersResult = await contactService.GetCustomersAsync(code: Int32.Parse(message.Text));
                    break;
                case SearchType.VAT:
                    customersResult = await contactService.GetCustomersAsync(vat: message.Text);
                    break;
                case SearchType.Name:
                default:
                    customersResult = await contactService.GetCustomersAsync(namePart: message.Text);
                    break;
            }

            return customersResult;
        }

        private async Task ResumePrompt(IDialogContext context, IAwaitable<Customer> result)
        {
            Customer partner = await result;
            context.Done(partner);
        }

        public static SearchType IdentifySearchType(string input)
        {
            // Základní implementace rozpoznání, jestli vyhledáváme podle jména, id nebo DIČ.
            if (input.StartsWith("CZ"))
            {
                return SearchType.VAT;
            }

            int res;
            if (Int32.TryParse(input, out res))
            {
                return SearchType.Code;
            }

            return SearchType.Name;
        }
    }
    public class SearchTestDialog : IDialog<bool>
    {
        private Customer _partner;
        public SearchTestDialog(Customer partner)
        {
            _partner = partner;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (_partner != null)
            {
                context.Done(false);
            }
            else
            {
                context.Done(true);
            }
            await Task.CompletedTask;
        }
    }
}