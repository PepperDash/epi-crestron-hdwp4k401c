using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.VideoWindowing;
using PepperDash.Essentials.Core;
using Crestron.SimplSharpPro.DM;
using PepperDash.Essentials.Core.Bridges;

namespace epi.videoProcessor.crestron.wp401
{
    public class Wp401Controller : CrestronGenericBridgeableBaseDevice
    {
        private readonly HdWp4k401C _401C;
        public bool DisableAutoMode { get; private set; }

        public IntFeedback LayoutFeedback { get; private set; }
        //public IntFeedback ImageFeedback { get; private set; }
        public StringFeedback NameFeedback { get; private set; }

        public StringFeedback LayoutNamesFeedback { get; private set; }
        //public StringFeedback ImageNamesFeedback { get; private set; }

        private readonly Dictionary<uint, string> _layoutNameDictionary;


        private string _layoutName;
        public string LayoutName {
            get { return _layoutName; }
            set
            {
                _layoutName = value;
                LayoutNamesFeedback.FireUpdate();
            }
        }




        public Wp401Controller(string key, string name, HdWp4k401C device, Wp401PropertiesConfig props)
            : base(key, name, device)
        {
            _401C = device;
            DisableAutoMode = props.DisableAutoMode;
            LayoutNamesFeedback = new StringFeedback(() => LayoutName);
            LayoutFeedback = new IntFeedback("LayoutFeedback", () => (int) _401C.HdWpWindowLayout.LayoutFeedback);
            NameFeedback = new StringFeedback("ImagePathFeedback", () => Name);

            //_401C.ImageProperties.OverlayPropertiesChange += ImageProperties_OverlayPropertiesChange;
            _401C.HdWpWindowLayout.WindowLayoutChange += HdWpWindowLayout_WindowLayoutChange;

            _layoutNameDictionary = props.Screen.LayoutNames;
            //_imagesDictionary = props.Screen.ImageData;



        }

        void HdWpWindowLayout_WindowLayoutChange(object sender, GenericEventArgs args)
        {
            if (args.EventId == WindowLayoutEventIds.LayoutFeedbackEventId)
                LayoutFeedback.FireUpdate();
        }


        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new Wp401ControllerJoinMap(joinStart);
            
            bridge.AddJoinMap(Key, joinMap);

            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.Online.JoinNumber]);
            NameFeedback.LinkInputSig(trilist.StringInput[joinMap.Name.JoinNumber]);

            LayoutFeedback.LinkInputSig(trilist.UShortInput[joinMap.SelectLayout.JoinNumber]);

            //ImageFeedback.LinkInputSig(trilist.UShortInput[joinMap.SelectBackground.JoinNumber]);

            LayoutNamesFeedback.LinkInputSig(trilist.StringInput[joinMap.LayoutNames.JoinNumber]);

            trilist.SetUShortSigAction(joinMap.SelectLayout.JoinNumber,
                (a) =>
                {
                    if (DisableAutoMode)
                    {
                        if(a > 0)
                            _401C.HdWpWindowLayout.Layout = (WindowLayout.eLayoutType)a;
                    }
                        
                    else
                        _401C.HdWpWindowLayout.Layout = (WindowLayout.eLayoutType)a;
                });

            //trilist.SetUShortSigAction(joinMap.SelectBackground.JoinNumber, (a) => SetBackGround(a));

            UpdateXsig();

            trilist.OnlineStatusChange += (d, args) =>
            {
                if (!args.DeviceOnLine) return;
                NameFeedback.FireUpdate();
                LayoutFeedback.FireUpdate();
                UpdateXsig();
            };
        }



        public void UpdateXsig()
        {
            //UpdateLayoutNames();
            //UpdateBackgroundNames();
        }

        public void UpdateLayoutNames()
        {
            LayoutName = XSigHelper.ClearData();
            foreach (var item in _layoutNameDictionary)
            {
                var layout = item;
                var index = (int)layout.Key - 1;
                var value = layout.Value;

                LayoutName = XSigHelper.CreateByteString(index, value);
            }
        }


    }

    
}