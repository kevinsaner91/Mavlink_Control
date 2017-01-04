using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radar_Config_and_Measurement_Tool.Properties;

namespace Radar_Config_and_Measurement_Tool
{
    class SettingsCollector
    {
        private static string[] BoardText = { "Tank Gauging", "RRC01", "Fractional-N PLL", "BGT24MTR11", "RRC160 Integer-N", "RRC160 Frac-N", "FindMine"};
        private static char[] BoardModule = { 'A', 'E', 'F', 'B', 'G', 'H', 'I' };
        public enum Board : uint { TankGauging = 0, RRC01 = 1, FractionalN_PLL = 2, BGT24 = 3, RRC160_IntN = 4, RRC160_FracN = 5, FindMine = 6 };
        public static Board BoardSelection {get; set;}

        private static string[] TrackText = { "Millimeter Track", "FEW Lab", "Uni Ulm" }; //element 99 is "Simulation"
        public enum Track:uint { MillimeterTrack=0, FEW_Lab=1, Uni_Ulm=2, SimulatorTrack = 99 };
        public static Track TrackSelection { get; set; }

        public static string DSP_Com { get; set; }

        public struct st_FFT_Settings
        {
            public double fres;
            public double rres;
            public double rmax;
        }
        public static st_FFT_Settings fft_settings;

        public static void init(Settings setting)
        {
            //get data from config xml
            TrackSelection = (Track)setting.SelectedTrack;
            DSP_Com = setting.DSPCom;

            fft_settings.fres = 0;
            fft_settings.rres = 0;
            fft_settings.rmax = 0;
            BoardSelection = Board.TankGauging;
        }

        public static string BoardName()
        {
            return BoardText[(uint)BoardSelection];
        }

        public static char BoardDSPModule()
        {
            return BoardModule[(uint)BoardSelection];
        }

        public static string TrackName()
        {
            if((uint)TrackSelection == 99)
            {
                return "Simulation";
            }
            return TrackText[(uint)TrackSelection];
        }
    }
}
