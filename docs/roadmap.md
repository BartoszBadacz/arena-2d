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

Do zaplanowania po setupie: pętla rozgrywki, lista „musi być” / „może kiedyś”, kolejność budowania.
Pytanie, na które prototyp odpowiada: **czy to bawi?**

## Etap 2 — Przepisanie na docelową architekturę

Tylko to, co przetrwało prototyp. Zasady w `CLAUDE.md`.

## Etap 3 — Grafika, dźwięk, szlif

## Etap 4 — Publikacja na itch.io (WebGL/WebGPU)

## Może kiedyś

(pomysły spoza zakresu lądują tutaj, nie w kodzie)
