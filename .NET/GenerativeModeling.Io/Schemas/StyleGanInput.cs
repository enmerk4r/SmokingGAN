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
        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("inputImage")]
        public string InputImage { get; set; }

        public StyleGanInput() { }
        public StyleGanInput(string style, string inputImage)
        {
            this.Style = style;
            this.InputImage = inputImage;
        }
        public StyleGanInput(string style, Bitmap inputImage)
        {
            ImageConverter converter = new ImageConverter();
            var inputBytes = (byte[])converter.ConvertTo(inputImage, typeof(byte[]));
            this.Style = style;
            this.InputImage = Convert.ToBase64String(inputBytes);
        }
    }
}
