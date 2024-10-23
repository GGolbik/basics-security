
namespace GGolbik.SecurityToolsApp.Web.Models;
public class StreamWrapper : Stream
{
    public override bool CanRead => _stream.CanRead;

    public override bool CanSeek => _stream.CanSeek;

    public override bool CanWrite => _stream.CanWrite;

    public override long Length => _stream.Length;

    public override long Position { get => _stream.Position; set => _stream.Position = value; }

    private readonly bool _leaveOpen;
    private readonly Stream _stream;

    public StreamWrapper(Stream stream, bool leaveOpen)
    {
        this._stream = stream;
        this._leaveOpen = leaveOpen;
    }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }

    public override void Close()
    {
        if (this._leaveOpen)
        {
            return;
        }
        base.Close();
    }
}