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

2026-11-04 · decyzja · Zmiana po pierwszym playteście: fale „do wybicia”
Playtest: granice rund były niewyraźne, wrogowie pojawiali się zbyt blisko gracza („niesprawiedliwe”), różnica między falami była niewyczuwalna (szybkie zgony). Zmiany: fala = określona liczba wrogów (5, +3 na falę), kończy się po zabiciu ostatniego, potem 3 s przerwy z odliczaniem. Wrogowie pojawiają się co najmniej 6 jednostek od gracza. Rekord to teraz najwyższa fala, bo czas nagradzałby wolne zabijanie. Lekcja: decyzja z 15 minut wcześniej („fale na czas”) nie przetrwała pierwszego kontaktu z graczem. Dlatego prototyp jest tani.

2026-11-04 · tarcie · Domyślne wartości w kodzie nie zmieniają istniejących obiektów
Pole `[SerializeField] int x = 5;` daje tylko wartość początkową przy dodaniu komponentu. Potem w scenie lub prefabie zapisana jest wartość z YAML-a i to ona wygrywa. Przez to zmiana parametrów fal w kodzie (1.2 / 0.85) w ogóle nie trafiła do gry, a playtest szedł na starych liczbach. Reguła: po zmianie domyślnych wartości ustawić je też w scenie/prefabie albo stroić tylko w Inspectorze. W architekturze docelowej liczby żyją w ScriptableObjectach, więc źródło prawdy jest jedno. Kandydat na regułę lub automatyczną kontrolę dla agenta.

2026-11-04 · decyzja · Zagęszczenie zamiast tylko liczebności (playtest 2)
Playtest: fale wydawały się takie same i płaskie. Przyczyna: wrogowie wychodzili pojedynczo i byli na bieżąco zestrzeliwani, więc zagęszczenie na planszy się nie zmieniało. Zmiany: wrogowie wychodzą grupami z jednego miejsca (1 → maks. 4, grupa rośnie co 2 fale), w fali jest ich więcej (6, +4 na falę), szybkość rośnie skokowo co 3 fale (+0.6) z komunikatem „WROGOWIE SZYBSI!”. Do strojenia pole `Start Wave`. Lekcja z game designu: presję tworzy liczba wrogów na planszy naraz, nie ich łączna liczba.

2026-11-04 · decyzja · Zlepianie się wrogów: separacja teraz, łączenie jako eksperyment
Playtest 3: przy większych falach wrogowie zlepiają się w kłąb lub „węża”, bo wszyscy idą prosto do jednego punktu, a fizyka rozpycha ich dopiero po zderzeniu. Jako osobne byty wyglądają przez to źle. Pomysł użytkownika: zrobić z tego mechanikę, czyli łączenie w większe byty (slime, Asteroids). To zmiana kierunku gry, więc trafia do roadmapy jako osobny eksperyment. Testujemy jedną rzecz naraz.

2026-11-04 · decyzja · Wrogowie naraz, ale każdy w innym miejscu (playtest 3, cd.)
Paczki z jednego punktu wyglądały źle. Teraz każde „tyknięcie” spawnera wypuszcza 1–4 wrogów naraz, każdego w osobnym losowym punkcie na krawędzi (z bezpieczną odległością). Zagęszczenie zostaje, wrogowie nadciągają ze wszystkich stron. Przy samym graczu nadal się zbijają, bo wszyscy idą do jednego punktu. To osobny problem (ruch, nie spawn).

2026-11-04 · decyzja · Separacja wrogów (boids)
Każdy wróg dodaje do kierunku „do gracza” wektor „od sąsiadów” w promieniu 2.0 (siła 3), im bliżej sąsiad, tym mocniej. Gdy gracz ucieka, tłum idzie za nim luźnym rojem zamiast węża. Gdy gracz stoi, zwarty krąg wokół niego jest nieunikniony (20 wrogów chce dotknąć jednego punktu), a separacja tego nie rozwiąże. To raczej uczciwa informacja „dałeś się otoczyć”. Przy okazji: niejednoznaczne `Random` przy `using System` + `using UnityEngine` — potrzebna pełna nazwa `UnityEngine.Random`.

