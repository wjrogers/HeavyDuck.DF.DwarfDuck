// Generated by ProtoGen, Version=2.4.1.473, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace dfproto {
  
  namespace Proto {
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
    public static partial class Plant {
    
      #region Extension registration
      public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
      }
      #endregion
      #region Static variables
      #endregion
      #region Extensions
      internal static readonly object Descriptor;
      static Plant() {
        Descriptor = null;
      }
      #endregion
      
    }
  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public sealed partial class Plant : pb::GeneratedMessageLite<Plant, Plant.Builder> {
    private Plant() { }
    private static readonly Plant defaultInstance = new Plant().MakeReadOnly();
    private static readonly string[] _plantFieldNames = new string[] { "is_shrub", "material", "x", "y" };
    private static readonly uint[] _plantFieldTags = new uint[] { 24, 32, 8, 16 };
    public static Plant DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override Plant DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override Plant ThisMessage {
      get { return this; }
    }
    
    public const int XFieldNumber = 1;
    private bool hasX;
    private uint x_;
    public bool HasX {
      get { return hasX; }
    }
    [global::System.CLSCompliant(false)]
    public uint X {
      get { return x_; }
    }
    
    public const int YFieldNumber = 2;
    private bool hasY;
    private uint y_;
    public bool HasY {
      get { return hasY; }
    }
    [global::System.CLSCompliant(false)]
    public uint Y {
      get { return y_; }
    }
    
    public const int IsShrubFieldNumber = 3;
    private bool hasIsShrub;
    private bool isShrub_;
    public bool HasIsShrub {
      get { return hasIsShrub; }
    }
    public bool IsShrub {
      get { return isShrub_; }
    }
    
    public const int MaterialFieldNumber = 4;
    private bool hasMaterial;
    private uint material_;
    public bool HasMaterial {
      get { return hasMaterial; }
    }
    [global::System.CLSCompliant(false)]
    public uint Material {
      get { return material_; }
    }
    
    public override bool IsInitialized {
      get {
        if (!hasX) return false;
        if (!hasY) return false;
        if (!hasIsShrub) return false;
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _plantFieldNames;
      if (hasX) {
        output.WriteUInt32(1, field_names[2], X);
      }
      if (hasY) {
        output.WriteUInt32(2, field_names[3], Y);
      }
      if (hasIsShrub) {
        output.WriteBool(3, field_names[0], IsShrub);
      }
      if (hasMaterial) {
        output.WriteUInt32(4, field_names[1], Material);
      }
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasX) {
          size += pb::CodedOutputStream.ComputeUInt32Size(1, X);
        }
        if (hasY) {
          size += pb::CodedOutputStream.ComputeUInt32Size(2, Y);
        }
        if (hasIsShrub) {
          size += pb::CodedOutputStream.ComputeBoolSize(3, IsShrub);
        }
        if (hasMaterial) {
          size += pb::CodedOutputStream.ComputeUInt32Size(4, Material);
        }
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    #region Lite runtime methods
    public override int GetHashCode() {
      int hash = GetType().GetHashCode();
      if (hasX) hash ^= x_.GetHashCode();
      if (hasY) hash ^= y_.GetHashCode();
      if (hasIsShrub) hash ^= isShrub_.GetHashCode();
      if (hasMaterial) hash ^= material_.GetHashCode();
      return hash;
    }
    
    public override bool Equals(object obj) {
      Plant other = obj as Plant;
      if (other == null) return false;
      if (hasX != other.hasX || (hasX && !x_.Equals(other.x_))) return false;
      if (hasY != other.hasY || (hasY && !y_.Equals(other.y_))) return false;
      if (hasIsShrub != other.hasIsShrub || (hasIsShrub && !isShrub_.Equals(other.isShrub_))) return false;
      if (hasMaterial != other.hasMaterial || (hasMaterial && !material_.Equals(other.material_))) return false;
      return true;
    }
    
    public override void PrintTo(global::System.IO.TextWriter writer) {
      PrintField("x", hasX, x_, writer);
      PrintField("y", hasY, y_, writer);
      PrintField("is_shrub", hasIsShrub, isShrub_, writer);
      PrintField("material", hasMaterial, material_, writer);
    }
    #endregion
    
    public static Plant ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static Plant ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static Plant ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static Plant ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static Plant ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static Plant ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static Plant ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static Plant ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static Plant ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static Plant ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private Plant MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(Plant prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
    public sealed partial class Builder : pb::GeneratedBuilderLite<Plant, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(Plant cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private Plant result;
      
      private Plant PrepareBuilder() {
        if (resultIsReadOnly) {
          Plant original = result;
          result = new Plant();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override Plant MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override Plant DefaultInstanceForType {
        get { return global::dfproto.Plant.DefaultInstance; }
      }
      
      public override Plant BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessageLite other) {
        if (other is Plant) {
          return MergeFrom((Plant) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(Plant other) {
        if (other == global::dfproto.Plant.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasX) {
          X = other.X;
        }
        if (other.HasY) {
          Y = other.Y;
        }
        if (other.HasIsShrub) {
          IsShrub = other.IsShrub;
        }
        if (other.HasMaterial) {
          Material = other.Material;
        }
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_plantFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _plantFieldTags[field_ordinal];
            else {
              ParseUnknownField(input, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                return this;
              }
              ParseUnknownField(input, extensionRegistry, tag, field_name);
              break;
            }
            case 8: {
              result.hasX = input.ReadUInt32(ref result.x_);
              break;
            }
            case 16: {
              result.hasY = input.ReadUInt32(ref result.y_);
              break;
            }
            case 24: {
              result.hasIsShrub = input.ReadBool(ref result.isShrub_);
              break;
            }
            case 32: {
              result.hasMaterial = input.ReadUInt32(ref result.material_);
              break;
            }
          }
        }
        
        return this;
      }
      
      
      public bool HasX {
        get { return result.hasX; }
      }
      [global::System.CLSCompliant(false)]
      public uint X {
        get { return result.X; }
        set { SetX(value); }
      }
      [global::System.CLSCompliant(false)]
      public Builder SetX(uint value) {
        PrepareBuilder();
        result.hasX = true;
        result.x_ = value;
        return this;
      }
      public Builder ClearX() {
        PrepareBuilder();
        result.hasX = false;
        result.x_ = 0;
        return this;
      }
      
      public bool HasY {
        get { return result.hasY; }
      }
      [global::System.CLSCompliant(false)]
      public uint Y {
        get { return result.Y; }
        set { SetY(value); }
      }
      [global::System.CLSCompliant(false)]
      public Builder SetY(uint value) {
        PrepareBuilder();
        result.hasY = true;
        result.y_ = value;
        return this;
      }
      public Builder ClearY() {
        PrepareBuilder();
        result.hasY = false;
        result.y_ = 0;
        return this;
      }
      
      public bool HasIsShrub {
        get { return result.hasIsShrub; }
      }
      public bool IsShrub {
        get { return result.IsShrub; }
        set { SetIsShrub(value); }
      }
      public Builder SetIsShrub(bool value) {
        PrepareBuilder();
        result.hasIsShrub = true;
        result.isShrub_ = value;
        return this;
      }
      public Builder ClearIsShrub() {
        PrepareBuilder();
        result.hasIsShrub = false;
        result.isShrub_ = false;
        return this;
      }
      
      public bool HasMaterial {
        get { return result.hasMaterial; }
      }
      [global::System.CLSCompliant(false)]
      public uint Material {
        get { return result.Material; }
        set { SetMaterial(value); }
      }
      [global::System.CLSCompliant(false)]
      public Builder SetMaterial(uint value) {
        PrepareBuilder();
        result.hasMaterial = true;
        result.material_ = value;
        return this;
      }
      public Builder ClearMaterial() {
        PrepareBuilder();
        result.hasMaterial = false;
        result.material_ = 0;
        return this;
      }
    }
    static Plant() {
      object.ReferenceEquals(global::dfproto.Proto.Plant.Descriptor, null);
    }
  }
  
  #endregion
  
}

#endregion Designer generated code
