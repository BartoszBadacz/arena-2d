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
- **Potem świadome przepisanie** tego, co przetrwało, na docelową architekturę.
- **Bez frameworka na zapas.** Nie budujemy teraz frameworka do wytwarzania gier. Zamiast tego prowadzimy `docs/notatki.md` (powtórzenia, tarcia, decyzje) — dopisuj tam wpisy na bieżąco, gdy coś się zdarzy.

## Architektura (po prototypie)

Clean architecture w lekkiej formie dopasowanej do Unity.

- **Cienki MonoBehaviour, gruba logika.** MonoBehaviour to tylko klej między silnikiem a logiką. Logika w zwykłych klasach C#, niezależnych od Unity i testowalnych.
- **Komunikacja przez zdarzenia**, nie przez bezpośrednie wywołania między systemami. Np. przeciwnik ogłasza „zginąłem”, a punkty, dźwięk i UI same nasłuchują.
- **Bez wszechwiedzącego `GameManager` i bez nadużywania singletonów.**
- **Gra sterowana danymi.** Wszystkie liczby (życie, szybkości, obrażenia, fale) w ScriptableObjectach, nie w kodzie.
- **Granice modułów przez Assembly Definitions (`.asmdef`).** Moduł `Core` nie zależy od Unity.
- **DI ręcznie, w jednym miejscu startowym.** Bez bibliotek DI na tym etapie.

```
Assets/_Project/
  Scripts/
    Core/          ← czysta logika C# (Health, WaveLogic, Score) – osobny .asmdef, testy
    Gameplay/      ← MonoBehaviour: gracz, przeciwnicy, broń, spawner
    Presentation/  ← UI, efekty, dźwięk – nasłuchują zdarzeń
    Data/          ← definicje ScriptableObject (EnemyDefinition, WaveConfig)
  Data/            ← konkretne pliki danych: Slime.asset, Wave01.asset
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
