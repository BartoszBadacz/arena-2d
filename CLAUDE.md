# Arena 2D (Unity) — zasady projektu

Pierwsza, celowo mała gra. Cel: przejść cały proces od pomysłu do publikacji.
Etapy i bieżący stan: [docs/roadmap.md](docs/roadmap.md). Dziennik decyzji i tarć: [docs/notatki.md](docs/notatki.md).

## Gra

- **Gatunek:** arena 2D z widokiem z góry — sterujesz postacią, przeciwnicy nadchodzą falami, celem jest przetrwać jak najdłużej i pobić rekord.
- **Silnik:** Unity, najnowsza wersja LTS, szablon „Universal 2D” (URP).
- **Publikacja:** przeglądarka (build WebGL/WebGPU) na itch.io. Później ten sam projekt na PC.
- **Zakres:** 2–4 tygodnie pracy wieczorami. Pierwsza gra ma być 10x mniejsza, niż podpowiada ambicja.
- **Poza zakresem:** 3D, multiplayer, fabuła, zapis postępu, wiele poziomów. Pilnuj tego — propozycje spoza zakresu odkładaj do „może kiedyś” w roadmapie.

## Zasady procesu

- **Prototyp najpierw, brudny i do wyrzucenia.** Szare prostokąty, jedna scena, cel: sprawdzić, czy pętla rozgrywki bawi. Zasady architektury poniżej w prototypie NIE obowiązują.
- **Prototyp żyje w `Assets/_Prototype/`** (kod, scena, prefaby, bez `.asmdef`). Nic z `_Project/` nie zależy od `_Prototype/`. Po prototypie folder usuwamy w całości.
- **Potem świadome przepisanie** tego, co przetrwało, na docelową architekturę.
- **Bez frameworka na zapas.** Nie budujemy teraz frameworka do wytwarzania gier. Zamiast tego prowadzimy `docs/notatki.md` (powtórzenia, tarcia, decyzje) — dopisuj tam wpisy na bieżąco, gdy coś się zdarzy.

## Architektura (po prototypie)

Clean architecture w lekkiej formie dopasowanej do Unity.

- **Cienki MonoBehaviour, gruba logika.** MonoBehaviour to tylko klej między silnikiem a logiką. Logika w zwykłych klasach C#, niezależnych od Unity i testowalnych.
- **Komunikacja przez zdarzenia**, nie przez bezpośrednie wywołania między systemami. Np. przeciwnik ogłasza „zginąłem”, a punkty, dźwięk i UI same nasłuchują.
- **Bez wszechwiedzącego `GameManager` i bez nadużywania singletonów.**
- **Gra sterowana danymi.** Wszystkie liczby (życie, szybkości, obrażenia, fale) w ScriptableObjectach, nie w kodzie.
- **Granice modułów przez Assembly Definitions (`.asmdef`).** Moduł `Core` nie zależy od Unity.
- **DI ręcznie, w jednym miejscu startowym** (`GameBootstrap` w module `App`, wstrzykiwanie metodą `Init(...)`). Bez bibliotek DI na tym etapie.
- **Stan czytamy, na fakty reagujemy.** Stan (HP, fala, liczniki) to właściwości tylko do odczytu w `Core`, a UI czyta je co klatkę. Fakty (trafienie, śmierć, start fali) to zdarzenia C# (`event`). Bez globalnej szyny zdarzeń.
- **`Core` nie zna Unity ani ScriptableObjectów:** czas jako argument, losowość jako `System.Random`, ustawienia przez `ToSettings()` na SO, wektory `System.Numerics.Vector2`.

```
Assets/_Project/
  Scripts/
    Core/          ← czysta logika C# (Health, WaveLogic, Score) – osobny .asmdef, testy
    Gameplay/      ← MonoBehaviour: gracz, przeciwnicy, broń, spawner
    Presentation/  ← UI, efekty, dźwięk – czytają stan i nasłuchują zdarzeń z Core
    Data/          ← definicje ScriptableObject (EnemyDefinition, WaveConfig)
    App/           ← composition root (GameBootstrap) – jedyny moduł, który widzi wszystkie
  Data/            ← konkretne pliki danych: Slime.asset, Runner.asset, Waves.asset
  UI/              ← UXML, USS, PanelSettings, motyw (.tss)
  Prefabs/  Art/  Audio/  Scenes/
```

