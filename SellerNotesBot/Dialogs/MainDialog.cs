using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using SellerNotesBot.Forms;
using SellerNotesBot.Model;
using SellerNotesBot.Services;

namespace SellerNotesBot.Dialogs
{
    [Serializable]
    public class MainDialog : IDialog<object>
    {

        private Customer selectedCustomer = null;
        private ContactForm filledForm = null;

        public async Task StartAsync(IDialogContext context)
        {
            var contactDialog = CreateChain();
            context.Call(contactDialog, AfterContact);
        }

        private IDialog<bool> CreateChain()
        {
            // 1. kontrola přihlášení -> login dialog
            // 2. vyhledání kontaktu -> search dialog
            // 3. vyplnění formuláře -> contact form
            // 4. kontrola formuláře
            // 5. uložení

            return Chain.From(() => new LoginDialog())
                .ContinueWith<bool, Customer>(async (ctx, loginResult) =>
                {
                    return await Task.FromResult(new SearchDialog());
                })
                .While(partner => new SearchTestDialog(partner), partner => new SearchDialog())
                .ContinueWith<Customer, ContactForm>(async (ctx, partner) =>
                {
                    selectedCustomer = await partner;
                    await ctx.PostAsync($"Pracujete s partnerem:\n - {selectedCustomer.Name}\n - DIČ: {selectedCustomer.VAT}\n - Kód: {selectedCustomer.Code}");

                    var formDialog = ContactForm.BuildFormDialog();
                    return formDialog;
                })
                .ContinueWith<ContactForm, bool>(async (ctx, form) =>
                {
                    try
                    {
                        filledForm = await form;
                    }
                    catch (FormCanceledException<ContactForm> ex)
                    {
                        return Chain.Return(false); //TODO: uživatel zrušil form - uložit aktuální stav a smysluplně navázat
                    }

                    return Chain.Return(true); //TODO: krok schválení uživatelem
                });
        }

        private async Task AfterContact(IDialogContext context, IAwaitable<bool> result)
        {
            var res = await result;

            filledForm.CustomerId = selectedCustomer.Id;
            var token = context.GetAccessToken();
            var cs = new ContactService(token);
            var saveResult = await cs.SaveContactAsync(filledForm);
            if (!saveResult.IsSuccessStatusCode || saveResult.HasErrors)
            {
                await context.PostAsync("Došlo k problému při uložení");
            }
            await context.PostAsync("Kontakt byl v pořádku uložen.\n Díky za spolupráci.");
            context.Done(res);
        }
    }
}