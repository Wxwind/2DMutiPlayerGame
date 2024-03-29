// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: PlayerInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace NetMessage {

  /// <summary>Holder for reflection information generated from PlayerInfo.proto</summary>
  public static partial class PlayerInfoReflection {

    #region Descriptor
    /// <summary>File descriptor for PlayerInfo.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static PlayerInfoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBQbGF5ZXJJbmZvLnByb3RvEgRUZXN0IncKClBsYXllckluZm8SEgoKUGxh",
            "eWVyTmFtZRgBIAEoCRIKCgJpcBgCIAEoCRIQCghwbGF5ZXJJZBgDIAEoBRIM",
            "CgRQb3NYGAQgASgCEgwKBFBvc1kYBSABKAISDwoHQ29sb3JJZBgGIAEoBRIK",
            "CgJIcBgHIAEoBUINqgIKTmV0TWVzc2FnZWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::NetMessage.PlayerInfo), global::NetMessage.PlayerInfo.Parser, new[]{ "PlayerName", "Ip", "PlayerId", "PosX", "PosY", "ColorId", "Hp" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class PlayerInfo : pb::IMessage<PlayerInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<PlayerInfo> _parser = new pb::MessageParser<PlayerInfo>(() => new PlayerInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<PlayerInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::NetMessage.PlayerInfoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PlayerInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PlayerInfo(PlayerInfo other) : this() {
      playerName_ = other.playerName_;
      ip_ = other.ip_;
      playerId_ = other.playerId_;
      posX_ = other.posX_;
      posY_ = other.posY_;
      colorId_ = other.colorId_;
      hp_ = other.hp_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PlayerInfo Clone() {
      return new PlayerInfo(this);
    }

    /// <summary>Field number for the "PlayerName" field.</summary>
    public const int PlayerNameFieldNumber = 1;
    private string playerName_ = "";
    /// <summary>
    ///进入房间时展示的玩家信息
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PlayerName {
      get { return playerName_; }
      set {
        playerName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "ip" field.</summary>
    public const int IpFieldNumber = 2;
    private string ip_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Ip {
      get { return ip_; }
      set {
        ip_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "playerId" field.</summary>
    public const int PlayerIdFieldNumber = 3;
    private int playerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PlayerId {
      get { return playerId_; }
      set {
        playerId_ = value;
      }
    }

    /// <summary>Field number for the "PosX" field.</summary>
    public const int PosXFieldNumber = 4;
    private float posX_;
    /// <summary>
    ///仅用于进入游戏时初始化玩家实体信息
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float PosX {
      get { return posX_; }
      set {
        posX_ = value;
      }
    }

    /// <summary>Field number for the "PosY" field.</summary>
    public const int PosYFieldNumber = 5;
    private float posY_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float PosY {
      get { return posY_; }
      set {
        posY_ = value;
      }
    }

    /// <summary>Field number for the "ColorId" field.</summary>
    public const int ColorIdFieldNumber = 6;
    private int colorId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ColorId {
      get { return colorId_; }
      set {
        colorId_ = value;
      }
    }

    /// <summary>Field number for the "Hp" field.</summary>
    public const int HpFieldNumber = 7;
    private int hp_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Hp {
      get { return hp_; }
      set {
        hp_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as PlayerInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(PlayerInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerName != other.PlayerName) return false;
      if (Ip != other.Ip) return false;
      if (PlayerId != other.PlayerId) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(PosX, other.PosX)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(PosY, other.PosY)) return false;
      if (ColorId != other.ColorId) return false;
      if (Hp != other.Hp) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerName.Length != 0) hash ^= PlayerName.GetHashCode();
      if (Ip.Length != 0) hash ^= Ip.GetHashCode();
      if (PlayerId != 0) hash ^= PlayerId.GetHashCode();
      if (PosX != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(PosX);
      if (PosY != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(PosY);
      if (ColorId != 0) hash ^= ColorId.GetHashCode();
      if (Hp != 0) hash ^= Hp.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (PlayerName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(PlayerName);
      }
      if (Ip.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Ip);
      }
      if (PlayerId != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PlayerId);
      }
      if (PosX != 0F) {
        output.WriteRawTag(37);
        output.WriteFloat(PosX);
      }
      if (PosY != 0F) {
        output.WriteRawTag(45);
        output.WriteFloat(PosY);
      }
      if (ColorId != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(ColorId);
      }
      if (Hp != 0) {
        output.WriteRawTag(56);
        output.WriteInt32(Hp);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (PlayerName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(PlayerName);
      }
      if (Ip.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Ip);
      }
      if (PlayerId != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PlayerId);
      }
      if (PosX != 0F) {
        output.WriteRawTag(37);
        output.WriteFloat(PosX);
      }
      if (PosY != 0F) {
        output.WriteRawTag(45);
        output.WriteFloat(PosY);
      }
      if (ColorId != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(ColorId);
      }
      if (Hp != 0) {
        output.WriteRawTag(56);
        output.WriteInt32(Hp);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (PlayerName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PlayerName);
      }
      if (Ip.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Ip);
      }
      if (PlayerId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerId);
      }
      if (PosX != 0F) {
        size += 1 + 4;
      }
      if (PosY != 0F) {
        size += 1 + 4;
      }
      if (ColorId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ColorId);
      }
      if (Hp != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Hp);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(PlayerInfo other) {
      if (other == null) {
        return;
      }
      if (other.PlayerName.Length != 0) {
        PlayerName = other.PlayerName;
      }
      if (other.Ip.Length != 0) {
        Ip = other.Ip;
      }
      if (other.PlayerId != 0) {
        PlayerId = other.PlayerId;
      }
      if (other.PosX != 0F) {
        PosX = other.PosX;
      }
      if (other.PosY != 0F) {
        PosY = other.PosY;
      }
      if (other.ColorId != 0) {
        ColorId = other.ColorId;
      }
      if (other.Hp != 0) {
        Hp = other.Hp;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            PlayerName = input.ReadString();
            break;
          }
          case 18: {
            Ip = input.ReadString();
            break;
          }
          case 24: {
            PlayerId = input.ReadInt32();
            break;
          }
          case 37: {
            PosX = input.ReadFloat();
            break;
          }
          case 45: {
            PosY = input.ReadFloat();
            break;
          }
          case 48: {
            ColorId = input.ReadInt32();
            break;
          }
          case 56: {
            Hp = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            PlayerName = input.ReadString();
            break;
          }
          case 18: {
            Ip = input.ReadString();
            break;
          }
          case 24: {
            PlayerId = input.ReadInt32();
            break;
          }
          case 37: {
            PosX = input.ReadFloat();
            break;
          }
          case 45: {
            PosY = input.ReadFloat();
            break;
          }
          case 48: {
            ColorId = input.ReadInt32();
            break;
          }
          case 56: {
            Hp = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
