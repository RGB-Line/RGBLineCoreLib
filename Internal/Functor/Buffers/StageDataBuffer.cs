using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;


namespace RGBLineCoreLib
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
                if(value.BIsValid())
                {
                    m_stageData = value;
                }
            }
        }

        internal bool TryLoadStageData(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            string path = DataPathInterface.GetStageDataPath(targetStageName, majorDifficulty);

            if(!File.Exists(path))
            {
                return false;
            }

            string jsonData = File.ReadAllText(path);
            StageData = JsonConvert.DeserializeObject<StageData>(jsonData);

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