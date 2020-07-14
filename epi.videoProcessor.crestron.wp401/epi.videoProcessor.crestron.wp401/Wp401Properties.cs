using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

using PepperDash.Core;

namespace epi.videoProcessor.crestron.wp401
{
    public class Wp401PropertiesConfig
    {
        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }

        [JsonProperty("Screen")]
        public ScreenInfo Screen { get; set; }

    }

    public class ScreenInfo
    {
        [JsonProperty("layoutNames")]
        public Dictionary<uint, string> LayoutNames { get; set; }

        [JsonProperty("imageData")]
        public Dictionary<uint, ImageData> ImageData { get; set; }
    }

    public class ImageData
    {
        [JsonProperty("imageName")]
        public string ImageName { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }
    }
}