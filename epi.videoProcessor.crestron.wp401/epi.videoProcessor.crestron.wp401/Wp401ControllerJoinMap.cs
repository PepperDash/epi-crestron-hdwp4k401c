using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Essentials.Core;

namespace epi.videoProcessor.crestron.wp401
{
    public class Wp401ControllerJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("Name")]
        public JoinDataComplete Name = new JoinDataComplete(new JoinData() { JoinNumber = 1, JoinSpan = 1 },
            new JoinMetadata() { Description = "Device Name", JoinCapabilities = eJoinCapabilities.ToSIMPL, JoinType = eJoinType.Serial });

        [JoinName("Online")]
        public JoinDataComplete Online = new JoinDataComplete(new JoinData() { JoinNumber = 1, JoinSpan = 1 },
            new JoinMetadata() { Description = "Device Online", JoinCapabilities = eJoinCapabilities.ToSIMPL, JoinType = eJoinType.Digital });

        [JoinName("SelectLayout")]
        public JoinDataComplete SelectLayout = new JoinDataComplete(new JoinData() { JoinNumber = 1, JoinSpan = 4 },
            new JoinMetadata() { Description = "Select Layout For Screen", JoinCapabilities = eJoinCapabilities.FromSIMPL, JoinType = eJoinType.Analog });

        [JoinName("LayoutNames")]
        public JoinDataComplete LayoutNames = new JoinDataComplete(new JoinData() { JoinNumber = 2, JoinSpan = 4 },
            new JoinMetadata() { Description = "Layout Names For Screen", JoinCapabilities = eJoinCapabilities.FromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("SelectBackground")]
        public JoinDataComplete SelectBackground = new JoinDataComplete(new JoinData() { JoinNumber = 6, JoinSpan = 1 },
            new JoinMetadata() { Description = "Select Background for Screen", JoinCapabilities = eJoinCapabilities.FromSIMPL, JoinType = eJoinType.Analog });

        [JoinName("BackgroundNames")]
        public JoinDataComplete BackgroundNames = new JoinDataComplete(new JoinData() { JoinNumber = 6, JoinSpan = 1 },
            new JoinMetadata() { Description = "Background Names for Screen", JoinCapabilities = eJoinCapabilities.FromSIMPL, JoinType = eJoinType.Serial });

        public Wp401ControllerJoinMap(uint joinStart)
            : base(joinStart, typeof (Wp401ControllerJoinMap)) {}
    }
}