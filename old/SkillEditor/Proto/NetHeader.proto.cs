//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: NetHeader.proto
namespace NetHeader
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ErrorMessage")]
  public partial class ErrorMessage : global::ProtoBuf.IExtensible
  {
    public ErrorMessage() {}
    
    private Sys _sys;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"sys", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public Sys sys
    {
      get { return _sys; }
      set { _sys = value; }
    }
    private ErrorCode _code;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"code", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ErrorCode code
    {
      get { return _code; }
      set { _code = value; }
    }
    private string _err;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"err", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string err
    {
      get { return _err; }
      set { _err = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"Sys")]
    public enum Sys
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"ERROR", Value=0)]
      ERROR = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CURRENTCY", Value=1)]
      CURRENTCY = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER", Value=2)]
      USER = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MAP", Value=3)]
      MAP = 3
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"CurrencyResult")]
    public enum CurrencyResult
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SUCCESS", Value=0)]
      SUCCESS = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FAIL", Value=1)]
      FAIL = 1
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"ErrorCode")]
    public enum ErrorCode
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SERVER_EXCEPTION", Value=0)]
      SERVER_EXCEPTION = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PARAM_NOT_FOUND", Value=1)]
      PARAM_NOT_FOUND = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PARAM_ERROR", Value=2)]
      PARAM_ERROR = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"TOKEN_ERROR", Value=3)]
      TOKEN_ERROR = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOT_FOUND_SERVER", Value=4)]
      NOT_FOUND_SERVER = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MAP_ERROR", Value=5)]
      MAP_ERROR = 5
    }
  
}