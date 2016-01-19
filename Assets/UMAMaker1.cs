using UnityEngine;
using System.Collections;
using UMA;
using System.IO;
using UMA.PoseTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UMAMaker1 : MonoBehaviour 
{
	public UMAGeneratorBase generator;
	public SlotLibrary slotLibraray;
	public OverlayLibrary overlayLibrary;
	public RaceLibrary raceLibrary;
	public RuntimeAnimatorController animController;
	
	private UMADynamicAvatar umaDynamicAvatar;
	private UMAData umaData;
	private UMADnaHumanoid umaDna;
	private UMADnaTutorial umaTutorialDNA;
	
	private int numberOfSlots = 20;
	
	[Range (0.0f,1.0f)]
	public float bodyMass = 0.5f;
	
	[Range (-1.0f,1.0f)]
	public float happy = 0f;
	
	public bool vestState = false;
	private bool lastVestState = false;
	
	public Color vestColor = Color.white;
	private Color lastVestColor = Color.white;
	
	public bool hairState = false;
	private bool lastHairState = false;
	
	public Color hairColor ;
	private Color lastHairColor ;
	
	public string SaveString = "";
	public bool save;
	public bool load;
	
	public UMAExpressionPlayer expressionPlayer;
	
	void Start()
	{
		GenerateUMA();
	}
	
	void Update()
	{
		if(bodyMass != umaDna.upperMuscle)
		{
			SetBodyMass(bodyMass);
			umaData.isShapeDirty = true;
			umaData.Dirty();
		}
		
		if(happy != expressionPlayer.midBrowUp_Down)
		{
			expressionPlayer.midBrowUp_Down = happy;
			expressionPlayer.leftMouthSmile_Frown = happy;
			expressionPlayer.rightMouthSmile_Frown = happy;
		}
		
		if(vestState && !lastVestState)
		{
			lastVestState = true;
			AddOverlay(3,"SA_Tee",vestColor);
			AddOverlay(3,"SA_Logo");
			umaData.isTextureDirty = true;
			umaData.Dirty();
		}
		if(!vestState && lastVestState)
		{
			lastVestState = false;
			RemoveOverlay(3,"SA_Tee");
			RemoveOverlay(3,"SA_Logo");
			umaData.isTextureDirty = true;
			umaData.Dirty();
		}
		if(vestColor != lastVestColor && vestState)
		{
			lastVestColor = vestColor;
			ColorOverlay(3,"SA_Tee",vestColor);
			umaData.isTextureDirty = true;
			umaData.Dirty();
		}
		
		if(hairState && !lastHairState)
		{
			lastHairState = hairState;
			SetSlot(7,"M_Hair_Shaggy");
			AddOverlay(7,"M_Hair_Shaggy",hairColor);
			umaData.isMeshDirty = true;
			umaData.isTextureDirty = true;
			umaData.isShapeDirty = true;
			umaData.Dirty();
		}
		if(!hairState && lastHairState)
		{
			lastHairState = hairState;
			RemoveSlot(7);
			umaData.isMeshDirty = true;
			umaData.isTextureDirty = true;
			umaData.isShapeDirty = true;
			umaData.Dirty();
		}
		if(hairColor != lastHairColor && hairState)
		{
			lastHairColor = hairColor;
			ColorOverlay(7,"M_Hair_Shaggy",hairColor);
			umaData.isTextureDirty = true;
			umaData.Dirty();
		}
		
		if(save)
		{
			save = false;
			SaveAsset();
		}
		if(load)
		{
			load = false;
			LoadAsset();
		}
		
	}
	
	void GenerateUMA()
	{
		// Create a new game object and add UMA componenets to it
		GameObject GO = new GameObject("MyUMA");
		umaDynamicAvatar = GO.AddComponent<UMADynamicAvatar>();	
		
		// Initialise Avatar and grab a reference to it's data component
		umaDynamicAvatar.Initialize();
		umaData = umaDynamicAvatar.umaData;
		umaData.OnCharacterCreated += CharacterCreatedCallback;
		
		// Attach our generator
		umaDynamicAvatar.umaGenerator = generator;
		umaData.umaGenerator = generator;
		
		// Set up slot Array
		umaData.umaRecipe.slotDataList = new SlotData[numberOfSlots];
		
		// Set up our Morph references
		umaDna = new UMADnaHumanoid();
		umaTutorialDNA = new UMADnaTutorial();
		umaData.umaRecipe.AddDna(umaDna);
		umaData.umaRecipe.AddDna(umaTutorialDNA);
		
		
		// >>> This is where the fun will happen <<<
		CreateMale();
		
		umaDynamicAvatar.animationController = animController;
		
		// Generate our UMA
		umaDynamicAvatar.UpdateNewRace();
		
		GO.transform.parent = this.gameObject.transform;
		GO.transform.localPosition = Vector3.zero;
		GO.transform.localRotation = Quaternion.identity;
		
	}
	void CreateMale()
	{
		// Grab a reference to our recipe
		var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
		umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));
		
		umaData.umaRecipe.slotDataList[0] = slotLibraray.InstantiateSlot("MaleEyes");
		umaData.umaRecipe.slotDataList[0].AddOverlay(overlayLibrary.InstantiateOverlay("EyeOverlay"));
		
		umaData.umaRecipe.slotDataList[1] = slotLibraray.InstantiateSlot("MaleInnerMouth");
		umaData.umaRecipe.slotDataList[1].AddOverlay(overlayLibrary.InstantiateOverlay("InnerMouth"));
		
		umaData.umaRecipe.slotDataList[2] = slotLibraray.InstantiateSlot("MaleFace");
		umaData.umaRecipe.slotDataList[2].AddOverlay(overlayLibrary.InstantiateOverlay("MaleHead02"));
		
		umaData.umaRecipe.slotDataList[3] = slotLibraray.InstantiateSlot("MaleTorso");
		umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody02"));
		
		umaData.umaRecipe.slotDataList[4] = slotLibraray.InstantiateSlot("MaleHands");
		umaData.umaRecipe.slotDataList[4].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList());
		
		umaData.umaRecipe.slotDataList[5] = slotLibraray.InstantiateSlot("MaleLegs");
		umaData.umaRecipe.slotDataList[5].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList());
		
		umaData.umaRecipe.slotDataList[6] = slotLibraray.InstantiateSlot("MaleFeet");
		umaData.umaRecipe.slotDataList[6].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList());
		
		umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));
		
		umaData.umaRecipe.slotDataList[2].AddOverlay(overlayLibrary.InstantiateOverlay("MaleEyebrow01",Color.black));
		
		SetSlot(6,"Shoes");
		AddOverlay(6,"Shoes");
		
	}
	
	///////////////// Uma morph routines /////////////////
	
	void SetBodyMass(float mass)
	{
		umaDna.upperMuscle = mass;
		umaDna.upperWeight = mass;
		umaDna.lowerMuscle = mass;
		umaDna.lowerWeight = mass;
		umaDna.armWidth = mass;
		umaDna.forearmWidth = mass;
	}
	//////////////// Overlay Helpers //////////////////
	
	void AddOverlay(int slot, string overlayName)
	{
		umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName));
	}
	
	void AddOverlay(int slot, string overlayName, Color color)
	{
		umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName, color));
	}
	
	void LinkOverlay(int slotNumber, int slotToLink)
	{
		umaData.umaRecipe.slotDataList[slotNumber].SetOverlayList(umaData.umaRecipe.slotDataList[slotToLink].GetOverlayList());
	}
	
	void RemoveOverlay(int slotNumber, string overlayName)
	{
		umaData.umaRecipe.slotDataList[slotNumber].RemoveOverlay(overlayName);
	}
	
	void ColorOverlay(int slotNumber, string overlayName, Color color)
	{
		umaData.umaRecipe.slotDataList[slotNumber].SetOverlayColor(color, overlayName);
	}

	////////////////// Slot Helpers ////////////////
	
	void SetSlot(int slotnumber, string SlotName)
	{
		umaData.umaRecipe.slotDataList[slotnumber] = slotLibraray.InstantiateSlot(SlotName);
	}
	
	void RemoveSlot(int slotNumber)
	{
		umaData.umaRecipe.slotDataList[slotNumber] = null;
	}
	
	////////////////// Load and Save //////////////////
	
	void Save()
	{	
		//Generate UMA String
		UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
		recipe.Save(umaDynamicAvatar.umaData.umaRecipe, umaDynamicAvatar.context);
		SaveString = recipe.recipeString;
		Destroy(recipe);
		
		//Save string to text file		
		string fileName = "Assets/Test.txt";
		StreamWriter stream = File.CreateText(fileName);
		stream.WriteLine (SaveString);
		stream.Close();
	}
	
	void Load()
	{
		//Load string from text file
		string fileName ="Assets/Test.txt";
		StreamReader stream = File.OpenText(fileName);
		SaveString = stream.ReadLine();
		stream.Close();
		
		//Regenerate UMA using string
		UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
		recipe.recipeString = SaveString;
		umaDynamicAvatar.Load(recipe);
		Destroy(recipe);		
	}
	
	void LoadAsset()
	{
		UMARecipeBase recipe = Resources.Load("Troll 1") as UMARecipeBase;
		umaDynamicAvatar.Load(recipe);
	}
	
	void SaveAsset()
	{
		#if UNITY_EDITOR
		var asset = ScriptableObject.CreateInstance<UMATextRecipe>();
		asset.Save(umaDynamicAvatar.umaData.umaRecipe, umaDynamicAvatar.context);
		AssetDatabase.CreateAsset(asset, "Assets/Boom.asset");
		AssetDatabase.SaveAssets();
		#endif
	}
	
	void CharacterCreatedCallback(UMAData umaData)
	{
		GrabStaff();
		UMAExpressionSet expressionSet = umaData.umaRecipe.raceData.expressionSet;
		expressionPlayer = umaData.gameObject.AddComponent<UMAExpressionPlayer>();
		expressionPlayer.expressionSet = expressionSet;
		expressionPlayer.umaData = umaData;
		expressionPlayer.Initialize();
		expressionPlayer.enableBlinking = true;
		expressionPlayer.enableSaccades = true;
	}
	
	void GrabStaff()
	{
		GameObject staff = GameObject.Find("staff");
		Transform hand =  umaDynamicAvatar.gameObject.transform.FindChild("Root/Global/Position/Hips/LowerBack/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand");
		staff.transform.SetParent(hand);
		staff.transform.localPosition = new Vector3(-0.1f,0,-0.05f);
		staff.transform.localRotation = Quaternion.Euler(new Vector3(18f,0,0));
	}
	
	

}