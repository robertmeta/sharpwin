using System;
using System.Speech.Synthesis;

class Program
{
    static void Main(string[] args)
    {
        // Create a new instance of the SpeechSynthesizer
        using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
        {
            // Set the voice (optional)
            synthesizer.SelectVoice("Microsoft David Desktop");

            // Configure the audio output (optional)
            synthesizer.SetOutputToDefaultAudioDevice();

            Console.WriteLine("Enter text to speak (press Enter without typing anything to exit):");

            string input;
            while ((input = Console.ReadLine()) != "")
            {
                // Speak the input text
                synthesizer.Speak(input);
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
