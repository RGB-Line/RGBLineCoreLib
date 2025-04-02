using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Data
{
    public struct GameConfigData
    {
        /// <summary>
        /// 지원하는 FPS 빈도를 지정한다
        /// </summary>
        public enum MaxFrameRate : byte
        {
            Frame_30,
            Frame_60,
            Frame_90,
            Frame_120
        }

        /// <summary>
        /// 각 판정마다의 히트 박스 크기를 지정한다
        /// </summary>
        [Serializable] public struct NoteHitJudgingStrandard
        {
            public enum HitJudgingType
            {
                Perfect,
                Good,
                Miss
            }


            public List<float> HitJudgingRanges;
        }
        [Serializable] public struct SpecialKeySet
        {
            public KeyCode RedNoteKeyCode_Left;
            public KeyCode RedNoteKeyCode_Right;

            //public KeyCode RedLineCornerNoteKeyCode;

            public KeyCode TrackerMovementKeyCode_ToLeft;
            public KeyCode TrackerMovementKeyCode_ToRight;
        }


        public NoteHitJudgingStrandard NoteHitJudgeStrandard;

        /// <summary>
        /// 게임 전체 볼륨을 조절한다
        /// </summary>
        /// <remarks>
        /// 0.0f ~ 1.0f 사이의 값으로 지정한다
        /// </remarks>
        public float MusicVolume;
        public float SyncAdjustment;
        public MaxFrameRate MaxFrame;
        /// <summary>
        /// 게임의 V Sync 수준을 조절한다
        /// </summary>
        /// <remarks>
        /// 0 ~ 4 사이의 값으로 지정한다
        /// </remarks>
        public int VSyncCount;
        public SpecialKeySet SpecialKeySetting;
    }
}