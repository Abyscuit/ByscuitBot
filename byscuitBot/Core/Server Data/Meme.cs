using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace byscuitBot.Core.Server_Data
{
    public class Meme
    {
        public string path;
        public string name;
        public string[] tags;
    }

    public class MemeLoader
    {
        public static List<Meme> memes = new List<Meme>();
        private static string memeFile = "Resources/memes.json";

        public MemeLoader()
        {

        }

        public static void LoadMemes()
        {
            if (DataStorage.SaveExists(memeFile))
            {
                memes = DataStorage.LoadMemes(memeFile).ToList();
            }
            else
            {
                memes = new List<Meme>();
                SaveMemes();
            }
            if (!scanned)
                ScanFiles();
        }

        public static void SaveMemes()
        {
            DataStorage.SaveMemes(memes, memeFile);
        }

        public static Meme GetMeme(string path, string name, string[] tags)
        {
            return GetOrCreateMeme(path, name, tags);
        }

        private static Meme GetOrCreateMeme(string path, string name, string[] tags)
        {
            IEnumerable<Meme> result = from a in memes
                                               where a.path == path
                                               select a;

            Meme meme = result.FirstOrDefault();
            if (meme == null) meme = CreateMeme(path, name, tags);

            //return account
            return meme;
        }

        private static Meme CreateMeme(string path, string name, string[] tags)
        {
            Meme meme = new Meme()
            { 
                path = path,
                tags = tags,
                name = name.ToLower()
            };

            memes.Add(meme);
            SaveMemes();
            return meme;
        }

        public static void UpdateMeme(string path, Meme meme)
        {
            Meme delme = GetOrCreateMeme(path, "", null);
            memes.Remove(delme);
            memes.Add(meme);
            SaveMemes();
        }
        public static bool scanned = false;
        public static void ScanFiles()
        {
            DirectoryInfo d = new DirectoryInfo(@"Memes\");//Assuming Test is your Folder
            Console.WriteLine("Scanning Directory: " + d.FullName);
            FileInfo[] Files = d.GetFiles(); //Getting Text files
            Console.WriteLine("Found Files: " + Files.Length);
            int fileMax = Files.Length;
            string[] filePath = new string[fileMax];
            for (int i = 0; i < fileMax; i++)
            {
                filePath[i] = Files[i].Name;
                string name = filePath[i].Remove(filePath[i].Length - 4, 4);
                //Console.WriteLine(name);
                Meme meme = GetMeme(Files[i].Name, name, null);
            }
            scanned = true;
        }

        public static List<Meme> SearchMeme(string[] tags)
        {
            List<Meme> memeList = new List<Meme>();
            foreach(Meme meme in memes)
            {
                foreach(string tag in tags)
                {
                    if (meme.tags != null)
                    {
                        if (meme.tags.Contains(tag.ToLower()) || meme.name.ToLower().Contains(tag.ToLower()))
                        {
                            memeList.Add(meme);
                            break;
                        }
                    }
                }
            }
            return memeList;
        }
        
        public static Meme SelectMeme(string name, List<Meme> list = null)
        {
            if (list == null)
                list = SearchMeme(new string[] { name });

            IEnumerable<Meme> result = from a in list
                                       where a.name == name
                                       select a;

            Meme meme = result.FirstOrDefault();
            return meme;
        }
    }
}
