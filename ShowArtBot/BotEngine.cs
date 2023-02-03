using ShowArtBot.Hermitage;
using ShowArtBot.MetApi;
using ShowArtBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static ShowArtBot.MetApi.ShowMetAPI;
using File = System.IO.File;


namespace ShowArtBot
{
    public class BotEngine
    {
        private readonly TelegramBotClient _botClient;
        private static IMetApi? _metApi;
        private List<ArtObject> _hermArts;

        public BotEngine(TelegramBotClient botClient, IMetApi metApi, List<ArtObject> hermArts)
        {
            _botClient = botClient;
            _metApi = metApi;
            _hermArts = hermArts;
        }



        public async Task ListenForMassageAsync()
        {

            while (true)
            {
                using var cts = new CancellationTokenSource();
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                };

                try
                {
                    
                 

                    _botClient.StartReceiving(
                        updateHandler: HandleUpdateAsync,
                        pollingErrorHandler: HandlePollingErrorAsync,
                        receiverOptions,
                        cancellationToken: cts.Token
                        );
                    var me = await _botClient.GetMeAsync();

                    Console.WriteLine($"Start listening for @{me.Username}");
                    Console.ReadLine();
                    
                    
                   
                }
                catch (Exception)
                {

                   
                }

                cts.Cancel();
                while (!cts.Token.IsCancellationRequested)
                {
                    Thread.Sleep(10000);
                } ;
            }




        }


        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
            {
                return;
            }

            if (message.Text is not { } messageText)
            {
                return;
            }
            Console.WriteLine($"{DateTime.Now} : Received a '{messageText}' message in chat {message.Chat.Id}.");



            switch (message.Text.ToLower())
            {
                case "/start":
                    await botClient.SendTextMessageAsync(message.Chat, $"Привет {message.From.FirstName}!  ");
                    await botClient.SendTextMessageAsync(message.Chat, $"Я умею показывать картины и предметы исскуства!");
                    await botClient.SendTextMessageAsync(message.Chat, $"Eсли хочешь увидеть картины из Эрмитажа - напиши Эрм");
                    await botClient.SendTextMessageAsync(message.Chat, $"Для того что б увидеть предмет исскусства из \"The Metropolitan Museum of Art\" напиши Метр ");
                    await botClient.SendTextMessageAsync(message.Chat, $"Или воспользуйся командами в МЕНЮ");
                    break;
                case "старт":
                    goto case "/start";
                case "!randommet":
                    try
                    {
                        var randomCollectionItem = await RandomImageRequestAsync();
                        await SendPhotoMetAsync(botClient, message, randomCollectionItem, cancellationToken);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Ошибка в меню switch !randommet ");
                        break;
                    }

                    break;
                case "showmet":
                    goto case "!randommet";
                case "/met":
                    goto case "!randommet";
                case "метр!":
                    goto case "!randommet";
                case "метр":
                    goto case "!randommet";
                case "/show":
                    goto case "showherm";
                case "showherm":
                    await SendPhotoHermitageAsync(botClient, message, cancellationToken, _hermArts);
                    break;
                case "randomherm":
                    goto case "showherm";
                case "эрмитаж":
                    goto case "showherm";
                case "эрм":
                    goto case "showherm";
                case "/hermitage":
                    goto case "showherm";
                default:
                    await botClient.SendTextMessageAsync(message.Chat, "Что то пошло не так! Чтобы узнать что я могу  - напиши  СТАРТ!  ");
                    break;

            }

            //if (message.Text == "!random")
            //{
            //    var randomCollectionItem = await RandomImageRequestAsync();

            //    await SendPhotoMessageAsync(botClient, message, randomCollectionItem, cancellationToken);
            //}

            //if (message.Text.Contains("!search"))
            //{
            //    var collectionItem = await SearchImageRequestAsync(message);

            //    if (!string.IsNullOrEmpty(collectionItem.primaryImage))
            //    {
            //        await SendPhotoMessageAsync(botClient, message, collectionItem, cancellationToken);
            //    }
            //}
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };


            Console.WriteLine(ErrorMessage);
            //throw new Exception(ErrorMessage);
            return Task.CompletedTask;



        }
        private static async Task SendPhotoMetAsync(ITelegramBotClient botClient, Message message, CollectionItem collectionItem, CancellationToken cancellationToken)
        {
            try
            {
                Message sendArtworkMet = await botClient.SendPhotoAsync(
                     chatId: message.Chat.Id,
                     photo: collectionItem.primaryImage,
                     caption: "<b>" + collectionItem.artistDisplayName + "</b>" + " <i>Artwork</i>: " + collectionItem.title,
                     parseMode: ParseMode.Html,
                     cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

                Console.WriteLine("Ошибка в SendPhotoMetAsync");
            }


        }

        private static async Task SendPhotoHermitageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, List<ArtObject> hermArts)
        {
            string Path = "G:\\HermitageBot\\TempDownlods\\";
            var rnd = new Random();
            ArtObject ranomArt = hermArts[rnd.Next(hermArts.Count)];

            Message sendArtworkHerm = await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: File.Open(string.Concat(Path, ranomArt.ImageName), FileMode.Open),
                caption: "Название работы: " + "<b>" + ranomArt.ArtName + "</b>" + "<i> Автор : </i> " + "<b>" + ranomArt.ArtDiscrAuthor + "</b>" + "   Регион:  " + "<b>" + ranomArt.ArtDiscrFrom + "</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }

        private static async Task<CollectionItem> SearchImageRequestAsync(Message message)
        {
            string[] s = message.Text.Split(" ");

            var searchList = await _metApi.SearchCollectionAsync(s[1]);

            var collectionObject = HelperMethods.RandomNumberFromList(searchList.objectIDs);

            var collectionItem = await _metApi.GetCollectionItemAsync(collectionObject.ToString());

            return collectionItem;
        }

        // Returns a random artwork from the entire collection
        private static async Task<CollectionItem> RandomImageRequestAsync()
        {
            var objectList = await _metApi.GetCollectionObjectsAsync();

            // Keep getting new items from the collection until we find one with an image
            var validImage = false;

            while (!validImage)
            {
                var collectionObject = HelperMethods.RandomNumberFromList(objectList.objectIDs);

                var collectionItem = await _metApi.GetCollectionItemAsync(collectionObject.ToString());

                if (!string.IsNullOrEmpty(collectionItem.primaryImage))
                {
                    validImage = true;

                    return collectionItem;
                }
            }

            // Probably not the best way to handle this, will need to change it at some point.
            throw new Exception("Error: Can't get random image");
        }





    }
}
