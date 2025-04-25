using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;

using RGBLineCoreLib.Data;


namespace RGBLineCoreLib.Functor
{
    internal sealed class StageMetadataBuffer : SingleTon<StageMetadataBuffer>
    {
        private StageMetadata? m_stageMetadata;


        internal StageMetadata StageMetadata
        {
            get
            {
                if (m_stageMetadata == null)
                {
                    throw new NullReferenceException("StageMetadata is null");
                }
                return m_stageMetadata.Value;
            }
            set
            {
                m_stageMetadata = value;
            }
        }

        public bool TrySaveStageMetadata(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficultyLevel)
        {
            string path = DataPathInterface.GetStageMetadataPath(targetStageName, majorDifficultyLevel);

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonData = JsonConvert.SerializeObject(StageMetadata, Formatting.Indented);
            File.WriteAllText(path, jsonData);

            return true;
        }

        internal bool TryLoadStageMetadata(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty, in bool bisNewData = false, in float musicLength = 0.0f)
        {
            string path = DataPathInterface.GetStageMetadataPath(targetStageName, majorDifficulty);

            if(bisNewData && musicLength > 0.0f)
            {
                StageMetadata = new StageMetadata()
                {
                    Title = "New Stage",
                    Artist = "New Artist",
                    MusicLength = musicLength,

                    MajorDifficulty = StageMetadata.MajorDifficultyLevel.Easy,
                    MinorDifficulty = 1,

                    BestScore = 0,

                    LobbyMusicStartTime = 0.0f,
                    LobbyMusicEndTime = musicLength,
                    LobbyMusicFadeOutTime = 0.0f
                };
            }
            else
            {
                if (!File.Exists(path))
                {
                    UnityEngine.Debug.LogError("StageMetadataBuffer::TryLoadStageMetadata - File not found");
                    return false;
                }

                try
                {
                    string jsonData = File.ReadAllText(path);
                    StageMetadata = JsonConvert.DeserializeObject<StageMetadata>(jsonData);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {
                if (m_stageMetadata != null)
                {
                    m_stageMetadata.Value.Dispose();
                }

                m_stageMetadata = null;
            }
        }
    }
}