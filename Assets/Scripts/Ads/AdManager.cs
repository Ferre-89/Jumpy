using System;
using UnityEngine;

// IMPORTANTE: Descomentar la siguiente linea cuando instales Google Mobile Ads SDK
// using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private AdConfig adConfig;

    // Eventos para notificar cuando los anuncios estan listos
    public event Action OnInterstitialClosed;

    private bool isInitialized = false;
    private bool isBannerLoaded = false;
    private bool isInterstitialLoaded = false;

    // Descomentar cuando tengas el SDK instalado:
    // private BannerView bannerView;
    // private InterstitialAd interstitialAd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAds()
    {
        if (adConfig == null)
        {
            Debug.LogWarning("AdManager: No AdConfig assigned. Ads disabled.");
            return;
        }

        Debug.Log("AdManager: Initializing with " + (adConfig.useTestAds ? "TEST" : "PRODUCTION") + " ads");

        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        MobileAds.Initialize(initStatus =>
        {
            isInitialized = true;
            Debug.Log("AdMob SDK Initialized");
            LoadInterstitial();
        });
        */

        // Placeholder - remover cuando actives el SDK real
        isInitialized = true;
        Debug.Log("AdManager: SDK not installed. Using placeholder mode.");
    }

    #region Banner Ads

    public void ShowBanner()
    {
        if (!isInitialized || adConfig == null) return;

        Debug.Log("AdManager: ShowBanner called");

        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        // Destroy existing banner
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create new banner at bottom of screen
        bannerView = new BannerView(adConfig.BannerId, AdSize.Banner, AdPosition.Bottom);

        // Register for events
        bannerView.OnBannerAdLoaded += () =>
        {
            isBannerLoaded = true;
            Debug.Log("Banner loaded successfully");
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            isBannerLoaded = false;
            Debug.LogWarning("Banner failed to load: " + error.GetMessage());
        };

        // Load the banner
        bannerView.LoadAd(new AdRequest());
        */

        // Placeholder
        isBannerLoaded = true;
        Debug.Log("AdManager: Banner would show here (SDK not installed)");
    }

    public void HideBanner()
    {
        if (!isInitialized) return;

        Debug.Log("AdManager: HideBanner called");

        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        if (bannerView != null)
        {
            bannerView.Hide();
        }
        */

        isBannerLoaded = false;
    }

    public void DestroyBanner()
    {
        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
        */

        isBannerLoaded = false;
    }

    #endregion

    #region Interstitial Ads

    private void LoadInterstitial()
    {
        if (!isInitialized || adConfig == null) return;

        Debug.Log("AdManager: Loading interstitial...");

        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        // Destroy existing ad
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest();

        InterstitialAd.Load(adConfig.InterstitialId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                isInterstitialLoaded = false;
                Debug.LogWarning("Interstitial failed to load: " + error?.GetMessage());
                return;
            }

            interstitialAd = ad;
            isInterstitialLoaded = true;
            Debug.Log("Interstitial loaded successfully");

            RegisterInterstitialEvents(ad);
        });
        */

        // Placeholder
        isInterstitialLoaded = true;
        Debug.Log("AdManager: Interstitial would load here (SDK not installed)");
    }

    /*
    // DESCOMENTAR CUANDO INSTALES EL SDK
    private void RegisterInterstitialEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial closed");
            isInterstitialLoaded = false;
            OnInterstitialClosed?.Invoke();
            LoadInterstitial(); // Preload next one
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogWarning("Interstitial failed to show: " + error.GetMessage());
            isInterstitialLoaded = false;
            OnInterstitialClosed?.Invoke();
            LoadInterstitial();
        };
    }
    */

    public bool TryShowInterstitial(int gamesPlayed, Action onComplete = null)
    {
        if (adConfig == null)
        {
            onComplete?.Invoke();
            return false;
        }

        // Check if it's time to show an interstitial
        if (gamesPlayed <= 0 || gamesPlayed % adConfig.gamesBeforeInterstitial != 0)
        {
            onComplete?.Invoke();
            return false;
        }

        Debug.Log("AdManager: Attempting to show interstitial (Game #" + gamesPlayed + ")");

        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            // Set up one-time callback
            void OnClosed()
            {
                OnInterstitialClosed -= OnClosed;
                onComplete?.Invoke();
            }
            OnInterstitialClosed += OnClosed;

            interstitialAd.Show();
            return true;
        }
        else
        {
            Debug.Log("Interstitial not ready, skipping...");
            LoadInterstitial(); // Try to load for next time
            onComplete?.Invoke();
            return false;
        }
        */

        // Placeholder - simulate showing ad
        Debug.Log("AdManager: Interstitial would show here (SDK not installed)");
        onComplete?.Invoke();
        return false;
    }

    #endregion

    private void OnDestroy()
    {
        DestroyBanner();

        // ==============================================
        // DESCOMENTAR ESTE BLOQUE CUANDO INSTALES EL SDK
        // ==============================================
        /*
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
        */
    }
}
