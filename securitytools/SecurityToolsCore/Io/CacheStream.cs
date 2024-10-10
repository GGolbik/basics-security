
namespace GGolbik.SecurityTools.Io;

/// <summary>
/// A stream which keeps data in memory until the buffer size is reached. Otherwise, the data is kept in a temp file.
/// </summary>
public sealed class CacheStream : WrapperStream
{
    public const int OneMibiByte = 1024 * 1024;
    /// <summary>
    /// The size of the in memory buffer. If there is more data, the cache stream switches to a file stream.
    /// </summary>
    public readonly int BufferSize = OneMibiByte;
    /// <summary>
    /// The buffer size to use with the file stream.
    /// </summary>
    public readonly int FileBufferSize;

    public CacheStream() : base(new MemoryStream(), false)
    {
    }

    public CacheStream(int bufferSize) : this()
    {
        this.BufferSize = Math.Max(0, bufferSize);
    }

    public CacheStream(int bufferSize, int fileBufferSize) : this(bufferSize)
    {
        this.FileBufferSize = fileBufferSize;
    }

    private void SwitchToFileStream()
    {
        if(_stream is MemoryStream memStream)
        {
            // create file mem
            FileStream fileStream = new FileStream(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read, this.FileBufferSize > 0 ? this.FileBufferSize : 4096, FileOptions.DeleteOnClose);
            // copy mem to file
            fileStream.Write(memStream.ToArray());
            fileStream.Position = memStream.Position;
            // dispose mem
            memStream.Dispose();
            // swap
            _stream = fileStream;
        }
    }

    public override void SetLength(long value)
    {
        lock (this)
        {
            if (value > this.BufferSize)
            {
                this.SwitchToFileStream();
            }
            _stream.SetLength(value);
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        lock (this)
        {
            if(this.Position + count > this.BufferSize)
            {
                this.SwitchToFileStream();
            }
            _stream.Write(buffer, offset, count);
        }
    }

}