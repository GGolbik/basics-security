
namespace GGolbik.SecurityTools.Io;

public class WrapperStream : Stream
{
    public override bool CanRead { get { lock (this) { return _stream.CanRead; } } }

    public override bool CanSeek { get { lock (this) { return _stream.CanSeek; } } }

    public override bool CanWrite { get { lock (this) { return _stream.CanWrite; } } }

    public override long Length { get { lock (this) { return _stream.Length; } } }

    public override long Position
    {
        get { lock (this) { return _stream.Position; } }
        set { lock (this) { _stream.Position = value; } }
    }

    private readonly bool LeaveOpen;
    protected Stream _stream;

    public WrapperStream(Stream stream, bool leaveOpen)
    {
        this._stream = stream;
        this.LeaveOpen = leaveOpen;
    }

    public override void Flush()
    {
        lock (this) { _stream.Flush(); }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        lock (this) { return _stream.Read(buffer, offset, count); }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        lock (this) { return _stream.Seek(offset, origin); }
    }

    public override void SetLength(long value)
    {
        lock (this) { _stream.SetLength(value); }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        lock (this) { _stream.Write(buffer, offset, count); }
    }

    public override void Close()
    {
        lock (this)
        {
            if (this.LeaveOpen)
            {
                return;
            }
            base.Close();
        }
    }
}
