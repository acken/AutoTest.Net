using System;
using System.IO;
using System.Runtime.Serialization;
using AutoTest.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace AutoTest.Messages.Serializers
{
	[StructLayout(LayoutKind.Explicit)]  
	public struct IntToBytes  
	{  
	    public IntToBytes(Int32 _value) { b0 = b1 = b2 = b3 = 0; i32 = _value; }  
	    public IntToBytes(byte _b0, byte _b1, byte _b2, byte _b3) {  
	        i32 = 0;  
	        b0 = _b0;  
	        b1 = _b1;  
	        b2 = _b2;  
	        b3 = _b3;  
	    }  
	    [FieldOffset(0)]  
	    public Int32 i32;  
	    [FieldOffset(0)]  
	    public byte b0;  
	    [FieldOffset(1)]  
	    public byte b1;  
	    [FieldOffset(2)]  
	    public byte b2;  
	    [FieldOffset(3)]  
	    public byte b3;  
	} 
	
	public class CustomBinaryFormatter : IFormatter  
	{  
	    private SerializationBinder m_Binder;  
	    private StreamingContext m_StreamingContext;  
	    private ISurrogateSelector m_SurrogateSelector;  
	    private readonly MemoryStream m_WriteStream;
	    private readonly MemoryStream m_ReadStream;  
	    private readonly BinaryWriter m_Writer;  
	    private readonly BinaryReader m_Reader;
	    private readonly Dictionary<Type, int> m_ByType = new Dictionary<Type, int >();  
	    private readonly Dictionary<int, Type> m_ById = new Dictionary<int, Type>();  
	    private readonly byte[] m_LengthBuffer = new byte[4];  
	    private readonly byte[] m_CopyBuffer;  
	  
	    public CustomBinaryFormatter()  
	    {  
	        m_CopyBuffer = new byte[20000];  
	        m_WriteStream = new MemoryStream(10000);  
	        m_ReadStream = new MemoryStream(10000);  
	        m_Writer = new BinaryWriter(m_WriteStream);  
	        m_Reader = new BinaryReader(m_ReadStream);  
			
			Register<BuildMessage>(1);
			Register<BuildRunMessage>(2);
			Register<BuildRunResults>(3);
			Register<RunFinishedMessage>(4);
			Register<RunReport>(5);
			Register<RunAction>(6);
			Register<ErrorMessage>(7);
			Register<InformationMessage>(8);
			Register<RunInformationMessage>(9);
			Register<RunStartedMessage>(10);
			Register<WarningMessage>(11);
			Register<FileChangeMessage>(12);
			Register<AssemblyChangeMessage>(13);
			Register<ProjectChangeMessage>(14);
			Register<TestRunResults>(15);
			Register<TestResult>(16);
			Register<TestRunMessage>(17);
	    }  
	  
	    public void Register<T>(int _TypeId) where T:ICustomBinarySerializable
	    {  
	        m_ById.Add(_TypeId, typeof(T));  
	        m_ByType.Add(typeof (T), _TypeId);  
	    }  
	  
	    public object Deserialize(Stream serializationStream)  
	    {  
	        if(serializationStream.Read(m_LengthBuffer, 0, 4) != 4)  
	            throw new SerializationException("Could not read length from the stream.");  
	        IntToBytes length = new IntToBytes(m_LengthBuffer[0], m_LengthBuffer[1], m_LengthBuffer[2], m_LengthBuffer[3]);  
	        //TODO make this support partial reads from stream  
	        if(serializationStream.Read(m_CopyBuffer, 0, length.i32) != length.i32)   
	            throw new SerializationException("Could not read " + length + " bytes from the stream.");  
	        m_ReadStream.Seek(0L, SeekOrigin.Begin);  
	        m_ReadStream.Write(m_CopyBuffer, 0, length.i32);  
	        m_ReadStream.Seek(0L, SeekOrigin.Begin);  
	        int typeid = m_Reader.ReadInt32();  
	        Type t;  
	        if(!m_ById.TryGetValue(typeid, out t))  
	            throw new SerializationException("TypeId " + typeid + " is not a registerred type id");  
	        object obj = FormatterServices.GetUninitializedObject(t);  
	        ICustomBinarySerializable deserialize = (ICustomBinarySerializable) obj;  
	        deserialize.SetDataFrom(m_Reader);  
	        if(m_ReadStream.Position != length.i32)   
	            throw new SerializationException("object of type " + t + " did not read its entire buffer during deserialization. This is most likely an inbalance between the writes and the reads of the object.");  
	        return deserialize;  
	    }  
	  
	    public void Serialize(Stream serializationStream, object graph)  
	    {  
	        int key;  
	        if (!m_ByType.TryGetValue(graph.GetType(), out key))  
	            throw new SerializationException(graph.GetType() + " has not been registered with the serializer");  
	        ICustomBinarySerializable c = (ICustomBinarySerializable) graph; //this will always work due to generic constraint on the Register  
	        m_WriteStream.Seek(0L, SeekOrigin.Begin);  
	        m_Writer.Write((int) key);  
	        c.WriteDataTo(m_Writer);  
	        IntToBytes length = new IntToBytes((int) m_WriteStream.Position);  
	        serializationStream.WriteByte(length.b0);  
	        serializationStream.WriteByte(length.b1);  
	        serializationStream.WriteByte(length.b2);  
	        serializationStream.WriteByte(length.b3);  
	        serializationStream.Write(m_WriteStream.GetBuffer(), 0, (int) m_WriteStream.Position);  
	    }  
	  
	    public ISurrogateSelector SurrogateSelector  
	    {  
	        get { return m_SurrogateSelector; }  
	        set { m_SurrogateSelector = value; }  
	    }  
	  
	    public SerializationBinder Binder  
	    {  
	        get { return m_Binder; }  
	        set { m_Binder = value; }  
	    }  
	  
	    public StreamingContext Context  
	    {  
	        get { return m_StreamingContext; }  
	        set { m_StreamingContext = value; }  
	    }  
	}
}

