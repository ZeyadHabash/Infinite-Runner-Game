using UnityEngine;
using TMPro;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;
using InfiniteRunner;
using System;
using Obvious.Soap;

namespace LeaderboardCreatorDemo
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _entryTextObjects;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private StringVariable _username;

        // Make changes to this section according to how you're storing the player's score:
        // ------------------------------------------------------------
        // private GameManager _infiniteRunnerGameManager;

        // private int Score => (int)_infiniteRunnerGameManager.Score;
        // ------------------------------------------------------------

        private void Start()
        {
            if (_usernameInputField != null)
                _usernameInputField.text = _username.Value;
            // _infiniteRunnerGameManager = GameManager.Instance;
            LoadEntries();
        }

        public void LoadEntries()
        {
            // Q: How do I reference my own leaderboard?
            // A: Leaderboards.<NameOfTheLeaderboard>

            Leaderboards.InfiniteRunnerLeaderboard.GetEntries(entries =>
            {
                foreach (var t in _entryTextObjects)
                    t.text = "";
                var length = Mathf.Min(_entryTextObjects.Length, entries.Length);
                for (int i = 0; i < length; i++)
                {
                    string truncatedUsername = entries[i].Username.Length > 30 ? entries[i].Username.Substring(0, 30) + "..." : entries[i].Username;
                    _entryTextObjects[i].text = $"{entries[i].Rank}. {truncatedUsername} - {entries[i].Score}";
                }
            });
        }

        public void UploadEntry()
        {
            int score = (int)Math.Round(GameManager.Instance.Score, 0);
            String username = String.IsNullOrEmpty(_username.Value) ? "Some guy" : _username.Value;
            Leaderboards.InfiniteRunnerLeaderboard.UploadNewEntry(username, score, isSuccessful =>
            {
                if (isSuccessful)
                    LoadEntries();
            });
        }

        public void SetUsername()
        {
            _username.Value = _usernameInputField.text;
        }
    }
}