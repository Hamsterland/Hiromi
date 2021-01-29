using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace Hiromi.Services.Monkey
{
    public class MonkeyService : IMonkeyService
    {
        private readonly Random _random = new Random();
        
        public string DrawMonkey(string name)
        {
            var random = _random.Next(1, 5);
            var imageFilePath = $"{Directory.GetCurrentDirectory()}/MonkeyImages/m{random}.png";
            var bitmap = (Bitmap) Image.FromFile(imageFilePath);

            PointF firstLocation = new PointF(bitmap.Width / 2f, bitmap.Height / 5f);
            PointF secondLocation = new PointF(bitmap.Width / 2f, bitmap.Height - bitmap.Height / 5f);
            
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font arialFont =  new Font("Arial", 40))
                {
                    var sf = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center, 
                        Alignment = StringAlignment.Center
                    };
                    
                    graphics.DrawString("WHERE", arialFont, Brushes.White, firstLocation, sf);
                    graphics.DrawString(name.ToUpper(), arialFont, Brushes.White, secondLocation, sf);
                }
            }

            var savePath = $"{Directory.GetCurrentDirectory()}/MonkeyImages/Result.png";
            bitmap.Save(savePath);
            return savePath;
        }
    }
}