using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UMA;
using System;
using UnityEngine.UI;

public class KinectUMAController {

    private BodySourceManager bodyManager;
    private UMAData avatarData;
    private ulong trackedId;
    public Vector3 angles;
    

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private Dictionary<string, Kinect.JointType> KinectToUMA = new Dictionary<string, Kinect.JointType>() {
        //{ "RightArm", Kinect.JointType.ElbowRight },
        { "RightForeArm", Kinect.JointType.ElbowRight },
        //{ "LeftArm", Kinect.JointType.ElbowLeft },
        //{ "LeftForeArm", Kinect.JointType.WristLeft }
    };

    public KinectUMAController(UMAData avatarData, BodySourceManager bodyManager) {
        this.avatarData = avatarData;
        this.bodyManager = bodyManager;
        this.trackedId = 0;
        this.angles = new Vector3(0, 0, 0);
    }

    public void Update() {
        
        if (bodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = bodyManager.GetData();
        if (data == null)
        {
            return;
        }
        Kinect.Body body = trackedId == 0 ? findFirstValidBody(data) : findBodyById(data, trackedId);
        if (body != null)
        {
            trackedId = body.TrackingId;
        }
        else {
            return;
        }

        if (body.IsTracked) {
            TransformSkeleton(avatarData, body);
        }

    }

    private Kinect.Body findFirstValidBody(Kinect.Body[] data) {
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                return body;
            }
        }
        return null;
    }

    private Kinect.Body findBodyById(Kinect.Body[] data, ulong id) {
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.TrackingId == id)
            {
                return body;
            }
        }
        return null;
    }

    private void TransformSkeleton(UMAData data, Kinect.Body body) {


        
        foreach (int code in avatarData.skeleton.BoneHashes)
        {
            var go = avatarData.skeleton.GetBoneGameObject(code);
            var name = go.name;
            if (KinectToUMA.ContainsKey(name))
            {
                var joint = body.JointOrientations[KinectToUMA[name]].Orientation;
                var kinectQuat = new Quaternion(joint.X, joint.Y, joint.Z, joint.W);
                var local = go.transform.localRotation;
                go.transform.localRotation = new Quaternion(local.x, kinectQuat.y, local.z, local.w);
                angles = kinectQuat.eulerAngles;
            }

        }
    }
}
