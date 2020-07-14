using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming.Overlay;
using Crestron.SimplSharpPro.DM.VideoWindowing;
using PepperDash.Essentials.Core;
using Crestron.SimplSharpPro.DM;
using PepperDash.Essentials.Core.Bridges;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace epi.videoProcessor.crestron.wp401
{
    public class Wp401Controller : CrestronGenericBridgeableBaseDevice
    {
        private readonly HdWp4k401C _401C;

        public IntFeedback LayoutFeedback { get; private set; }
        //public IntFeedback ImageFeedback { get; private set; }
        public StringFeedback NameFeedback { get; private set; }

        public StringFeedback LayoutNamesFeedback { get; private set; }
        public StringFeedback ImageNamesFeedback { get; private set; }

        private readonly Dictionary<uint, string> _layoutNameDictionary;
        //private readonly Dictionary<uint, ImageData> _imagesDictionary;

        /*
        private int _imageInt;

        public int ImageInt
        {
            get { return _imageInt; }
            set
            {
                _imageInt = value;
                ImageFeedback.FireUpdate();
            }
        }
         */

        private string _layoutName;
        public string LayoutName {
            get { return _layoutName; }
            set
            {
                _layoutName = value;
                LayoutNamesFeedback.FireUpdate();
            }
        }

        /*
        private string _imageName;
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                ImageNamesFeedback.FireUpdate();
            }
        }
         */


        public Wp401Controller(string key, string name, HdWp4k401C device, Wp401PropertiesConfig props)
            : base(key, name, device)
        {
            _401C = device;

            LayoutNamesFeedback = new StringFeedback(() => LayoutName);
            //ImageNamesFeedback = new StringFeedback(() => ImageName);          
            LayoutFeedback = new IntFeedback("LayoutFeedback", () => (int) _401C.HdWpWindowLayout.LayoutFeedback);
            //ImageFeedback = new IntFeedback("ImageIntFeedback", () => ImageInt);
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

        /*
        void ImageProperties_OverlayPropertiesChange(object device, OverlayPropertiesEventArgs args)
        {
            if (args.EventId == OverlayEventIds.DisabledEventId)
            {
                ImageInt = 0;
                return;
            }
            if (args.EventId != OverlayEventIds.PathFeedbackEventId && args.EventId != OverlayEventIds.EnabledEventId)
                return;
            var imageIntFb =
                _imagesDictionary.FirstOrDefault(
                    o => o.Value.ImageName.Contains(_401C.ImageProperties.PathFeedback.StringValue));
            if (imageIntFb.Value != null)
            {
                ImageInt = (int) imageIntFb.Key;
            }
        }
         */

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new Wp401ControllerJoinMap(joinStart);
            
            bridge.AddJoinMap(Key, joinMap);

            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.Online.JoinNumber]);
            NameFeedback.LinkInputSig(trilist.StringInput[joinMap.Name.JoinNumber]);

            LayoutFeedback.LinkInputSig(trilist.UShortInput[joinMap.SelectLayout.JoinNumber]);

            //ImageFeedback.LinkInputSig(trilist.UShortInput[joinMap.SelectBackground.JoinNumber]);

            LayoutNamesFeedback.LinkInputSig(trilist.StringInput[joinMap.LayoutNames.JoinNumber]);
            ImageNamesFeedback.LinkInputSig(trilist.StringInput[joinMap.BackgroundNames.JoinNumber]);

            trilist.SetUShortSigAction(joinMap.SelectLayout.JoinNumber,
                (a) => _401C.HdWpWindowLayout.Layout = (WindowLayout.eLayoutType) a);

            //trilist.SetUShortSigAction(joinMap.SelectBackground.JoinNumber, (a) => SetBackGround(a));

            UpdateXsig();

            trilist.OnlineStatusChange += (d, args) =>
            {
                if (!args.DeviceOnLine) return;
                NameFeedback.FireUpdate();
                //ImageFeedback.FireUpdate();
                LayoutFeedback.FireUpdate();
                UpdateXsig();
            };
        }

        /*
        public void SetBackGround(uint data)
        {
            if(data == 0) {_401C.ImageProperties.Disable();
                return;
            }
            _401C.ImageProperties.Enable();
            //if(data == 99) _401C.ImageProperties.Path
            _401C.ImageProperties.Path.StringValue = _imagesDictionary[data].ImagePath;
        }
         */

        public void UpdateXsig()
        {
            UpdateLayoutNames();
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

        /*
        public void UpdateBackgroundNames()
        {
            ImageName = XSigHelper.ClearData();
            foreach (var item in _imagesDictionary)
            {
                var image = item;
                var index = (int)image.Key - 1;
                var value = image.Value.ImageName;

                ImageName = XSigHelper.CreateByteString(index, value);
            }
        }
         * */
    }

    
}