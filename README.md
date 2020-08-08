<p align="center">
  <img src="https://i.imgur.com/mEaRNpV.png" alt="Icon">
</p>

# Hiromi
A Discord bot.

|**Prefix**: -|**Lead:** [@Hamsterland](https://github.com/Hamsterland)|**Lang:** C#|**Library:** [Discord.Net](https://github.com/discord-net/Discord.Net)|**Runtime**: .NET Core 3.1
|---|---|---|---|---|
## Features
* Tags
* Auto-quote 
* Logging (WIP)
* Reminders (WIP)

## Roadmap
* Complete WIP (lol)
* Upgrade to .NET 5 Preview

## Repository
All contributions are welcome. Please don't feel the need to discuss, just make whatever contributions you want. However, do be aware of Hiromi's basic architecture.

### Hiromi.Data
Database models, data transfer objects (DTOs), and migrations belong here. There should be no business logic and no repositories. EF Core is a repository itself and using somewhat repetitive or similar EF Core queries should not be frowned upon.

### Hiromi.Services
All reusable business logic belongs here. There should ideally be as few references to Discord.NET's API as possible. For example, take a `ulong` user Id as a parameter rather than an `IGuildUser`. 

#### Creating a public API
* Methods with `Task` or `Task<T>` return types must have `Async` appended to their name.
* If you are creating a new feature, it should go in its own folder.
* Service logic should belong in one class, which implements an interface (i.e. `TagService : ITagService`).
* Logic that does not require any injected dependencies should be in a static utilities class (i.e. `TagUtilities`).
* Logic that renders data into a view for Discord (such as an embed) should be in a static views class (i.e. `TagViews`).

### Hiromi.Bot
The part that users interfact with. This contains commands that usually rely on injected services, attributes, type readers, and so on.