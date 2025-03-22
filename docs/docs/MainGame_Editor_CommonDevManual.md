# Main Game, Editor Common Dev Manual

## 기본적인 활용법

### <code>StageData</code>, <code>StageMetadata</code> Read & Write

RGBLineCoreLib에서는 Data를 담고 관리하는 핵심 Buffer는 대게 직접 접근이 불가하며, 이는 가장 기초적인 Data 무결성을 유지하기 위한 조치이다

때문에 대신하여 적절한 Interface Class를 두어 간접적으로 Data에 접근할 수 있게 하고 있다

RGB Line Project의 핵심 Data 종류는 다음과 같다
```
RGB Line Core Datas
├── StageData
│   ├── RegionData
│   ├── LineData
│   ├── NoteData
│   └── StageConfigData
├── StageMetadata
└── GameConfigData
```

각각의 Data 종류에 대해 제공되는 Interface Class는 다음과 같다

```
RGB Line Core Datas
├── StageData               - StageDataInterface
│   ├── RegionData          - StageDataInterface.RegionDataInterface
│   ├── LineData            - StageDataInterface.LineDataInterface
│   ├── NoteData            - StageDataInterface.NoteDataInterface
│   └── StageConfigData     - StageDataInterface.StageConfigDataInterface
├── StageMetadata           - StageMetadataInterface
└── GameConfigData          - GameConfigDataBuffer
```

<code>GameConfigData</code>에 대해서는 예외적으로 Buffer에 직접 접근하는 것을 허용한다. 다만, Editor에서는 <code>GameConfigData</code>를 직접 조작할 일이 없을 것으라 예상됨으로 크게 신경쓸 필요는 없다

<code>RGBLineCoreLib.dll</code>에서는 모든 Interface애서 Read만을 지원하며, <code>RGBLineCoreLib_ForEditor.dll</code>에서만 모든 Interface에서 Read/Write를 지원한다

각 Interface Class가 제공하는 기능은 본 문서의 API 탭을 참고하길 바란다

## <code>StageData</code> Data Load & Release

<code>StageData</code>를 Load할 때 다음과 같은 Coding Style을 지켜주는 것이 바람직하다

```
if(!StageDataInterface.TryLoadStageData(StageName, stageLoadInfo.MajorDifficulty))
{
    // TODO - Error Handling
    throw new Exception("Failed to load stage data.");
}
else if (!StageDataInterface.BIsStageDataValid())
{
    // TODO - Error Handling
    throw new Exception("Invalid stage data.");
}
```

- <code>StageDataInterface.TryLoadStageData(...)</code>의 경우 정상적으로 <code>*.rgbline</code> 파일을 읽고, StageData로 Deserialize했을 때 true를, 그렇지 않을 때 false를 Return한다
- <code>StageDataInterface.BIsStageDataValid()</code>는 <code>StageDataInterface.TryLoadStageData(...)</code>를 통해 Load된 StageData가 Valid한 지 검사한다. 만약 문제가 없으면 true를, 그렇지 않을 때 false를 Return한다

Load된 StageData의 사용이 끝나면 다음과 같은 Coding Style을 지켜주는 것이 바람직하다

```
StageDataInterface.DisposeStageData();
GC.Collect();
```

- <code>StageDataInterface.DisposeStageData()</code>는 현재 Load된 StageData 내부의 Reference Count를 제거하여 GC에 의해 필요없는 모든 자원이 최대한 Release 될 수 있도록 한다
- <code>GC.Collect()</code>를 임의로 호출함으로서 필요없는 자원을 검출해 Release하도록 한다

## <code>StageMetadata</code> Data Load & Release

<code>StageMetadata</code>를 Load할 때 다음과 같은 Coding Style을 지켜주는 것이 바람직하다

```
if(!StageMetadataInterface.TryLoadStageMetadata(m_stageNames[m_curSelectedStageIndex], m_curMajorDifficulty))
{
    // TODO - Error Handling
}
else if(!StageMetadataInterface.BIsStageMetadataValid())
{
    // TODO - Error Handling
}
```

- <code>StageMetadataInterface.TryLoadStageMetadata(...)</code>의 경우 정상적으로 <code>*.rgblinemt</code> 파일을 읽고, StageMetadata로 Deserialize했을 때 true를, 그렇지 않을 때 false를 Return한다
- <code>StageMetadataInterface.BIsStageMetadataValid()</code>는 <code>StageMetadataInterface.TryLoadStageMetadata(...)</code>를 통해 Load된 StageMetadata가 Valid한 지 검사한다. 만약 문제가 없으면 true를, 그렇지 않을 때 false를 Return한다