2026-11-07 · decyzja · Prototyp odpowiada „tak” — pętla bawi
Playtest 4: „nawet w tej formie działa nieźle, przebieg gry szybki, krótkie fale sprawiają, że nie szkoda zacząć od nowa”. Niski koszt porażki i szybki restart to fundament gier typu arcade/roguelite. Do zachowania przy przepisaniu: fale do wybicia z przerwami, bezpieczny spawn, wielu wrogów naraz w różnych miejscach, przyspieszenia skokowe, separacja, nietykalność po trafieniu, ciężki gracz. Wnioski z drogi: 4 playtesty w jeden wieczór zmieniły model fal dwa razy, czyli tani prototyp się opłacił.

2026-11-07 · decyzja · Zakres wersji 1
Wchodzą: rdzeń z prototypu + typy wrogów jako domieszka (np. biegacz od fali 3 z szansą 15%, tank 1 na falę od fali 5). Domieszka zamiast fal jednego typu, bo daje płynną różnorodność bez skoków trudności (pomysł użytkownika). Wypadają: łączenie wrogów (za dużo projektowania, cięcie zakresu 10x). Ulepszenia między falami zostają pierwszym kandydatem po wersji 1. Kolejność: najpierw przepisanie rdzenia na architekturę z danymi w ScriptableObjectach, potem typy. Wtedy balansowanie to edycja plików danych, a nie kodu.

2026-11-07 · decyzja · Plan przepisania: rozkład prototypu na moduły
Logika z prototypu przechodzi do czystych klas w `Core` (`Health` wspólne dla gracza i wrogów, `Weapon` – kadencja, `Separation` – boidy, `WaveDirector` – maszyna stanów fal, `EnemyMixer` – skład fali, `SpawnPointPicker`, `RunSession`, `RunStats`, `SessionRecord`). MonoBehavioury w `Gameplay` tylko łączą silnik z logiką. `Core` nie zna `Time`, `UnityEngine.Random` ani ScriptableObjectów: czas dostaje jako argument, losowość jako `System.Random` z ziarnem, a ustawienia jako zwykłe obiekty z `ToSettings()` na SO (strzałka zależności Data → Core). Wektory w `Core`: `System.Numerics.Vector2`. Obrażenia od kontaktu przechodzą z gracza na `EnemyDefinition` (tank ma bić mocniej). Prototyp żyje do playtestu A/B (nowa scena vs prototyp przy tych samych liczbach), potem usuwamy go w całości.

2026-11-07 · decyzja · Stan czytamy, na fakty reagujemy
Dwa mechanizmy zamiast jednego. Stan (HP, fala, wrogowie do końca, zabici) to właściwości tylko do odczytu na obiektach `Core`, a UI czyta je co klatkę. Pętla gry przelicza wszystko co klatkę, więc `computed` z Angulara to tu zwykła właściwość, a sygnały są niepotrzebne. Fakty (trafienie, śmierć wroga, start fali, Game Over) to zdarzenia C# (`event`), bo sygnał scala zmiany, a 3 zabitych w jednej klatce to 3 dźwięki. Odrzucone: globalna szyna zdarzeń i kanały-ScriptableObjecty (ukrywają zależności), R3/UniRx (zależność bez potrzeby, wrócić przy złożonym UI), wiązanie danych UI Toolkit (wymaga atrybutów Unity w `Core`, wrócić, jeśli UI urośnie). Agregacja wrogów: spawner podpina się pod `Died` każdego wroga i wystawia jedno `EnemyKilled` (jak rodzic zbierający `@Output` dzieci). Zastępuje statyczne `Enemy.Killed`.

2026-11-07 · decyzja · Composition root w osobnym module `App`
`GameBootstrap` (pierwszy w kolejności wykonania) tworzy obiekty `Core` i wstrzykuje je do MonoBehaviourów metodą `Init(...)`, bo Unity samo tworzy komponenty i nie da się użyć konstruktora. Nie ma `Update` ani logiki gry, więc nie jest GameManagerem. Leży w `Scripts/App/` (`Arena.App.asmdef`), jedynym module, który widzi wszystkie pozostałe: `Gameplay` i `Presentation` nie powinny widzieć siebie nawzajem.

