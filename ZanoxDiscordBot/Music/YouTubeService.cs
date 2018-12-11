using Google.Apis.Services;

namespace ZanoxDiscordBot.Music
{
    internal class YouTubeService
    {
        private BaseClientService.Initializer initializer;

        public YouTubeService(BaseClientService.Initializer initializer)
        {
            this.initializer = initializer;
        }
    }
}