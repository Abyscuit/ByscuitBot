using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using unirest_net.http;
using System.Drawing.Imaging;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace byscuitBot.Core
{
    public class MemeGenerator
    {

        public static string CreateMeme(string url, string topTxt, string botTxt)
        {
            string file = SaveFile(url);
            Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(file);
            int offset = 5;
            float large = 150;
            int fontSize = (int)large;
            int minSize = 45;
            float width = 1024;
            float height = 1024;
            float scale = Math.Min(width / bitmap.Width, height / bitmap.Height);
            var brush = new SolidBrush(System.Drawing.Color.Black);
            var rImg = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(rImg);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(bitmap.Width * scale);
            var scaleHeight = (int)(bitmap.Height * scale);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(bitmap, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
            Font memeFont = new Font("Impact", fontSize, FontStyle.Bold, GraphicsUnit.Point);
            Font memeFontBot = new Font("Impact", fontSize, FontStyle.Bold, GraphicsUnit.Point);
            SizeF topSize = graph.MeasureString(topTxt, memeFont);
            while(topSize.Width >= width - 40)
            {
                if (fontSize > minSize)
                    fontSize--;
                else
                {
                    string[] words = topTxt.Split(' ');
                    topTxt = "";
                    bool split = false;
                    for(int i =0;i<words.Length;i++)
                    {
                        topTxt += words[i] + " ";
                        if(i > words.Length / 2 && !split)
                        {
                            topTxt += Environment.NewLine;
                            split = true;
                        }
                    }
                    fontSize = (int)large;
                }
                memeFont = new Font("Impact", fontSize, FontStyle.Bold, GraphicsUnit.Point);
                topSize = graph.MeasureString(topTxt, memeFont);
            }
            float topTxtCenter = topSize.Width / 2;
            SizeF botSize = graph.MeasureString(botTxt, memeFontBot);
            fontSize = (int)large;
            while (botSize.Width >= width - 40)
            {
                if (fontSize > minSize)
                    fontSize--;
                else
                {
                    string[] words = botTxt.Split(' ');
                    botTxt = "";
                    bool split = false;
                    for (int i = 0; i < words.Length; i++)
                    {
                        botTxt += words[i] + " ";
                        if (i > words.Length / 2 && !split)
                        {
                            botTxt += Environment.NewLine;
                            split = true;
                        }
                    }
                    fontSize = (int)large;
                }
                memeFontBot = new Font("Impact", fontSize, FontStyle.Bold, GraphicsUnit.Point);
                botSize = graph.MeasureString(botTxt, memeFontBot);
            }
            float botTxtCenter = botSize.Width / 2;
            float topX = width / 2 - topTxtCenter;              //X Value for top text
            float botX = width / 2 - botTxtCenter;              //X Value for bottom text
            float padding = 10;
            float botY = height - botSize.Height - padding;
            RectangleF TopSize = new RectangleF(topX, padding, topSize.Width, topSize.Height);
            RectangleF OutlineTopSize = new RectangleF(topX - offset, padding, topSize.Width, topSize.Height);
            RectangleF Outline2TopSize = new RectangleF(topX + offset, padding, topSize.Width, topSize.Height);
            RectangleF Outline3TopSize = new RectangleF(topX, padding + offset, topSize.Width, topSize.Height);
            RectangleF Outline4TopSize = new RectangleF(topX, padding - offset, topSize.Width, topSize.Height);
            RectangleF BotSize = new RectangleF(botX, botY, botSize.Width, botSize.Height);
            RectangleF OutlineBotSize = new RectangleF(botX - offset, botY, botSize.Width, botSize.Height);
            RectangleF Outline2BotSize = new RectangleF(botX + offset, botY, botSize.Width, botSize.Height);
            RectangleF Outline3BotSize = new RectangleF(botX, botY - offset, botSize.Width, botSize.Height);
            RectangleF Outline4BotSize = new RectangleF(botX, botY + offset, botSize.Width, botSize.Height);

            graph.DrawString(topTxt, memeFont, Brushes.Black, OutlineTopSize);
            graph.DrawString(topTxt, memeFont, Brushes.Black, Outline2TopSize);
            graph.DrawString(topTxt, memeFont, Brushes.Black, Outline3TopSize);
            graph.DrawString(topTxt, memeFont, Brushes.Black, Outline4TopSize);
            graph.DrawString(botTxt, memeFontBot, Brushes.Black, OutlineBotSize);
            graph.DrawString(botTxt, memeFontBot, Brushes.Black, Outline2BotSize);
            graph.DrawString(botTxt, memeFontBot, Brushes.Black, Outline3BotSize);
            graph.DrawString(botTxt, memeFontBot, Brushes.Black, Outline4BotSize);
            graph.DrawString(topTxt, memeFont, Brushes.White, TopSize);
            graph.DrawString(botTxt, memeFontBot, Brushes.White, BotSize);
        
            
            string fileExt = file.Remove(0,file.Length - 4);
            var format = System.Drawing.Imaging.ImageFormat.Jpeg;
            switch(fileExt)
            {
                case ".png":
                    {
                        format = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    }
                case ".gif":
                    {
                        format = System.Drawing.Imaging.ImageFormat.Gif;
                        break;
                    }
                case ".bmp":
                    {
                        format = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    }
            }
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                rImg.Save(ms, format);
                
                data = ms.ToArray();
                File.WriteAllBytes("1"+file, data);
            }
            bitmap.Dispose();
            rImg.Dispose();
            File.Delete(file);
            return "1"+file;
        }

        public class Response
        {
            public string status;
            public string name;
        }

        public static string SaveFile(string url)
        {
            string file = url.Split('/')[url.Split('/').Count() - 1];
            if (file.Contains("?size=1024"))
                file = file.Replace("?size=1024", "");
            DownloadImage(file, url);
            return file;
        }

        public static void DownloadImage(string fileName, string fileUrl)
        {
            Byte[] bytes = new WebClient().DownloadData(fileUrl);
            File.WriteAllBytes(fileName, bytes);
        }
        
    }
}