2026-11-07 · decyzja · UI, restart, domieszki
UI w UI Toolkit (UXML/USS ≈ HTML/CSS, skalowanie przez Panel Settings). Restart = przeładowanie sceny (świat od zera, bez sprzątania). Rekord sesji w jednym udokumentowanym polu `static` w `SessionRecord`, czytanym tylko przez bootstrap, jako jedyny wyjątek od zasady „bez statyków”. Domieszka typu wroga zastępuje podstawowego wroga w danym miejscu fali (łączna liczba wrogów zostaje z krzywej). Skład fali losowany na jej starcie, więc łatwo go testować.

2026-11-07 · powtórzenie · Czekanie na edytor po zmianie skryptu (3. raz) → reguła
`unity recompile` sam czeka na koniec kompilacji i zwraca błędy, ale gdy auto-refresh już zaczął przeładowanie po zapisie plików, kończy się błędem 400. Pewniejsza droga: poczekać na `unity status` → `ready`, potem `unity cmd console status` i odczytać `groundTruth` (`compiling`, `compilationFailed`, `consoleErrors`). Dopisane do reguł w `CLAUDE.md`.

2026-11-07 · decyzja · Test nowej sceny bez ruszania otwartej
`EditorSceneManager.playModeStartScene` = `Arena.unity`: Play ładuje wskazaną scenę, a po wyjściu z Play edytor wraca do otwartej sceny z jej niezapisanymi zmianami. Agent nie musi zamykać ani zapisywać sceny użytkownika. Ustawienie żyje tylko w sesji edytora (nie trafia do repo). Nową scenę agent buduje jako dodatkową (`NewSceneMode.Additive`) i zamyka po zapisie.

2026-11-07 · decyzja · Parametry fizyki zostają w prefabie
Masa (20), kolizja ciągła, interpolacja i rozmiar collidera to konfiguracja komponentów, więc zostają w prefabie gracza. W ScriptableObjectach są liczby rozgrywki (prędkość, życie, obrażenia, fale), które stroi się przy balansie.

2026-11-07 · tarcie · Testy EditMode blokują edytor oknem „Scene(s) Have Been Modified”
Test Runner przed testami EditMode zamyka otwarte sceny. Gdy któraś jest oznaczona jako zmieniona, Unity pokazuje modalne okno z pytaniem o zapis, a edytor i MCP przestają odpowiadać (`test run` → 504 po 10 min, `unity status` → `unreachable`). Diagnoza: lista okien procesu Unity (WinAPI z PowerShella) pokazała tytuł okna dialogowego. Flaga „zmieniona” bywa pusta (scena na dysku identyczna z repo, prawdopodobnie ślad odczytów przez `script_eval`). Reguła: przed testami sprawdzić `isDirty` otwartych scen i w razie potrzeby zapytać użytkownika; gdy edytor nagle przestaje odpowiadać, najpierw szukać okna dialogowego.

2026-11-07 · decyzja · Prefaby w ScriptableObjectach jako `GameObject`
`WeaponDefinition` (Data) wskazuje prefab pocisku, ale typ `Projectile` leży w Gameplay, a Data nie może znać Gameplay (cykl zależności). Pole ma typ `GameObject`, a Gameplay przy tworzeniu sprawdza `TryGetComponent<Projectile>` i zgłasza czytelny błąd. Tak samo będzie z `EnemyDefinition` → prefab wroga.

2026-11-07 · decyzja · Pierwsza klasa Core i granica `.asmdef` w praktyce
`Weapon` (kadencja strzałów) z 5 testami EditMode (całość ~3 s, z tego testy < 1 ms). Skompilowane `Arena.Core` zależy wyłącznie od `netstandard`, czyli granica „Core bez Unity” działa. Kolizje pocisków przez warstwy `Player` / `PlayerProjectile` i Layer Collision Matrix zamiast `IgnoreCollision` i sprawdzania typu w kodzie. Kadencja liczona jak w prototypie (`now + 1/rate`), żeby wyczucie w teście A/B było identyczne.

2026-11-07 · tarcie · „Edytor gotowy” nie znaczy „nowy kod wczytany”
Po zapisie skryptów `unity status` pokazywał `ready`, a `groundTruth` `compiling: false`, ale `script_eval` nie widział nowych typów: auto-refresh nie ruszył, prawdopodobnie bo okno Unity nie miało fokusu. Pomogło `unity recompile`. Reguła w `CLAUDE.md`: po zmianie `.cs` zawsze `recompile` po `ready` (przy 400 poczekać i powtórzyć).

