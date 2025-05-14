using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

namespace NotionPlay.Tools
{
    public static class AudioHelper
    {
        private static readonly Dictionary<Key, string> _audioFileMappings = new()
        {
            { Key.Q, "Q.mp3" },
            { Key.W, "W.mp3" },
            { Key.E, "E.mp3" },
            { Key.R, "R.mp3" },
            { Key.T, "T.mp3" },
            { Key.Y, "Y.mp3" },
            { Key.U, "U.mp3" },

            { Key.A, "A.mp3" },
            { Key.S, "S.mp3" },
            { Key.D, "D.mp3" },
            { Key.F, "F.mp3" },
            { Key.G, "G.mp3" },
            { Key.H, "H.mp3" },
            { Key.J, "J.mp3" },

            { Key.Z, "Z.mp3" },
            { Key.X, "X.mp3" },
            { Key.C, "C.mp3" },
            { Key.V, "V.mp3" },
            { Key.B, "B.mp3" },
            { Key.N, "N.mp3" },
            { Key.M, "M.mp3" }
        };

        private static readonly Dictionary<Key, MediaPlayer> _activePlayers = [];
        private static readonly string _audioDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AudioSources");

        public static void Initialize()
        {
            if (!Directory.Exists(_audioDirectory))
            {
                Directory.CreateDirectory(_audioDirectory);
            }

            foreach (var key in _audioFileMappings.Keys)
            {
                string filePath = Path.Combine(_audioDirectory, _audioFileMappings[key]);
                if (File.Exists(filePath))
                {
                    var player = new MediaPlayer();
                    player.Open(new Uri(filePath));
                    _activePlayers[key] = player;
                }
            }
        }

        public static void PlayNote(Key key)
        {
            if (_activePlayers.TryGetValue(key, out var player))
            {
                player.Stop();
                player.Position = TimeSpan.Zero;
                player.Play();
            }
        }

        public static void StopNote(Key key)
        {
            if (_activePlayers.TryGetValue(key, out var player))
            {
                player.Stop();
                player.Position = TimeSpan.Zero;
            }
        }

        public static void StopAllNotes()
        {
            foreach (var player in _activePlayers.Values)
            {
                player.Stop();
                player.Position = TimeSpan.Zero;
            }
        }
    }
}