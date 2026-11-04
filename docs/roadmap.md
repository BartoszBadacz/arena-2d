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
- [ ] granice `.asmdef` sprawdzone w praktyce (przy pierwszym skrypcie w `_Project/`)

## Etap 1 — Prototyp (szare prostokąty)

Pytanie, na które prototyp odpowiada: **czy to bawi?**

Pętla: ruch → strzał → wróg ginie → przetrwana fala → trudniejsza fala → śmierć → wynik vs rekord → „jeszcze raz”.
Sterowanie: WASD + celowanie i strzał myszką (twin-stick). Bez ulepszeń między falami.

Kolejność (każdy krok kończy się czymś grywalnym w Play):
- [x] 1. Ruch gracza w zamkniętej arenie
- [x] 2. Strzelanie w stronę kursora
- [x] 3. Przeciwnik goni gracza, da się go zabić
- [x] 4. Życie gracza, śmierć, Game Over + restart ← od tu to jest gra
- [ ] 5. Fale (coraz liczniejsze i szybsze) + licznik + rekord w sesji
- [ ] Ocena: czy to bawi? Co zostaje, co wyrzucamy?

## Etap 2 — Przepisanie na docelową architekturę

Tylko to, co przetrwało prototyp. Zasady w `CLAUDE.md`.

## Etap 3 — Grafika, dźwięk, szlif

## Etap 4 — Publikacja na itch.io (WebGL/WebGPU)

## Może kiedyś

(pomysły spoza zakresu lądują tutaj, nie w kodzie)

- ulepszenia między falami (wybór 1 z 3) — pierwszy kandydat, jeśli prototyp okaże się płaski
- 2–3 typy przeciwników (szybki, gruby, strzelający)
- dash / unik
- juice: potrząsanie kamerą, błysk przy trafieniu, cząsteczki
- dźwięk, grafika (Kenney)
- trwały rekord (formalnie poza zakresem: „zapis postępu”)
- tryb auto-strzału w stylu Vampire Survivors
