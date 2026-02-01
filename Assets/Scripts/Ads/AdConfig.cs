using UnityEngine;

[CreateAssetMenu(fileName = "AdConfig", menuName = "JumpyPro/Ad Configuration")]
public class AdConfig : ScriptableObject
{
    [Header("Test Mode")]
    [Tooltip("Enable to use test ad IDs")]
    public bool useTestAds = true;

    [Header("Test Ad IDs (Google)")]
    public string testBannerId = "ca-app-pub-3940256099942544/6300978111";
    public string testInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    public string testAppId = "ca-app-pub-3940256099942544~3347511713";

    [Header("Production Ad IDs")]
    [Tooltip("Your real Banner Ad Unit ID from AdMob")]
    public string productionBannerId = "";
    [Tooltip("Your real Interstitial Ad Unit ID from AdMob")]
    public string productionInterstitialId = "";
    [Tooltip("Your real App ID from AdMob")]
    public string productionAppId = "";

    [Header("Interstitial Settings")]
    [Tooltip("Show interstitial every X games")]
    public int gamesBeforeInterstitial = 3;

    public string BannerId => useTestAds ? testBannerId : productionBannerId;
    public string InterstitialId => useTestAds ? testInterstitialId : productionInterstitialId;
    public string AppId => useTestAds ? testAppId : productionAppId;
}