2026-11-07 · decyzja · Interfejs `IHasHealth` w Core jako punkt styku modułów
Wrogowie powstają w trakcie gry, więc bootstrap nie może podpiąć ich efektów, a spawner (Gameplay) nie widzi Presentation. `IHasHealth` w Core: `EnemyAgent` go implementuje, `HitFlash` (Presentation) i `Projectile` tylko go szukają na obiekcie. Moduły znają Core, nie siebie nawzajem (jak `InjectionToken` we wspólnej bibliotece w Angularze). Ten sam interfejs posłuży graczowi.

2026-11-07 · decyzja · Świadome odstępstwa od prototypu przy wrogu
Dwie poprawki, które minimalnie zmieniają wyczucie, więc warto je pamiętać przy teście A/B: (1) separacja pomija sąsiadów dalej niż promień. W prototypie `OverlapCircle` łapał też tych, których tylko krawędź wchodziła w okrąg, a waga `1 - d/r` robiła się ujemna, czyli lekko przyciągała. (2) Pocisk trafia tylko raz (flaga `spent`), wcześniej mógł zranić dwóch wrogów w jednym kroku fizyki. Sąsiedzi wyszukiwani po warstwie `Enemy` zamiast `GetComponent<Enemy>()`. 19 testów w Core.

2026-11-07 · tarcie · Błąd „More than one global light” przy dwóch otwartych scenach
Gdy agent otwiera `Arena.unity` obok prototypu, obie sceny mają swoje Global Light 2D i URP zgłasza błąd w konsoli. To artefakt edycji, nie błąd gry: znika po zamknięciu dodatkowej sceny. Zniknie całkiem po usunięciu prototypu.

2026-11-11 · decyzja · Krok 4a: przebieg rozgrywki w Core, wynik na razie w konsoli
`RunSession` słucha `Died` gracza i robi zdjęcie wyniku (`RunResult`), `RunStats` liczy zabitych, czas i falę, `SessionRecord` trzyma najwyższą falę. Dwa doprecyzowania planu: (1) pole `static` z rekordem jest w `GameBootstrap`, nie w `SessionRecord`, bo klasa w Core bez statyków daje niezależne testy. (2) `GamePause` i `RestartInput` połączone w jeden cienki `RunFlow` (czas → sesja, `timeScale = 0` przy Game Over, R → przeładowanie sceny). `RunFlow` ustawia `timeScale = 1` w `Awake`, bo ta wartość przeżywa przeładowanie sceny. Obrażenia od kontaktu biorą się z `EnemyDefinition.ContactDamage`. Spawner zbiera `Died` każdego wroga w jedno `EnemyKilled`, a bootstrap łączy je z `RunStats`. Core ma 26 testów.

2026-11-11 · decyzja · Miganie gracza czyta stan, martwy zawsze widoczny
`HitBlink` co klatkę pyta `Health.IsInvulnerable(now)`, bo „czy migać” to stan, a nie fakt. Poprawka ukrytego błędu prototypu: przy Game Over `timeScale = 0` zamraża `Time.time`, więc gracz mógł zostać „zamrożony” w fazie zgaszonej i zniknąć z ekranu. Martwy gracz jest teraz zawsze widoczny.

2026-11-11 · obserwacja · Nowe pole w ScriptableObjecie bierze wartość z kodu
Dopisanie `maxHealth = 5` do `PlayerDefinition` dało w istniejącym `Player.asset` od razu 5, bo pola nie było jeszcze w YAML-u. To druga strona pułapki z `[SerializeField]`: wartość z kodu działa tylko dla pól, których plik jeszcze nie zna. Gdy plik raz je zapisze, wygrywa plik.

2026-11-11 · decyzja · `Arena.unity` główną sceną w edytorze
Po przerwie edytor otworzył prototyp, a `playModeStartScene` działało tylko w poprzedniej sesji. Agent otwiera teraz `Arena.unity` jako główną scenę (prototyp był zapisany), więc Play od razu uruchamia nową wersję. Prototyp otwiera się ręcznie do testu A/B.

