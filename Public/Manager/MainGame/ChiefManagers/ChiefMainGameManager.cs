using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonUtilLib.ThreadSafe;


namespace RGBLineCoreLib
{
    public sealed class ChiefMainGameManager : SingleTonForGameObject<ChiefMainGameManager>
    {
        public void Awake()
        {
            SetInstance(this);
        }

        internal void OpenStage(in string targetStageName, in StageMetadata.MajorDifficultyLevel majorDifficulty)
        {
            StageDataBuffer.Instance.TryLoadStageData(targetStageName, majorDifficulty);

            RegionManager.Instance.SpawnRegionProps();
            LineManager.Instance.SpawnLineProps();
            NoteManager.Instance.SpawnNoteProps();
        }
        internal void CloseStage()
        {
            NoteManager.Instance.DespawnNoteProps();
            LineManager.Instance.DespawnLineProps();
            RegionManager.Instance.DespawnRegionProps();

            StageDataBuffer.Instance.Dispose();
        }

        protected override void Dispose(bool bisDisposing)
        {
            if(bisDisposing)
            {
                CloseStage();
            }
        }
    }
}