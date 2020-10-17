using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenerativeModeling.Io
{
    public class StyleGanInput
    {
        [JsonProperty("styleImage")]
        public string StyleImage { get; set; }

        [JsonProperty("inputImage")]
        public string InputImage { get; set; }

        public StyleGanInput() { }
        public StyleGanInput(string styleImage, string inputImage)
        {
            this.StyleImage = styleImage;
            this.InputImage = inputImage;
        }
        public StyleGanInput(Bitmap styleImage, Bitmap inputImage)
        {
            ImageConverter converter = new ImageConverter();
            var inputBytes = (byte[])converter.ConvertTo(inputImage, typeof(byte[]));
            var styleBytes = (byte[])converter.ConvertTo(styleImage, typeof(byte[]));
            this.StyleImage = Convert.ToBase64String(styleBytes);
            this.InputImage = Convert.ToBase64String(inputBytes);
        }
    }
}