2026-11-11 · decyzja · HUD w UI Toolkit (krok 4b)
Pliki UI w `Assets/_Project/UI/`: `Hud.uxml` (struktura), `Hud.uss` (style, klasy w konwencji BEM: `hud__stat`, `game-over--hidden`), `PanelSettings.asset` (Scale With Screen Size, 1280×720, dopasowanie do wysokości, jak `GUI.matrix` w prototypie) i `ArenaTheme.tss` (`@import url("unity-theme://default")` daje domyślną czcionkę; bez motywu tekst się nie renderuje). Logika HUD w `Presentation/Hud.cs`: stan czyta co klatkę, ale napis zmienia tylko przy zmianie liczby (bez nowego stringa co klatkę, ważne dla GC w WebGL). Ekran Game Over pojawia się na zdarzenie `GameOver` przez zdjęcie klasy `game-over--hidden`, bez stylów inline. HUD to prefab z `UIDocument`. Do sprawdzenia w kroku 5: polskie znaki w domyślnej czcionce (`UKOŃCZONA`, `SZYBSI`).

2026-11-11 · tarcie · Prędkość gracza: 9 w scenie prototypu, 6 w jego kodzie
Przed krokiem 5 porównanie wartości zapisanych w `Prototype.unity` z liczbami przepisanymi do `_Project`. W `PlayerMovement` scena ma `speed: 9` (podkręcone w Inspectorze podczas playtestów), a kod 6. W kroku 1 do `Player.asset` trafiło 6, więc nowy gracz był o 1/3 wolniejszy od ocenionego „bawi”. Poprawione na 9. Trzecie wystąpienie pułapki z `[SerializeField]` (patrz 2026-11-04), tym razem przy przenoszeniu: źródłem prawdy prototypu jest scena/prefab, nie kod. Reguła: przy przepisywaniu czytać wartości z YAML-a sceny i prefabów (grep po GUID skryptu), nie z domyślnych w kodzie. Reszta liczb (fale, wróg, broń, życie) zgodna.

2026-11-11 · decyzja · Fale w Core (krok 5)
`WaveDirector` przenosi maszynę stanów z prototypu 1:1 (Break → Spawning → Fighting, liczności, interwały, paczki, przyspieszenia). Czas dostaje przez `Tick(dt)`, wrogów nie tworzy, tylko prosi zdarzeniem `SpawnRequested(count)`, a śmierci przyjmuje przez `EnemyDied()`. `SpawnPointPicker` to bezpieczny punkt na krawędzi z awaryjnym rogiem po przeciwnej stronie (w prototypie awaryjny punkt przy graczu w (0,0) wypadał na graczu). Liczby w `Waves.asset` (`WaveConfig.ToSettings()`) i `Arena.asset`. Spawner (Gameplay) tylko tyka, tworzy i melduje. HUD: fala, wrogowie, baner „FALA N” / „WROGOWIE SZYBSI!” i przerwa z odliczaniem (przed 1. falą „PRZYGOTUJ SIĘ” zamiast pustego tytułu). Polskie znaki w domyślnej czcionce UI Toolkit działają. 13 nowych testów, Core ma 39. Test w Play z testowym „autozabójcą” (handler na `SpawnRequested`): czasy fal 1–4 zgodne z rachunkiem co do 0.1 s.

2026-11-11 · obserwacja · Edytor w tle zwalnia czas gry
Przy oknie Unity bez fokusu czas gry płynął ok. 2× wolniej niż zegar ścienny (`Czas: 5 s` po ~10 s). Testy czasowe agenta: odpytywać stan gry (konsola, `script_eval`), nie spać według zegara.

2026-11-11 · decyzja · Playtest A/B zaliczony, prototyp usunięty
Użytkownik potwierdził, że prędkość 9 w scenie prototypu to jego zmiana w Inspectorze z playtestów, o której zapomniał. Nowa wersja „wygląda spoko”, więc test A/B zaliczony. `Assets/_Prototype` usunięty przez `AssetDatabase.DeleteAsset` (razem z `.meta`), scena zdjęta z Build Settings (zostaje tylko `Arena.unity`). Przed usunięciem kontrola GUID: z `_Project` nic nie wskazywało na prototyp, jedynym odwołaniem był wpis w `EditorBuildSettings`. Ostatni commit z prototypem oznaczony tagiem `prototype-final` (powrót: `git checkout prototype-final`). Wniosek na przyszłość: w prototypie notować w `notatki.md` każdą zmianę liczby w Inspectorze, bo pamięć zawodzi, a scena wygrywa z kodem.

