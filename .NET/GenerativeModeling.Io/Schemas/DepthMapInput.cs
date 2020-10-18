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
    public class DepthMapInput
    {

        [JsonProperty("inputImage")]
        public string InputImage { get; set; }

        public DepthMapInput() { }
        public DepthMapInput(string inputImage)
        {
            this.InputImage = inputImage;
        }
        public DepthMapInput(Bitmap inputImage)
        {
            ImageConverter converter = new ImageConverter();
            var inputBytes = (byte[])converter.ConvertTo(inputImage, typeof(byte[]));
            this.InputImage = Convert.ToBase64String(inputBytes);
        }
    }
}
