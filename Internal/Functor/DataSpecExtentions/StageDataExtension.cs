using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RGBLineCoreLib.StageData;


namespace RGBLineCoreLib
{
    internal static class StageDataExtension
    {
        internal static bool BIsValid(this StageData targetStageData)
        {
            #region Check Region
            foreach (Guid regionID in targetStageData.RegionDataTable.Keys)
            {
                if(!StageDataInterface.RegionDataInterface.BIsRegionDataValid(new Tuple<Guid, RegionData>(regionID, targetStageData.RegionDataTable[regionID])))
                {
                    return false;
                }
            }
            List<Guid> regionIDs = new List<Guid>(targetStageData.RegionDataTable.Keys);
            #endregion

            #region Check Line
            foreach(Guid lineID in targetStageData.LineDataTable.Keys)
            {
                if (!StageDataInterface.LineDataInterface.BIsLineDataValid(new Tuple<Guid, LineData>(lineID, targetStageData.LineDataTable[lineID])))
                {
                    return false;
                }
            }
            List<Guid> lineIDs = new List<Guid>(targetStageData.LineDataTable.Keys);
            #endregion

            #region Check Note
            //{
            //    foreach(Guid noteID in targetStageData.NoteDataTable.Keys)
            //    {
            //        if (noteID == Guid.Empty)
            //        {
            //            return false;
            //        }
            //    }

            //    foreach(StageData.NoteData noteData in targetStageData.NoteDataTable.Values)
            //    {
            //        if (noteData.AttachedLineID == Guid.Empty)
            //        {
            //            return false;
            //        }
            //        else if (!lineIDs.Contains(noteData.AttachedLineID))
            //        {
            //            return false;
            //        }


            //    }
            //}
            #endregion

            #region Check Stage Config

            #endregion

            return true;
        }

        internal static void Dispose(this StageData targetStageData)
        {
            #region Dispose Region
            targetStageData.RegionDataTable.Clear();
            targetStageData.RegionDataTable = null;
            #endregion

            #region Dispose Line
            Dictionary<Guid, StageData.LineData>.KeyCollection lineIDs = targetStageData.LineDataTable.Keys;
            foreach (Guid lineID in lineIDs)
            {
                targetStageData.LineDataTable[lineID].CurvedLinePoints.Clear();
                targetStageData.LineDataTable[lineID].MinorOffsetTimes.Clear();
            }

            targetStageData.LineDataTable.Clear();
            targetStageData.LineDataTable = null;
            #endregion

            #region Dispose Note
            targetStageData.NoteDataTable.Clear();
            targetStageData.NoteDataTable = null;
            #endregion
        }
    }
}