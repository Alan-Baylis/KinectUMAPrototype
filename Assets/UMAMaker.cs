using UnityEngine;
using System.Collections;
using UMA;

public class UMAMaker : MonoBehaviour
{
    public UMAGeneratorBase generator;
    public SlotLibrary slotLibrary;
    public OverlayLibrary overlayLibrary;
    public RaceLibrary raceLibrary;

    private UMADynamicAvatar avatar;
    private UMAData umaData;
    private UMADnaHumanoid umaDna;
    private UMADnaTutorial umaTutorialDna;

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

        avatar.UpdateNewRace();


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
        umaData.umaRecipe.slotDataList[4].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody02"));

        umaData.umaRecipe.slotDataList[5] = slotLibrary.InstantiateSlot("MaleLegs");
        umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody02"));

        umaData.umaRecipe.slotDataList[6] = slotLibrary.InstantiateSlot("MaleFeet");
        umaData.umaRecipe.slotDataList[6].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody02"));

        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));
        umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));
    }

    void Start()
    {
        GenerateUMA();
    }
}
