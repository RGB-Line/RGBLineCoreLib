using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    /// <summary>
    /// GameConfigData를 담은 SingleTon Buffer Class
    /// </summary>
    public sealed class GameConfigDataBuffer : SingleTon<GameConfigDataBuffer>
    {
        private GameConfigData? m_gameConfigData;


        /// <summary>
        /// GameConfigData를 받아온다
        /// </summary>
        /// <remarks>
        /// 만약 먼저 LoadGameConfigData()를 호출하지 않았거나, 혹은 호출했더라도 문제가 있어 올바르게 Load 되지 않았을 경우 예외를 던진다
        /// </remarks>
        /// <exception cref="NullReferenceException"></exception>
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

        /// <summary>
        /// DataPathTable의 내용을 바탕으로 Load를 시도하며, 만약 해당 경로에 GameConfigData File이 존재하지 않을 경우 새로운 GameConfigData를 생성하고 저장해준다
        /// </summary>
        /// <remarks>
        /// GameConfigDataBuffer.Instance.ConfigData를 호출하기 전에 필히 호출해야만 한다
        /// </remarks>
        public void LoadGameConfigData()
        {
            string path = DataPathInterface.GetConfigPath();
            if (!File.Exists(path))
            {
                m_gameConfigData = new GameConfigData()
                {
                    NoteHitJudgeStrandard = new GameConfigData.NoteHitJudgingStrandard()
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
        /// <summary>
        /// DataPathTable의 내용을 바탕으로 Save한다
        /// </summary>
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