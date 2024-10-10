
namespace GGolbik.SecurityTools.Io;

/// <summary>
/// A stream which holds a copy of the read and written data.
/// </summary>
public sealed class BufferStream : WrapperStream
{
    public const int OneMibiByte = 1024 * 1024;
    /// <summary>
    /// The size of the buffer.
    /// </summary>
    public readonly int BufferSize = OneMibiByte;

    private MemoryStream _readBuffer { get; } = new MemoryStream();
    public byte[] ReadBuffer { get { lock (this) { return _readBuffer.ToArray(); } } }
    private MemoryStream _writeBuffer { get; } = new MemoryStream();
    public byte[] WriteBuffer { get { lock (this) { return _writeBuffer.ToArray(); } } }
    private bool _hasSeekError = false;
    public bool HasSeekError { get { lock(this) { return _hasSeekError; } } }

    public BufferStream(Stream stream) : base(stream, false)
    {
    }
    public BufferStream(Stream stream, bool leaveOpen, int bufferSize) : base(stream, leaveOpen)
    {
        this.BufferSize = bufferSize;
    }

    public void ReadAllBytes(int bufferSize = 255)
    {
        lock (this)
        {
            bufferSize = bufferSize > 0 ? bufferSize : 256;
            var buffer = new byte[bufferSize];
            while (this.Read(buffer, 0, bufferSize) > 0)
            {
                if (this.BufferSize > 0)
                {
                    if (_readBuffer.Length >= this.BufferSize)
                    {
                        return;
                    }
                }
            }
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        lock (this)
        {
            count = base.Read(buffer, offset, count);
            if (count > 0 && this.BufferSize > 0)
            {
                count = Math.Min(count, this.BufferSize - (int)_readBuffer.Length);
            }
            if (count > 0)
                _readBuffer.Write(buffer, 0, count);
            return count;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        lock (this)
        {
            base.Write(buffer, offset, count);
            if (count > 0 && this.BufferSize > 0)
            {
                count = Math.Min(count, this.BufferSize - (int)_writeBuffer.Length);
            }
            _writeBuffer.Write(buffer, offset, count);
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        lock (this)
        {
            var result = base.Seek(offset, origin);
            if (origin == SeekOrigin.Current)
            {
                try
                {
                    if (offset > 0)
                    {
                        offset = Math.Min(offset, this.BufferSize - _readBuffer.Length);
                        byte[] buffer = new byte[offset];
                        _readBuffer.Write(buffer, 0, (int)offset);
                    }
                    _readBuffer.Seek(offset, origin);
                }
                catch
                {
                    _hasSeekError = true;
                }
            }
            else
            {
                _hasSeekError = true;
            }
            return result;
        }
    }
}
