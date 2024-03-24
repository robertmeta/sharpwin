using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

public class SoundManager
{
    private static readonly Lazy<SoundManager> _instance =
        new Lazy<SoundManager>(() => new SoundManager());

    public static SoundManager Instance => _instance.Value;

    private SoundPlayer _currentSound;

    private SoundManager()
    {
        // Private constructor to enforce singleton pattern
    }

    public async Task PlaySoundAsync(string filePath, float volume)
    {
        // Stop the currently playing sound, if any
        if (_currentSound != null && _currentSound.IsLoadCompleted)
        {
            _currentSound.Stop();
            _currentSound.Dispose();
        }

        // Create a new sound instance
        _currentSound = new SoundPlayer(filePath);

        // Set the volume (0.0f to 1.0f)
        _currentSound.Volume = volume;

        // Load the sound asynchronously
        await Task.Run(() => _currentSound.Load());

        // Play the new sound
        _currentSound.Play();
    }

    public void StopCurrentSound()
    {
        if (_currentSound != null && _currentSound.IsLoadCompleted)
        {
            _currentSound.Stop();
            _currentSound.Dispose();
        }
    }
}
