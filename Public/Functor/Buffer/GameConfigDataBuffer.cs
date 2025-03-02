using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;


namespace RGBLineCoreLib
{
    public sealed class GameConfigDataBuffer : SingleTon<GameConfigDataBuffer>
    {
        private GameConfigData? m_gameConfigData;


        public GameConfigData ConfigData
        {
            get
            {
                if(m_gameConfigData == null)
                {
                    throw new NullReferenceException("GameConfigData is null");
                }

                return m_gameConfigData.Value;
            }
            set
            {
                m_gameConfigData = value;
            }
        }

        public void LoadGameConfigData()
        {
            string path = DataPathInterface.GetConfigPath();
            if (!File.Exists(path))
            {
                m_gameConfigData = new GameConfigData()
                {
                    noteHitJudgingStrandard = new GameConfigData.NoteHitJudgingStrandard()
                    {
                        HitJudgingRanges = new List<float>() { 50.0f, 100.0f, 200.0f }
                    },
                    MusicVolume = 0.8f,
                    MaxFrame = GameConfigData.MaxFrameRate.Frame_60,
                    VSyncCount = 0
                };
                SaveGameConfigData();

                return;
            }

            string jsonData = File.ReadAllText(path);
            m_gameConfigData = JsonConvert.DeserializeObject<GameConfigData>(jsonData);
        }
        public void SaveGameConfigData()
        {
            string path = DataPathInterface.GetConfigPath();
            string directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            string jsonData = JsonConvert.SerializeObject(m_gameConfigData);
            File.WriteAllText(path, jsonData);
        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {
                m_gameConfigData = null;
            }
        }
    }
}