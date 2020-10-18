using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerativeModeling.Io
{
    public class DepthMapOutput
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        public Bitmap ToBitmap()
        {
            Bitmap bmp = null;
            byte[] buffer = Convert.FromBase64String(this.Result);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                stream.Position = 0;
                bmp = (Bitmap)Bitmap.FromStream(stream);
            }
            return bmp;
        }
    }
}
