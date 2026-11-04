# Notatki pod przyszły framework

Materiał do wyciągania wniosków po kilku grach. Trzy rodzaje wpisów:

- **Powtórzenie** — coś zrobione po raz kolejny. Po raz trzeci = kandydat na regułę/automatyzację.
- **Tarcie** — co się zepsuło, gdzie praca stanęła, co agent zrobił źle w edytorze.
- **Decyzja** — co wybraliśmy i dlaczego.

Format: `RRRR-MM-DD · typ · krótko` + 1–3 zdania.

---

2026-11-04 · decyzja · Podział kontekstu projektu
Kontekst z rozmowy w claude.ai rozdzielony na trzy miejsca: `CLAUDE.md` — tylko trwałe zasady (gra, proces, architektura, git), `docs/roadmap.md` — etapy i bieżący stan, pamięć Claude — profil autora i dalsza ścieżka (inne gry). Powód: `CLAUDE.md` czyta agent w każdej sesji, więc status i informacje, które szybko się dezaktualizują, tylko go zaśmiecają.

2026-11-04 · tarcie · Pierwsze uruchomienie edytora blokuje okno licencji
Okno „Unity Editor Software Terms” zatrzymało edytor na ~45 min, a CLI widziało tylko stan `starting`. Przy pierwszym uruchomieniu nowej wersji edytora warto od razu powiedzieć użytkownikowi, że musi zaakceptować warunki.

2026-11-04 · tarcie · Repo powstało przed projektem Unity
Folder repo już istniał (z `CLAUDE.md` i `docs/`), a `unity projects create` tworzy nowy folder, więc projekt powstał w folderze tymczasowym i został przeniesiony do repo (czy CLI poradziłoby sobie z istniejącym folderem — nie sprawdzone). Potem trzeba było poprawić `productName` (nazwa tymczasowego folderu) i ponownie zarejestrować projekt w Hubie. Przy następnej grze: najpierw utworzyć projekt, potem w nim `git init` i dopisać dokumenty.

2026-11-04 · decyzja · Szablon i ustawienia startowe
Unity 6.3 LTS, szablon `com.unity.template.universal-2d`. Szablon ma już ustawione Force Text i Visible Meta Files, Input System, Test Framework i pakiet Visual Studio Editor. Platforma buildu od początku przełączona na WebGL, bo zmiana platformy później oznacza pełny reimport.