Load된 StageMetadata의 사용이 끝나면 다음과 같은 Coding Style을 지켜주는 것이 바람직하다

```
StageMetadataInterface.DisposeStageMetadata();
GC.Collect();
```

- <code>StageMetadataInterface.DisposeStageMetadata()</code>는 현재 Load된 StageMetadata 내부의 Reference Count를 제거하여 GC에 의해 필요없는 모든 자원이 최대한 Release 될 수 있도록 한다
- <code>GC.Collect()</code>를 임의로 호출함으로서 필요없는 자원을 검출해 Release하도록 한다

## Region, Line, Note Props Spawn & Despawn
<code>StageData</code>를 정상적으로 Load한 뒤라면 Region, Line, Note의 Prop들의 Rendering을 시도할 것이다

먼저, Region, Line, Note를 Rendering하기 위해서는 Scene에 <code>RegionManager</code>, <code>LineManager</code>, <code>NoteManager</code>를 배치해야만 한다

<p align="center">
    <image src="https://github.com/user-attachments/assets/025560d9-5213-473f-a90d-45da3fbe8b9f"
    width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/ce949e5c-b5b3-4c57-a36e-bfff733706cc" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/d4714af3-8ed4-4317-a43d-13b066eb44ba" width=400>
</p>

<code>LineManager</code>에 들어가는 Prefab의 경우 다음의 구조를 가진다

<p align="center">
    <image src="https://github.com/user-attachments/assets/aa9450db-790f-46f0-85f9-40d7a429fec7" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/152b9f82-81c2-4f15-89d6-6b73c10ae2f4" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/8e3adc2f-438a-4a94-b500-c1962b173152" width=400>
</p>

<code>NoteManager</code>에 들어가는 Prefab의 경우 다음의 구조를 가진다

<p align="center">
    <image src="https://github.com/user-attachments/assets/38b3a66e-755f-4695-a94b-562973dcfe5b" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/a0887dd5-bf27-4a58-b16f-10e68461cce8" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/7f95c8cd-e15f-4ea8-af89-2ff6c7e7014d" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/07706c7d-2818-429c-8ed7-287584de9e8f" width=400>
    <br>
    <image src="https://github.com/user-attachments/assets/38e96b10-0706-42e2-9c52-201b62c2199d" width=400>
</p>

<code>LineItem</code>와 <code>NoteItem</code>의 경우 각 Item들은 다음의 상속 구조를 가진다

```
LineItem
├── [RequireComponent(typeof(CurvedLineRenderer))] class LineItem : MonoBehaviour, ILineItem
├── [RequireComponent(typeof(CurvedLinePoint))] class LinePoint : MonoBehaviour, ILinePoint
└── class RedLineCornerNote : MonoBehaviour, IRedLineCornerNote

NoteItem
├── class NoteItem : MonoBehaviour, INoteItem
├── class RedAndBlueNote : MonoBehaviour, IRedAndBlueNote
└── class GreenNote : MonoBehaviour, IGreenNote
```

<code>RegionManager</code>, <code>LineManager</code>, <code>NoteManager</code>의 준비가 모두 끝난 후에는 각각 적절한 Method를 호출하여 Rendering을 시도할 수 있는데, 다음과 같은 Coding Style을 지켜주는 것이 바람직하다

```
RegionManager.Instance.SpawnRegionProps();
LineManager.Instance.SpawnLineProps();
Invoke("SpawnNotePropsCallback", 0.5f);

private void SpawnNotePropsCallback()
{
    NoteManager.Instance.SpawnNoteProps();
}
```

위의 코드는 Region, Line, Note의 Prop들을 Rendering해준다

Note Item들의 Spawn에 다소 딜레이를 주는 것은 Line Item이 모두 Rendering 되어야만 정상적으로 Note의 위치를 잡을 수 있기애 Line Item의 Rendering이 끝날 때까지 기다려주는 것이다

Prop들의 사용이 모두 끝나 Despawn하려 하는 경우에는 다음의 Coding Style을 지켜주는 것이 바람직하다

```
NoteManager.Instance.DespawnNoteProps();
LineManager.Instance.DespawnLineProps();
RegionManager.Instance.DespawnRegionProps();
```

Spawn의 경우 Region -> Line -> Note의 순으로 진행되었다면, Despawn의 경우 Note -> Line -> Region의 순으로 진행되는 것이다