2026-11-11 · decyzja · Domieszki typów wrogów (krok 7)
`EnemyMixer<TKind>` w Core jest generyczny (w grze `TKind = EnemyDefinition`, w testach `string`), więc Core dalej nie zna ScriptableObjectów. Skład fali losowany na starcie fali (`WaveStarted` → `StartWave(wave, WaveSize)`): dla każdego miejsca reguły sprawdzane po kolei, wygrywa pierwsza aktywna, która wylosuje swoją szansę i nie wyczerpała limitu. Potem tasowanie Fisher–Yates (bez niego tank „1 na falę” zawsze wychodziłby pierwszy). Spawner bierze po prostu `Next()`. Reguły w `Waves.asset` (`mixIns`: wróg, od fali, szansa, maks. na falę; 0 = bez limitu). Kolejność reguł ma znaczenie: tank (100%, maks. 1) przed biegaczem (15%). Nowe typy to warianty prefabu Slime (dziedziczą wszystko, nadpisują kolor, skalę, masę) plus pliki `Runner.asset` (HP 1, prędkość 5, mały, pomarańczowy) i `Tank.asset` (HP 10, prędkość 1.8, kontakt 2, duży, fioletowy, masa 4). Liczby wstępne, do strojenia playtestem. Premia przyspieszenia od fali dodawana do każdego typu, jak w prototypie. Test w Play z `Start Wave = 5`: fala 5 = Slime×18, Runner×3, Tank×1 (22, zgodnie z krzywą). Core ma 49 testów. Nowy typ wroga o tym samym zachowaniu = wariant prefabu + plik `.asset` + wiersz w `mixIns`, bez kodu.

2026-11-11 · decyzja · Playtest balansu 1: fale o 10% rzadsze, punkty, uciekający Runner
Playtest użytkownika: podoba się, ale fala 5 za gęsta, a za zabicie szybkiego i dużego przydałaby się nagroda (duży = więcej punktów, szybki = +1 HP); szybki powinien uciekać, a nie atakować. Zmiany: (1) przyrost wrogów na falę 4 → 3.6 (pole ułamkowe, liczność fali zaokrąglana): fala 1 bez zmian, fala 5: 22 → 20, fala 10: 42 → 38. (2) Punkty za zabicie (`scoreValue` w `EnemyDefinition`), rekord sesji liczony w punktach zamiast najwyższej fali. Przy falach „do wybicia” punktów nie da się farmić, bo każdą falę trzeba wyczyścić, a punkty różnicują biegi kończące się na tej samej fali. (3) Runner ucieka (zachowanie `Flee` w danych) i nie zadaje obrażeń, a zabity leczy gracza o 1 HP, maksymalnie do pełnego. Pomysł użytkownika: zasoby na ulepszenia (np. szybkostrzelność) i tank, który pojawia się na chwilę i znika niezabity. Odłożony do „może kiedyś” razem z ulepszeniami między falami. Punkty mogą wtedy stać się walutą.

2026-11-11 · obserwacja · Uciekający Runner ląduje w rogu
Czysta ucieczka („prosto od gracza”) wbijała biegacza w ścianę, a tarcie zatrzymywało go w miejscu (prędkość 0 przy x = ±13.6). Poprawka w Core: `Steering.KeepInside` usuwa składową „na zewnątrz” areny i zachowuje prędkość, więc biegacz ślizga się wzdłuż ściany. Skutek: zjeżdża do rogu i tam stoi, a przy dalekim zasięgu pocisków jest łatwym celem. Do oceny w playteście. Kandydat na poprawkę: „trzymanie dystansu” (krąży wokół gracza w zadanym promieniu, ucieka dopiero, gdy gracz się zbliży). Wtedy jest ruchomym celem i nie utyka w rogach.

