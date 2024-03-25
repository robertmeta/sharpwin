public class TonePlayer
{
    public async Task PlayPureToneAsync(int frequencyInHz, int durationInMillis)
    {
        await Task.Run(() =>
        {
            Console.Beep(frequencyInHz, durationInMillis);
        });
    }

    public void Stop()
    {
        // No need to implement the stop method since Console.Beep is a blocking call
        // and will automatically stop after the specified duration
    }
}
