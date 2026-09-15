# RAXY Dialogue System

> **DEPRECATED:** This package is replaced by [RAXY Narrative](https://github.com/RobyRAX/RAXY-Narrative). Prefer the replacement for new projects.

RAXY Dialogue System provides a modular dialogue foundation for Unity projects: dialogue data, controllers, timeline integration, portraits, and options.

## Features

- **DialogueSO / DialogueData** — branching dialogue graphs with actions and options
- **DialogueManager** — routes dialogue by type to registered controllers
- **DialogueController_MainTimeline / MainNonTimeline** — timeline-driven and manual dialogue playback
- **DialoguePortrait / PortraitState** — animated portrait display with DOTween
- **OptionController / OptionData** — player choice UI with event hooks
- **Timeline markers & track** — `StartDialogueMarker`, `PauseTimelineMarker`, `DialogueMarkerTrack`
- **DialogueTrigger** — start dialogue from world interactions

## Setup

1. Add `DialogueManager` to your bootstrap scene and register `DialogueControllerBase` implementations.
2. Create `DialogueSO` assets and assign actors via `DialogueActorSO`.
3. For timeline dialogue, use `DialogueController_MainTimeline` with PlayableDirector.
4. Wire `DialogueTrigger` or call `DialogueManager` APIs to start conversations.

## Dependencies

- **RAXY Event** (`com.raxy.event`) — dialogue action events
- **RAXY UI** (`com.raxy.ui`) — `TextTyper` and UI helpers
- **RAXY Utility** (`com.raxy.utility`) — `Singleton`, `CustomDebug`
- **RAXY Localization** (`com.raxy.utility.localization`) — localized option text
- **UniTask** (`com.cysharp.unitask`) — async dialogue manager hooks
- **Unity Addressables** — actor portrait addressable references
- **Unity Timeline** — timeline marker integration
- **Unity UGUI** — TextMeshPro and UI components
- **DOTween** (project plugin) — portrait animations; required in consuming project
- **Odin Inspector** (project plugin) — editor attributes and custom inspectors

## Notes

Game-specific dialogue bridges and notification hooks should live in your project, not in this package.
