# Train Survival Shooter (Unity URP, 2022.3+ LTS)

## 1) Краткая архитектура

Проект разделён на независимые системы:

- **Core**: `GameManager`, валюта, интерфейсы взаимодействия/урона.
- **Player**: управление third-person через `CharacterController`, здоровье, интеракции.
- **Weapons**: стартовый пистолет + прокачка через `WeaponData`.
- **Enemies**: пауки-преследователи с ближней атакой.
- **Waves**: ручной запуск волн с масштабированием сложности.
- **Train**: интерактивная консоль запуска волны и визуальный фейк-движения поезда.
- **Upgrades**: станции прокачки через `ScriptableObject`-апгрейды.
- **UI**: HUD, панель апгрейдов, game over.
- **Visuals**: кинематографичный sway/импульсы камеры.

Архитектура строится на событиях (`OnHealthChanged`, `OnCurrencyChanged`, `OnWaveChanged`) и ScriptableObject-конфигах для масштабирования.

## 2) Структура проекта

```text
Assets/
  Scenes/
    Main.unity (создаётся в Editor)
  Scripts/
    Core/
    Player/
    Weapons/
    Enemies/
    Waves/
    Train/
    Upgrades/
    UI/
    Visuals/
  Settings/
```

## 3) Список созданных скриптов

- `Assets/Scripts/Core/IDamageable.cs`
- `Assets/Scripts/Core/IInteractable.cs`
- `Assets/Scripts/Core/CurrencySystem.cs`
- `Assets/Scripts/Core/GameManager.cs`
- `Assets/Scripts/Player/PlayerStats.cs`
- `Assets/Scripts/Player/PlayerHealth.cs`
- `Assets/Scripts/Player/PlayerController.cs`
- `Assets/Scripts/Player/PlayerInteractor.cs`
- `Assets/Scripts/Weapons/WeaponData.cs`
- `Assets/Scripts/Weapons/WeaponController.cs`
- `Assets/Scripts/Enemies/EnemyData.cs`
- `Assets/Scripts/Enemies/SpiderEnemy.cs`
- `Assets/Scripts/Waves/WaveState.cs`
- `Assets/Scripts/Waves/WaveConfig.cs`
- `Assets/Scripts/Waves/WaveManager.cs`
- `Assets/Scripts/Train/WaveStartConsole.cs`
- `Assets/Scripts/Train/TrainMotionVisual.cs`
- `Assets/Scripts/Upgrades/UpgradeType.cs`
- `Assets/Scripts/Upgrades/UpgradeDefinition.cs`
- `Assets/Scripts/Upgrades/UpgradeStation.cs`
- `Assets/Scripts/UI/UIManager.cs`
- `Assets/Scripts/UI/CloseUpgradePanelButton.cs`
- `Assets/Scripts/Visuals/CameraImpulse.cs`

## 4) Пошаговая сборка сцены (Main)

### 4.1 URP и проект
1. Создайте Unity 2022.3+ LTS URP project.
2. Скопируйте папку `Assets/Scripts` из репозитория.
3. В `Project Settings > Player` включите New Input старый Input Manager оставить включённым (Both), т.к. код использует `Input.GetAxis`.

### 4.2 База сцены (поезд)
1. Создайте `GameObject > 3D Object > Cube`: `TrainPlatform`.
   - Scale: `(8, 1, 40)`.
2. Добавьте 2-3 боковых куба как борта вагона.
3. Позади поезда создайте 6 пустых `SpawnPoint_*` (z от `-20` до `-35`, x в диапазоне `-7..7`).
4. Создайте `WaveConsole` (Cylinder + emissive material), добавьте `WaveStartConsole`, collider оставить.
5. Создайте 2 станции прокачек (`PlayerUpgradeStation`, `WeaponUpgradeStation`) как светящиеся терминалы.

### 4.3 Игрок (third-person)
1. Создайте `Player` (Capsule) с `CharacterController`.
2. Добавьте компоненты:
   - `PlayerStats`
   - `PlayerHealth`
   - `PlayerController`
   - `PlayerInteractor`
3. Создайте дочерний `CameraPivot` (примерно y=1.6).
4. Камеру сделайте дочерней у `CameraPivot` с локальной позицией `(0, 1.4, -3.2)` и лёгким наклоном вниз.
5. В `PlayerController` привяжите `stats`, `cameraPivot`, `playerCamera`.
6. В `PlayerInteractor` укажите `cam`, `range=4`, `interactionMask` = слой `Interactable`.

