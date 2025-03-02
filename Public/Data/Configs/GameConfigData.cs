using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib
{
    public struct GameConfigData
    {
        public enum MaxFrameRate
        {
            Frame_30,
            Frame_60,
            Frame_90,
            Frame_120
        }

        [Serializable]
        public struct NoteHitJudgingStrandard
        {
            public enum HitJudgingType
            {
                Perfect,
                Good,
                Miss
            }


            public List<float> HitJudgingRanges;
        }


        public NoteHitJudgingStrandard noteHitJudgingStrandard;

        public float MusicVolume;
        public MaxFrameRate MaxFrame;
        public int VSyncCount;
    }
}