using ShowArtBot;
using ShowArtBot.Hermitage;
using System.Diagnostics;
using Telegram.Bot;
using static ShowArtBot.MetApi.ShowMetAPI;





    var botClient = new TelegramBotClient(AccesToken.Value);
    var metApi = new MetApi();
    ArtGalleryContext ArtDB = new ArtGalleryContext();

    var Arts = ArtDB.ArtObjects.ToList();
    var ArtBot = new BotEngine(botClient, metApi, Arts);


    await ArtBot.ListenForMassageAsync();



    
    

        








