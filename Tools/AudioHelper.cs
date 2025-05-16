using System.IO;
using System.Windows;
using System.Windows.Media;
using WindowsInput.Native;

namespace NotionPlay.Tools
{
    public static class AudioHelper
    {
        private static readonly Dictionary<VirtualKeyCode, string> _audioFileMappings = new()
        {
            { VirtualKeyCode.VK_Q, "Q.mp3" },
            { VirtualKeyCode.VK_W, "W.mp3" },
            { VirtualKeyCode.VK_E, "E.mp3" },
            { VirtualKeyCode.VK_R, "R.mp3" },
            { VirtualKeyCode.VK_T, "T.mp3" },
            { VirtualKeyCode.VK_Y, "Y.mp3" },
            { VirtualKeyCode.VK_U, "U.mp3" },

            { VirtualKeyCode.VK_A, "A.mp3" },
            { VirtualKeyCode.VK_S, "S.mp3" },
            { VirtualKeyCode.VK_D, "D.mp3" },
            { VirtualKeyCode.VK_F, "F.mp3" },
            { VirtualKeyCode.VK_G, "G.mp3" },
            { VirtualKeyCode.VK_H, "H.mp3" },
            { VirtualKeyCode.VK_J, "J.mp3" },

            { VirtualKeyCode.VK_Z, "Z.mp3" },
            { VirtualKeyCode.VK_X, "X.mp3" },
            { VirtualKeyCode.VK_C, "C.mp3" },
            { VirtualKeyCode.VK_V, "V.mp3" },
            { VirtualKeyCode.VK_B, "B.mp3" },
            { VirtualKeyCode.VK_N, "N.mp3" },
            { VirtualKeyCode.VK_M, "M.mp3" }
        };

        private static readonly Dictionary<VirtualKeyCode, MediaPlayer> _activePlayers = [];
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
        public static void PlayNote(VirtualKeyCode key)
        {
            if (_activePlayers.TryGetValue(key, out var player))
            {
                player.Stop();
                player.Position = TimeSpan.Zero;
                player.Play();
            }
        }
        public static void StopNote(VirtualKeyCode key)
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