using NAudio.Wave;

public class MonoToStereoProvider : IWaveProvider
{
    private readonly IWaveProvider sourceWaveProvider;
    public float LeftVolume { get; set; } = 0.0f; // Default volume is full
    public float RightVolume { get; set; } = 1.0f; // Default volume is full

    public MonoToStereoProvider(IWaveProvider sourceWaveProvider)
    {
        if (sourceWaveProvider.WaveFormat.Channels != 1)
        {
            throw new InvalidOperationException("Source must be mono.");
        }

        this.sourceWaveProvider = sourceWaveProvider;
        this.WaveFormat = new WaveFormat(sourceWaveProvider.WaveFormat.SampleRate, 16, 2); // Output is stereo
    }

    public WaveFormat WaveFormat { get; }

    public int Read(byte[] buffer, int offset, int count)
    {
        // Ensure count is even
        count -= count % 4;
        var sourceBuffer = new byte[count / 2];
        int bytesRead = sourceWaveProvider.Read(sourceBuffer, 0, sourceBuffer.Length);
        int outIndex = offset;

        for (int n = 0; n < bytesRead; n += 2)
        {
            short sample = (short)((sourceBuffer[n + 1] << 8) | sourceBuffer[n]);

            // Apply volume to left channel
            short leftSample = (short)(sample * LeftVolume);
            buffer[outIndex++] = (byte)(leftSample & 0xFF);
            buffer[outIndex++] = (byte)((leftSample >> 8) & 0xFF);

            // Apply volume to right channel
            short rightSample = (short)(sample * RightVolume);
            buffer[outIndex++] = (byte)(rightSample & 0xFF);
            buffer[outIndex++] = (byte)((rightSample >> 8) & 0xFF);
        }

        return bytesRead * 2; // Because output is stereo
    }
}
