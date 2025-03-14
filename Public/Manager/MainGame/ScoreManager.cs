using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using RGBLineCoreLib.Data;
using RGBLineCoreLib.Functor;


namespace RGBLineCoreLib.Manager
{
    public sealed class ScoreManager : SingleTonForGameObject<ScoreManager>
    {
        private bool m_bisStartScoring = false;

        private Stack<Guid> m_noteCandidates = new Stack<Guid>();
        private Dictionary<Guid, Tuple<float, float>> m_longNoteTable = new Dictionary<Guid, Tuple<float, float>>();

        [SerializeField] private List<KeyCode> m_bannedNoteKeyCode;

        private float m_curAudioSourceTime = 0.0f;

        private bool m_bisGreenRegion, m_bisMouseOnGreenLine;

        private float m_curScore = 0.0f;
        private float m_HP = 1.0f;


        public void Awake()
        {
            SetInstance(this);
        }
        public void Update()
        {
            if (!m_bisStartScoring)
            {
                return;
            }

            // For Green Region - Mouse Tracking
            if (m_bisGreenRegion && !m_bisMouseOnGreenLine)
            {
                return;
            }

            // For Long Notes
            int curPressedBasicNoteKeyCount = GetCurPressedBasicNoteKeyCount();
            if (curPressedBasicNoteKeyCount < m_longNoteTable.Count)
            {
                int unmatchedPressedKeyCount = m_longNoteTable.Count - curPressedBasicNoteKeyCount;
                for (int count = 0; count < unmatchedPressedKeyCount; count++)
                {
                    float minTime = float.MaxValue;
                    int minItemIndex = -1;
                    for (int index = 0; index < m_longNoteTable.Values.Count; index++)
                    {
                        if (m_longNoteTable.Values.ElementAt(index).Item1 < minTime)
                        {
                            minTime = m_longNoteTable.Values.ElementAt(index).Item1;
                            minItemIndex = index;
                        }
                    }

                    m_longNoteTable.Remove(m_longNoteTable.Keys.ElementAt(minItemIndex));
                }
            }

            // For New Notes(Include Long Note)
            int freshPressedBasicNoteKeyCount = GetFreshPressedBasicNoteKeyCount();
            for (int count = 0; count < freshPressedBasicNoteKeyCount; count++)
            {
                Guid targetNoteID = Guid.Empty;
                if(m_noteCandidates.Count == 0)
                {
                    break;
                }
                else
                {
                    targetNoteID = m_noteCandidates.Pop();
                }

                // Red Line Curved Note
                if (NoteManager.Instance.GetRedLineCornerNote(targetNoteID) != null && Input.GetKeyDown(KeyCode.Space))
                {
                    IRedLineCornerNote targetNoteItem = NoteManager.Instance.GetRedLineCornerNote(targetNoteID);
                    m_curScore += GetSingleNoteScore(GetYPos2Time(targetNoteItem.Transform.position.y), m_curAudioSourceTime);

                    m_HP += 0.1f;

                    targetNoteItem.Transform.gameObject.SetActive(false);
                }
                // Red, Green, Blue Note
                else if(StageDataInterface.NoteDataInterface.BIsNoteIDValid(targetNoteID))
                {
                    StageData.NoteData curNoteData = StageDataInterface.NoteDataInterface.GetNoteData(targetNoteID);
                    switch (curNoteData.CurNoteType)
                    {
                        case StageData.NoteData.NoteType.Common:
                            {
                                if(!(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)))
                                {
                                    INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(targetNoteID);
                                    m_curScore += GetSingleNoteScore(GetYPos2Time(targetNoteItem.RedAndBlueNote.Transform.position.y), m_curAudioSourceTime);

                                    m_HP += 0.1f;

                                    targetNoteItem.Transform.gameObject.SetActive(false);
                                }
                            }
                            break;

                        case StageData.NoteData.NoteType.Flip:
                            {
                                bool bisHit = false;
                                if (curNoteData.flipNoteDirection == StageData.NoteData.FlipNoteDirection.Left &&
                                    Input.GetKeyDown(KeyCode.Mouse0))
                                {
                                    bisHit = true;
                                }
                                else if (curNoteData.flipNoteDirection == StageData.NoteData.FlipNoteDirection.Right &&
                                    Input.GetKeyDown(KeyCode.Mouse1))
                                {
                                    bisHit = true;
                                }

                                if (bisHit)
                                {
                                    INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(targetNoteID);
                                    m_curScore += GetSingleNoteScore(GetYPos2Time(targetNoteItem.RedAndBlueNote.Transform.position.y), m_curAudioSourceTime);

                                    m_HP += 0.1f;

                                    targetNoteItem.Transform.gameObject.SetActive(false);
                                }
                            }
                            break;

                        case StageData.NoteData.NoteType.Long:
                            {
                                if (!(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)))
                                {
                                    INoteItem targetNoteItem = NoteManager.Instance.GetNoteItem(targetNoteID);
                                    m_longNoteTable.Add(targetNoteID, new Tuple<float, float>(GetYPos2Time(targetNoteItem.GreenNote.CurveStartYPos), m_curAudioSourceTime));
                                }
                            }
                            break;
                    }
                }
            }
        }

        public float CurAudioSourceTime
        {
            set
            {
                m_curAudioSourceTime = value;
            }
        }

        public float CurScore
        {
            get
            {
                return m_curScore;
            }
        }
        public float HP
        {
            get
            {
                return m_HP;
            }
        }

        internal bool BIsGreenRegion
        {
            set
            {
                m_bisGreenRegion = value;
            }
        }
        internal bool BIsMouseOnGreenLine
        {
            set
            {
                m_bisMouseOnGreenLine = value;
            }
        }

        public void StartScoring()
        {
            m_bisStartScoring = true;
        }

        internal void PushNoteCandidate(in Guid noteID)
        {
            m_noteCandidates.Push(noteID);

            SortNoteCandidates();
        }
        internal void RemoveNoteCandidate(in Guid noteID)
        {
            if (m_longNoteTable.ContainsKey(noteID))
            {
                m_curScore += GetSingleNoteScore(m_longNoteTable[noteID].Item1, m_longNoteTable[noteID].Item2);

                m_HP += 0.1f;

                m_longNoteTable.Remove(noteID);
            }
            else
            {
                List<Guid> noteCandidateBuffer = m_noteCandidates.ToList();
                if (noteCandidateBuffer.Contains(noteID))
                {
                    m_HP -= 0.1f;

                    noteCandidateBuffer.Remove(noteID);

                    Stack<Guid> resultNoteCandidates = new Stack<Guid>();
                    foreach (Guid curNoteID in noteCandidateBuffer)
                    {
                        resultNoteCandidates.Push(curNoteID);
                    }

                    m_noteCandidates = resultNoteCandidates;

                    SortNoteCandidates();
                }
            }
        }

        private int GetCurPressedBasicNoteKeyCount()
        {
            IEnumerable<KeyCode> keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();

            List<KeyCode> keyCodeBuffer = keyCodes.ToList();
            foreach (KeyCode keyCode in m_bannedNoteKeyCode)
            {
                if (keyCodeBuffer.Contains(keyCode))
                {
                    keyCodeBuffer.Remove(keyCode);
                }
            }

            keyCodes = keyCodeBuffer.Cast<KeyCode>();

            int curPressedKeyCount = keyCodes.Count(keyCode => Input.GetKey(keyCode));

            return curPressedKeyCount;
        }
        private int GetFreshPressedBasicNoteKeyCount()
        {
            IEnumerable<KeyCode> keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();

            List<KeyCode> keyCodeBuffer = keyCodes.ToList();
            foreach (KeyCode keyCode in m_bannedNoteKeyCode)
            {
                if (keyCodeBuffer.Contains(keyCode))
                {
                    keyCodeBuffer.Remove(keyCode);
                }
            }

            keyCodes = keyCodeBuffer.Cast<KeyCode>();

            int curPressedKeyCount = keyCodes.Count(keyCode => Input.GetKeyDown(keyCode));

            return curPressedKeyCount;
        }

        private float GetSingleNoteScore(in float noteAppearTime, in float keyPressedTime)
        {
            //Debug.Log("noteAppearTime : " + noteAppearTime + " / keyPressedTime : " + keyPressedTime);

            int greenRegionCount = 0;
            Guid[] regionIDs = StageDataInterface.RegionDataInterface.GetRegionIDs();
            foreach(Guid regionID in regionIDs)
            {
                if(StageDataInterface.RegionDataInterface.GetRegionData(regionID).CurColorType == StageData.RegionData.ColorType.Green)
                {
                    greenRegionCount++;
                }
            }

            float resultScore = 1000000.0f / (StageDataInterface.NoteDataInterface.GetNoteIDs().Length + NoteManager.Instance.RedLineCornerNoteCount + greenRegionCount);

            float timeDistance = Mathf.Abs(noteAppearTime - keyPressedTime);
            if (0.1f <= timeDistance && timeDistance < 0.2f)
            {
                resultScore *= 0.7f;
            }
            else if (0.2f <= timeDistance)
            {
                resultScore = 0.0f;
            }

            return resultScore;
        }

        private float GetYPos2Time(in float yPos)
        {
            StageData.StageConfigData stageConfig = StageDataInterface.StageConfigDataInterface.GetStageConfigData();
            float curNoteRatio = ((yPos - DeterminLine.Instance.transform.localPosition.y)
                                / (stageConfig.LengthPerBit / stageConfig.BitSubDivision)) / GridManager.Instance.GetTotalFrameCount();
            
            StageMetadata stageMetadata = StageMetadataInterface.GetStageMetadata();
            float curTime = stageMetadata.MusicLength * curNoteRatio;
            curTime -= stageConfig.MusicStartOffsetTime;

            return curTime;
        }

        private void SortNoteCandidates()
        {
            Guid[] noteIDs = StageDataInterface.NoteDataInterface.GetNoteIDs();
            Dictionary<float, List<Guid>> noteByAppearYPos = new Dictionary<float, List<Guid>>();
            foreach (Guid noteID in m_noteCandidates)
            {
                float curNoteAppearYPos = float.MinValue;
                if (noteIDs.Contains(noteID))
                {
                    curNoteAppearYPos = NoteManager.Instance.GetNoteItem(noteID).Transform.position.y;
                }
                else if (NoteManager.Instance.GetRedLineCornerNote(noteID) != null)
                {
                    curNoteAppearYPos = NoteManager.Instance.GetRedLineCornerNote(noteID).Transform.position.y;
                }

                if (!noteByAppearYPos.ContainsKey(curNoteAppearYPos))
                {
                    noteByAppearYPos.Add(curNoteAppearYPos, new List<Guid>());
                }
                noteByAppearYPos[curNoteAppearYPos].Add(noteID);
            }

            foreach (float key in noteByAppearYPos.Keys)
            {
                noteByAppearYPos[key].Sort((noteID_0, noteID_1) =>
                {
                    int result = 0;

                    float appearYPos_0 = float.MinValue;
                    if (noteIDs.Contains(noteID_0))
                    {
                        appearYPos_0 = NoteManager.Instance.GetNoteItem(noteID_0).Transform.position.y;
                    }
                    else if (NoteManager.Instance.GetRedLineCornerNote(noteID_0) != null)
                    {
                        appearYPos_0 = NoteManager.Instance.GetRedLineCornerNote(noteID_0).Transform.position.y;
                    }

                    float appearYPos_1 = float.MinValue;
                    if (noteIDs.Contains(noteID_1))
                    {
                        appearYPos_1 = NoteManager.Instance.GetNoteItem(noteID_1).Transform.position.y;
                    }
                    else if (NoteManager.Instance.GetRedLineCornerNote(noteID_1) != null)
                    {
                        appearYPos_1 = NoteManager.Instance.GetRedLineCornerNote(noteID_1).Transform.position.y;
                    }

                    if (appearYPos_0 < appearYPos_1)
                    {
                        result = -1;
                    }
                    else if (appearYPos_0 > appearYPos_1)
                    {
                        result = 1;
                    }

                    return result;
                });
            }

            var sortedKeys = noteByAppearYPos.Keys.ToList();
            sortedKeys.Sort();

            Stack<Guid> resultNoteCandidates = new Stack<Guid>();
            foreach (float key in sortedKeys)
            {
                foreach (Guid noteID in noteByAppearYPos[key])
                {
                    resultNoteCandidates.Push(noteID);
                }
            }

            m_noteCandidates = resultNoteCandidates;
        }

        protected override void Dispose(bool bisDisposing)
        {
            if (bisDisposing)
            {

            }
        }
    }
}