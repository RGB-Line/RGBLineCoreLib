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
                if (value.BIsValid())
                {
                    m_stageMetadata = value;
                }
            }
        }

        internal bool TryLoadStageMetadata(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            string path = DataPathInterface.GetStageMetadataPath(targetStageName, majorDifficulty);

            if (!File.Exists(path))
            {
                return false;
            }

            string jsonData = File.ReadAllText(path);
            StageMetadata = JsonConvert.DeserializeObject<StageMetadata>(jsonData);

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