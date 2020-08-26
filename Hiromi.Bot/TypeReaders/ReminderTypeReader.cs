using System;
using System.Threading.Tasks;
using Discord.Commands;
using Hiromi.Services.Reminders;
using Microsoft.Extensions.DependencyInjection;

namespace Hiromi.Bot.TypeReaders
{
    public class ReminderTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var reminderService = services.GetService<IReminderService>();

            if (int.TryParse(input, out var id))
            {
                var reminder = await reminderService.GetActiveReminder(context.User.Id, id);

                if (reminder is null)
                {
                    await context.Channel.SendMessageAsync($"{context.User.Mention} You do not have an active reminder with Id {id}.");
                    return TypeReaderResult.FromError(CommandError.ObjectNotFound, $"No Reminder found with Id {id}.");
                }
                
                return TypeReaderResult.FromSuccess(reminder);
            }

            await context.Channel.SendMessageAsync($"{context.User.Mention} \"{input}\" is not a valid Id.");
            return TypeReaderResult.FromError(CommandError.ParseFailed, $"Invalid Id \"{input}\"");
        }
    }
}