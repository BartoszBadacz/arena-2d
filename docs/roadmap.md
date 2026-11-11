# Roadmap

Stan na bieżąco: odhaczaj zrobione, dopisuj, co wyszło po drodze.

## Etap 0 — Setup

Po mojej stronie:
- [x] konto Unity i zalogowanie w Unity Hub (licencja Personal)
- [x] Git + Git LFS zainstalowane
- [x] `gh` zalogowane do GitHuba
- [x] repo na GitHubie: `BartoszBadacz/arena-2d` (publiczne)

Po stronie Claude:
- [x] instalacja Unity 6.3 LTS (6000.3.28f1) przez Unity CLI + moduł WebGL
- [x] projekt z szablonu „Universal 2D”, platforma buildu: WebGL
- [x] `.gitignore`, `.gitattributes` z LFS, ustawienia edytora (Visible Meta Files, Force Text)
- [x] struktura `Assets/_Project/` i pliki `.asmdef`, folder `Assets/_Prototype/`
- [x] pierwszy commit w dobrej kolejności i push
- [x] projekt otwiera się bez błędów (0 błędów, 0 ostrzeżeń)
- [x] VS Code: pakiet „Visual Studio Editor” w Unity + rozszerzenie Unity w VS Code, VS Code ustawiony jako edytor skryptów
- [x] Unity MCP — agent steruje edytorem (`.mcp.json`, pełna ścieżka do `unity.exe`)
- [x] granice `.asmdef` sprawdzone w praktyce (`Arena.Core` zależy tylko od `netstandard`, krok 2 Etapu 2)

## Etap 1 — Prototyp (szare prostokąty)

Pytanie, na które prototyp odpowiada: **czy to bawi?**

Pętla: ruch → strzał → wróg ginie → przetrwana fala → trudniejsza fala → śmierć → wynik vs rekord → „jeszcze raz”.
Sterowanie: WASD + celowanie i strzał myszką (twin-stick). Bez ulepszeń między falami.

Kolejność (każdy krok kończy się czymś grywalnym w Play):
- [x] 1. Ruch gracza w zamkniętej arenie
- [x] 2. Strzelanie w stronę kursora
- [x] 3. Przeciwnik goni gracza, da się go zabić
- [x] 4. Życie gracza, śmierć, Game Over + restart ← od tu to jest gra
- [x] 5. Fale do wybicia (coraz liczniejsze i szybsze) + przerwy z odliczaniem + rekord (najwyższa fala) w sesji
- [x] Ocena: czy to bawi? → **tak**: szybki przebieg, krótkie rundy, restart „nie boli” (playtest 4, 2026-11-07)
- [x] Decyzja: zakres wersji 1 → rdzeń z prototypu + typy wrogów jako domieszka (patrz Etap 2)

## Etap 2 — Przepisanie na docelową architekturę

Tylko to, co przetrwało prototyp. Zasady w `CLAUDE.md`.

Zakres wersji 1:
- rdzeń z prototypu (ruch, strzelanie, fale do wybicia, bezpieczny spawn, separacja, życie, Game Over, rekord w sesji — od playtestu balansu liczony w punktach)
- liczby w ScriptableObjectach: `EnemyDefinition` (typ wroga), `WaveConfig` (krzywa fal + domieszki)
- typy wrogów jako **domieszka** w falach, nie osobne fale: reguły „od fali X, szansa Y%, maks. Z na falę” — balans przez playtesty

Kolejność (każdy krok grywalny w Play, scena `Assets/_Project/Scenes/Arena.unity`; plan i decyzje w `docs/notatki.md`, 2026-11-07):
- [x] 1. Szkielet + ruch: moduł `App` z `GameBootstrap`, `PlayerDefinition` (`Player.asset`), `PlayerMover`, prefaby `Player` i `Arena`
- [x] 2. Strzelanie: `Weapon` (Core, pierwsze testy), `WeaponDefinition`, `PlayerShooter`, `Projectile`, warstwy + Layer Collision Matrix
- [x] 3. Wróg: `Health` i `Separation` (Core + testy), `Slime.asset`, prefab wroga, `HitFlash`, tymczasowy prosty spawner
- [x] 4. Życie gracza, Game Over, restart: nietykalność, `RunSession`, `RunStats`, `SessionRecord`, minimalny HUD (UI Toolkit) ← od tu znowu gra
  - [x] 4a. logika: życie gracza, obrażenia od kontaktu, Game Over z pauzą (wynik w konsoli), restart R, rekord sesji
  - [x] 4b. HUD i ekran Game Over w UI Toolkit