Zaimportowane paczki assetów leżą poza `_Project/`, w osobnych folderach.

## Repo i kontrola wersji

- **Pliki `.meta` zawsze w repo.** Unity łączy assety po identyfikatorze (GUID) z `.meta`, nie po ścieżce.
- **Przenoszenie i zmiana nazw plików tylko w edytorze Unity** albo razem z `.meta`. Nigdy samo `mv` na pliku bez jego `.meta`.
- **Ustawienia edytora:** Version Control = Visible Meta Files, Asset Serialization = Force Text.
- **`.gitignore`:** standardowy dla Unity (`Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`, `UserSettings/`, `*.csproj`, `*.sln`).
- **Git LFS** na pliki binarne (png, psd, wav, mp3, fbx, ttf itd.) przez `.gitattributes`.
- **Pierwszy commit:** najpierw `.gitignore` i `.gitattributes`, dopiero potem projekt.
- Mało rzeczy bezpośrednio w scenach, dużo w prefabach (mniej konfliktów).
- Scen i assetów nie edytuj ręcznie w YAML, jeśli da się to zrobić przez edytor (Unity CLI / MCP).

## Assety

- **Na start:** darmowe paczki (np. Kenney, CC0) jako materiał zastępczy.
- **Skala ustalona z góry** (piksele na jednostkę świata, rozmiar grafiki) i zapisana w `docs/notatki.md` — podmiana grafiki ma być wymianą plików, nie przepisywaniem kodu.
- **Higgsfield** — później: okładka i zrzuty na itch.io, ewentualnie tła. Nie do animacji postaci.
- **Blender** — odłożony.

## Praca agenta z edytorem Unity

Reguły wyciągnięte z tarć w `docs/notatki.md`.

- **Narzędzia:** serwer MCP `unity-editor` (`.mcp.json`) do sceny/prefabów/Play/zrzutów; Unity CLI pod pełną ścieżką `C:/Users/Bartek/AppData/Local/Unity/bin/unity.exe` (w PATH sesji może go nie być).
- **Przed edycją `.cs`:** sprawdź, czy edytor jest w trybie Play, i zatrzymaj go. Rekompilacja w trakcie gry podmienia kod na żywo i sypie wyjątkami.
- **Po edycji `.cs`:** edytor przeładowuje kod (domain reload), a MCP na ten czas się rozłącza. Poczekaj przez CLI na `unity status` → `ready`, potem wywołaj `unity recompile` (wymusza wczytanie zmian i zwraca błędy kompilacji) i dopiero wtedy wróć do MCP. Samo `ready` nie znaczy „nowy kod wczytany”: gdy okno Unity nie ma fokusu, auto-refresh może nie ruszyć. Jeśli `recompile` zwróci 400, trafił w przeładowanie z auto-refresh — poczekaj na `ready` i powtórz.
- **Przed testami EditMode:** sprawdź, czy otwarte sceny nie mają niezapisanych zmian (`isDirty`). Test Runner pyta wtedy o zapis modalnym oknem, które blokuje edytor i MCP. Gdy edytor nagle przestaje odpowiadać, najpierw szukaj okna dialogowego. Wyjątki `ObjectDisposedException` z serwera Pipeline w konsoli to artefakt tego wyścigu, nie błąd gry — wyczyść konsolę.
- **Wartości `[SerializeField]`:** zmiana wartości domyślnej w kodzie NIE zmienia istniejących obiektów w scenie ani prefabów. Ustaw je też przez `SerializedObject` (scena) albo `PrefabUtility.LoadPrefabContents` (prefab).
- **Testy w Play:** logikę sprawdzaj przez `script eval` (stan, refleksja), wynik wizualny przez zrzut Game view. Symulowane klawisze nie zawsze docierają (fokus okna Game) — nie opieraj na nich testu.
- **Użytkownik może właśnie grać** w edytorze. Zanim uruchomisz testy lub zatrzymasz Play, sprawdź, czy gra nie jest aktywna z jego udziałem. Przed każdym wejściem w Play i każdą zmianą stanu gry (refleksja, nieśmiertelność, handlery testowe) sprawdź `editor_status` — jeśli Play trwa, a nie włączył go agent, to gra użytkownika: nie ruszaj jej.
- **Rytm pracy:** mały krok → test w Play → commit + push → wpis w `docs/notatki.md` przy tarciu lub decyzji → odhaczenie w `docs/roadmap.md`.
