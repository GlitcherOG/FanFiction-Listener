using CefSharp;
using CefSharp.OffScreen;
using Discord;
using Discord.Webhook;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    static class Program
    {
        //Todo
        //Make it so when clicking on notifcation it opens the webpage
        //Fix so it always loads the first page then make it so you can add on any page
        //Add FFN

        static NoficationIcon Icon;
        static ChromiumWebBrowser chromiumWebBrowser1;
        //Settings
        public static string Webhook = "";
        public static int waittime = 15;
        public static bool discordWebhook = false;
        public static bool pageCheck = true;
        public static SettingsSaveLoad Saving = new SettingsSaveLoad();
        public static TrackingSavingAndLoading SavingTrack = new TrackingSavingAndLoading();
        static DiscordWebhookClient client;
        public static string[] datasplit;
        public static List<string> Test= new List<string>();
        public static List<Works> AllWorks = new List<Works>();
        public static List<Tracker> Track = new List<Tracker>();
        public static bool Checkingworks = false;
        static EmbedBuilder embed = new EmbedBuilder();
        static string Error;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var settings = new CefSettings();
            settings.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Ao3Listen\\CefSharp\\Cache");
            Cef.Initialize(settings);
            chromiumWebBrowser1 = new ChromiumWebBrowser();
            LoadSavedData();
            Icon = new NoficationIcon();
            Application.Run(Icon);
        }
        public static void LoadSavedData()
        {
            Saving = SettingsSaveLoad.Load();
            SavingTrack = TrackingSavingAndLoading.Load();
            if (SavingTrack != null)
            {
                Track = SavingTrack.Track;
            }
            else
            {
                SavingTrack = new TrackingSavingAndLoading();
            }
            if (Saving != null)
            {
                waittime = (int)Saving.waitTimeMin;
                Webhook = Saving.webhook;
                pageCheck = Saving.pageCheck;
                discordWebhook = Saving.discordWebhook;
            }
            else
            {
                Saving = new SettingsSaveLoad();
                Settings Wsettings = new Settings();
                Wsettings.Show();
            }
        }
        public static void StartChecking()
        {
            client = new DiscordWebhookClient(Webhook);
            Task.Run(() => refresh());
        }
        private static async Task refresh()
        {
            while (true)
            {
                try
                {
                    await Startup();
                }
                catch
                {
                    Icon.Notification("Error", "Something has gone wrong. \n" + Error);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                //await Task.Delay(1000 * waittime * 60);
            }
        }

        private static async Task Startup()
        {
            for (int a = 0; a < Track.Count; a++)
            {
                Error = "101";
                Checkingworks = true;
                AllWorks = new List<Works>();
                List<Works> TempWorksHolder = new List<Works>();
                string data = await LoadWebPage(Track[a].Link);
                datasplit = data.Split('\n');
                List<Works> TempWorks = StripAo3WorksSearch(datasplit);
                for (int i = 0; i < TempWorks.Count; i++)
                {
                    AllWorks.Add(StripAo3PageData(TempWorks[i].Raw));
                }
                TempWorks = AllWorks;
                if(Track[a].OldWorks.Count==0)
                {
                    Tracker temp = Track[a];
                    temp.OldWorks = AllWorks;
                    Track[a] = temp;
                }
                Error = "102";
                if (AllWorks[0].ID != Track[a].OldWorks[0].ID || AllWorks[0].lastUpdated != Track[a].OldWorks[0].lastUpdated)
                {
                    int NewStories = 0;
                    int NewChapters = 0;
                    int page = 1;
                    bool test = false;
                    DateTime oldwork = new DateTime();
                    DateTime newwork = new DateTime();
                    string Notifcation = "";
                    while (!test)
                    {
                        for (int i = 0; i < AllWorks.Count; i++)
                        {
                            string[] Datetime0 = AllWorks[i].lastUpdated.Split(' ');
                            newwork = new DateTime(Int32.Parse(Datetime0[2]), ConvertDate(Datetime0[1]), Int32.Parse(Datetime0[0]));
                            for (int b = 0; b < 3; b++)
                            {
                                string[] Datetime = Track[a].OldWorks[b].lastUpdated.Split(' ');
                                oldwork = new DateTime(Int32.Parse(Datetime[2]), ConvertDate(Datetime[1]), Int32.Parse(Datetime[0]));
                                if (AllWorks[i].ID == Track[a].OldWorks[b].ID && AllWorks[i].lastUpdated == Track[a].OldWorks[b].lastUpdated)
                                {
                                    test = true;
                                    break;
                                }
                                else if (oldwork > newwork)
                                {
                                    test = true;
                                    break;
                                }
                            }
                            if (test)
                            {
                                i = AllWorks.Count;
                            }
                            else
                            {
                                Error = "201";
                                embed = new EmbedBuilder();
                                if (AllWorks[i].currentChapters != "1")
                                {
                                    NewChapters++;

                                    embed = new EmbedBuilder
                                    {
                                        Title = "Story Update (" + Track[a].Name + ")",
                                    };
                                }
                                else
                                {
                                    embed = new EmbedBuilder
                                    {
                                        Title = "New Story(" + Track[a].Name + ")",
                                    };
                                    NewStories++;
                                }
                                string temp1 = "";
                                embed.ThumbnailUrl = "https://cdn.discordapp.com/attachments/814311805689528350/890833049762275328/Archive_of_Our_Own_logo.png";
                                embed.Timestamp = DateTimeOffset.Now;
                                if (AllWorks[i].description != "" && AllWorks[i].description.Length < 1000)
                                {
                                    embed.AddField(AllWorks[i].Name, AllWorks[i].description);
                                }
                                else
                                {
                                    embed.AddField(AllWorks[i].Name, "No Description");
                                }
                                for (int e = 0; e < AllWorks[i].fandom.Count; e++)
                                {
                                    temp1 += AllWorks[i].fandom[e] + ", ";
                                }
                                if (temp1 != "" && temp1.Length < 1000)
                                {
                                    embed.AddField("Fandoms", temp1);
                                }
                                temp1 = "";
                                for (int e = 0; e < AllWorks[i].character.Count; e++)
                                {
                                    temp1 += AllWorks[i].character[e] + ", ";
                                }
                                if(temp1!= "" && temp1.Length < 1000)
                                {
                                    embed.AddField("Characters", temp1);
                                }
                                temp1 = "";
                                for (int e = 0; e < AllWorks[i].ships.Count; e++)
                                {
                                    temp1 += AllWorks[i].ships[e] + ", ";
                                }
                                if (temp1 != "" && temp1.Length<1000)
                                {
                                    embed.AddField("Relationships", temp1);
                                }
                                Error = "202";
                                embed.AddField("Last Updated", AllWorks[i].lastUpdated, true);
                                embed.AddField("Length", AllWorks[i].words + " Words , " + AllWorks[i].currentChapters + AllWorks[i].allChapters + " Chapters", true);
                                embed.AddField("Rating : " + AllWorks[i].Rating, AllWorks[i].Orientations, true);
                                embed.AddField("Language", AllWorks[i].language, true);
                                embed.AddField("Link", "[Click Here](https://www.archiveofourown.org/works/" + AllWorks[i].ID + ")", true);
                                embed.Color = Color.DarkRed;
                                if (discordWebhook && Webhook != "")
                                {
                                    try
                                    {
                                        await client.SendMessageAsync("", false, embeds: new[] { embed.Build() }, "Ao3 Listner", "https://cdn.discordapp.com/attachments/814311805689528350/890833049762275328/Archive_of_Our_Own_logo.png");
                                        await Task.Delay(2000);
                                    }
                                    catch
                                    {
                                        Icon.Notification("Error", "Discord Webhook Error", Track[a].Link);
                                    }
                                    //await client.SendMessageAsync("", false, embeds: new[] { embed.Build() }, "Ao3 Listner", "https://cdn.discordapp.com/attachments/814311805689528350/890833049762275328/Archive_of_Our_Own_logo.png");
                                }
                            }
                        }
                        //If there is still works left
                        if (test == false)
                        {
                            Error = "103";
                            AllWorks = new List<Works>();
                            page++;
                            if (Track[a].Link.Contains("https://archiveofourown.org/works"))
                            {
                                data = await LoadWebPage(Track[a].Link + "&page=" + page);
                            }
                            else
                            {
                                data = await LoadWebPage(Track[a].Link + "?page=" + page);
                            }
                            datasplit = data.Split('\n');
                            TempWorks = StripAo3WorksSearch(datasplit);
                            for (int i = 0; i < TempWorks.Count; i++)
                            {
                                AllWorks.Add(StripAo3PageData(TempWorks[i].Raw));
                            }
                        }
                    }
                    if (NewStories != 0)
                    {
                        Notifcation += NewStories + " New Stories.\n";
                    }
                    if (NewChapters != 0)
                    {
                        Notifcation += NewChapters + " New Updates.";
                    }
                    Error = "104";
                    Tracker temp = Track[a];
                    temp.OldWorks = TempWorksHolder;
                    Track[a] = temp;
                    Icon.Notification(Track[a].Name, Notifcation, Track[a].Link);
                }
                await Task.Delay(3000);
            }
            Error = "301";
            SavingTrack.Save();
            Checkingworks = false;
        }

        public static List<Works> StripAo3WorksSearch(string[] datasplit)
        {
            List<Works> RawWorks = new List<Works>();
            bool test = false;
            Works Raw2 = new Works();
            List<string> Test = new List<string>();
            for (int i = 0; i < datasplit.Length; i++)
            {
                if (!test)
                {
                    if (datasplit[i].TrimStart(' ').StartsWith("<li id=\"work_"))
                    {
                        test = true;
                        Test.Add(datasplit[i]);
                    }
                }
                else
                {
                    if (datasplit[i] != "" && datasplit[i] != "  " && datasplit[i] != "        " && datasplit[i] != "      ")
                    {
                        Test.Add(datasplit[i]);
                        if (datasplit[i].StartsWith("</li>"))
                        {
                            test = false;
                            Raw2.Raw = Test;
                            RawWorks.Add(Raw2);
                            Test = new List<string>();
                            Raw2 = new Works();
                        }
                    }
                }
            }
            return RawWorks;
        }
        public static void RemoveAddTracker(string link, bool add)
        {
            Task.Run(() => RemoveAddTrackerA(link,add));
        }

        private static async Task RemoveAddTrackerA(string link, bool add)
        {
            while(Checkingworks)
            {
                await Task.Delay(1000);
            }
            Checkingworks = true;
            if (add)
            {
                Tracker Temp = new Tracker();
                Temp.Link = link;
                string temp;
                temp = await LoadWebPage(link);
                datasplit = temp.Split('\n');
                for (int i = 0; i < datasplit.Length; i++)
                {
                    if (datasplit[i].Contains("<title>"))
                    {
                        Temp.Name = datasplit[i + 1].TrimStart(' ');
                    }
                }
                List<Works> TempWorks = StripAo3WorksSearch(datasplit);
                for (int i = 0; i < TempWorks.Count; i++)
                {
                    TempWorks[i] = StripAo3PageData(TempWorks[i].Raw);
                }
                Temp.OldWorks = TempWorks;
                Temp.Website = "AO3";
                Track.Add(Temp);
            }
            else
            {
                for (int i = 0; i < Track.Count; i++)
                {
                    if (Track[i].Link == link)
                    {
                        Track.RemoveAt(i);
                    }
                }
            }
            SavingTrack.Save();
            Checkingworks = false;
        }

        private static async Task<string> LoadWebPage(string uri, bool wait = false)
        {
            chromiumWebBrowser1.Load(uri);
            await Task.Delay(500);
            while (chromiumWebBrowser1.IsLoading)
            {
                await Task.Delay(500);
            }
            string temp;
            Task<string> task = chromiumWebBrowser1.GetSourceAsync();
            while (task == null && task.Result == "")
            {
                task = Task.Run(() => chromiumWebBrowser1.GetSourceAsync());
            }
            temp = task.Result;
            return temp;
        }

        public static int ConvertDate(string date)
        {
            switch (date)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "July":
                    return 7;
                case "Aug":
                    return 8;
                case "Oct":
                    return 9;
                case "Sep":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return 0; 
            }
        }

        public static Works StripAo3PageData(List<string> HTML)
        {
            Works Work = new Works();
            string[] Split = new string[] { };
            Work.Raw = HTML;
            for (int i = 0; i < HTML.Count; i++)
            {
                //ID
                if (HTML[i].Contains("<li id=\"work_"))
                {
                    Split = HTML[i].Split('_', '"');
                    Work.ID = Split[2];
                }
                //Name
                if (HTML[i].Contains("<a href=\"/works") && HTML[i - 1].Contains("heading"))
                {
                    Split = HTML[i].Split('>', '<');
                    Work.Name = Split[2];
                }
                //Author
                if (HTML[i].Contains("<a rel=\"author\""))
                {
                    Split = HTML[i].Split('>', '<');
                    Work.author = Split[2];
                }
                //Rating
                if (HTML[i].Contains("<a class=\"help symbol question modal modal-attached\" title=\"Symbols key\" aria-controls=\"#modal\" href=\"/help/symbols-key.html\"><span class=\"rating-"))
                {
                    Split = HTML[i].Split('>', '<');
                    Work.Rating = Split[8];
                }
                //Orenations
                if (HTML[i].Contains("<a class=\"help symbol question modal modal-attached\" title=\"Symbols key\" aria-controls=\"#modal\" href=\"/help/symbols-key.html\"><span class=\"category-"))
                {
                    Split = HTML[i].Split('>', '<');
                    Work.Orientations = Split[8];
                }
                //Complete
                if (HTML[i].Contains("<a class=\"help symbol question modal modal-attached\" title=\"Symbols key\" aria-controls=\"#modal\" href=\"/help/symbols-key.html\"><span class=\"complete-"))
                {
                    Split = HTML[i].Split('>', '<');
                    Work.Finished = Split[8];
                }
                //Last Updated
                if (HTML[i].Contains("<p class=\"datetime\">"))
                {
                    Split = HTML[i].Split('>', '<');
                    Work.lastUpdated = Split[2];
                }
                if(HTML[i].Contains("<blockquote class=\"userstuff summary\">"))
                {
                    Work.description = HTML[i + 1].TrimStart().TrimStart("<p>".ToCharArray()).TrimEnd("</p>".ToCharArray()).Replace("<p>", "\n").Replace("</p>", "");
                    Work.description = Work.description.Replace("<br>", "\n");
                }
                //Language
                if (HTML[i].Contains("<dt class=\"language\">Language:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.language = Split[2];
                }
                //Words
                if (HTML[i].Contains("<dt class=\"words\">Words:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.words = Split[2];
                }
                //Chapters 
                if (HTML[i].Contains("<dt class=\"chapters\">Chapters:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.currentChapters = Split[4];
                    if (Split.Length > 5)
                    {
                        Work.allChapters = Split[6];
                    }
                }
                //Collections
                if (HTML[i].Contains("<dt class=\"collections\">Collections:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.collections = Split[4];
                }
                //Comments
                if (HTML[i].Contains("<dt class=\"comments\">Comments:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.comments = Split[4];
                }
                //Kudos
                if (HTML[i].Contains("<dt class=\"kudos\">Kudos:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.kudos = Split[4];
                }
                //Bookmarks
                if (HTML[i].Contains("<dt class=\"bookmarks\">Bookmarks:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.bookmarks = Split[4];
                }
                //Hits
                if (HTML[i].Contains("<dt class=\"hits\">Hits:</dt>"))
                {
                    Split = HTML[i + 1].Split('>', '<');
                    Work.hits = Split[2];
                }

                //Fandoms
                if (HTML[i].Contains("<a class=\"tag\"") && HTML[i - 1].Contains("Fandoms:"))
                {
                    List<string> Fandoms = new List<string>();
                    Split = HTML[i].Split('>', '<');
                    for (int a = 2; a < Split.Length; a++)
                    {
                        if (Split[a] != ", " && Split[a] != "/a" && !Split[a].Contains("a class=\"tag\"") && Split[a] != "")
                        {
                            Fandoms.Add(Split[a]);
                        }
                    }
                    Fandoms.TrimExcess();
                    Work.fandom = Fandoms;
                }
                //Content Warning, Ships, Tags, Characters
                if (HTML[i].Contains("<li class=\"warnings\">"))
                {
                    List<string> FullTags = new List<string>();
                    List<string> Ships = new List<string>();
                    List<string> Characters = new List<string>();
                    List<string> Tags = new List<string>();
                    List<string> ContentWarings = new List<string>();
                    Split = HTML[i].Split('>', '<');
                    for (int a = 0; a < Split.Length; a++)
                    {
                        if (Split[a] != "")
                        {
                            FullTags.Add(Split[a]);
                        }
                    }

                    for (int a = 0; a < FullTags.Count; a++)
                    {
                        //Warnings
                        if (FullTags[a].Contains("warnings"))
                        {
                            ContentWarings.Add(FullTags[a + 3]);
                        }
                        //Ships
                        if (FullTags[a].Contains("relationship"))
                        {
                            Ships.Add(FullTags[a + 2]);
                        }
                        //Characters
                        if (FullTags[a].Contains("character"))
                        {
                            Characters.Add(FullTags[a + 2].Replace("&amp;", "&"));
                        }
                        //Tags
                        if (FullTags[a].Contains("freeforms"))
                        {
                            Tags.Add(FullTags[a + 2]);
                        }
                    }
                    Work.ships = Ships;
                    Work.character = Characters;
                    Work.tags = Tags;
                    Work.ContentWarnings = ContentWarings;
                }
            }
            return Work;
        }
    }
}
[Serializable]
public struct Tracker
{
    public string Name;
    public string Link;
    public string Website;
    public List<Works> OldWorks;
}
[Serializable]
public struct Works
{
    public List<string> Raw;
    //Top Half
    public string ID;
    public string Name;
    public string author;
    public string Rating;
    public string Orientations;
    public string Finished;
    public string dateCreated;
    public string lastUpdated;
    public string description;
    //Bottom Half
    public string language;
    public string words;
    public string currentChapters;
    public string allChapters;
    public string collections;
    public string comments;
    public string kudos;
    public string bookmarks;
    public string hits;

    //Tags
    public List<string> fandom;
    public List<string> ContentWarnings;
    public List<string> ships;
    public List<string> character;
    public List<string> tags;
}