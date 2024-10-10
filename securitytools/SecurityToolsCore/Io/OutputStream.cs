
namespace GGolbik.SecurityTools.Io;

public sealed class OutputStream : WrapperStream
{
    public override bool CanRead => false;
    public OutputStream(Stream stream, bool leaveOpen) : base(stream, leaveOpen)
    {

    }
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}