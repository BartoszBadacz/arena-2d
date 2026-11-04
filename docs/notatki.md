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

2026-11-04 · tarcie · MCP nie startował po restarcie VS Code
Instalator Unity CLI dopisał `unity` do PATH użytkownika, ale zrestartowany VS Code dziedziczył stare środowisko, więc serwer MCP kończył się błędem „Connection closed”, a prośba o zgodę się nie pojawiała. Rozwiązanie: pełna ścieżka do `unity.exe` w `.mcp.json`. Na przyszłość: po instalacji narzędzia z PATH zakładać, że do wylogowania z Windows go nie widać.

2026-11-04 · decyzja · Zakres prototypu
Twin-stick (WASD + celowanie myszką), jeden typ wroga, fale, bez ulepszeń. Celowanie daje frajdę już na szarych prostokątach, a auto-strzał (Vampire Survivors) robi się ciekawy dopiero przy wielu broniach i ulepszeniach. Ulepszenia to najsilniejszy mechanizm „jeszcze jednej rundy”, ale najpierw sprawdzamy, czy bawi samo strzelanie i unikanie.

2026-11-04 · tarcie · MCP rozłącza się przy rekompilacji
Każda zmiana skryptu wywołuje w edytorze przeładowanie kodu (domain reload), a serwer MCP na ten czas się rozłącza i wywołanie narzędzia kończy się błędem 400. Obejście: po zmianie `.cs` poczekać przez CLI (`unity status` → `ready`) i dopiero wtedy wracać do narzędzi MCP. Przy trzecim wystąpieniu warto zrobić z tego skrypt lub regułę.

2026-11-04 · tarcie · Magiczne metody Unity nie są typowane
`Update`, `FixedUpdate`, `OnTriggerEnter2D` itd. Unity znajduje po nazwie: literówka albo zła sygnatura nie daje błędu kompilacji, metoda po prostu się nie wywołuje. Częściowa ochrona to analizatory Unity w IDE (ostrzegają o złej sygnaturze, np. `OnTriggerEnter2D(Collider)` z kolizją 3D zamiast 2D). Kandydat do frameworka: reguła lub analizator przy nazwach podobnych do metod Unity. Kolejny argument, by logika żyła w `Core`, a nie w MonoBehaviourach.

2026-11-04 · powtórzenie · Czekanie na edytor po zmianie skryptu (2. raz)
Polecenie „przekompiluj” wysłane w trakcie przeładowania kodu zostawiło w konsoli 2 błędy serwera Pipeline (ObjectDisposedException). To nie były błędy gry. Przy trzecim razie: skrypt „zapisz .cs → poczekaj na ready → wyczyść konsolę → sprawdź błędy kompilacji”.

2026-11-04 · decyzja · Wrogowie zderzają się z graczem, ale prawie go nie przepychają
Kontakt z wrogiem zadaje obrażenia (`OnCollisionStay2D` + 1 s nietykalności), kolizja fizyczna zostaje. Gracz ma masę 20, wróg 1, więc przepychanie jest minimalne. Rozważane przenikanie wrogów przez gracza odrzucone, bo tłum, który „napiera”, daje poczucie zagrożenia.

2026-11-04 · decyzja · Fale „na czas”, wynikiem jest czas przetrwania
Co 15 s nowa fala: odstęp między wrogami ×0.8 (minimum 0.2 s), prędkość wroga +0.3. Niezabici wrogowie się kumulują, więc presja rośnie sama. Przy falach „do wybicia” wynikiem musiałby być numer fali, bo przy wyniku liczonym w czasie opłacałoby się zabijać wolno. Rekord trzymany w polu `static`: przeżywa restart sceny, ginie po zamknięciu gry (trwały zapis jest poza zakresem).

2026-11-04 · tarcie · Rekompilacja w trybie Play
Zmiana skryptu, gdy edytor był w trybie Play, podmieniła kod w trakcie gry. Nowe, niepodpięte pole dało ~900 wyjątków NullReference w `OnGUI`. Przed zmianą `.cs` zatrzymać tryb Play. Kandydat na regułę dla agenta: sprawdzić `isPlaying` przed edycją kodu.

2026-11-04 · tarcie · Symulowane klawisze nie zawsze docierają do gry
`runtime simulate_key` zadziałał dla ruchu (D), ale nie dla restartu (R przy `timeScale = 0`). Prawdopodobnie zależy to od fokusu okna Game. Test restartu zrobiony przez bezpośrednie `SceneManager.LoadScene`. Klawisz R sprawdził użytkownik ręcznie.

2026-11-04 · decyzja · Skalowanie UI w prototypie
`OnGUI` liczy w pikselach ekranu, więc na dużym oknie napisy były drobne. Skalowanie `GUI.matrix` do wysokości bazowej 720 px. W docelowym UI (UI Toolkit lub uGUI) skalowanie daje Canvas Scaler / Panel Settings.
