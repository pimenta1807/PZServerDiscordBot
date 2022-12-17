﻿using Discord;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class BotUtility
{
    private static bool announcedEarlyAccessVersion = false;

    public static async Task<Tuple<string, string>> GetLatestBotVersion()
    {
        const string apiURL = "https://api.github.com/repos/egebilecen/PZServerDiscordBot/releases/latest";
     
        string version = null;
        string releaseText = null;
        string result  = await EB_Utility.WebRequest.GetAsync(SteamWebAPI.HttpClient, apiURL);

        if(string.IsNullOrEmpty(result))
            return null;

        try
        {
            JObject jsonObj = JObject.Parse(result);
            version = jsonObj["tag_name"].Value<string>();
            releaseText = jsonObj["body"].Value<string>();    
        }
        catch(Exception ex)
        {
            Logger.LogException(ex, "Error occured during GetLatestBotVersion().");
        }

        return new Tuple<string, string>(version, releaseText);
    }

    public static async Task NotifyLatestBotVersion()
    {
        var commandChannel = DiscordUtility.GetTextChannelById(Application.BotSettings.CommandChannelId);

        if(commandChannel == null)
        {
            Logger.WriteLog("BotUtility.NotifyLatestBotVersion() - commandChannel is null.");
            return;
        }

        Tuple<string, string> lastReleaseResult = await GetLatestBotVersion();
        bool parseResult = SemanticVersion.TryParse(lastReleaseResult.Item1, out SemanticVersion latestBotVersion);

        if(parseResult)
        {
            if(Application.BotVersion < latestBotVersion)
            {
                if(latestBotVersion.Stage == DevelopmentStage.Release)
                {
                    string warningText = string.Format(Localization.Get("info_bot_new_version"), latestBotVersion, Application.BotVersion, Application.BotRepoURL);
                    var lastMessages = await commandChannel.GetMessagesAsync(1).FlattenAsync();

                    if(!lastMessages.First().Content.Equals(warningText))
                    {
                        await commandChannel.SendMessageAsync(warningText);
                        
                        if(!string.IsNullOrEmpty(lastReleaseResult.Item2))
                            await commandChannel.SendMessageAsync($"```\n{lastReleaseResult.Item2}```");
                    }

                    Scheduler.RemoveItem("BotVersionChecker");
                }
                else if(!announcedEarlyAccessVersion)
                {
                    string warningText = string.Format(Localization.Get("info_bot_new_early_version"), latestBotVersion, Application.BotVersion, Application.BotRepoURL);
                    var lastMessages = await commandChannel.GetMessagesAsync(1).FlattenAsync();

                    if(!lastMessages.First().Content.Equals(warningText))
                    {
                        await commandChannel.SendMessageAsync(warningText);
                        
                        if(!string.IsNullOrEmpty(lastReleaseResult.Item2))
                            await commandChannel.SendMessageAsync($"```\n{lastReleaseResult.Item2}```");
                    }

                    announcedEarlyAccessVersion = true;
                }
            }
        }
        else Logger.WriteLog(string.Format("[CheckLatestBotVersion()] Couldn't parse the version string. String: {0}", lastReleaseResult.Item1));
    }

    // Credits: https://www.c-sharpcorner.com/code/2562/c-sharp-code-to-calculate-relative-time.aspx
    public static string GetPastRelativeTimeStr(DateTime currentTime, DateTime passedTime)
    {
        const int SECOND = 1;  
        const int MINUTE = 60 * SECOND;  
        const int HOUR   = 60 * MINUTE;  
        const int DAY    = 24 * HOUR;  
        const int MONTH  = 30 * DAY;  
  
        var ts = new TimeSpan(currentTime.Ticks - passedTime.Ticks);  
        double delta = Math.Abs(ts.TotalSeconds);  
  
        if (delta < 1 * MINUTE)  
          return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";  
  
        if (delta < 2 * MINUTE)  
          return "a minute ago";  
  
        if (delta < 45 * MINUTE)  
          return ts.Minutes + " minutes ago";  
  
        if (delta < 90 * MINUTE)  
          return "an hour ago";  
  
        if (delta < 24 * HOUR)  
          return ts.Hours + " hours ago";  
  
        if (delta < 48 * HOUR)  
          return "yesterday";  
  
        if (delta < 30 * DAY)  
          return ts.Days + " days ago";  
  
        if (delta < 12 * MONTH)  
        {  
          int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));  
          return months <= 1 ? "one month ago" : months + " months ago";  
        }  
        else  
        {  
          int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));  
          return years <= 1 ? "one year ago" : years + " years ago";  
        }  
    }
}
