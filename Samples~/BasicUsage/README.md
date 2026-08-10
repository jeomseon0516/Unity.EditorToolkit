# EditorToolkit 기본 예제

`EditorToolkitSample`은 다른 패키지가 재사용하는 EditorToolkit의 공개 API를
한 화면에서 확인하는 예제입니다. `Jeomseon/Samples/Editor Toolkit Sample`
메뉴로 창을 엽니다.

## 확인 항목

1. **`ToggleEnumerator<T>` + `GUIStyleTexture`**: 과일 이름 3개를 토글로 나열합니다.
   하나를 선택하면 `GUIStyleTexture.Create`로 생성된 배경색으로 강조되고,
   "Selected" 라벨에 선택된 값이 표시됩니다.
2. **`EditorDropdownController<T>`**: "Priority" 드롭다운에서 옵션을 선택하면
   "Selected Value" 라벨이 즉시 갱신됩니다.
3. **`EditorGUILayoutActions.ActionEditorVerticalBox`**: 10줄짜리 목록이
   테두리가 있는 박스 안에서 스크롤되는지 확인합니다.
4. **`EditorGUILayoutActions.ActionEditorVertical`**: `HelpBox`가 박스 스타일의
   세로 그룹 안에 감싸져 표시되는지 확인합니다.
5. **`EditorTypeDiscovery`**: "Discover Concrete Component Types" 버튼을 누르면
   현재 프로젝트의 구체적인 `Component` 파생 타입 개수와 상위 5개 이름이 표시됩니다.
6. **`SerializedPropertyExtensions.GetPropertyType()`**: 예제 데이터의
   `_title`(String), `_amount`(Int32), `_isEnabled`(Boolean) 필드 각각에서
   리플렉션으로 해석한 실제 C# 타입 이름이 표시되는지 확인합니다.

## IconCreator 검증

`Icons/SampleIconRed.png`, `SampleIconGreen.png`, `SampleIconBlue.png`, `SampleIconYellow.png`
4개는 128x128 단색 아이콘으로 미리 생성해 Sprite(Single) 임포트 설정까지 포함해뒀습니다.
`SampleIconCreatorPreset.asset`은 이 4개 스프라이트를 `size=128`, `divideCount=2`로 미리
구성한 `IconCreatorPreset`입니다(2x2 격자로 정확히 채워짐).

검증 절차:

1. `Jeomseon/Icon Creator` 메뉴로 IconCreator 창을 엽니다.
2. Preset 필드에 `SampleIconCreatorPreset`을 지정하고 "Load Preset"을 누릅니다.
   Icon Sources 목록에 4개 스프라이트가, Size/Divide Count에 128/2가 채워지는지 확인합니다.
3. "Preview"를 눌러 256x256 아틀라스(2x2, 128px 셀)에 4가지 색이 올바른 위치에
   배치되는지 확인합니다.
4. "Create Icon"을 눌러 저장 경로를 지정하고, 저장된 PNG의 Sprite 메타(4개 SpriteRect)와
   위치가 Preview와 일치하는지 확인합니다. `generateTmpSpriteAsset`은 샘플 프리셋에서
   기본값을 꺼두었으니, TMP_SpriteAsset 생성까지 확인하려면 Preset Inspector에서
   체크박스를 켠 뒤 다시 시도합니다.

샘플 이미지와 프리셋은 Unity가 아닌 외부에서 생성한 자산이라, Import 시 Sprite 임포트
설정과 Preset의 스프라이트 참조가 깨지지 않았는지 1회 확인이 필요합니다(깨졌다면 Preset의
`Default Icon Sources` 필드가 "Missing" 또는 "None"으로 표시됩니다 — 이 경우 4개 PNG의
Texture Type을 Sprite(2D and UI)/Single로 직접 재설정한 뒤 Preset 필드를 다시 채워주세요).
