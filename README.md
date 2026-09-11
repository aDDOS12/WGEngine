Markdown

# WGEngine (Tactical Battle Simulator)
**Version:** Alpha v0.1.0

## 🇬🇧 English Description
WGEngine is a custom-built 2D tactical battle engine tailored for medieval and renaissance eras. Inspired by the combat resolution systems of the *Total War* series, it provides a robust architecture for simulating text-based/forum-based RPG battles (WG). The engine automates movement, engagements, and casualty calculations, replacing manual spreadsheet management with a visual, data-driven sandbox.

### Key Features:
* **Total War-Inspired Combat:** Full combat simulation handling unit movement, directional facing, melee/ranged attacks, and automated damage/morale resolution.
* **Built-in Editors:** Dedicated in-engine editors for creating custom units (graphical NATO-style tokens, RGB faction colors, custom naming) and modifier templates.
* **Real-Time Flexibility:** Live editing of unit statistics directly on the battlefield.
* **Custom Tactical Maps:** Support for loading custom `.png` maps as battle backgrounds via the local AppData directory.
* **Automated Reporting:** A dedicated `BattleReportGenerator` that exports comprehensive end-of-battle statistics (KIA, wounded, routing status) to `.txt` files.
* **Architecture:** Event-driven design utilizing C# delegates, coupled with a proprietary JSON serialization system ensuring battle state integrity.

---

## 🇵🇱 Opis Polski
WGEngine to autorski silnik taktycznych bitew 2D osadzony w realiach ery średniowieczno-renesansowej. Projekt, zainspirowany mechaniką starć z serii *Total War*, służy do symulowania i rozstrzygania bitew na potrzeby gier tekstowych (WG). Silnik w pełni automatyzuje ruch, ataki oraz przeliczanie strat i morale, zastępując ręczne kalkulacje w arkuszach elastycznym środowiskiem graficznym.

### Główne funkcjonalności:
* **Symulacja w stylu Total War:** Pełny system obsługujący ruch jednostek, pozycjonowanie, ataki wręcz/dystansowe oraz zautomatyzowane liczenie obrażeń.
* **Wbudowane Edytory:** Narzędzia do tworzenia jednostek (wybór grafik dla plakietek, kolory RGB, własne nazewnictwo) oraz szablonów modyfikatorów.
* **Elastyczność Statystyk:** Możliwość swobodnej edycji parametrów bojowych oddziałów znajdujących się aktualnie na planszy.
* **Własne Mapy:** System wczytywania plików `.png` jako tła taktycznego bezpośrednio z folderu systemowego.
* **Zapis i Raportowanie:** Zautomatyzowany generator eksportujący szczegółowe raporty po bitwie (polegli, ranni, stan morale) do plików `.txt`.
* **Architektura:** W pełni zintegrowany, autorski system zapisu JSON oraz wzorce event-driven zapewniające wysoką stabilność logiki gry.

---

## Controls & Shortcuts
* **Drag & Drop:** Spawn units from the deployment list onto the board.
* **Shift + X:** Delete the currently selected unit (Deployment phase only).
* **Shift + N / Shift + M:** Align selected units horizontally or vertically.
* **ESC:** Open the Pause Menu (Save, Generate Report, Exit).

## System Directories (Windows)
WGEngine uses the `AppData/LocalLow` directory for all external assets and saves:
* **Custom Maps:** `%USERPROFILE%\AppData\LocalLow\DefaultCompany\WGEngine\Maps`
* **Battle Reports:** `%USERPROFILE%\AppData\LocalLow\DefaultCompany\WGEngine\Reports`
* **Save Files:** `%USERPROFILE%\AppData\LocalLow\DefaultCompany\WGEngine\Saves`

## Credits
* **Core Programming & Architecture:** Daniel Powichrowski (`aDDOS12`)
* **Graphical Assets & Icons:** Pulson

## License & Copyright
Copyright (c) 2026 Daniel Powichrowski. All rights reserved.

This repository is public for portfolio and educational purposes. You may view the code, but you may not copy, distribute, modify, or use it for commercial or non-commercial purposes without explicit permission. Graphics and visual assets are the property of their respective creators and are equally protected.
