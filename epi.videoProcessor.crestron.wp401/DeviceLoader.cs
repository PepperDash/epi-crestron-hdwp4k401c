using System;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       				// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.DM.VideoWindowing;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using PepperDash.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using Crestron.SimplSharpPro;


namespace epi.videoProcessor.crestron.wp401
{
    public class Wp401Factory : EssentialsPluginDeviceFactory<Wp401Controller>
    {
        public Wp401Factory()
        {
            MinimumEssentialsFrameworkVersion = "1.5.5";

            TypeNames = new List<string> {"HdWp4k401c"};
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            Debug.Console(1, "Factory Attempting to create new HD-WP-4K-401-C Device");

            var props = JsonConvert.DeserializeObject<Wp401PropertiesConfig>(dc.Properties.ToString());

            var type = dc.Type.ToLower();
            var control = props.Control;
            var ipid = control.IpIdInt;

            return new Wp401Controller(dc.Key, dc.Name, new HdWp4k401C(ipid, Global.ControlSystem), props);
        }
    }
}