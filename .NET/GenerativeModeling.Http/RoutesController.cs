using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerativeModeling.Http
{
    public static class RoutesController
    {
        public static string Server => "http://localhost:5000";
        public static string StyleTransferRoute => $"{Server}/inference/style-transfer/";
    }
}
