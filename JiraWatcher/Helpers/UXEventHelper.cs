using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;

namespace JiraWatcher.Helpers
{
    internal static class UXEventHelper
    {
        private static readonly Dictionary<string, string> CustomSoundFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Gasp",        "Gasp.wav" },
            { "Bright Ping", "BrightPing.wav" },
        };

        // Pre-loaded players — file data is in memory, no disk I/O at play time.
        private static readonly Dictionary<string, SoundPlayer> _cachedPlayers =
            new Dictionary<string, SoundPlayer>(StringComparer.OrdinalIgnoreCase);

        // Silent WAV buffer used to wake the Windows audio device before playing the
        // real sound, preventing start-of-sound clipping caused by the audio endpoint
        // going into a low-power idle state between plays.
        private static readonly byte[] _silentWav = BuildSilentWav(durationMs: 150);

        // If more than this has elapsed the audio device may have gone idle again.
        private static DateTime _lastPlayUtc = DateTime.MinValue;
        private static readonly TimeSpan _deviceSleepThreshold = TimeSpan.FromSeconds(5);

        static UXEventHelper()
        {
            foreach (var (name, fileName) in CustomSoundFiles)
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Resources", "Sounds", fileName);
                if (File.Exists(path))
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Load();
                    _cachedPlayers[name] = player;
                }
            }
        }

        internal static void Notification()
        {
            PlayConfiguredSound();
            FlashWindowHelper.FlashWindow(Process.GetCurrentProcess().MainWindowHandle, 2000);
        }

        internal static void PlayNotificationSound(string? sound)
        {
            PlaySound(sound);
        }

        private static void PlayConfiguredSound()
        {
            PlaySound(Properties.Settings.Default.NotificationSound);
        }

        private static void PlaySound(string? sound)
        {
            if (string.Equals(sound, "None", StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                // Wake the audio device with a brief silent pre-roll when it may have
                // gone idle. PlaySync blocks until the silence finishes, guaranteeing
                // the hardware is fully initialized before the real sound starts.
                if ((DateTime.UtcNow - _lastPlayUtc) > _deviceSleepThreshold)
                    WarmUpAudioDevice();

                _lastPlayUtc = DateTime.UtcNow;

                if (sound != null && _cachedPlayers.TryGetValue(sound, out SoundPlayer? cachedPlayer))
                {
                    cachedPlayer.Play();
                    return;
                }

                switch (sound)
                {
                    case "Asterisk":
                        SystemSounds.Asterisk.Play();
                        break;
                    case "Hand":
                        SystemSounds.Hand.Play();
                        break;
                    case "Beep":
                        SystemSounds.Beep.Play();
                        break;
                    case "Exclamation":
                    default:
                        SystemSounds.Exclamation.Play();
                        break;
                }
            }
            catch
            {
                Console.Beep(880, 180);
            }
        }

        private static void WarmUpAudioDevice()
        {
            try
            {
                using MemoryStream ms = new MemoryStream(_silentWav);
                using SoundPlayer warmUp = new SoundPlayer(ms);
                warmUp.PlaySync();
            }
            catch { /* warm-up failure is non-critical */ }
        }

        /// Builds a minimal valid PCM WAV containing silence.
        /// 8-bit mono at 8 kHz — small and fast to play through waveOut.
        private static byte[] BuildSilentWav(int durationMs)
        {
            const int sampleRate = 8000;
            const int channels = 1;
            const int bitsPerSample = 8;

            int numSamples = sampleRate * durationMs / 1000;
            int dataSize = numSamples * channels * (bitsPerSample / 8);

            using MemoryStream ms = new MemoryStream(44 + dataSize);
            using BinaryWriter bw = new BinaryWriter(ms);

            bw.Write("RIFF".ToCharArray());
            bw.Write(36 + dataSize);
            bw.Write("WAVE".ToCharArray());
            bw.Write("fmt ".ToCharArray());
            bw.Write(16);
            bw.Write((short)1);   // PCM
            bw.Write((short)channels);
            bw.Write(sampleRate);
            bw.Write(sampleRate * channels * bitsPerSample / 8);
            bw.Write((short)(channels * bitsPerSample / 8));
            bw.Write((short)bitsPerSample);
            bw.Write("data".ToCharArray());
            bw.Write(dataSize);

            // 8-bit unsigned PCM: silence is 0x80 (midpoint), not 0x00
            for (int i = 0; i < numSamples; i++)
                bw.Write((byte)128);

            return ms.ToArray();
        }
    }
}
