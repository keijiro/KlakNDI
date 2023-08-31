package jp.keijiro.klak.ndi;

import android.net.nsd.NsdManager;
import android.net.nsd.NsdServiceInfo;

public class DiscoveryListener implements NsdManager.DiscoveryListener {
    public void onDiscoveryStarted(String str) {}
    public void onDiscoveryStopped(String str) {}
    public void onStartDiscoveryFailed(String str, int i) {}
    public void onStopDiscoveryFailed(String str, int i) {}
    public void onServiceFound(NsdServiceInfo nsdServiceInfo) {}
    public void onServiceLost(NsdServiceInfo nsdServiceInfo) {}
}