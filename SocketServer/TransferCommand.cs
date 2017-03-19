using System.Collections;
using ProtoBuf;
using System.Collections.Generic;
using System;

[ProtoContract]
public class TransferCommand  {
    public TransferCommand() { }
    public TransferCommand(int userID1, ushort objID1, UserStyle userStyle1, string userName1)
    {
        userID = userID1;
        objID = objID1;
        userStyle = userStyle1;
        userName = userName1;
    }

    [ProtoMember(1)]
    public int userID { get; set; }
    [ProtoMember(2)]
    public ushort objID { get; set; }
    [ProtoMember(3)]
    public UserStyle userStyle { get; set; }
    [ProtoMember(4)]
    public string userName { get; set; }
    [ProtoMember(5)]
    public ThreeDInfo threeDInfo { get; set; }//可以每10s同步一次
    [ProtoMember(6)]
    public MoveStyle moveStyle { get; set; }
    [ProtoMember(7)]
    public Interaction interaction { get; set; }
    [ProtoMember(8)]
    public List<TransferSignal> globalSignalList { get; set; }

    public enum UserStyle:byte
    { Teacher,Student,Technology,Unkown}

    public enum MoveStyle:byte
    { None,GoForward,GoBack,GoLeft,GoRight }
}

[ProtoContract]
public class TransferSignal
{
    [ProtoMember(1)]
    Object obj1;
    [ProtoMember(2)]
    Object obj2;
    [ProtoMember(3)]
    Object obj3;
    [ProtoMember(4)]
    Object obj4;

    [ProtoMember(4)]
    public SignalStyle style { get; set; }

    public enum SignalStyle : byte
    {
        LogInSignal, LogedInSignal, LogOutSignal, ProcedureSignal, MicControlSignal
    }
}

[ProtoContract]
public class Interaction
{
    [ProtoMember(1)]
    public ActionStyle actionStyle { get; set; }
    [ProtoMember(2)]
    public ushort interactObjID { get; set; }
    public Interaction() { }
    public Interaction(ActionStyle style, ushort actObjID)
    {
        actionStyle = style;
        interactObjID = actObjID;
    }
    public enum ActionStyle:byte
    {
        None,//无动作
        Push,//推
        Pull,//拉
        Raise,//举起
        Prise,//表扬
        WaveHand,//招手
        like //点赞
    }
}

[ProtoContract]
public class ThreeDInfo
{
    [ProtoMember(1)]
    public Position3f position;
    [ProtoMember(2)]
    public Rotation3f rotation;
    [ProtoMember(3)]
    public Scale3f scale;

    public ThreeDInfo() {}
    //public ThreeDInfo(Vector3 position1, Vector3 rotation1, Vector3 scale1)
    //{
    //    position = new Position3f(position1);
    //    rotation = new Rotation3f(rotation1);
    //    scale = new Scale3f(scale1);
    //}
    //public ThreeDInfo(Vector3 position1, Vector3 rotation1)
    //{
    //    position = new Position3f(position1);
    //    rotation = new Rotation3f(rotation1);
    //    scale = null;
    //}

    //public ThreeDInfo(Vector3 position1)
    //{
    //    position = new Position3f(position1);
    //    rotation = null;
    //    scale = null;
    //}
}

[ProtoContract]
public class Position3f
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;
    public Position3f()
    {  }
    //public Position3f(Vector3 position)
    //{
    //    x = position.x;
    //    y = position.y;
    //    z = position.z;
    //}
}

[ProtoContract]
public class Rotation3f
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;

    public Rotation3f() { }
    //public Rotation3f(Vector3 rotation)
    //{
    //    x = rotation.x;
    //    y = rotation.y;
    //    z = rotation.z;
    //}
}

[ProtoContract]
public class Scale3f
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;

    public Scale3f() { }
    //public Scale3f(Vector3 scale)
    //{
    //    x = scale.x;
    //    y = scale.y;
    //    z = scale.z;
    //}
}