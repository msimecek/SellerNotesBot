using Microsoft.Bot.Builder.FormFlow;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace SellerNotesBot.Forms
{
    public enum CommChannel
    {
        [Describe("Osobně")]
        [Terms("osobne", "osobně", "o")]
        InPerson = 1,
        [Describe("Telefonicky")]
        [Terms("telefonicky", "tel", "t")]
        Phone,
        [Describe("E-mailem")]
        [Terms("e-mailem", "email", "e")]
        Email
    }

    [Serializable]
    public class ContactForm
    {
        [Prompt("Konala se schůzka dnes? {||}")]
        [Describe("Schůzka dnes")]
        public bool MeetingToday { get; set; }

        [Prompt("Který den proběhla schůzka? Zadejte den v týdnu nebo konkrétní datum:")]
        [Describe("Datum")]
        public string Date { get; set; }

        [Prompt("Vyberte prosím komunikační kanál: {||}")]
        [Describe("Komunikační kanál")]
        public CommChannel CommunicationChannel { get; set; }

        [Prompt("Zadejte jméno kontaktované osoby:")]
        [Describe("Jméno osoby")]
        public string PersonName { get; set; }

        [Prompt("Zapište podrobnosti kontaktní schůzky:")]
        [Describe("Podrobnosti")]
        public string Details { get; set; }
        public int CustomerId { get; set; }
        public long UtcDateTicks { get; private set; }

        public static IFormDialog<ContactForm> BuildFormDialog()
        {
            return FormDialog.FromForm(() =>
            {
                return new FormBuilder<ContactForm>()
                    .Field(nameof(MeetingToday))
                    .Field(nameof(Date),
                        active: (formState) => {
                            if (formState.MeetingToday)
                            {
                                var utcNow = DateTime.UtcNow;
                                formState.Date = utcNow.ToString();
                                formState.UtcDateTicks = utcNow.Ticks;
                                return false; // nechceme zobrazit otázku na datum
                            }

                            return true;
                        },
                        validate: async (formState, value) => {
                            ValidateResult result = new ValidateResult();

                            DateTime parsedDate;
                            bool parsingResult = DateTime.TryParse(value.ToString(), out parsedDate);
                            if (parsingResult)
                            {
                                result.IsValid = true;
                                result.Value = parsedDate.ToString();
                                formState.UtcDateTicks = parsedDate.ToUniversalTime().Ticks;
                            }
                            else
                            {
                                var keywordResult = Utils.CzechDayToDate(value.ToString());
                                if (keywordResult != null)
                                {
                                    result.IsValid = true;
                                    result.Feedback = "Používám datum " + keywordResult.Value.ToShortDateString();
                                    result.Value = keywordResult.ToString();
                                    formState.UtcDateTicks = keywordResult.Value.ToUniversalTime().Ticks;
                                }
                                else
                                {
                                    result.IsValid = false;
                                    result.Feedback = "Formát data není správný. Zadejte přímo datum (např. 3. 11. 2016) nebo den v týdnu (např. pondělí) nebo dnes/včera.";
                                }
                            }

                            return await Task.FromResult(result);
                        })
                    .Field(nameof(CommunicationChannel))
                    .Field(nameof(PersonName))
                    .Field(nameof(Details))
                    .OnCompletion(FormCompleted)
                    .Build();
            }, FormOptions.PromptInStart);
        }

        private async static Task FormCompleted(IDialogContext context, ContactForm state)
        {
            context.Done(state);
        }
    }
}