- [x] 5. Fale: `WaveDirector`, `SpawnPointPicker` (+ testy), `Waves.asset` z liczbami z prototypu, baner/odliczanie → **playtest A/B z prototypem**
  - [x] 5a. kod i dane fal, HUD fal; prędkość gracza poprawiona na 9 (jak w scenie prototypu)
  - [x] 5b. playtest A/B: na zmianę `Prototype.unity` i `Arena.unity` — czy czuć to samo? → tak (2026-11-11)
- [x] 6. Usunięcie `Assets/_Prototype` (po A/B; najpierw kontrola odwołań GUID z `_Project` do `_Prototype`) → tag `prototype-final`
- [x] 7. Domieszki: `EnemyMixer` (+ testy), `mixIns` w `WaveConfig`, `Runner.asset`, `Tank.asset`, warianty prefabu
- [ ] Playtest balansu typów wrogów: liczby Runner/Tank i reguły domieszek w `Waves.asset` (strojenie bez kodu)
  - [x] runda 1 (2026-11-11): fale rzadsze, punkty i rekord w punktach, Runner nie atakuje i leczy +1 HP
  - [x] runda 2: Runner chowa się w tłumie, limit 1 na falę rosnący z falami; krzywa fal z malejącym przyrostem; skoki w bok „jak mucha”
  - [ ] dalej: użytkownik zbiera pomysły (balans „jeszcze do pracy”)

Kolejność: najpierw rdzeń w nowej architekturze, potem typy (wtedy nowy typ = nowy plik danych, nie nowy kod).

## Następna sesja

Kolejność ustalona 2026-11-11:
1. **Próbny build WebGL** („staging” z Etapu 4 wyciągnięty na początek): przejście całej ścieżki od zera do gry w przeglądarce, zanim zaczniemy upiększać. Sprawdzić: mysz i celowanie w ramce strony, wydajność późnych fal, rozmiar i czas ładowania, skalowanie UI.
2. **Podsumowanie projektu dla użytkownika**: przejście przez to, co jest (moduły, przepływ zdarzeń, dane, testy), bo zmiany z 2026-11-11 poszły szybko i trzeba je spokojnie zrozumieć.

## Etap 3 — Grafika, dźwięk, szlif

Zarys (do rozpisania po próbnym buildzie):
- skala grafiki ustalona i zapisana w `notatki.md` przed pierwszym obrazkiem (PPU, rozmiar sprite'ów)
- grafika Kenney (CC0): gracz, 3 typy wrogów, pocisk, podłoga, ściany — podmiana sprite'ów w prefabach i wariantach
- dźwięk w Presentation, nasłuchujący istniejących zdarzeń (`Damaged`, `EnemyKilled`, `WaveStarted`, `GameOver`)
- juice: potrząsanie kamerą przy trafieniu gracza, cząsteczki przy śmierci wroga
- ekran startowy („kliknij, aby zacząć” — w przeglądarce potrzebny też do odblokowania dźwięku)

## Etap 4 — Publikacja na itch.io (WebGL/WebGPU)

## Może kiedyś

(pomysły spoza zakresu lądują tutaj, nie w kodzie)

- ulepszenia między falami (wybór 1 z 3) — pierwszy kandydat po wersji 1. Pomysł z playtestu balansu: punkty/zasoby jako waluta na ulepszenia (np. szybkostrzelność), a tank jako „skarb” — pojawia się na jakiś czas i znika niezabity
- ~~eksperyment: wrogowie się łączą w większe byty~~ (slime: łączenie przy kontakcie, rozpad po trafieniu jak w Asteroids; wymaga mniejszych pojedynczych wrogów). Pomysł z playtestu 3, odrzucony z wersji 1 — cięcie zakresu 10x.
- ~~2–3 typy przeciwników (szybki, gruby)~~ — weszły do wersji 1 (Runner, Tank); zostaje: strzelający
- dash / unik
- juice: potrząsanie kamerą, błysk przy trafieniu, cząsteczki
- dźwięk, grafika (Kenney)
- trwały rekord (formalnie poza zakresem: „zapis postępu”)
- tryb auto-strzału w stylu Vampire Survivors
