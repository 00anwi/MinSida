using System.Diagnostics;
using Blazored.LocalStorage;
using Bogus;
namespace MinSida.Services.Games
{
    public class WordGameService
    {
        private readonly ILocalStorageService _localStorage;

        private string locale = "en";
        public string Locale { 
            get{ 
                return locale; 
            } 
            set{ 
                locale = value;
                _faker = new Faker(value); 
            } 
        }

        public int GameWidth = 800;
        public int GameHeight = 600;

        public int Score { get; set; } = 0;
        public int Lives { get; set; } = 3;
        public int Highscore { get; set; }
        
        public List<GameWord> _words;
        public Stopwatch _stopwatch;

        private Faker _faker;
        private Random Random = new Random();

        public WordGameService(ILocalStorageService localStorage)
        {
            _faker = new Faker(Locale);
            _words = new List<GameWord>();
            _stopwatch = new Stopwatch();
            _localStorage = localStorage;
            GetHighscore();
        }

        private async void GetHighscore()
        {
            Highscore = await _localStorage.GetItemAsync<int>(nameof(Highscore));
        }
        
        //Start game with 10 random words using linq
        public async Task StartGameAsync()
        {
            Score = 0;
            Lives = 3;
            lastGameLoopMilliseconds = 0;
            _words.Clear();
            _stopwatch.Reset();
            _stopwatch.Start();
            StopGame = true;
            await Task.Delay(100);
            StopGame = false;
            GameLoop();
        }

        long lastGameLoopMilliseconds = 0;
        bool StopGame = false;

        async void GameLoop()
        {
            if (Lives < 1 || StopGame)
            {
                _words.Clear();
                if (Score > Highscore)
                {
                    Highscore = Score;
                    await _localStorage.SetItemAsync(nameof(Highscore), Score);
                }
                return;
            }

            for (int i = 0; i < _words.Count; i++)
            {
                _words[i].Y++;
                if (_words[i].Y > GameHeight)
                {
                    _words.RemoveAt(i);
                    Lives--;
                }
            }

            var randomNumber = Random.Next(1, 100);
            if (_words.Count < 10 && (randomNumber < (Score < 0 ? 1 : Score) || _words.Count < 2))
            {
                var newWord = GenerateNewWord(GetFirstName());

                if (!_words.Where(x => x.Y < 20 && x.X > newWord.X && x.X < newWord.X + 50).Any())
                {
                    _words.Add(newWord);
                }
            }

            var currentSeconds = (int)_stopwatch.ElapsedMilliseconds / 1000;
            var speedup = currentSeconds / 10;
            int delay = (42 - speedup) - (int)(_stopwatch.ElapsedMilliseconds - lastGameLoopMilliseconds);
            await Task.Delay(delay < 0 ? 0 : delay);
            lastGameLoopMilliseconds = _stopwatch.ElapsedMilliseconds;
            GameLoop();
        }

        public GameWord GenerateNewWord(string word)
        {
            return new GameWord(word, Random.Next(1, GameWidth - 50));
        }

        public void SetSeed(int seed)
        {
            Random = new Random(seed);
            Randomizer.Seed = Random;
        }

        public string GetLoremWord()
        {
            return _faker.Lorem.Word();
        }

        public string GetFirstName()
        {
            return _faker.Name.FirstName().ToLower();
        }

        public string GetLastName()
        {
            return _faker.Name.LastName().ToLower();
        }

        public string GetMusicGenre()
        {
            return _faker.Music.Genre();
        }
        
        public string GetVehicleManufacturer()
        {
            return _faker.Vehicle.Manufacturer();
        }
        
        public string GetVehicleModel()
        {
            return _faker.Vehicle.Model();
        }


        public static Dictionary<string,string> LanguageCodes = new Dictionary<string, string>()
        {
            {"af_ZA","Afrikaans"},
            {"fr_CH","French (Switzerland)"},
            {"ar","Arabic"},
            {"ge","Georgian"},
            {"az","Azerbaijani"},
            {"hr","Hrvatski"},
            {"cz","Czech"},
            {"id_ID","Indonesia"},
            {"de","German"},
            {"it","Italian"},
            {"de_AT","German (Austria)"},
            {"ja","Japanese"},
            {"de_CH","German (Switzerland)"},
            {"ko","Korean"},
            {"el","Greek"},
            {"lv","Latvian"},
            {"en","English"},
            {"nb_NO","Norwegian"},
            {"en_AU","English (Australia)"},
            {"ne","Nepalese"},
            {"en_AU_ocker","English (Australia Ocker)"},
            {"nl","Dutch"},
            {"en_BORK","English (Bork)"},
            {"nl_BE","Dutch (Belgium)"},
            {"en_CA","English (Canada)"},
            {"pl","Polish"},
            {"en_GB","English (Great Britain)"},
            {"pt_BR","Portuguese (Brazil)"},
            {"en_IE","English (Ireland)"},
            {"pt_PT","Portuguese (Portugal)"},
            {"en_IND","English (India)"},
            {"ro","Romanian"},
            {"en_NG","Nigeria (English)"},
            {"ru","Russian"},
            {"en_US","English (United States)"},
            {"sk","Slovakian"},
            {"en_ZA","English (South Africa)"},
            {"sv","Swedish"},
            {"es","Spanish"},
            {"tr","Turkish"},
            {"es_MX","Spanish (Mexico)"},
            {"uk","Ukrainian"},
            {"fa","Farsi"},
            {"vi","Vietnamese"},
            {"fi","Finnish"},
            {"zh_CN","Chinese"},
            {"fr","French"},
            {"zh_TW","Chinese (Taiwan)"},
            {"fr_CA","French (Canada)"},
            {"zu_ZA","Zulu (South Africa)"}
        };
    }

    public class GameWord
    {
        public GameWord(string word,int x)
        {
            Word = word;
            X = x;
            Y = 15;
        }

        public string Word { get; set; }
        public string CurrentChars { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
    }
}