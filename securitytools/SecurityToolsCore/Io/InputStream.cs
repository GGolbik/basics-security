
namespace GGolbik.SecurityTools.Io;

public sealed class InputStream : WrapperStream
{
    public override bool CanWrite => false;
    public InputStream(Stream stream, bool leaveOpen) : base(stream, leaveOpen)
    {
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}

