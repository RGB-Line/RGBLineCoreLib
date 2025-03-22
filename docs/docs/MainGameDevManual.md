# Main Game Dev Manual

Main Game의 경우 Lobby에서는 <code>StageMetadata</code>, 주 Game Screen에서는 <code>StageData</code>를 주로 사용하게 된다

## Lobby에서

Lobby에서는 음악을 바꾸고, 같은 음악 내에서도 난이도를 바꿀 때 모두 새로운 <code>StageMetadata</code>를 Load하고 기존의 것을 Release해줘야만 한다

<code>StageMetadata</code>의 Load/Release 방법과 바람직한 Coding Style을 <U>***Main Game, Editor Common Dev Manual***</U>에서 확인하길 바란다

## 주 Game Screen에서

주 Game Screen의 경우 Lobby로부터 <code>StageLoadInfoDTO</code>를 전달받아 이를 바탕으로 <code>StageData</code>를 Load하고 Region, Line, Note의 Prop들을 Spawn하게 된다. 그리고 모종의 이유로 Game Over/Game End가 될 경우 Prop들을 Despawn하고 그 다음으로 <code>StageData</code>를 Release하게 될 것이다

<code>StageData</code>의 Load/Release 그리고 Region, Line, Note Prop들의 Spawn/Despawn 방법과 바람직한 Coding Style을 <U>***Main Game, Editor Common Dev Manual***</U>에서 확인하길 바란다