### 4.4 Оружие
1. Создайте `Weapon_Pistol` как дочерний объект игрока.
2. Добавьте `WeaponController`.
3. Добавьте `Muzzle` точку на стволе, `ParticleSystem` для вспышки.
4. Создайте `TrailRenderer` prefab для трассера.
5. Создайте `WeaponData` (`Create > TrainSurvival > Weapon Data`) и назначьте в `WeaponController`.
   - Damage: 20, FireRate: 4, Range: 80, MagSize: 12, Reload: 1.35.
   - `hitMask`: враги + world.

### 4.5 Враг-паук
1. Создайте prefab `Enemy_Spider`:
   - body: сфера + 8 цилиндров-лап (placeholder).
   - `CharacterController`.
   - `SpiderEnemy`.
2. Создайте `EnemyData` (`Create > TrainSurvival > Enemy Data`), назначьте в `SpiderEnemy`.
3. Добавьте небольшой `deathVfx` particle (красно-оранжевые искры/дым).

### 4.6 Волны
1. Создайте пустой `WaveSystem` и добавьте `WaveManager`.
2. Создайте `WaveConfig` (`Create > TrainSurvival > Wave Config`):
   - BaseEnemyCount=6
   - AddedPerWave=3
   - hp/speed/dmg multipliers умеренные.
3. Назначьте prefab врага, `WaveConfig` и все spawn points.

### 4.7 Экономика/менеджмент
1. Создайте пустой `Systems`.
2. Добавьте `CurrencySystem`, `GameManager`, `UIManager` (можно на Canvas object).
3. В `GameManager` привяжите `PlayerHealth`, `WaveManager`, `CurrencySystem`, `UIManager`.

### 4.8 Апгрейды
1. Создайте минимум 6 `UpgradeDefinition` ассетов:
   - Player HP, Move Speed, Damage Reduction
   - Weapon Damage, Fire Rate, Reload, Magazine
2. На `PlayerUpgradeStation` добавьте `UpgradeStation`, в список добавьте player-апгрейды.
3. На `WeaponUpgradeStation` добавьте `UpgradeStation`, в список weapon-апгрейды.
4. Назначьте ссылки на `PlayerStats`, `PlayerHealth`, `WeaponController`.

### 4.9 UI
1. Создайте `Canvas` (Screen Space Overlay), добавьте тёмный футуристичный HUD.
2. Текстовые поля TMP:
   - `HealthText`, `AmmoText`, `CurrencyText`, `WaveText`, `WaveStateText`, `InteractionText`.
3. Создайте `UpgradePanel`:
   - Title TMP
   - `VerticalLayoutGroup` root
   - Button prefab entry (`upgradeEntryPrefab`) + TMP label
   - Close button с `CloseUpgradePanelButton.Close()`
4. Создайте `GameOverPanel`:
   - затемнённый фон, заголовок, кнопка Restart → `GameManager.RestartScene()`.
5. Заполните все ссылки в `UIManager`.

### 4.10 Освещение и атмосфера (wow factor)
1. **Lighting**:
   - Directional Light: ночной сине-зелёный тон, низкий угол.
   - 2-4 spotlights на поезде (тёплые, контрастные).
2. **Fog**:
   - Включите RenderSettings fog (Exp2, тёмно-синий).
3. **URP Volume (Global)**:
   - Bloom (intensity ~0.6-1.2)
   - Color Adjustments (post exposure -0.15, contrast +20)
   - Lift/Gamma/Gain для teal-orange контраста
   - Vignette 0.2-0.35
   - Film Grain 0.1-0.2
4. **Материалы**:
   - Поезд: металлик 0.7, smoothness 0.6
   - Консоли/станции: emissive cyan/orange
   - Враги: тёмный матовый + emissive red eyes
5. **Фон-движение**:
   - Большие plane/cubes вокруг сцены с текстурным скроллом.
   - `TrainMotionVisual` на manager-объекте, привяжите renderers/looping props.
6. **VFX**:
   - muzzle flash, bullet trail, impact sparks, death smoke.
   - Добавьте sparks/embers particle около колёс/низа поезда.

## 5) Проверка игрового цикла

1. Play -> игрок на поезде.
2. Подойти к `WaveConsole`, нажать `E`.
3. Волна спавнится позади, враги бегут к игроку.
4. ЛКМ стреляет, `R` перезаряжает.
5. За убийства и волну выдаётся валюта.
6. На станции апгрейдов открыть панель (`E`), купить улучшения.
7. Снова стартовать волну вручную.
8. При смерти показать Game Over + Restart.

## 6) Рекомендации по дальнейшей полировке

- Добавить Animator пауку (run/attack/death).
- Camera shake при попаданиях и выстреле (вызовы `CameraImpulse.AddRecoil`).
- Добавить world-space UI маркеры врагов в тумане.
- Заменить static skybox на procedural storm sky + lightning flashes.
- Добавить низкочастотный wind/rail rumble в AudioMixer.

