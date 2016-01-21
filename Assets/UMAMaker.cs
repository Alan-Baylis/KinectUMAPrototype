using UnityEngine;
using System.Collections;
using UMA;
using Kinect = Windows.Kinect;
using System.Collections.Generic;
using UnityEngine.UI;

public class UMAMaker : MonoBehaviour
{
    public UMAGeneratorBase generator;
    public SlotLibrary slotLibrary;
    public OverlayLibrary overlayLibrary;
    public RaceLibrary raceLibrary;
    public RuntimeAnimatorController animController;
    public KinectUMAController kinectController;
    public BodySourceManager bodyManager;

    private UMADynamicAvatar avatar;
    private UMAData umaData;
    private UMADnaHumanoid umaDna;
    private UMADnaTutorial umaTutorialDna;
    public Text output;

    private const int NUM_SLOTS = 20;

    void GenerateUMA()
    {
        GameObject myUMA = new GameObject("MyUMA");
        avatar = myUMA.AddComponent<UMADynamicAvatar>();

        avatar.Initialize();
        umaData = avatar.umaData;
        avatar.umaGenerator = generator;
        umaData.umaGenerator = generator;
        umaData.umaRecipe = new UMAData.UMARecipe();
        umaData.umaRecipe.slotDataList = new SlotData[NUM_SLOTS];

        umaDna = new UMADnaHumanoid();
        umaTutorialDna = new UMADnaTutorial();
        
        umaData.umaRecipe.AddDna(umaDna);
        umaData.umaRecipe.AddDna(umaTutorialDna);

        

        CreateMale();

        avatar.animationController = animController;

        avatar.UpdateNewRace();

        myUMA.transform.parent = this.gameObject.transform;
        myUMA.transform.localPosition = Vector3.zero;
        myUMA.transform.localRotation = Quaternion.identity;
    }

    void CreateMale()
    {
        var umaRecipe = avatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

        umaData.umaRecipe.slotDataList[0] = slotLibrary.InstantiateSlot("MaleFace");
        umaData.umaRecipe.slotDataList[0].AddOverlay(overlayLibrary.InstantiateOverlay("MaleHead02"));

        umaData.umaRecipe.slotDataList[1] = slotLibrary.InstantiateSlot("MaleEyes");
        umaData.umaRecipe.slotDataList[1].AddOverlay(overlayLibrary.InstantiateOverlay("EyeOverlay"));

        umaData.umaRecipe.slotDataList[2] = slotLibrary.InstantiateSlot("MaleInnerMouth");
        umaData.umaRecipe.slotDataList[2].AddOverlay(overlayLibrary.InstantiateOverlay("InnerMouth"));

        umaData.umaRecipe.slotDataList[3] = slotLibrary.InstantiateSlot("MaleTorso");
        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody02"));

        umaData.umaRecipe.slotDataList[4] = slotLibrary.InstantiateSlot("MaleHands");
        umaData.umaRecipe.slotDataList[4].SetOverlayList(umaRecipe.slotDataList[3].GetOverlayList());

        umaData.umaRecipe.slotDataList[5] = slotLibrary.InstantiateSlot("MaleLegs");
        umaData.umaRecipe.slotDataList[5].SetOverlayList(umaRecipe.slotDataList[3].GetOverlayList());

        umaData.umaRecipe.slotDataList[6] = slotLibrary.InstantiateSlot("MaleFeet");
        umaData.umaRecipe.slotDataList[6].SetOverlayList(umaRecipe.slotDataList[3].GetOverlayList());

        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));
        umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));

        umaData.umaRecipe.slotDataList[0].AddOverlay(overlayLibrary.InstantiateOverlay("MaleEyebrow01", Color.black));

        // umaDna.headSize = 1f;
    }

    void Start()
    {
        GenerateUMA();
        kinectController = new KinectUMAController(avatar.umaData, bodyManager);
        
    }

    void Update()
    {
       /* var enumerable =  umaData.skeleton.BoneHashes.GetEnumerator();
        enumerable.MoveNext();
        int firstCode = enumerable.Current;
        

        foreach (int code in umaData.skeleton.BoneHashes) {
            var go = umaData.skeleton.GetBoneGameObject(code);
            var name = go.name;
            if (name == "RightForeArm") {
                go.transform.Rotate(0.0f, 40.0f * Time.deltaTime, 0.0f);
            }
        }*/

        kinectController.Update();
        Debug.Log(kinectController.angles);
        if (kinectController.angles != null)
        {
            output.text = kinectController.angles.ToString();
        }
        
    }
}
