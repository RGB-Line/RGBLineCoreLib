using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib.Data
{
    public struct StageMetadata
    {
        /// <summary>
        /// 해당 Stage의 Major Difficulty를 나타내는 Flag
        /// </summary>
        public enum MajorDifficultyLevel : byte
        {
            Easy, Normal, Hard
        }


        public string Title;
        public string Artist;
        public float MusicLength;

        public MajorDifficultyLevel MajorDifficulty;
        public int MinorDifficulty;

        public int BestScore;

        public float LobbyMusicStartTime;
        public float LobbyMusicEndTime;
        public float LobbyMusicFadeOutTime;
    }
}