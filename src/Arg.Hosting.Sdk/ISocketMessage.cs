namespace Arg.Hosting.Sdk
{
    public interface ISocketMessage
    {
        public byte FirstByte { get; }

        //TODO: Research 'ref' return values in interface method signatures. 
        ref byte[] GetRawBytes(); // whaaat 
    }
}