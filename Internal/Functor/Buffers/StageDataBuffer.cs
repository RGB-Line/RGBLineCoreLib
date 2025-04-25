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
    internal sealed class StageDataBuffer : SingleTon<StageDataBuffer>
    {
        private StageData? m_stageData;


        internal StageData StageData
        {
            get
            {
                if (m_stageData == null)
                {
                    throw new NullReferenceException("StageData is null");
                }
                return m_stageData.Value;
            }
            set
            {
                m_stageData = value;
            }
        }

        public bool TrySaveStageData(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficultyLevel)
        {
            string path = DataPathInterface.GetStageDataPath(targetStageName, majorDifficultyLevel);

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonData = JsonConvert.SerializeObject(StageData, Formatting.Indented);
            File.WriteAllText(path, jsonData);

            return true;
        }

        internal bool TryLoadStageData(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty, in bool bisNewData = false)
        {
            string path = DataPathInterface.GetStageDataPath(targetStageName, majorDifficulty);

            if(bisNewData)
            {
                StageData = new StageData()
                {
                    RegionDataTable = new Dictionary<Guid, StageData.RegionData>(),
                    LineDataTable = new Dictionary<Guid, StageData.LineData>(),
                    NoteDataTable = new Dictionary<Guid, StageData.NoteData>(),
                    StageConfig = new StageData.StageConfigData()
                    {
                        BPM = 120,
                        BitSubDivision = 1,
                        LengthPerBit = 1.0f,
                        MusicStartOffsetTime = 0.0f
                    }
                };
            }
            else
            {
                if (!File.Exists(path))
                {
                    UnityEngine.Debug.LogError("StageDataBuffer::TryLoadStageData - File not found");
                    return false;
                }

                try
                {
                    string jsonData = File.ReadAllText(path);
                    StageData = JsonConvert.DeserializeObject<StageData>(jsonData);
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
            if(bisDisposing)
            {
                if(m_stageData != null)
                {
                    m_stageData.Value.Dispose();
                }

                m_stageData = null;
            }
        }
    }
}