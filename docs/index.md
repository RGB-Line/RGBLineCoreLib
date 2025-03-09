<p align="center">
  <img src="https://github.com/user-attachments/assets/9f2e8c0d-7701-4050-ae0e-4d59992ec7b6" alt="Simple Icons" width=70>
  <h3 align="center">RGB Line Core Lib</h3>
</p>

<p align="center">
  <img alt="Static Badge" src="https://img.shields.io/badge/Lang-CSharp-blue">
  <img alt="Static Badge" src="https://img.shields.io/badge/Target-Unity-green">
  <br>
  <img alt="Static Badge" src="https://img.shields.io/badge/Usage-Plugin-red">
</p>

<br>

## Abstract
본 Plugin은 RGB Line Project의 핵심 기능을 담고 있다

Plugin은 ```RGBLineCoreLib.dll```과 ```RGBLineCoreLib_ForEditor.dll```의 두 가지 형태로 제공된다

## Usage
> [!IMPORTANT]\
> Editor의 경우 필히 ```RGBLineCoreLib_ForEditor.dll```를 사용할 것

## Common Dependency
```
UnityEngine.dll
CommonUtilLib.dll
Newtonsoft.Json.dll
```



## Features
- Common Utils
- Resource Management
- Map Generation
- Tool
- In-Game Management
    - Map Streaming Management
    - PC/NPC Movement

## Module Tree
```
Fogotten
├── Common Util
│   └── <Repo> CommonUtilLib
├── Resource Management
│   ├── <Repo> ResourcePathManagementLib
│   └── <Repo> ResourceDataManagementLib
├── Map Generation
│   ├── <Repo> ChiefMapGenerationLib
│   └── <Repo> MapGenerationAgent
├── Tool
│   └── <Repo> MapGenerationInputTool
└── In-Game Management
    └── PC/NPC Movement
        └── <Repo> PcNpcMovementLib
```

## Hands-On Manual