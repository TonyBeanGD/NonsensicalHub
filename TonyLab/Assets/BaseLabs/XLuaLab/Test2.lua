local unityEngine = CS.UnityEngine;

function Awake()
    unityEngine.Debug.Log("Awake");
end

function Update()

end

function OnDestroy()
    unityEngine.Debug.Log("OnDestroy");
end