2026-11-11 · tarcie · `script_eval` z `internals: true` psuje kolejne wywołania
Odczyt konsoli edytora przez wewnętrzne `LogEntries` (`internals: true`) zostawił w pamięci dynamiczne assembly bez ścieżki. Każdy następny `script_eval` kończył się błędem 500 w telemetrii Pipeline (`Asset path cannot be null or empty`). Nie pomogło zatrzymanie Play ani `script recompile` („up_to_date”). Pomogło dopiero przeładowanie domeny wymuszone prawdziwą zmianą w pliku `.cs`. Reguła: nie używać `internals: true`. Gdy `consoleErrors` w `groundTruth` nie zgadza się z buforem MCP, to zwykle artefakty Pipeline (`ObjectDisposedException`) i wystarczy wyczyścić konsolę.

2026-11-11 · decyzja · Runner chowa się w tłumie (pomysł użytkownika)
Zamiast stać w rogu, biegacz biegnie do losowego innego wroga (tylko spośród goniących) i co `retargetInterval` (2.5 s) losuje kolejnego, innego niż poprzedni. Gdy kryjówka zginie, losuje od razu. Bez kryjówek ucieka przed graczem. Zachowanie `HideInCrowd` w danych, logika wyboru w Core jako generyczny `TargetSwitcher<T>` (6 testów). Spawner prowadzi listę żywych wrogów. Wspólne otoczenie wroga (gracz, losowość, granice areny, żywi) zebrane w `EnemyContext`, zamiast coraz dłuższej listy parametrów `Init`. Efekt w Play: biegacze krążą po obrzeżach roju, zasłaniani przez innych wrogów, i co chwilę zmieniają miejsce. Limit domieszki może rosnąć z falami (`maxGrowthEveryWaves`): biegacz od fali 3 najwyżej 1 na falę, +1 co 4 fale (fala 7: 2, fala 11: 3), co sprawdzone w Play na fali 7 (Slime×25, Runner×2, Tank×1). Core ma 67 testów.

2026-11-11 · decyzja · Krzywa liczności fal z malejącym przyrostem (playtest balansu 2)
Playtest: „jest dużo lepiej”, użytkownik doszedł do fali 7, ale fala 7 to już chmara. Liniowy przyrost (+3.6 na falę) zastąpiony malejącym: fala 2 dokłada `extraEnemiesPerWave`, każda następna ×`growthDecayPerWave`, nie mniej niż `minExtraPerWave`. Wartości: start 8 (było 6), przyrost 4, ×0.85, minimum 1. Liczności: 8, 12, 15, 18, 21, 23, 25, 26, 27, 28 (fala 7: 28 → 25, fala 10: 38 → 28), potem +1 na falę. Przy `growthDecayPerWave = 1` zachowanie liniowe jak wcześniej (stare testy bez zmian). Do obserwacji: chmarę w późnych falach tworzy też tempo wychodzenia (paczki do 4 wrogów co ~0.8 s). Kolejne pokrętła to `wavesPerBatchGrowth`, `maxBatchSize` i `intervalMultiplierPerWave` w tym samym pliku.

2026-11-11 · decyzja · Runner skacze w bok jak mucha
Prośba z playtestu: żółtego nieco przyspieszyć i dodać skoki w bok. `Darting` w Core (5 testów): co losowy czas (0.3–0.8 s) krótki zryw (0.15 s) w lewo lub w prawo względem kierunku ruchu, ×1.8 prędkości, z 35% „do przodu” (skos zamiast czystego bokiem), potem powrót na kurs. Liczony przed `KeepInside`, więc nie wyskakuje za krawędź. Parametry w `EnemyDefinition` (sekcja „Skoki w bok”). `dartDuration = 0` wyłącza skoki, więc Slime i Tank bez zmian. Runner: prędkość 5 → 6. Core ma 73 testy.

2026-11-11 · tarcie · Agent wszedł w rozgrywkę użytkownika
Między testami agenta użytkownik sam uruchomił Play. `playmode_start` zwrócił „Already in play mode”, ale agent zdążył już ustawić graczowi nieśmiertelność (do testu) w rozgrywce użytkownika. Zmianę cofnięto, gdy tylko stan gry (fala 1, gracz poza środkiem) to ujawnił. Reguła: przed każdym wejściem w Play sprawdzić `editor_status`. Jeśli Play już trwa, a agent go nie włączał, to gra użytkownik i nie wolno ruszać stanu gry.
